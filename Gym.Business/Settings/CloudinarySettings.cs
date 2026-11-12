namespace Gym.Business.Settings;

public class CloudinarySettings
{
    public const string SectionName = "CloudinarySettings";

    public string CloudName { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string ApiSecret { get; set; } = string.Empty;

    /// <summary>
    /// When false, photos are stored under wwwroot instead of Cloudinary.
    /// Local HTTP Development can opt out through appsettings.Development.json.
    /// </summary>
    public bool Enabled { get; set; }
}
