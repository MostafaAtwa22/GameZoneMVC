using AutoMapper;
using Azure;
using GameZone.Core.Models;
using GameZone.Repository.Repository;
using GameZone.Services.ViewModels.ManagesVM;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace GameZone.Services.Services.ManageServices
{
    public class ManageServices : IManageServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHostingEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly string _imagesPath;
        public ManageServices(IUnitOfWork unitOfWork,
            IHostingEnvironment webHostEnvironment,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _signInManager = signInManager;
            _imagesPath = $"{_webHostEnvironment.WebRootPath}{FileSettings.ImagesPath}/users";
        }

        public async Task<ApplicationUser?> GetById(string? id)
        {
            var user = await _unitOfWork.ApplicationUsers
                .Find(u => u.Id == id);

            if (user is null)
                return null!;

            return user;
        }

        public async Task<ApplicationUser?> Update(UpdateProfileVM model)
        {
            var user = await _unitOfWork.ApplicationUsers.FindWithTrack(u => u.Id == model.Id);
            if (user is null)
                return null;

            user.UserName = model.UserName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.City = model.City;
            user.Country = model.Country;

            string? oldCover = user.Cover; 

            if (model.Cover is not null)
            {
                user.Cover = await SaveCover(model.Cover);
            }

            _unitOfWork.ApplicationUsers.Update(user);
            var affectedRows = await _unitOfWork.Complete();

            if (affectedRows > 0)
            {
                if (oldCover is not null && model.Cover is not null)
                {
                    var oldCoverPath = Path.Combine(_imagesPath, oldCover);
                    if (File.Exists(oldCoverPath))
                    {
                        File.Delete(oldCoverPath);
                    }
                }

                return user;
            }

            if (model.Cover is not null)
            {
                var newCoverPath = Path.Combine(_imagesPath, user.Cover!);
                if (File.Exists(newCoverPath))
                {
                    File.Delete(newCoverPath);
                }
            }

            return null;
        }

        public async Task<string?> Email(EmailVM model, string oldEmail)
        {
            if (model.Email == oldEmail)
                return oldEmail;

            var user = await _userManager.FindByEmailAsync(model.Email);

            var changeUser = await _userManager.FindByEmailAsync(oldEmail);

            if (user is not null)
                return null!;

            var result = await _userManager.SetEmailAsync(changeUser, model.Email);
            if (result.Succeeded)
                return model.Email;

            return null!;
        }

        public async Task<bool> ChangePassword(string userId, ChangePasswordVM model)
        {
            var user = await _unitOfWork.ApplicationUsers
                .FindWithTrack(u => u.Id == userId);

            if (user is null)
                return false;

            var result = await _userManager.ChangePasswordAsync(user, model.oldPassword, model.NewPassword);

            if (result.Succeeded)
                return true;

            return false;
        }

        public async Task<(byte[] FileContent, string ContentType, string FileName)?> DownloadPersonalData(string userId)
        {
            var user = await GetById(userId);

            if (user is null)
                return null;

            var personalData = new Dictionary<string, string>();

            var personalDataProps = user.GetType().GetProperties() // Use actual user type
                .Where(prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));

            foreach (var prop in personalDataProps)
            {
                personalData[prop.Name] = prop.GetValue(user)?.ToString() ?? "null";
            }

            var logins = await _userManager.GetLoginsAsync(user);
            foreach (var login in logins)
            {
                personalData[$"{login.LoginProvider} external login provider key"] = login.ProviderKey;
            }

            var authenticatorKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (!string.IsNullOrWhiteSpace(authenticatorKey)) 
            {
                personalData["Authenticator Key"] = authenticatorKey;
            }

            var jsonBytes = JsonSerializer.SerializeToUtf8Bytes(personalData);

            return (jsonBytes, "application/json", "PersonalData.json");
        }

        public async Task<bool> DeletePersonData(DeleteDataVM model)
        {
            var user = await _unitOfWork.ApplicationUsers
                .FindWithTrack(u => u.Id == model.UserId);

            if (user is null)
                return false;

            if (!await _userManager.CheckPasswordAsync(user, model.Password))
                return false;

            var res = await _userManager.DeleteAsync(user);

            if (res.Succeeded)
                return false;

            await _signInManager.SignOutAsync();

            return true;
        }

        private async Task<string> SaveCover(IFormFile cover)
        {
            var coverName = $"{Guid.NewGuid()}{Path.GetExtension(cover.FileName)}";

            var path = Path.Combine(_imagesPath, coverName);

            using var stream = File.Create(path);
            await cover.CopyToAsync(stream);

            return coverName;
        }
    }
}
