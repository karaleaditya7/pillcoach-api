using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OntrackDb.Assets;
using OntrackDb.Authentication;
using OntrackDb.Authorization;
using OntrackDb.Context;
using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Enums;
using OntrackDb.Filter;
using OntrackDb.Helper;
using OntrackDb.Model;
using OntrackDb.Service;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace OntrackDb.Repositories
{
    public class UserData:IUserData
    {
        private readonly ApplicationDbContext _applicationDbcontext;
        private readonly UserManager<User> _userManager;
        private readonly IJwtUtils _jwtUtils;
        private readonly IImageService _imageService;
        private readonly IEmailService _emailService;
        readonly IPasswordHasher<User> _passwordHasher;


        private readonly IConfiguration _configuration;

        public UserData(ApplicationDbContext applicationDbcontext, IJwtUtils jwtUtils, UserManager<User> userManager, IConfiguration configuration, IImageService imageservice, IEmailService emailService, IPasswordHasher<User> passwordHasher)
        {
            this._applicationDbcontext = applicationDbcontext;
            this._userManager = userManager;
            _configuration = configuration;
            _jwtUtils = jwtUtils;
            _imageService = imageservice;
            _emailService = emailService;
            _passwordHasher = passwordHasher;
        }

        public async Task<List<User>> GetUsers(PaginationFilter filter)
        {
            List<User> query = await _applicationDbcontext.Users.Skip(filter.PageNumber).Take(filter.PageSize).ToListAsync();

            return query.ToList();
        }

        public async Task<Response<User>> Register(User user, string password)
        {
            Response<User> response = new Response<User>();

            var result = await _userManager.CreateAsync(user, password);

            if (result == null)
            {
                response.Success = false;
                response.Message = result.ToString();
                return response;
            }

            await UpdatePasswordHistoryAsync(user.Id);

            await _userManager.AddToRoleAsync(user, Roles.Employee.ToString());

            var Url = _configuration["EmailConfirmationUrl"];
            var code = await this._userManager.GenerateEmailConfirmationTokenAsync(user);
            code = System.Web.HttpUtility.UrlEncode(code);
            Url += "/verifyUser" + "?token=" + code.ToString() + "&email=" + user.Email;
            String htmlText = EmailTemplates.GetEmailConfirmationTemplate(Url);
            await _emailService.SendEmailAsync(user.Email, "Confirm your email address", code.ToString(), htmlText);

            response.Success = true;
            response.Data = user;
            response.Message = "User created successfully!";
            return response;
        }

        private async Task UpdatePasswordHistoryAsync(string userId)
        {
            try
            {
                var user = await _userManager.Users
                    .Include(u => u.PasswordHistory)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null) return;

                user.PasswordSetDateUTC = DateTime.UtcNow;
                user.LockoutEnd = null; // clear lockout, if any

                // add current password to history
                user.PasswordHistory.Add(new UserPasswordHistory { UserID = userId, HashPassword = user.PasswordHash });

                try
                {
                    // remove passwords older than a year, in order to keep the history table lighter
                    var oldPasswords = user.PasswordHistory
                        .Where(p => p.CreatedDateUTC < DateTime.UtcNow.AddDays(-365));

                    foreach (var password in oldPasswords)
                    {
                        user.PasswordHistory.Remove(password);
                    }
                }
                catch { }

                await _userManager.UpdateAsync(user);
            }
            catch { }
        }
        public async Task<List<User>> GetAllAssignedUserForAdmin(string userId)
        {
            var query = _applicationDbcontext.Users.Include(u => u.PharmacyUsers).ThenInclude(pu => pu.Pharmacy)
                      .Where(u => !u.IsDeleted && u.IsEnabled && u.PharmacyUsers.Any(p => _applicationDbcontext.PharmacyUsers.Where(pu => pu.UserId == userId).Select(pu => pu.PharmacyId).Contains(p.PharmacyId)))
                      .Join(_applicationDbcontext.UserRoles, user => user.Id, userRole => userRole.UserId, (u, ur) => new { User = u, UserRole = ur })
                      .Join(_applicationDbcontext.Roles.Where(r => r.Name != Roles.SuperAdmin.ToString() && r.Name != Roles.Admin.ToString()), ur => ur.UserRole.RoleId, role => role.Id, (ur, r) => ur.User)
                      .AsNoTracking();

            return await query.ToListAsync();
        }

        public async Task<WebSocket> GetWebSocketByUser(string userId)
        {
            return await _applicationDbcontext.WebSockets.Where(w => w.User.Id == userId).FirstOrDefaultAsync();
        }

        public  async Task<WebSocket> AddWebSocket(WebSocket webSocket)
        {
            var result =await _applicationDbcontext.WebSockets.AddAsync(webSocket);
            await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<WebSocket> UpdateWebSocket(WebSocket webSocket)
        {
            var result =  _applicationDbcontext.WebSockets.Update(webSocket);
           await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<Response<User>> AddUser(User user, string password)
        {
            Response<User> response = new Response<User>();

            var result = await _userManager.CreateAsync(user, password);
            if (result == null)
            {
                response.Success = false;
                response.Message = result.ToString();
                return response;
            }
            if(user.JobPosition == "Admin")
            {
                await _userManager.AddToRoleAsync(user, Roles.Admin.ToString());
            }
            else if(user.JobPosition == "SuperAdmin")
            {
                await _userManager.AddToRoleAsync(user, Roles.SuperAdmin.ToString());
            }
            else
            {
                await _userManager.AddToRoleAsync(user, Roles.Employee.ToString());
            }            
            response.Success = true;
            response.Data = user;
            response.Message = "User created successfully!";
            return response;
        }

        public async Task<List<AdminNotification>> GetAdminNotificationByUserIdForDelete(string userId)
        {
            var notification = await _applicationDbcontext.AdminNotifications.Include(u => u.User).
            Where(p => p.User.Id == userId).
            ToListAsync();
            return notification;
        }

     

        public async Task<Response<User>> UpdateUser(User user) {
            Response<User> response = new Response<User>();
            var result = await _userManager.UpdateAsync(user);

            if (result == null)
            {
                response.Success = false;
                response.Message = "User Not Updated";
                return response;
            }
            response.Success = true;
            response.Data = user;
            response.Message = "User Updated successfully!";
            return response;
        }


      

        public async Task<User> GetUserByUserName(string userName) {

            var userExists = await _userManager.FindByNameAsync(userName);
            return userExists;
        }
        public async Task<User> GetUserByUserId(string Id)
        {
            var userExists = await _userManager.FindByIdAsync(Id);
            return userExists;
        }
        public async Task<User> GetUserByEmail(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }
        public async Task<User> GetUser(string userName)
        {

            var userExists = await _applicationDbcontext.Users.Include(x => x.Licenses).Include(x => x.Address).
                                                              
                                                               SingleAsync(x => x.UserName == userName);
            return userExists;
        }
        public async Task<User> GetUserById(string id)
        {
            var userExists = await _applicationDbcontext.Users
                .Include(x => x.Licenses)
                .Include(x => x.Address)
                .Include(x => x.Compliance)
                .FirstOrDefaultAsync(x => x.Id == id);

            return userExists;
        }

        public async Task<UserDto> GetUserInfoById(string id)
        {
            var userExists = await _applicationDbcontext.Users.Where(x => x.Id == id)
              .Select(x => new UserDto
              {
                  Id = x.Id,
                  FirstName = x.FirstName,
                  LastName = x.LastName,
                  JobPosition = x.JobPosition,
                  ImageName = x.ImageName,
                  TwilioPhoneNumber=x.TwilioPhoneNumber,
                  Email = x.Email,
                  ImportEnabled = x.ImportEnabled,
                  ImageUri = _imageService.GetImageURI(x.ImageName)
              }).FirstOrDefaultAsync();


            return userExists;
        }

        public async Task<UserPDCDto> GetUserByIdForPDC(string id)
        {

            var userExists = await _applicationDbcontext.Users.Select(x=> new UserPDCDto
            {
                Id = x.Id
            }).SingleAsync(x => x.Id == id);

            return userExists;
        }

        public async Task<bool> AssignPharmacies(string id, List<string> pharmacyIds, bool replaceExisting = true)
        {
           
            try
            {
                if (replaceExisting)
                    _applicationDbcontext.PharmacyUsers.RemoveRange(_applicationDbcontext.PharmacyUsers.Where(pharmacyUser => pharmacyUser.User.Id == id));
                
                var user =  _applicationDbcontext.Users.Find(id);
                pharmacyIds.ForEach(x => {
                    PharmacyUser pharmacyUser = new PharmacyUser();
                    pharmacyUser.Pharmacy = _applicationDbcontext.Pharmacies.Find(Convert.ToInt32(x));
                    pharmacyUser.User = user;
                    _applicationDbcontext.PharmacyUsers.Add(pharmacyUser);
                    
                });

                await _applicationDbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An exception occurred: {ex.Message}");
                return false;
            }
            
        }

        public async Task<List<Pharmacy>> GetPharmacy(string authorization)
        {
            var parameter = "";
            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                parameter = headerValue.Parameter;
            }
               string userId =  _jwtUtils.ValidateToken(parameter);
            List<Pharmacy> pharmacies = await _applicationDbcontext.PharmacyUsers.
                                Include(x=>x.Pharmacy.Address).
                               Include(x=>x.Pharmacy.Contact).
                               Where(x => x.UserId == userId).
                               Select(x=>x.Pharmacy)
                               .AsNoTracking().ToListAsync();


            return pharmacies;
        }

        public async Task<Licenses> GetLicenseById(int id)
        {
            var licensesExists = await _applicationDbcontext.Licenses.FindAsync(id);
            return licensesExists;
        }
        public async Task<Address> GetAddressById(int id)
        {
            var addressExists = await _applicationDbcontext.Address.FindAsync(id);
            return addressExists;
        }

        public async Task<bool> GetUserByUserAndPassword(User user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }
        public Task<string> GeneratePasswordResetToken(User user)
        {

            return _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<Response<User>> ResetPasswordAsync(User user, string token, string password)
        {
            Response<User> response = new Response<User>();

            if (await IsPreviousPassword(user.Id, password))
            {
                response.Message = "Cannot repeat password that was used within a year. Please specify a different password.";
                response.Success = false;
                return response;
            }

            var resetPassResult = await _userManager.ResetPasswordAsync(user, token, password);

            if (!resetPassResult.Succeeded)
            {
                response.Success = false;
                response.Message = "Failed to reset password at this moment. Please try again.";
                return response;
            }

            await UpdatePasswordHistoryAsync(user.Id);

            response.Success = true;
            response.Message = "Password Reset Successfully";
            return response;
        }

        public async Task<User> UpdateLastLoginForUser(User user, string deviceId)
        {
            DateTime dateTime = DateTime.Now;
            
            user.LastLogin = dateTime;
            user.LastDeviceId = deviceId;
            user.LastVerifiedDateUTC = DateTime.UtcNow.Date;
            user.VerificationCode = null;
            user.CodeExpiryDateUTC = null;
            user.FailedLoginCount = 0;
            user.LockoutEnd = null;

            await _userManager.UpdateAsync(user);

            return user;
        }

        public async Task<List<User>> GetAdminUsers()
        {
            var users = await _userManager.GetUsersInRoleAsync("Admin");
            List<User> userList = new List<User>();

            foreach (User user in users.ToList())
            {
                if (!user.IsDeleted)
                {
                    if (user.ImageName != null && user.PhoneNumber != null)
                    {

                        user.ImageUri = _imageService.GetImageURI(user.ImageName).ToString();

                    }
                    userList.Add(user);
                }

            }
            return userList;
        }



        public async Task<List<UserDto>> GetAllUsersForAdmin(int recordNumber, int pageLimit,string userId, string userRole, bool activeUser, string keywords, string sortDirection, string filterType, string filterValue, string filterCategory)
        {
            List<User> query=null;
            var userList = new List<UserDto>();
            var users = new List<UserDto>();
            bool condition = false;
            if(sortDirection == "asc")
            {
                condition = true;
            }

            UserDto userInfo = await GetUserInfoById(userId);
            if ((userInfo.Email == _configuration.GetValue<string>("Master:User1")) || (userInfo.Email == _configuration.GetValue<string>("Master:User2")) || (userInfo.Email == _configuration.GetValue<string>("Master:User3")))
            {
                query = await _applicationDbcontext.Users
                    .Where(u => !u.IsDeleted && u.IsEnabled == activeUser)
                    .Join(_applicationDbcontext.UserRoles, user => user.Id, userRole => userRole.UserId, (u, ur) => new { User = u, UserRole = ur })
                    .Join(_applicationDbcontext.Roles, ur => ur.UserRole.RoleId, role => role.Id, (ur, r) => ur.User)
                    .Include(x => x.Licenses)
                    .Include(x => x.Address)
                    
                    .ToListAsync();
            }
            else if (Roles.SuperAdmin.ToString().Equals(userRole, StringComparison.OrdinalIgnoreCase))
            {
                // user list for super admins
                // get all users in the system except super admins
                query = await _applicationDbcontext.Users
                    .Where(u => !u.IsDeleted && u.IsEnabled == activeUser)
                    .Join(_applicationDbcontext.UserRoles, user => user.Id, userRole => userRole.UserId, (u, ur) => new { User = u, UserRole = ur })
                    .Join(_applicationDbcontext.Roles.Where(r => r.Name != Roles.SuperAdmin.ToString()), ur => ur.UserRole.RoleId, role => role.Id, (ur, r) => ur.User).Include(x => x.Licenses).Include(x=>x.Address).ToListAsync();
            }
            else if (Roles.Admin.ToString().Equals(userRole, StringComparison.OrdinalIgnoreCase))
            {
                // user list for pharmacy admins
                // get all users that belong to the pharmacies assigned to this admin
                // exclude super admins from the list
                query =await _applicationDbcontext.Users
                     .Where(u => !u.IsDeleted
                     && u.IsEnabled == activeUser
                    && u.PharmacyUsers.Any(p => _applicationDbcontext.PharmacyUsers.Where(pu => pu.UserId == userId).Select(pu => pu.PharmacyId).Contains(p.PharmacyId)))
                    .Join(_applicationDbcontext.UserRoles, user => user.Id, userRole => userRole.UserId, (u, ur) => new { User = u, UserRole = ur })
                    .Join(_applicationDbcontext.Roles.Where(r => r.Name != Roles.SuperAdmin.ToString()), ur => ur.UserRole.RoleId, role => role.Id, (ur, r) => ur.User).Include(x => x.Licenses).Include(x => x.Address).ToListAsync();
            }
            if (query == null) return userList;



            if (!string.IsNullOrEmpty(filterType))
            {
                switch (filterType)
                {
                    case "Licenses":
                          if(filterValue == "Active")
                         {
                                users =  query
                               .Where(u => u.Licenses.ExpirationDate.CompareTo(DateTime.Now) > 0)
                               .Select(x => new UserDto
                               {
                                   Id = x.Id,
                                   FirstName = x.FirstName,
                                   LastName = x.LastName,
                                   DateOfBirth=x.DateOfBirth,
                                   JobPosition = x.JobPosition,
                                   Licenses = x.Licenses,
                                   LastLogin = x.LastLogin,
                                   ImageName = x.ImageName,
                                   TwilioPhoneNumber = x.TwilioPhoneNumber,
                                   Email = x.Email,
                                   PhoneNumber = x.PhoneNumber,
                                   Address = x.Address,
                                   CreatedDate = x.CreatedDate,
                                   EmailConfirmed = x.EmailConfirmed,
                                   IsEnabled = x.IsEnabled,
                                   IsDisabled = x.IsDisabled,
                               }).Distinct().Skip(recordNumber).Take(pageLimit).ToList();
                          }
                        else
                        {
                                users =  query
                                .Where(u => u.Licenses.ExpirationDate.CompareTo (DateTime.Now) < 0)
                                .Select(x => new UserDto
                                {
                                    Id = x.Id,
                                    FirstName = x.FirstName,
                                    LastName = x.LastName,
                                    DateOfBirth = x.DateOfBirth,
                                    JobPosition = x.JobPosition,
                                    Licenses = x.Licenses,
                                    LastLogin = x.LastLogin,
                                    ImageName = x.ImageName,
                                    TwilioPhoneNumber = x.TwilioPhoneNumber,
                                    Email = x.Email,
                                    PhoneNumber = x.PhoneNumber,
                                    Address = x.Address,
                                    CreatedDate = x.CreatedDate,
                                    EmailConfirmed = x.EmailConfirmed,
                                    IsEnabled = x.IsEnabled,
                                    IsDisabled = x.IsDisabled,
                                }).Distinct().Skip(recordNumber).Take(pageLimit).ToList();
                        }
                        
                        break;
                    case "Position":
                             users =  query
                              .Where(u =>u.JobPosition.ToLower().Contains(filterValue.ToLower()))
                              .Select(x => new UserDto
                              {
                                  Id = x.Id,
                                  FirstName = x.FirstName,
                                  LastName = x.LastName,
                                  DateOfBirth = x.DateOfBirth,
                                  JobPosition = x.JobPosition,
                                  Licenses = x.Licenses,
                                  LastLogin = x.LastLogin,
                                  ImageName = x.ImageName,
                                  TwilioPhoneNumber = x.TwilioPhoneNumber,
                                  Email = x.Email,
                                  PhoneNumber = x.PhoneNumber,
                                  Address = x.Address,
                                  CreatedDate = x.CreatedDate,
                                  EmailConfirmed = x.EmailConfirmed,
                                  IsEnabled = x.IsEnabled,
                                  IsDisabled = x.IsDisabled,
                              }).Distinct().Skip(recordNumber).Take(pageLimit).ToList();

                        break;
                }
                //foreach (UserDto user in users.ToList())
                //{
                //    if (user.ImageName != null)
                //    {
                //        user.ImageUri = _imageService.GetImageURI(user.ImageName).ToString();
                //    }

                //    userList.Add(user);
                //}
                userList = users.Select(user =>
                {
                    if (user.ImageName != null)
                    {
                        user.ImageUri = _imageService.GetImageURI(user.ImageName).ToString();
                    }
                    return user;
                }).ToList();
                return userList;
            }

            else
            {
                 users =  query
                .Where(u => (keywords == null || keywords == string.Empty) ||
                ((u.FirstName + " " + u.LastName).ToLower().Contains(keywords.ToLower())))
                .Select(x => new UserDto
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    DateOfBirth = x.DateOfBirth,
                    JobPosition = x.JobPosition,
                    Licenses = x.Licenses,
                    LastLogin = x.LastLogin,
                    ImageName = x.ImageName,
                    TwilioPhoneNumber = x.TwilioPhoneNumber,
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber,
                    Address = x.Address,
                    CreatedDate = x.CreatedDate,
                    EmailConfirmed = x.EmailConfirmed,
                    IsEnabled = x.IsEnabled,
                    IsDisabled = x.IsDisabled,
                    ImportEnabled = x.ImportEnabled
                }).Distinct().OrderBy(p => condition ? p.FirstName : null)
                .ThenByDescending(p => condition ? null : p.FirstName)
                .Skip(recordNumber >= 0 && pageLimit != 0 ? recordNumber : 0)
                .Take(recordNumber >= 0 && pageLimit != 0 ? pageLimit : int.MaxValue).ToList();
            }

            //foreach (UserDto user in users.ToList())
            //{
            //    if (user.ImageName != null)
            //    {
            //        user.ImageUri = _imageService.GetImageURI(user.ImageName).ToString();
            //    }

            //    userList.Add(user);
            //} 
            userList = users.Select(user =>
            {
                if (user.ImageName != null)
                {
                    user.ImageUri = _imageService.GetImageURI(user.ImageName).ToString();
                }
                return user;
            }).ToList();
            return userList;
        }

        public async Task<List<User>> GetAllUsers()
        {
            var users = await _applicationDbcontext.Users.Where(u=>u.IsEnabled && !u.IsDisabled).Include(u=>u.Address)
                .Include(u=>u.Licenses)
                .ToListAsync();

            List<User> userList = new List<User>();

            foreach (User user in users.ToList())
            {
                if (user.ImageName != null)
                {

                    user.ImageUri = _imageService.GetImageURI(user.ImageName).ToString();

                }
                userList.Add(user);

            }


            return userList;
        }

        public async Task<Response<User>> DeleteUserByUserId(User user)
        {
            Response<User> response = new Response<User>();
            bool val = user.IsDeleted;
            if (val)
            {
                response.Message = "User is already deleted";
                response.Success = false;
                return response;
            }
            user.IsDeleted = true;
            await _userManager.UpdateAsync(user);

            response.Message = "User deleted successfully";
            response.Success = true;
            return response;
        }

        public async Task DeleteUserByUserIdForPermanent(User user)
        {  
            _applicationDbcontext.Users.Remove(user);
            await _applicationDbcontext.SaveChangesAsync();
        }

        public async Task<Response<User>> ChangePasswordAsync(ChangePasswordModel model, string authorization)
        {
            Response<User> response = new Response<User>();
            string userId =  _jwtUtils.ValidateToken(authorization);

            var user = await GetUserById(userId);
            if (user == null)
            {
                response.Message = "No User Found";
                response.Success = false;
                return response;
            }

            if (await IsPreviousPassword(user.Id, model.NewPassword))
            {
                response.Message = "Cannot repeat password that was used within a year. Please specify a different password.";
                response.Success = false;
                return response;
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (!changePasswordResult.Succeeded)
            {
                response.Success = false;
                response.Message = "Failed to change password at this moment. Please try again.";
                return response;
            }

            await UpdatePasswordHistoryAsync(user.Id);

            response.Message = "Password Updated Successfully.";
            response.Success = true;

            return response;
        }

        private async Task<bool> IsPreviousPassword(string userId, string newPassword)
        {
            var user = await _userManager.Users
                    .Include(u => u.PasswordHistory)
                    .FirstOrDefaultAsync(u => u.Id == userId);

            if (user.PasswordHistory
                .Where(up => up.CreatedDateUTC >= DateTime.UtcNow.AddDays(-365) && 
                    _passwordHasher.VerifyHashedPassword(up.AppUser, up.HashPassword, newPassword) != PasswordVerificationResult.Failed)
                .Any())
            {
                return true;
            }

            return false;
        }


        public async Task<List<User>> getUserListByPatientId(int patientId)
        {
            var response = await _applicationDbcontext.PharmacyUsers.Where(m => m.Pharmacy.Patients.Any(Patient => Patient.Id == patientId)).Select(m => m.User).ToListAsync();
            return response;
        }

        public async Task<List<string>> GetAssignedTwilioNumbersAsync()
        {
            return await _applicationDbcontext.Users
                .Where(u => !u.IsDeleted && u.IsEnabled && u.TwilioPhoneNumber != null || u.TwilioPhoneNumber != "")
                .Select(u => u.TwilioPhoneNumber)
                .Distinct()
                .ToListAsync();
        }

        public async Task<User> GetUserByTwilioNumberAsync(string number)
        {
            if (string.IsNullOrWhiteSpace(number)) return null;

            return await _applicationDbcontext.Users
                .FirstOrDefaultAsync(u => !u.IsDeleted && u.IsEnabled && u.TwilioPhoneNumber == number);
        }

        public async Task<bool> SetVerificationCodeAsync(string userId, string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return false;

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) return false;

            user.VerificationCode = code;
            user.CodeExpiryDateUTC = DateTime.UtcNow.AddMinutes(10);

            return await _applicationDbcontext.SaveChangesAsync() > 0;
        }

        public async Task<bool> SetVerifiedDeviceAsync(string userId, string deviceId)
        {
            if (string.IsNullOrWhiteSpace(deviceId)) return false;

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) return false;

            user.LastDeviceId = deviceId;
            user.LastVerifiedDateUTC = DateTime.UtcNow.Date;
            user.VerificationCode = null;
            user.CodeExpiryDateUTC = null;

            return await _applicationDbcontext.SaveChangesAsync() > 0;
        }

        public async Task<bool> ToggleComplianceAsync(string userId, string complianceType)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(complianceType))
                return false;

            var user = await _applicationDbcontext.Users
                .Include(u => u.Compliance)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return false;

            user.Compliance ??= new UserCompliance();

            switch (complianceType)
            {
                case nameof(user.Compliance.BackgroundCheck):
                    user.Compliance.BackgroundCheck = !user.Compliance.BackgroundCheck;
                    break;

                case nameof(user.Compliance.LiabilityInsurance):
                    user.Compliance.LiabilityInsurance = !user.Compliance.LiabilityInsurance;
                    break;

                case nameof(user.Compliance.AnnualHIPPATraining):
                    user.Compliance.AnnualHIPPATraining = !user.Compliance.AnnualHIPPATraining;
                    user.Compliance.HippaTrainingRecordedOn = user.Compliance.AnnualHIPPATraining ? DateTime.Now : null;
                    break;

                case nameof(user.Compliance.AnnualFraudTraining):
                    user.Compliance.AnnualFraudTraining = !user.Compliance.AnnualFraudTraining;
                    user.Compliance.FraudTrainingRecordedOn = user.Compliance.AnnualFraudTraining ? DateTime.Now : null;
                    break;

                case nameof(user.Compliance.AnnualCyberTraining):
                    user.Compliance.AnnualCyberTraining = !user.Compliance.AnnualCyberTraining;
                    user.Compliance.CyberTrainingRecordedOn = user.Compliance.AnnualCyberTraining ? DateTime.Now : null;
                    break;
            }

            return await _applicationDbcontext.SaveChangesAsync() > 0;
        }

        public async Task<List<User>> GetAdminUsersByPharmacyId(int pharmacyId)
        {
            var query = _applicationDbcontext.Users
                .Where(u => u.PharmacyUsers.Any(p => p.PharmacyId == pharmacyId))
                .Join(_applicationDbcontext.UserRoles, p1 => p1.Id, p2 => p2.UserId, (p1, p2) => new { User = p1, UserRole = p2 })
                .Join(_applicationDbcontext.Roles.Where(r => r.Name == "Admin"), p1 => p1.UserRole.RoleId, p2 => p2.Id, (p1, p2) => p1.User)
                .Distinct();

            return await query.ToListAsync();
        }

        public async Task<List<User>> GetAdminUsersByUserId(string userId)
        {
            var query = _applicationDbcontext.Users
                .Where(u => u.PharmacyUsers.Any(p => _applicationDbcontext.PharmacyUsers.Where(pu => pu.UserId == userId).Any(x => x.PharmacyId == p.PharmacyId)))
                .Join(_applicationDbcontext.UserRoles, p1 => p1.Id, p2 => p2.UserId, (p1, p2) => new { User = p1, UserRole = p2 })
                .Join(_applicationDbcontext.Roles.Where(r => r.Name == "Admin"), p1 => p1.UserRole.RoleId, p2 => p2.Id, (p1, p2) => p1.User)
                .Distinct();

            var result = await query.ToListAsync();

            return result;
        }
    }
}
