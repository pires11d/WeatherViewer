namespace Weather.Lib.Utils
{
    public static class FileHelper
    {
        private static readonly string _defaultFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "history");

        public static void SaveFile(string folder, string file, string[] content)
        {
            var subFolderPath = Path.Combine(_defaultFolder, folder);

            if (!Directory.Exists(subFolderPath)) 
                Directory.CreateDirectory(subFolderPath);

            var filePath = Path.Combine(subFolderPath, file);
            filePath += ".csv";

            var entry = string.Join(",", content);

            File.AppendAllLines(filePath, [entry]);
        }

        public static List<string> GetFiles(string folder, DateOnly startDate, DateOnly endDate)
        {
            var result = new List<string>();
            var subFolderPath = Path.Combine(_defaultFolder, folder);
            var filePaths = Directory.GetFiles(subFolderPath);

            foreach (var filePath in filePaths)
            {
                var creationTime = Directory.GetCreationTime(filePath).Date;
                var creationDate = DateOnly.FromDateTime(creationTime);
                if (creationDate >= startDate && creationDate <= endDate)
                {
                    result.AddRange(File.ReadAllLines(filePath));
                }
            }

            return result;
        }
    }
}
