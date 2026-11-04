using Gym.Business.Common;
using Microsoft.AspNetCore.Http;

namespace Gym.Business.Services;

public interface IPhotoService
{
    Task<Result<string>> UploadPhotoAsync(IFormFile file, string folder = "members", CancellationToken cancellationToken = default);
    Task<Result> DeletePhotoAsync(string publicIdOrUrl, CancellationToken cancellationToken = default);
}
