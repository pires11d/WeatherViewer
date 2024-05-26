namespace Weather.Lib.Utils.Interfaces
{
    public interface IFileHelper
    {
        void SaveFile(string folder, string file, string[] content);

        List<string> GetFiles(string folder, DateOnly startDate, DateOnly endDate);
    }
}