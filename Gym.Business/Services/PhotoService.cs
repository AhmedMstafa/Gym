using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Gym.Business.Common;
using Gym.Business.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gym.Business.Services;

internal class PhotoService : IPhotoService
{
    private readonly Cloudinary? _cloudinary;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<PhotoService> _logger;
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    private const long MaxFileSizeInBytes = 5 * 1024 * 1024; // 5 MB

    public PhotoService(
        IOptions<CloudinarySettings> config,
        IWebHostEnvironment environment,
        ILogger<PhotoService> logger)
    {
        _environment = environment;
        _logger = logger;
        var settings = config.Value;

        // Opt-in only. Missing Enabled (or Development) always stores files on disk.
        var canUseCloudinary =
            settings.Enabled &&
            !_environment.IsDevelopment() &&
            !string.IsNullOrWhiteSpace(settings.CloudName) &&
            !string.IsNullOrWhiteSpace(settings.ApiKey) &&
            !string.IsNullOrWhiteSpace(settings.ApiSecret) &&
            !settings.CloudName.Equals("your-cloud-name", StringComparison.OrdinalIgnoreCase);

        if (canUseCloudinary)
        {
            var account = new Account(settings.CloudName, settings.ApiKey, settings.ApiSecret);
            _cloudinary = new Cloudinary(account);
        }
        else
        {
            _logger.LogInformation("Cloudinary is disabled. Member photos will be saved locally.");
        }
    }

    public async Task<Result<string>> UploadPhotoAsync(IFormFile file, string folder = "members", CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
        {
            return Result.Failure<string>("Photo.Empty", "File is empty or not provided.");
        }

        if (file.Length > MaxFileSizeInBytes)
        {
            return Result.Failure<string>("Photo.Size", "Image size exceeds the maximum limit of 5 MB.");
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
        {
            return Result.Failure<string>("Photo.Extension", "Invalid file format. Only JPG, PNG, and WebP are allowed.");
        }

        byte[] fileBytes;
        try
        {
            await using var buffer = new MemoryStream();
            await file.CopyToAsync(buffer, cancellationToken);
            fileBytes = buffer.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to read uploaded photo from the request.");
            return Result.Failure<string>("Photo.ReadFailed", "Could not read the uploaded photo.");
        }

        if (_cloudinary != null)
        {
            try
            {
                await using var cloudStream = new MemoryStream(fileBytes, writable: false);
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, cloudStream),
                    Folder = folder,
                    Transformation = new Transformation()
                        .Width(500)
                        .Height(500)
                        .Crop("fill")
                };

                var uploadTask = _cloudinary.UploadAsync(uploadParams);
                var timeoutTask = Task.Delay(TimeSpan.FromSeconds(20), cancellationToken);
                var completed = await Task.WhenAny(uploadTask, timeoutTask);

                if (completed == uploadTask)
                {
                    var uploadResult = await uploadTask;
                    if (uploadResult.Error == null)
                    {
                        var secureUrl = uploadResult.SecureUrl?.ToString() ?? uploadResult.Url?.ToString();
                        if (!string.IsNullOrWhiteSpace(secureUrl))
                        {
                            _logger.LogInformation("Photo uploaded to Cloudinary successfully: {Url}", secureUrl);
                            return Result.Success(secureUrl);
                        }
                    }

                    _logger.LogWarning("Cloudinary upload failed: {Error}. Saving locally instead.",
                        uploadResult.Error?.Message ?? "Cloudinary did not return an image URL.");
                }
                else
                {
                    _logger.LogWarning("Cloudinary upload timed out. Saving locally instead.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cloudinary upload failed. Saving locally instead.");
            }
        }

        return await SaveLocallyAsync(fileBytes, folder, extension, cancellationToken);
    }

    public async Task<Result> DeletePhotoAsync(string publicIdOrUrl, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(publicIdOrUrl))
        {
            return Result.Success();
        }

        if (publicIdOrUrl.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase) ||
            publicIdOrUrl.StartsWith("/images/", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                var fullPath = ResolveLocalPhotoPath(publicIdOrUrl);
                if (fullPath is not null && File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to delete local photo: {Url}", publicIdOrUrl);
                return Result.Success();
            }
        }

        if (_cloudinary != null && publicIdOrUrl.Contains("cloudinary.com", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                var publicId = ExtractPublicIdFromUrl(publicIdOrUrl);
                var deleteParams = new DeletionParams(publicId);
                await _cloudinary.DestroyAsync(deleteParams);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error deleting photo from Cloudinary.");
                return Result.Success();
            }
        }

        return Result.Success();
    }

    private async Task<Result<string>> SaveLocallyAsync(
        byte[] fileBytes,
        string folder,
        string extension,
        CancellationToken cancellationToken)
    {
        try
        {
            var uploadsFolder = Path.Combine(GetUploadsRoot(), folder);
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            await File.WriteAllBytesAsync(filePath, fileBytes, cancellationToken);

            var relativeUrl = $"/uploads/{folder}/{uniqueFileName}";
            _logger.LogInformation("Photo saved locally: {Url}", relativeUrl);
            return Result.Success(relativeUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save photo locally.");
            return Result.Failure<string>("Photo.LocalSaveFailed", "Could not save photo.");
        }
    }

    private string GetUploadsRoot() => LocalPhotoStorage.GetRootPath(_environment.ContentRootPath);

    private string GetWebRoot()
        => _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

    private string? ResolveLocalPhotoPath(string url)
    {
        var relativePath = url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);

        if (url.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase))
        {
            var withoutPrefix = relativePath["uploads".Length..].TrimStart(Path.DirectorySeparatorChar);
            return Path.GetFullPath(Path.Combine(GetUploadsRoot(), withoutPrefix));
        }

        return Path.GetFullPath(Path.Combine(GetWebRoot(), relativePath));
    }

    private static string ExtractPublicIdFromUrl(string url)
    {
        if (!url.Contains("cloudinary.com", StringComparison.OrdinalIgnoreCase))
        {
            return url;
        }

        try
        {
            var uri = new Uri(url);
            var segments = uri.AbsolutePath.Trim('/').Split('/');
            var uploadIndex = Array.IndexOf(segments, "upload");
            if (uploadIndex >= 0 && uploadIndex < segments.Length - 1)
            {
                var remaining = segments.Skip(uploadIndex + 1).ToList();
                if (remaining.Count > 0 && remaining[0].StartsWith("v", StringComparison.OrdinalIgnoreCase) && long.TryParse(remaining[0].Substring(1), out _))
                {
                    remaining.RemoveAt(0);
                }

                var joined = string.Join("/", remaining);
                var dotIndex = joined.LastIndexOf('.');
                return dotIndex > 0 ? joined[..dotIndex] : joined;
            }
        }
        catch
        {
            // fallback
        }

        return url;
    }
}
