using CloudinaryDotNet.Actions;
using FGS_BE.Repo.DTOs.Cloudinary;
using Microsoft.AspNetCore.Http;

namespace FGS_BE.Services.Interfaces
{
    public interface ICloudinaryService
    {
        Task<CloudinaryResponse> UploadImage(string fileName, IFormFile fileImage);
        Task<DeletionResult> DeleteFileAsync(string publicId);
    }
}
