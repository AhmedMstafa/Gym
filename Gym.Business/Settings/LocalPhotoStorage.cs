namespace Gym.Business.Settings;

public static class LocalPhotoStorage
{
    public const string RequestPath = "/uploads";

    public static string GetRootPath(string? contentRootPath = null)
    {
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string root;

        if (!string.IsNullOrWhiteSpace(localAppData))
        {
            root = Path.Combine(localAppData, "Gym", "uploads");
        }
        else if (!string.IsNullOrWhiteSpace(contentRootPath))
        {
            root = Path.Combine(contentRootPath, "App_Data", "uploads");
        }
        else
        {
            root = Path.Combine(Path.GetTempPath(), "Gym", "uploads");
        }

        root = Path.GetFullPath(root);
        Directory.CreateDirectory(root);
        return root;
    }
}
