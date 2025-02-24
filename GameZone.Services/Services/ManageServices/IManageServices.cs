using GameZone.Core.Models;
using GameZone.Services.ViewModels.ManagesVM;

namespace GameZone.Services.Services.ManageServices
{
    public interface IManageServices
    {
        Task<ApplicationUser?> GetById(string? id);
        Task<ApplicationUser?> Update(UpdateProfileVM user);
        Task<string?> Email(EmailVM user, string oldEmail);
        Task<bool> ChangePassword(string userId, ChangePasswordVM model);
        Task<(byte[] FileContent, string ContentType, string FileName)?> DownloadPersonalData(string userId);
        Task<bool> DeletePersonData(DeleteDataVM model);
    }
}
