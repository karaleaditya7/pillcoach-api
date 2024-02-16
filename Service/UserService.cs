using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OntrackDb.Assets;
using OntrackDb.Authentication;
using OntrackDb.Authorization;
using OntrackDb.Context;
using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Enums;
using OntrackDb.Filter;
using OntrackDb.Helper;
using OntrackDb.Hub;
using OntrackDb.Model;
using OntrackDb.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.WebPages;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace OntrackDb.Service
{
    public class UserService : IUserService
    {
        private readonly IUserData _userData;
        private readonly IJwtUtils _jwtUtils;
        private readonly IEmailService _emailService;
        private readonly UserManager<User> _userManager;
        private IConfiguration _configuration;
        private readonly IImageService _imageService;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IPharmacyService _pharmacyService;
        private readonly IPatientData _patientData;
        private readonly IPatientService _patientService;
        private readonly INotificationData _notificationData;
        private readonly ApplicationDbContext _applicationDbcontext;
        readonly IPatientPdcService _patientPdcService;
        readonly IMedicationService _medicationService;
        readonly IDoctorData _doctorData;
        readonly IPharmacyData _pharmacyData;
        readonly IMessageData _messageData;
        readonly IHubContext<ChatHub> _hubContext;
        readonly IAuditLogService _auditLogService;

        public UserService(IUserData userData, IMessageData messageData, IJwtUtils jwtUtils, IEmailService emailService, IPatientData patientData, UserManager<User> userManager, IConfiguration configuration, IImageService imageService, RoleManager<IdentityRole> roleManager, IPharmacyService pharmacyService, INotificationData notificationData, ApplicationDbContext applicationDbcontext, IPatientService patientService, IPatientPdcService patientPdcService, IMedicationService medicationService, IDoctorData doctorData, IPharmacyData pharmacyData, IHubContext<ChatHub> hubContext, IAuditLogService auditLogService)
        {
            _userData = userData;
            _jwtUtils = jwtUtils;
            _emailService = emailService;
            _userManager = userManager;
            _configuration = configuration;
            _imageService = imageService;
            _roleManager = roleManager;
            _pharmacyService = pharmacyService;
            _notificationData = notificationData;
            _applicationDbcontext = applicationDbcontext;
            _patientData = patientData;
            _patientService = patientService;
            _patientPdcService = patientPdcService;
            _medicationService = medicationService;
            _doctorData = doctorData;
            _pharmacyData = pharmacyData;
            _hubContext = hubContext;
            _auditLogService = auditLogService;
            _messageData = messageData;
        }

        public async Task<User> GetUserByUserName(string userName)
        {
            return await _userData.GetUserByUserName(userName);
        }

        public async Task<Response<User>> GetUsers(PaginationFilter filter)
        {
            Response<User> response = new Response<User>();
            var users = await _userData.GetUsers(filter);
            if (users == null)
            {
                response.Success = false;
                response.Message = "Users Not Found";
                return response;
            }
            response.Success = true;
            response.DataList = users;
            return response;
        }

        public async Task<Response<User>> SendInvite(InviteModel model)
        {
            Response<User> response = new Response<User>();

            if (model != null)
            {
                var Url = _configuration["SignupUrl"];
                string userName = model.UserName;
                string lastName = model.LastName;

                String htmlText = EmailTemplates.GetInviteUserTemplate(Url);

                await _emailService.SendEmailAsync(model.Email, "Invite Link", "Please click on below link to register app", htmlText);
                response.Success = true;
                response.Message = "Invite Email Send Successfully";
                return response;
            }
            response.Success = false;
            response.Message = "Something went wrong";
            return response;
        }

        public async Task<Response<User>> ForgotPassword(MailRequest model)
        {
            Response<User> response = new Response<User>();
            var user = await _userData.GetUserByEmail(model.Email);
            if (user != null)
            {
                var Url = _configuration["BaseUrl"];
                var code = await _userData.GeneratePasswordResetToken(user);
                Url += "?token=" + code.ToString() + "&&email=" + user.Email;

                String htmlText = EmailTemplates.GetForgotPasswordTemplate(Url);

                await _emailService.SendEmailAsync(user.Email, "Forgot Password", code.ToString(), htmlText);
                response.Success = true;
                response.Message = "Forgot Email Sent Successfully";
                return response;
            }
            response.Success = false;
            response.Message = "User Not Found";
            return response;
        }

        public async Task<Response<UserDto>> GetAllUsersForAdmin(int recordNumber, int pageLimit, string userId, string userRole, bool activeUser, DateTime startDate, DateTime endDate, int month, string keywords, string sortDirection, string filterType, string filterValue, string filterCategory)
        {
            Response<UserDto> response = new Response<UserDto>();
            List<UserDto> users = await _userData.GetAllUsersForAdmin(recordNumber, pageLimit, userId, userRole, activeUser, keywords, sortDirection, filterType, filterValue, filterCategory);

            List<UserDto> userListDto = new List<UserDto>();
            var pdcMonth = startDate.Date > endDate.Date ? startDate : endDate;
            var queryType = PdcQueryType.ByPdcMonth;

            if (month == 12)
            {
                var range = _patientPdcService.GetMedicationPeriodForGraph(month);
                pdcMonth = range.Item2;
                queryType = PdcQueryType.ByEndDate;
            }
            if (activeUser == false)
            {
                foreach (UserDto user in users)
                {
                    if (user.EmailConfirmed)
                    {

                        var cholesterolPDC = await _patientPdcService.GetPdcForUserAsync(user.Id, PDC.Cholesterol.ToString(), pdcMonth, month, queryType);
                        var diabetesPDC = await _patientPdcService.GetPdcForUserAsync(user.Id, PDC.Diabetes.ToString(), pdcMonth, month, queryType);
                        var rasaPDC = await _patientPdcService.GetPdcForUserAsync(user.Id, PDC.RASA.ToString(), pdcMonth, month, queryType);

                        user.CholestrolPDC = cholesterolPDC.Value;
                        user.DiabetesPDC = diabetesPDC.Value;
                        user.RASAPDC = rasaPDC.Value;
                        userListDto.Add(user);
                    }
                }

                response.Success = true;
                response.DataList = userListDto;
                return response;
            }
            else
            {
                foreach (UserDto user in users)
                {
                    var cholesterolPDC = await _patientPdcService.GetPdcForUserAsync(user.Id, PDC.Cholesterol.ToString(), pdcMonth, month, queryType);
                    var diabetesPDC = await _patientPdcService.GetPdcForUserAsync(user.Id, PDC.Diabetes.ToString(), pdcMonth, month, queryType);
                    var rasaPDC = await _patientPdcService.GetPdcForUserAsync(user.Id, PDC.RASA.ToString(), pdcMonth, month, queryType);

                    user.CholestrolPDC = cholesterolPDC.Value;
                    user.DiabetesPDC = diabetesPDC.Value;
                    user.RASAPDC = rasaPDC.Value;
                }
                response.Success = true;
                response.DataList = users;
                return response;
            }


        }

        public async Task<Response<UserPDCDto>> PDCCalculationForUser(string id, DateTime startDate, DateTime endDate, int month)
        {
            var response = new Response<UserPDCDto>();

            var user = await _userData.GetUserByIdForPDC(id);

            if (user == null)
            {
                response.Message = "User not found";
                return response;
            }

            // get PDC data

            var pdcMonth = startDate.Date > endDate.Date ? startDate : endDate;
            var queryType = PdcQueryType.ByPdcMonth;

            if (month == 12)
            {
                var range = _patientPdcService.GetMedicationPeriodForGraph(month);
                pdcMonth = range.Item2;
                queryType = PdcQueryType.ByEndDate;
            }

            var cholesterolPDC = await _patientPdcService.GetPdcForUserAsync(user.Id, PDC.Cholesterol.ToString(), pdcMonth, month, queryType);
            var diabetesPDC = await _patientPdcService.GetPdcForUserAsync(user.Id, PDC.Diabetes.ToString(), pdcMonth, month, queryType);
            var rasaPDC = await _patientPdcService.GetPdcForUserAsync(user.Id, PDC.RASA.ToString(), pdcMonth, month, queryType);

            user.CholestrolPDC = cholesterolPDC.Value;
            user.DiabetesPDC = diabetesPDC.Value;
            user.RASAPDC = rasaPDC.Value;

            user.CholesterolSummary = new()
            {
                PDC = cholesterolPDC.Value,
                TotalPatients = cholesterolPDC.TotalPatients,
                NonAdherenceCount = cholesterolPDC.NonAdherenceCount
            };

            user.DiabetesSummary = new()
            {
                PDC = diabetesPDC.Value,
                TotalPatients = diabetesPDC.TotalPatients,
                NonAdherenceCount = diabetesPDC.NonAdherenceCount
            };

            user.RASASummary = new()
            {
                PDC = rasaPDC.Value,
                TotalPatients = rasaPDC.TotalPatients,
                NonAdherenceCount = rasaPDC.NonAdherenceCount
            };

            response.Success = true;
            response.Message = "User Pdc Retrived Successfully";
            response.Data = user;
            return response;
        }

        public async Task<Response<User>> GetAllUsers()
        {

            Response<User> response = new Response<User>();
            List<User> users = await _userData.GetAllUsers();


            response.Success = true;
            response.DataList = users;
            return response;
        }

        public async Task<Response<User>> GetAllUsersForLicenseExpiry()
        {
            Response<User> response = new Response<User>();
            List<User> users = await _userData.GetAllUsers();

            List<User> userList = new List<User>();
            foreach (User user in users)
            {
                int days = (user.Licenses.ExpirationDate.Date - DateTime.Today.Date).Days;

                if (!user.IsDeleted && days <= 60)
                {
                    userList.Add(user);
                }
            }

            response.Success = true;
            response.DataList = userList;
            return response;
        }

        public async Task<Response<User>> ResetPassword(ResetPasswordModel model)
        {
            Response<User> response = new Response<User>();

            if (!model.Password.Equals(model.ConfirmPassword))
            {
                response.Message = "Confirmation password does not match new password.";
                response.Success = false;
                return response;
            }

            var user = await _userData.GetUserByEmail(model.Email);

            if (user != null)
            {
                return await _userData.ResetPasswordAsync(user, model.Token, model.Password);
            }

            response.Success = false;
            response.Message = "User Not Found";
            return response;
        }

        public async Task<Response<User>> Register(RegisterModel model)
        {

            Response<User> response = new Response<User>();
            response.Success = false;
            if (string.IsNullOrEmpty(model.Username))
            {
                response.Message = "Username Missing";
                return response;
            }
            var userExists = await _userData.GetUserByEmail(model.Email);
            if (userExists != null)
            {
                if (userExists.IsDeleted == true)
                {
                    response.Message = "This user has been deleted, please contact with administrator";
                }
                else
                {
                    response.Message = "Email Id Already Exists";
                }
                return response;
            }
            if (string.IsNullOrEmpty(model.FirstName))
            {
                response.Message = "Firstname Missing";
                return response;
            }
            if (string.IsNullOrEmpty(model.LastName))
            {
                response.Message = "LastName Missing";
                return response;
            }
            if (string.IsNullOrEmpty(model.Email))
            {
                response.Message = "Email Missing";
                return response;
            }
            if (string.IsNullOrEmpty(model.PhoneNumber))
            {
                response.Message = "PhoneNumber Missing";
                return response;
            }
            if (string.IsNullOrEmpty(model.Password))
            {
                response.Message = "Password Missing";
                return response;
            }

            if (string.IsNullOrWhiteSpace(model.NpiNumber))
            {
                response.Message = "NPI Number Missing";
                return response;
            }

            var isValidatePassword = IsValidatePassword(model.Password);
            if (!isValidatePassword)
            {
                response.Message = "Invalid Password";
                return response;
            }
            var isValidEmail = IsValidEmail(model.Email);
            if (!isValidEmail)
            {
                response.Message = "Invalid Email";
                return response;
            }

            var initialPharmacy = await _pharmacyService.GetPharmacyByNpiNumber(model.NpiNumber);

            if (initialPharmacy == null)
            {
                response.Message = "Invalid NPI Number";
                return response;
            }

            if (!string.IsNullOrWhiteSpace(model.TwilioPhoneNumber) && !model.TwilioPhoneNumber.StartsWith("+1"))
            {
                model.TwilioPhoneNumber = $"+1{model.TwilioPhoneNumber}";
            }

            User user = new User()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Licenses = model.Licenses,
                DateOfBirth = model.DateOfBirth,
                JobPosition = model.JobPosition,
                Address = model.Address,
                PhoneNumber = model.PhoneNumber,
                TwilioPhoneNumber = model.TwilioPhoneNumber,
                TwilioNumberAssignedOnUtc = !string.IsNullOrWhiteSpace(model.TwilioPhoneNumber) ? DateTime.UtcNow : null,
                CreatedDate = DateTime.Now
            };

            Response<User> user1 = await _userData.Register(user, model.Password);

            if (user1.Success && initialPharmacy != null)
            {
                _ = await AssignPharmacy(user1.Data.Id, new List<string>() { initialPharmacy.Id.ToString() });
            }

            return user1;
        }

        public async Task<Response<User>> EditProfile(RegisterModel model, string authorization)
        {

            var parameter = "";
            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                parameter = headerValue.Parameter;
            }

            string UserId = _jwtUtils.ValidateToken(parameter);
            var userRole = await _userData.GetUserById(UserId);

            Response<User> response = new Response<User>();
            response.Success = false;

            var user = await _userData.GetUserById(model.Id);

            if (user == null)
            {
                response.Message = "User not found";
                return response;
            }

            if (user.Email != model.Email)
            {
                var result = await _userData.GetUserByEmail(model.Email);
                if (result != null)
                {
                    response.Message = "This EmailId already Exists";
                    return response;
                }
            }

            if (!string.IsNullOrWhiteSpace(model.TwilioPhoneNumber) && !model.TwilioPhoneNumber.StartsWith("+1"))
            {
                model.TwilioPhoneNumber = $"+1{model.TwilioPhoneNumber}";
            }

            if (!string.IsNullOrWhiteSpace(model.TwilioPhoneNumber) && model.TwilioPhoneNumber != user.TwilioPhoneNumber)
            {
                var otherUser = await _userData.GetUserByTwilioNumberAsync(model.TwilioPhoneNumber);

                if (otherUser != null && otherUser.Id != user.Id)
                {
                    response.Message = "Twilio Number is already in use";
                    return response;
                }
            }

            if (string.IsNullOrEmpty(model.FirstName))
            {
                response.Message = "Firstname Missing";
                return response;
            }
            if (string.IsNullOrEmpty(model.LastName))
            {
                response.Message = "LastName Missing";
                return response;
            }
            if (string.IsNullOrEmpty(model.Email))
            {
                response.Message = "Email Missing";
                return response;
            }
            if (string.IsNullOrEmpty(model.PhoneNumber))
            {
                response.Message = "PhoneNumber Missing";
                return response;
            }

            if (model.Licenses == null)
            {
                response.Message = "Licenses Info Missing";
                return response;
            }
            if (string.IsNullOrEmpty(model.Licenses.Number))
            {
                response.Message = "Licenses Number Missing";
                return response;
            }
            if (string.IsNullOrEmpty(model.JobPosition))
            {
                response.Message = "JobPosition is Missing";
                return response;
            }

            var isValidEmail = IsValidEmail(model.Email);
            if (!isValidEmail)
            {
                response.Message = "Invalid Email";
                return response;
            }

            if (model.Address != null)
            {
                if (user.Address != null)
                {
                    var address = await _userData.GetAddressById(user.Address.Id);
                    address.AddressLineOne = model.Address.AddressLineOne;
                    address.AddressLineTwo = model.Address.AddressLineTwo;
                    address.State = model.Address.State;
                    address.City = model.Address.City;
                    address.ZipCode = model.Address.ZipCode;
                    user.Address = address;
                }
                else
                {
                    user.Address = model.Address;
                }


            }

            if (await _userManager.IsInRoleAsync(userRole, Roles.Admin.ToString()) || await _userManager.IsInRoleAsync(userRole, Roles.SuperAdmin.ToString()))
            {
                if (model.Licenses != null)
                {
                    var licenses = await _userData.GetLicenseById(user.Licenses.Id);
                    licenses.Number = model.Licenses.Number;
                    licenses.ExpirationDate = model.Licenses.ExpirationDate;
                    user.Licenses = licenses;
                }

                // before adding user to the new role, remove user from existing role(s) - it will be updated based on selected job position
                // so that there is only one role associated to a user

                if (model.JobPosition == "Admin" && !await _userManager.IsInRoleAsync(user, Roles.Admin.ToString()))
                {
                    if (await _userManager.IsInRoleAsync(user, Roles.Employee.ToString()))
                        await _userManager.RemoveFromRoleAsync(user, Roles.Employee.ToString());

                    if (await _userManager.IsInRoleAsync(user, Roles.SuperAdmin.ToString()))
                        await _userManager.RemoveFromRoleAsync(user, Roles.SuperAdmin.ToString());

                    await _userManager.AddToRoleAsync(user, Roles.Admin.ToString());
                }
                else if (model.JobPosition == "SuperAdmin" && !await _userManager.IsInRoleAsync(user, Roles.SuperAdmin.ToString()))
                {
                    if (await _userManager.IsInRoleAsync(user, Roles.Admin.ToString()))
                        await _userManager.RemoveFromRoleAsync(user, Roles.Admin.ToString());

                    if (await _userManager.IsInRoleAsync(user, Roles.Employee.ToString()))
                        await _userManager.RemoveFromRoleAsync(user, Roles.Employee.ToString());

                    await _userManager.AddToRoleAsync(user, Roles.SuperAdmin.ToString());
                }
                else if ((model.JobPosition == "Pharmacist" || model.JobPosition == "Pharmacy Technician") && !await _userManager.IsInRoleAsync(user, Roles.Employee.ToString()))
                {
                    if (await _userManager.IsInRoleAsync(user, Roles.Admin.ToString()))
                        await _userManager.RemoveFromRoleAsync(user, Roles.Admin.ToString());

                    if (await _userManager.IsInRoleAsync(user, Roles.SuperAdmin.ToString()))
                        await _userManager.RemoveFromRoleAsync(user, Roles.SuperAdmin.ToString());

                    await _userManager.AddToRoleAsync(user, Roles.Employee.ToString());
                }

                if (user.IsEnabled == false && model.IsEnabled == true)
                {
                    user.IsEnabled = model.IsEnabled;

                    var Url = _configuration["EmailConfirmationUrl"];

                    String htmlText = EmailTemplates.GetEmailApprovedTemplate(Url);
                    await _emailService.SendEmailAsync(user.Email, "Access Request", null, htmlText);
                }

                if(user.IsEnabled && !model.IsEnabled)
                {
                    user.IsEnabled = model.IsEnabled;
                }

                user.IsDisabled = model.IsDisabled;
            }
            else
            {
                if (model.Licenses != null)

                {
                    var licenses = await _userData.GetLicenseById(user.Licenses.Id);
                    licenses.IssueState = model.Licenses.IssueState;
                    licenses.Number = model.Licenses.Number;
                    user.Licenses = licenses;
                }
            }
            int days = (user.Licenses.ExpirationDate.Date - DateTime.Today.Date).Days;
            if (days <= 60)
            {

                AdminNotification notifi = await _notificationData.GetAdminNotificationByUserId(user.Id);
                if (notifi == null)
                {
                    AdminNotification notification = new AdminNotification
                    {
                        Status = "License Expired",
                        NotificationType = "License Expiry",
                        SendDateTime = DateTime.Now,
                        User = user
                    };
                    await _notificationData.AddAdminNotification(notification);

                }
                else
                {
                    notifi.IsDeleted = false;
                    notifi.SendDateTime = DateTime.Now;
                    notifi.ReadDateTime = new DateTime(0001, 01, 01);
                    await _notificationData.UpdateAdminNotification(notifi);
                }


                Notification notifi1 = await _notificationData.GetNotificationByUserId(user.Id);
                if (notifi1 == null)
                {
                    Notification notification = new Notification
                    {
                        Status = "License Expired",
                        NotificationType = "License Expiry",
                        SendDateTime = DateTime.Now,
                        User = user
                    };
                    await _notificationData.AddNotification(notification);

                }
                else
                {
                    notifi1.IsDeleted = false;
                    notifi1.SendDateTime = DateTime.Now;
                    notifi1.ReadDateTime = new DateTime(0001, 01, 01);
                    await _notificationData.UpdateNotification(notifi1);
                }


            }
            else
            {
                AdminNotification notifi = await _notificationData.GetAdminNotificationByUserId(user.Id);
                if (notifi != null)
                {
                    notifi.IsDeleted = true;
                    await _notificationData.UpdateAdminNotification(notifi);
                }
                Notification notifi1 = await _notificationData.GetNotificationByUserId(user.Id);
                if (notifi1 != null)
                {
                    notifi1.IsDeleted = true;
                    await _notificationData.UpdateNotification(notifi1);
                }
            }
            user.Email = model.Email;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            if ((user.DateOfBirth == model.DateOfBirth) || (model.DateOfBirth == new DateTime(0001, 01, 01)))
            {
                user.DateOfBirth = user.DateOfBirth;
            }
            else
            {
                user.DateOfBirth = model.DateOfBirth;
            }
            user.JobPosition = model.JobPosition;
            user.PhoneNumber = model.PhoneNumber;
            user.ImportEnabled = model.ImportEnabled;

            var oldTwilioNumber = user.TwilioPhoneNumber ?? string.Empty;
            var newTwilioNumber = model.TwilioPhoneNumber ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(oldTwilioNumber) && !oldTwilioNumber.Equals(newTwilioNumber))
            {
                await UpdateTwilioFriendlyNameAsync(oldTwilioNumber, null);
                user.TwilioNumberAssignedOnUtc = null;
            }

            if (!oldTwilioNumber.Equals(newTwilioNumber))
            {
                user.TwilioPhoneNumber = newTwilioNumber;
                user.TwilioNumberAssignedOnUtc = DateTime.UtcNow;

                await UpdateTwilioFriendlyNameAsync(user.TwilioPhoneNumber, user.Id);
            }

            return await _userData.UpdateUser(user);
        }

        public async Task<Response<User>> EditProfileAdmin(RegisterModel model)
        {

            Response<User> response = new Response<User>();
            response.Success = false;

            var user = await _userData.GetUser(model.Username);
            if (user == null)
            {
                response.Message = "User not found";
                return response;
            }
            if (string.IsNullOrEmpty(model.FirstName))
            {
                response.Message = "Firstname Missing";
                return response;
            }
            if (string.IsNullOrEmpty(model.LastName))
            {
                response.Message = "LastName Missing";
                return response;
            }
            if (string.IsNullOrEmpty(model.Email))
            {
                response.Message = "Email Missing";
                return response;
            }
            if (string.IsNullOrEmpty(model.PhoneNumber))
            {
                response.Message = "PhoneNumber Missing";
                return response;
            }

            if (model.Licenses == null)
            {
                response.Message = "Licenses Info Missing";
                return response;
            }
            if (string.IsNullOrEmpty(model.Licenses.Number))
            {
                response.Message = "Licenses Number Missing";
                return response;
            }
            if (string.IsNullOrEmpty(model.JobPosition))
            {
                response.Message = "JobPosition is Missing";
                return response;
            }

            var isValidEmail = IsValidEmail(model.Email);
            if (!isValidEmail)
            {
                response.Message = "Invalid Email";
                return response;
            }

            if (model.Address != null)
            {
                var address = await _userData.GetAddressById(user.Address.Id);
                address.AddressLineOne = model.Address.AddressLineOne;
                address.AddressLineTwo = model.Address.AddressLineTwo;
                address.State = model.Address.State;
                address.City = model.Address.City;
                address.ZipCode = model.Address.ZipCode;
                user.Address = address;
            }

            if (model.Licenses != null)
            {
                var licenses = await _userData.GetLicenseById(user.Licenses.Id);
                licenses.IssueState = model.Licenses.IssueState;
                licenses.Number = model.Licenses.Number;
                licenses.ExpirationDate = model.Licenses.ExpirationDate;
                user.Licenses = licenses;
            }

            user.Email = model.Email;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;

            user.DateOfBirth = model.DateOfBirth;
            user.JobPosition = model.JobPosition;
            user.PhoneNumber = model.PhoneNumber;

            if (user.JobPosition == "Admin")
            {
                await _userManager.AddToRoleAsync(user, Roles.Admin.ToString());
            }
            else
            {
                await _userManager.AddToRoleAsync(user, Roles.Employee.ToString());
            }

            return await _userData.UpdateUser(user);
        }

        public async Task<Response<User>> Login(LoginModel model)
        {
            Response<User> response = new();
            var user = (User)await _userData.GetUserByEmail(model.Email);

            if (user == null)
            {
                response.Success = false;
                response.Message = "Invalid Details";
                return response;
            }

            if (user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.Now)
            {
                response.Success = false;
                response.Message = "Your Account has been locked. Please use the Forgot Password link to reset your password.";
                return response;
            }

            if (user.IsEnabled == false)
            {
                response.Success = false;
                response.Message = "Your account has not been approved. Please contact Admin.";
                return response;
            }

            if (user.IsDisabled == true)
            {
                response.Success = false;
                response.Message = "Your account has not been approved. Please contact Admin.";
                return response;
            }

            if (user.IsDeleted == true)
            {
                response.Success = false;
                response.Message = "Your account has been deleted. Please contact your system administrator.";
                return response;
            }

            if (!await _userData.GetUserByUserAndPassword(user, model.Password))
            {
                try
                {
                    // add audit log
                    await _auditLogService.AddAuditLogAsync(new AuditLogModel
                    {
                        ActionSourceId = (int)Enums.AuditActionSourceType.AccountLogin,
                        ActionTypeId = (int)Enums.AuditActionType.LoginFail
                    }, user.Id);
                }
                catch { }

                if (user.FailedLoginCount < 4)
                {
                    user.FailedLoginCount++;

                    response.Success = false;
                    response.Message = $"Invalid Details. Account will be locked after another {4 - user.FailedLoginCount} failed attempt.";

                    if (user.FailedLoginCount == 4)
                    {
                        user.LastDeviceId = null; // so that 2FA is required on next valid login
                        user.LockoutEnd = DateTimeOffset.MaxValue;

                        response.Message = "Your Account has been locked. Please use the Forgot Password link to reset your password.";
                    }

                    await _userData.UpdateUser(user);

                    return response;
                }
                else
                {
                    response.Success = false;
                    response.Message = "Your Account has been locked. Please use the Forgot Password link to reset your password.";
                    return response;
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(model.DeviceId) // invalid/missing device id
                    || !model.DeviceId.Equals(user.LastDeviceId, StringComparison.OrdinalIgnoreCase) // different device
                    || user.LastVerifiedDateUTC == null // not verified at all
                    || user.LastVerifiedDateUTC.Value.AddDays(30) < DateTime.UtcNow.Date) // passed 30 days since last verification
                {
                    if (await SendVerificationCode(user))
                    {
                        response.Message = "EmailVerification";
                    }
                    else
                    {
                        response.Message = "Unable to complete login at this point. Please try again.";
                    }

                    return response;
                }
                else if (user.PasswordSetDateUTC == null || user.PasswordSetDateUTC.Value.AddDays(90) < DateTime.UtcNow)
                {
                    response.Message = "Password reset required. Please use the Forgot Password link to reset your password.";
                    response.Success = false;
                    return response;
                }
                else
                {
                    User UpdatedUser = await _userData.UpdateLastLoginForUser(user, model.DeviceId);
                    UpdatedUser.ImageUri = _imageService.GetImageURI(UpdatedUser.ImageName);
                    UpdatedUser.RoleList = await _userManager.GetRolesAsync(user);

                    response.Success = true;
                    response.Message = "USER LOGGED IN SUCCESSFULLY";
                    response.AccessToken = await _jwtUtils.GenerateToken(model);
                    response.Data = user;

                    return response;
                }
            }
        }

        public async Task<Response<User>> VerifyCodeAsync(string userEmail, string code, string deviceId)
        {
            var response = new Response<User>();

            if (string.IsNullOrWhiteSpace(code))
            {
                response.Message = "Invalid verification code";
                return response;
            }

            if (string.IsNullOrWhiteSpace(deviceId))
            {
                response.Message = "Invalid Device Id";
                return response;
            }

            var user = await _userData.GetUserByEmail(userEmail);

            if (user != null)
            {
                if ((user.Email == _configuration.GetValue<string>("Master:User1")) || (user.Email == _configuration.GetValue<string>("Master:User2")) || (user.Email == _configuration.GetValue<string>("Master:User3")))
                {
                    user.IsMasterUser = true;
                }
                if (user.CodeExpiryDateUTC < DateTime.UtcNow)
                {
                    response.Message = "Verification code expired.";
                }
                else if (!code.Equals(user.VerificationCode))
                {
                    response.Message = "Invalid Verification code.";
                }
                else
                {
                    User UpdatedUser = await _userData.UpdateLastLoginForUser(user, deviceId);
                    UpdatedUser.ImageUri = _imageService.GetImageURI(UpdatedUser.ImageName);
                    UpdatedUser.RoleList = await _userManager.GetRolesAsync(user);
                    response.Success = true;
                    response.Message = "USER LOGGED IN SUCCESSFULLY";
                    response.AccessToken = await _jwtUtils.GenerateToken(new LoginModel { Email = user.Email });
                    response.Data = user;
                    return response;
                }
            }
            else
            {
                response.Message = "User not found";
            }

            return response;
        }

        public async Task<Response<bool>> ResendVerificationCodeAsync(string userEmail)
        {
            var response = new Response<bool>();

            var user = await _userData.GetUserByEmail(userEmail);

            if (user == null)
            {
                response.Message = "Invalid user";
                return response;
            }

            if (await SendVerificationCode(user))
            {
                response.Success = true;
                response.Message = "Verification code sent to registered email address.";
            }
            else
            {
                response.Message = "Unable to send verification code. Please try again.";
            }

            return response;
        }

        private async Task<bool> SendVerificationCode(User user)
        {
            if (!string.IsNullOrWhiteSpace(user.VerificationCode) && user.CodeExpiryDateUTC >= DateTime.UtcNow)
            {
                return true; // code already generated and has not expired
            }

            var verificationCode = new Random().Next(111111, 999999).ToString();

            if (await _userData.SetVerificationCodeAsync(user.Id, verificationCode))
            {
                var htmlContent = EmailTemplates.Get2FACodeTemplate(verificationCode);

                if (!string.IsNullOrWhiteSpace(htmlContent))
                {
                    await _emailService.SendEmailAsync(user.Email, "PillCoach Verification Code", null, htmlContent);

                    return true;
                }
            }

            return false;
        }

        public async Task<Response<User>> ResendEmail(string email)
        {
            Response<User> response = new Response<User>();
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                response.Success = false;
                response.Message = "Invalid Email.";
                return response;
            }
            if (user.EmailConfirmed)
            {
                response.Success = false;
                response.Message = "Email is already verified.";
                return response;
            }
            var Url = _configuration["EmailConfirmationUrl"];
            var code = await this._userManager.GenerateEmailConfirmationTokenAsync(user);
            code = System.Web.HttpUtility.UrlEncode(code);
            Url += "/verifyUser" + "?token=" + code.ToString() + "&email=" + user.Email;
            String htmlText = EmailTemplates.GetEmailConfirmationTemplate(Url);
            await _emailService.SendEmailAsync(user.Email, "Confirm your email address", code.ToString(), htmlText);

            response.Success = true;
            response.Message = "Confirmation link has sent to your email.";
            return response;
        }

        public async Task<Response<User>> DeleteUserByUserId(String Id)
        {
            Response<User> response = new Response<User>();
            User user = await _userData.GetUserByUserId(Id);

            if (user == null)
            {
                response.Message = "Invalid Id";
                response.Success = false;
                return response;
            }
            else
            {
                //response = await _userData.DeleteUserByUserId(user);
                response = await this.DeleteUserById(user.Id);
                response.Success = true;
                if (response.Success && user.TwilioPhoneNumber != null)
                {
                    await UpdateTwilioFriendlyNameAsync(user.TwilioPhoneNumber, null);
                }

                return response;
            }
        }

        public async Task<Response<User>> DeleteUserById(String Id)
        {
            Response<User> response = new Response<User>();
            User user = await _userData.GetUserByUserId(Id);

            if (user == null)
            {
                response.Message = "Invalid Id";
                response.Success = false;
                return response;
            }
            else
            {
                var adminNotifications = await _userData.GetAdminNotificationByUserIdForDelete(user.Id);
                var notifications = await _notificationData.GetNotificationByUserIdForDelete(user.Id);
                var messages = await _messageData.GetAllMessagesForUser(user.Id);

                if (messages != null)
                {
                    _messageData.DeleteAllMessageOfUser(messages);
                }
                if (adminNotifications != null)
                {
                    _notificationData.DeleteNotificationForAdminNotificationById(adminNotifications);
                }
                if (notifications != null)
                {
                    _notificationData.DeleteNotificationForUser(notifications);
                }

                if (user.IsEnabled == false)
                {
                    var Url = _configuration["Contact_Us_Url"];

                    String htmlText = EmailTemplates.GetEmailDeclinedTemplate(Url);
                    await _emailService.SendEmailAsync(user.Email, "Access Request", null, htmlText);
                }
                await _userData.DeleteUserByUserIdForPermanent(user);
                response.Success = true;
                response.Message = "User Deleted Successfully!";

                //await UpdateTwilioFriendlyNameAsync(user.TwilioPhoneNumber, null);

                return response;
            }
        }

        public List<Patient> DashboardSearchPatient(string search)
        {

            var query = _applicationDbcontext.Patients.Where(p => p.IsDeleted == false).Include(p => p.Contact).ToList();
            List<Patient> patientList = new List<Patient>();



            if (!string.IsNullOrEmpty(search) && (DateTime.TryParse(search, out var dateTime)) == true)
            {

                query = query.Where(e => e.Contact.DoB.Date.ToShortDateString() == dateTime.Date.ToShortDateString()).ToList();
                return patientList;
            }
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(e => e.Contact.FirstName.ToLower().Contains(search.ToLower())
                 || e.Contact.LastName.ToLower().Contains(search.ToLower()) || e.Contact.PrimaryPhone.ToLower().Contains(search.ToLower())).ToList();
                return query;
            }

            return null;


        }

        public List<Pharmacy> DashboardSearchPharamcy(string searchText)
        {

            var query = _applicationDbcontext.Pharmacies.Where(p => p.IsDeleted == false).ToList();
            List<Pharmacy> pharmacyList = new List<Pharmacy>();

            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(e => e.Name.ToLower().Contains(searchText.ToLower()))
                 .ToList();
                return query;
            }
            return null;
        }

        public async Task<Response<User>> GetSpecificUser(string authorizatioin)
        {
            DateTime endDate = DateTime.Now;
            DateTime startDate = UtilityHelper.GetStartDateByMonth(endDate, 6);
            Response<User> response = new Response<User>();
            var parameter = "";
            if (AuthenticationHeaderValue.TryParse(authorizatioin, out var headerValue))
            {
                parameter = headerValue.Parameter;
            }
            string userId = _jwtUtils.ValidateToken(parameter);
            User user = await _userData.GetUserById(userId);
            user.ImageUri = _imageService.GetImageURI(user.ImageName);

            Response<Pharmacy> pharmacies = await _pharmacyService.GetPharmaciesByUserID(userId);
            List<Pharmacy> pharmacyList = new List<Pharmacy>();
            Double CholestorolSum = 0;
            Double DiabetesSum = 0;
            Double RASASum = 0;
            foreach (Pharmacy pharmacy in pharmacies.DataList)
            {

                CholestorolSum = CholestorolSum + pharmacy.CholestrolPDC;
                DiabetesSum = DiabetesSum + pharmacy.DiabetesPDC;
                RASASum = RASASum + pharmacy.RASAPDC;
            }
            CholestorolSum = Convert.ToDouble(String.Format("{0:0.0}", CholestorolSum));
            DiabetesSum = Convert.ToDouble(String.Format("{0:0.0}", DiabetesSum));
            RASASum = Convert.ToDouble(String.Format("{0:0.0}", RASASum));

            user.CholestrolPDC = CholestorolSum / pharmacies.DataList.Count;
            user.DiabetesPDC = DiabetesSum / pharmacies.DataList.Count;
            user.RASAPDC = RASASum / pharmacies.DataList.Count;

            var patients = await _patientService.GetPatientsByUserIdForEmployee(userId, startDate, endDate, 6, calculatePDCs: false);

            var newPatientsCount = _patientService.countNewPatient(patients.DataList);
            var inProgressCount = _patientService.countInProgressPatient(patients.DataList);

            var refillDueCount = patients.DataList.Count(p => (p.CholesterolRefillDue || p.DiabetesRefillDue || p.RasaRefillDue)
                && p.Medications.Any(m => m.IsActive && m.RefillDue && new string[] { "Diabetes", "Cholesterol", "RASA" }.Contains(m.Condition) && Convert.ToInt32(m.RefillsRemaining) > 0));

            var noRefillCount = patients.DataList.Count(p => (p.CholesterolRefillDue || p.DiabetesRefillDue || p.RasaRefillDue)
                && !p.Medications.Any(m => m.IsActive && m.RefillDue && new string[] { "Diabetes", "Cholesterol", "RASA" }.Contains(m.Condition) && Convert.ToInt32(m.RefillsRemaining) > 0));

            user.NewPatient = newPatientsCount;
            user.PatientInProgress = inProgressCount;
            user.DueForRefill = refillDueCount;
            user.NoRefillRemaining = noRefillCount;

            if (user == null)
            {
                response.Message = "Invalid token";
                response.Success = false;
                return response;
            }
            else
            {
                response.Success = true;
                response.Message = "User retrived successfully";
                response.Data = user;
                return response;
            }
        }

        public async Task<Response<User>> GetUserByIdWithPdcCalculation(string userId, DateTime startDate, DateTime endDate)
        {
            Response<User> response = new Response<User>();

            User user = await _userData.GetUserById(userId);
            user.ImageUri = _imageService.GetImageURI(user.ImageName);

            Response<Pharmacy> pharmacies = await _pharmacyService.GetPharmaciesByUserID(userId);
            List<Pharmacy> pharmacyList = new List<Pharmacy>();
            Double CholestorolSum = 0;
            Double DiabetesSum = 0;
            Double RASASum = 0;
            foreach (Pharmacy pharmacy in pharmacies.DataList)
            {

                CholestorolSum = CholestorolSum + pharmacy.CholestrolPDC;
                DiabetesSum = DiabetesSum + pharmacy.DiabetesPDC;
                RASASum = RASASum + pharmacy.RASAPDC;
            }
            CholestorolSum = Convert.ToDouble(String.Format("{0:0.0}", CholestorolSum));
            DiabetesSum = Convert.ToDouble(String.Format("{0:0.0}", DiabetesSum));
            RASASum = Convert.ToDouble(String.Format("{0:0.0}", RASASum));

            user.CholestrolPDC = CholestorolSum / pharmacies.DataList.Count;
            user.DiabetesPDC = DiabetesSum / pharmacies.DataList.Count;
            user.RASAPDC = RASASum / pharmacies.DataList.Count;

            if (user == null)
            {
                response.Message = "Invalid token";
                response.Success = false;
                return response;
            }
            else
            {
                response.Success = true;
                response.Message = "User retrived successfully";
                response.Data = user;
                return response;
            }
        }

        private Boolean IsValidatePassword(String password)
        {
            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMinimum8Chars = new Regex(@".{8,}");

            return hasNumber.IsMatch(password) && hasUpperChar.IsMatch(password) && hasMinimum8Chars.IsMatch(password);
        }

        public bool IsValidEmail(string emailaddress)
        {
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(emailaddress);
            if (match.Success)
                return true;
            else
                return false;
        }

        public async Task<Response<User>> ChangePasswordAsync(ChangePasswordModel model, string authorization)
        {
            Response<User> response = new Response<User>();

            if (model.CurrentPassword.Equals(model.NewPassword))
            {
                response.Message = "New password cannot be same as current password.";
                response.Success = false;
                return response;
            }
            else if (!model.NewPassword.Equals(model.ConfirmPassword))
            {
                response.Message = "Confirmation password does not match new password.";
                response.Success = false;
                return response;
            }

            var parameter = "";

            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                parameter = headerValue.Parameter;
            }

            return await _userData.ChangePasswordAsync(model, parameter);
        }

        public async Task<Response<User>> UpdateUserImageName(string imageName, string authorization)
        {
            Response<User> response = new Response<User>();
            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                var parameter = headerValue.Parameter;
                string userId = _jwtUtils.ValidateToken(parameter);
                User user = await _userData.GetUserById(userId);
                user.ImageName = imageName;
                var imageUri = _imageService.GetImageURI(imageName);
                Response<User> result = await _userData.UpdateUser(user);
                result.Data.ImageUri = imageUri;
                response = result;
                return response;
            }
            response.Success = false;
            response.Message = "Error Occur during uploading image";
            return response;
        }

        public async Task<Response<User>> UpdateUserForNotification(string authorizatioin, Boolean isNotification)
        {

            var parameter = "";
            if (AuthenticationHeaderValue.TryParse(authorizatioin, out var headerValue))
            {
                parameter = headerValue.Parameter;
            }
            string userId = _jwtUtils.ValidateToken(parameter);


            Response<User> response = new Response<User>();


            var result = await this.GetUserById(userId);

            result.Data.IsNotification = isNotification;

            await _userData.UpdateUser(result.Data);


            if (result == null)
            {
                response.Success = false;
                response.Message = "User Not Found";
                return response;
            }
            response.Success = true;
            response.Data = result.Data;
            response.Message = "User Updated successfully!";
            return response;
        }

        public async Task<Response<User>> AddUser(RegisterModel model)
        {
            Response<User> response = new Response<User>();
            response.Success = false;
            if (string.IsNullOrEmpty(model.Username))
            {
                response.Message = "Username Missing";
                return response;
            }
            var userExists = await _userData.GetUserByEmail(model.Email);
            if (userExists != null)
            {
                response.Message = "Email Already Exists";
                return response;
            }
            if (string.IsNullOrEmpty(model.FirstName))
            {
                response.Message = "Firstname Missing";
                return response;
            }
            if (string.IsNullOrEmpty(model.LastName))
            {
                response.Message = "LastName Missing";
                return response;
            }
            if (string.IsNullOrEmpty(model.Email))
            {
                response.Message = "Email Missing";
                return response;
            }
            if (string.IsNullOrEmpty(model.PhoneNumber))
            {
                response.Message = "PhoneNumber Missing";
                return response;
            }
            if (string.IsNullOrEmpty(model.Password))
            {
                response.Message = "Password Missing";
                return response;
            }

            if (string.IsNullOrWhiteSpace(model.NpiNumber) && model.JobPosition != "SuperAdmin")
            {
                response.Message = "NPI Number Missing";
                return response;
            }

            var isValidatePassword = IsValidatePassword(model.Password);
            if (!isValidatePassword)
            {
                response.Message = "Invalid Password";
                return response;
            }
            var isValidEmail = IsValidEmail(model.Email);
            if (!isValidEmail)
            {
                response.Message = "Invalid Email";
                return response;
            }

            if (!string.IsNullOrWhiteSpace(model.TwilioPhoneNumber) && !model.TwilioPhoneNumber.StartsWith("+1"))
            {
                model.TwilioPhoneNumber = $"+1{model.TwilioPhoneNumber}";
            }

            if (!string.IsNullOrWhiteSpace(model.TwilioPhoneNumber) && await _userData.GetUserByTwilioNumberAsync(model.TwilioPhoneNumber) != null)
            {
                response.Message = "Twilio Number is already in use";
                return response;
            }

            var initialPharmacy = await _pharmacyService.GetPharmacyByNpiNumber(model.NpiNumber);

            if(model.JobPosition != "SuperAdmin" && initialPharmacy == null)
            {
                    response.Message = "Invalid NPI Number";
                    return response;
            }

            User user = new User()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Licenses = model.Licenses,
                DateOfBirth = model.DateOfBirth,
                JobPosition = model.JobPosition,
                Address = model.Address,
                PhoneNumber = model.PhoneNumber,
                TwilioPhoneNumber = model.TwilioPhoneNumber,
                TwilioNumberAssignedOnUtc = !string.IsNullOrWhiteSpace(model.TwilioPhoneNumber) ? DateTime.UtcNow : null,
                IsEnabled = model.IsEnabled,
                IsDisabled = model.IsDisabled,
                ImportEnabled=model.ImportEnabled
            };

            var result = await _userData.AddUser(user, model.Password);

            if (result.Success && initialPharmacy != null)
            {
                _ = await AssignPharmacy(result.Data.Id, new List<string>() { initialPharmacy.Id.ToString() });

                if(user.TwilioPhoneNumber != null)
                {
                    await UpdateTwilioFriendlyNameAsync(user.TwilioPhoneNumber, result.Data.Id);
                }
               
            }

            return result;
        }

        public async Task<Response<User>> GetUserById(string id)
        {
            Response<User> response = new Response<User>();
            var user = await _userData.GetUserById(id);
            user.ImageUri = _imageService.GetImageURI(user.ImageName);
            user.RoleList = await _userManager.GetRolesAsync(user);

            if (user == null)
            {
                response.Message = "Error while getting user by id";
                response.Success = false;
                return response;
            }
            else
            {
                response.Success = true;
                response.Message = "User retrived successfully";
                response.Data = user;
                return response;
            }
        }

        public async Task<Response<UserDto>> GetUserInfoById(string id)
        {
            Response<UserDto> response = new Response<UserDto>();
            var user = await _userData.GetUserInfoById(id);

            if (user == null)
            {
                response.Message = "Error while getting user by id";
                response.Success = false;
                return response;
            }
            else
            {
                response.Success = true;
                response.Message = "User retrived successfully";
                response.Data = user;
                return response;
            }
        }

        public async Task<Response<User>> GetUserDetailsById(string id, DateTime startDate, DateTime endDate, int month)
        {
            Response<User> response = new Response<User>();
            var user = await _userData.GetUserById(id);
            if (user == null)
            {
                response.Message = "Error while getting user by id";
                response.Success = false;
                return response;
            }

            // update annual compliance flags based on their last recorded date - return false if it not from current year

            var compliance = user.Compliance;

            if (compliance != null)
            {
                int currentYear = DateTime.Today.Year;

                if (compliance.HippaTrainingRecordedOn != null)
                    compliance.AnnualHIPPATraining = compliance.HippaTrainingRecordedOn.Value.Year == currentYear;

                if (compliance.FraudTrainingRecordedOn != null)
                    compliance.AnnualFraudTraining = compliance.FraudTrainingRecordedOn.Value.Year == currentYear;

                if (compliance.CyberTrainingRecordedOn != null)
                    compliance.AnnualCyberTraining = compliance.CyberTrainingRecordedOn.Value.Year == currentYear;
            }

            user.ImageUri = _imageService.GetImageURI(user.ImageName);
            user.RoleList = await _userManager.GetRolesAsync(user);

            var pdcMonth = startDate.Date > endDate.Date ? startDate : endDate;
            var queryType = PdcQueryType.ByPdcMonth;

            if (month == 12)
            {
                var range = _patientPdcService.GetMedicationPeriodForGraph(month);
                pdcMonth = range.Item2;
                queryType = PdcQueryType.ByEndDate;
            }

            var cholesterolPDC = await _patientPdcService.GetPdcForUserAsync(user.Id, PDC.Cholesterol.ToString(), pdcMonth, month, queryType);
            var diabetesPDC = await _patientPdcService.GetPdcForUserAsync(user.Id, PDC.Diabetes.ToString(), pdcMonth, month, queryType);
            var rasaPDC = await _patientPdcService.GetPdcForUserAsync(user.Id, PDC.RASA.ToString(), pdcMonth, month, queryType);

            user.CholestrolPDC = cholesterolPDC.Value;
            user.DiabetesPDC = diabetesPDC.Value;
            user.RASAPDC = rasaPDC.Value;

            response.Success = true;
            response.Message = "User retrived successfully";
            response.Data = user;
            return response;

        }

        public async Task<Response<UserPDCDto>> GetUserByIdForPDC(string id)
        {
            Response<UserPDCDto> response = new Response<UserPDCDto>();
            var userDto = await _userData.GetUserByIdForPDC(id);

            if (userDto == null)
            {
                response.Message = "Error while getting user by id";
                response.Success = false;
                return response;
            }
            else
            {
                response.Success = true;
                response.Message = "User retrived successfully";
                response.Data = userDto;
                return response;
            }
        }

        public async Task<Response<User>> AssignPharmacy(string userId, List<string> pharmacyIds)
        {
            Response<User> response = new Response<User>();
            response.Success = await _userData.AssignPharmacies(userId, pharmacyIds);
            return response;
        }

        public async Task<Response<User>> EmailConfirmation(string code, string email)
        {
            Response<User> response = new Response<User>();
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                response.Message = "Error while getting user by email";
                response.Success = false;
                return response;
            }
            // string decode = System.Web.HttpUtility.UrlDecode(code);
            var result = await _userManager.ConfirmEmailAsync(user, code);


            if (result.Succeeded)
            {
                response.Success = true;
                response.Message = "Email Confirmation Has Successfully Done";
                AdminNotification notification = new AdminNotification
                {
                    Status = "User approval request",
                    NotificationType = "New User",
                    SendDateTime = DateTime.Now,
                    User = user,
                };
                await _notificationData.AddAdminNotification(notification);

                await SendNotificationForNewUser(user.Id, notification);

                return response;
            }
            else if (result.Succeeded == false)
                response.Success = false;
            response.Message = "Email Confirmation Has Failed";
            return response;

        }

        async Task SendNotificationForNewUser(string userId, AdminNotification notification)
        {
            try
            {
                var pharmacies = await _pharmacyData.GetPharmaciesByUserId(userId);
                var pharmacyId = pharmacies.FirstOrDefault()?.Id;

                if (pharmacyId.GetValueOrDefault() > 0)
                {
                    var adminUsers = await _userData.GetAdminUsersByPharmacyId(pharmacyId.Value) ?? new();

                    foreach (var adminUser in adminUsers)
                    {
                        try
                        {
                            await _hubContext.Clients.Group(adminUser.Id).SendAsync("SendNotification", notification);
                        }
                        catch { }
                    }
                }
            }
            catch { }
        }

        public async Task<Response<Pharmacy>> GetPharmacy(string authorization)
        {
            Response<Pharmacy> response = new Response<Pharmacy>();
            List<Pharmacy> pharmacies = await _userData.GetPharmacy(authorization);
            response.Success = true;
            response.DataList = pharmacies;
            return response;
        }

        public List<PdcModel> GetGraphDataByStartDateAndEndDate(DateTime startDate, DateTime endDate)
        {
            List<PdcModel> list = new List<PdcModel>();

            while (startDate <= endDate)
            {
                PdcModel pdcModels = new PdcModel();
                pdcModels.Date = startDate;
                pdcModels.Value = UtilityHelper.RandomNumber(60, 100);
                list.Add(pdcModels);
                startDate = startDate.AddMonths(1);
            }

            return list;
        }

        public async Task<Response<User>> getUserListByPatientId(int patientId)
        {
            Response<User> response = new Response<User>();
            var users = await _userData.getUserListByPatientId(patientId);

            if (users == null)
            {
                response.Success = false;
                response.Message = "users Not Found";
                return response;
            }
            response.Success = true;
            response.Message = "users retrived successfully";
            response.DataList = users;
            return response;

        }

        public async Task<Response<string>> GetAvailableTwilioNumbersAsync()
        {
            var result = new Response<string>()
            {
                DataList = new()
            };

            var accountSid = _configuration["twilioAccountSid"];
            var authToken = _configuration["twimlAuthToken"];

            TwilioClient.Init(accountSid, authToken);

            var incomingPhoneNumbers = await IncomingPhoneNumberResource.ReadAsync(limit: 40);

            if (incomingPhoneNumbers != null && incomingPhoneNumbers.Any())
            {
                var numbers = incomingPhoneNumbers.Select(x => x.PhoneNumber.ToString()).ToList();

                var assignedNumbers = await _userData.GetAssignedTwilioNumbersAsync();
                var assignedPharmacyNumbers = await _pharmacyData.GetAssignedTwilioNumbersAsync();
                var twilioCallerId = _configuration["twilioCallerId"];

                numbers.RemoveAll(n => assignedNumbers.Contains(n) || assignedNumbers.Contains(n.Replace("+1", "")));
                numbers.RemoveAll(n => assignedPharmacyNumbers.Contains(n) || assignedPharmacyNumbers.Contains(n.Replace("+1", "")));
                numbers.RemoveAll(n => twilioCallerId.Contains(n));
                numbers.Sort();

                result.DataList = numbers;
            }

            result.Success = true;
            return result;
        }

        public async Task<Response<TwilioCallDto>> GetTwilioCallHistoryAsync(string authorization, int recordNumber, int pageLimit, string filterType, string filterValue)
        {
            IEnumerable<TwilioCallDto> callLogsUsers = new List<TwilioCallDto>();
            var result = new Response<TwilioCallDto>()
            {
                DataList = new()
            };

            var parameter = "";

            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                parameter = headerValue.Parameter;
            }

            string userId = _jwtUtils.ValidateToken(parameter);
            string role = _jwtUtils.GetRole(parameter);

            var user = await _userData.GetUserById(userId);


            if (!string.IsNullOrEmpty(filterType) && !string.IsNullOrEmpty(filterValue))
            {

                if (user != null)
                {
                    DateTime? numberAssignedDate = null;

                    var callLogs = new List<TwilioCallDto>();
                    if (role.Equals("Admin"))
                    {
                        var accountSid = _configuration["twilioAccountSid"];
                        var authToken = _configuration["twimlAuthToken"];

                        TwilioClient.Init(accountSid, authToken);
                        DateTime startDate = DateTime.UtcNow.Date.AddDays(1);
                        DateTime endDate = startDate.AddDays(-14);
                        var users = await _userData.GetAllAssignedUserForAdmin(userId);
                        var adminPharmacy = await _userData.GetPharmacy(authorization);

                        foreach (User userAssign in users)
                        {
                            numberAssignedDate = userAssign.TwilioNumberAssignedOnUtc;
                            // query for outbound calls

                            var calls = await CallResource.ReadAsync(new ReadCallOptions
                            {
                                From = new PhoneNumber(userAssign.TwilioPhoneNumber),
                                StartTimeBefore = startDate,
                                EndTimeAfter = endDate
                            });

                            //foreach (var c in calls)
                            //{
                            //    //if (!c.Direction.Contains("outbound")) continue;

                            //    if (numberAssignedDate != null && (c.DateCreated == null || c.DateCreated < numberAssignedDate)) continue;

                            //    callLogs.Add(new TwilioCallDto
                            //    {
                            //        Sid = c.Sid,
                            //        CallDate = c.DateCreated.Value,
                            //        CallStatus = c.Status.ToString(),
                            //        Direction = c.Direction,
                            //        DurationSeconds = c.Duration,
                            //        OtherNumber = c.To.StartsWith("+") ? c.To : "",
                            //        OtherCaller = c.CallerName,
                            //        UserName = userAssign.FirstName + " " + userAssign.LastName
                            //    });
                            //}

                            // query for inbound calls

                            //calls = await CallResource.ReadAsync(new ReadCallOptions
                            // {
                            //     To = new PhoneNumber(userAssign.TwilioPhoneNumber),
                            //     StartTimeBefore = startDate,
                            //     EndTimeAfter = endDate
                            // });

                            foreach (var c in calls)
                            {
                                // if (!c.Direction.Contains("inbound")) continue;

                                if (numberAssignedDate != null && (c.DateCreated == null || c.DateCreated < numberAssignedDate)) continue;

                                try
                                {
                                    var dto = new TwilioCallDto
                                    {
                                        Sid = c.Sid,
                                        CallDate = c.DateCreated.Value,
                                        CallStatus = c.Status.ToString(),
                                        Direction = c.Direction,
                                        DurationSeconds = c.Duration,
                                        OtherNumber = c.Direction.Contains("outbound") ? (c.To.StartsWith("+") ? c.To : "") : (c.From.StartsWith("+") ? c.From : ""),
                                        OtherCaller = c.CallerName,
                                        UserName = userAssign.FirstName + " " + userAssign.LastName
                                    };
                                    if (c.Direction.Contains("inbound"))
                                    {
                                        try
                                        {
                                            var recordings = await RecordingResource.ReadAsync(callSid: c.Sid);

                                            if (recordings.Any()) dto.RecordingPathSid = Path.GetFileName(recordings.First()?.MediaUrl.AbsolutePath);
                                        }
                                        catch { }
                                    }
                                    callLogs.Add(dto);
                                }
                                catch { }
                            }
                            callLogs = callLogs.OrderByDescending(x => x.CallDate).ToList();
                            callLogsUsers = callLogsUsers.Union(callLogs);

                            callLogs.Clear();
                        }

                        if (callLogsUsers.Any())
                        {
                            // populate caller names, if not returned by twilio

                            //var callsWithoutCallerName = callLogs
                            //    .Where(r => !string.IsNullOrWhiteSpace(r.OtherNumber) && string.IsNullOrWhiteSpace(r.OtherCaller));

                            //if (callsWithoutCallerName.Any())
                            //{
                            //    // fetch patient names based on phone numbers
                            //    var phoneNumbers = callsWithoutCallerName
                            //        .Select(r => r.OtherNumber)
                            //        .Union(callsWithoutCallerName.Select(r => r.OtherNumber.Right(10)))
                            //        .Distinct().ToList();

                            //    var patients = await _patientData.GetPatientListByPhoneNumbersAsync(phoneNumbers);

                            //    if (patients.Any())
                            //    {
                            //        foreach (var item in callsWithoutCallerName)
                            //        {
                            //            if (!item.OtherCaller.IsEmpty()) continue;

                            //            var contact = patients.FirstOrDefault(p => item.OtherNumber.Contains(p.Contact.PrimaryPhone))?.Contact;
                            //            var pharmacy = patients.FirstOrDefault(p => item.OtherNumber.Contains(p.Contact.PrimaryPhone))?.Pharmacy;
                            //            if (contact != null && pharmacy.Id == pharmacy1.Id) item.OtherCaller = $"{contact.FirstName} {contact.LastName}";
                            //        }
                            //    }
                            //}

                            //callsWithoutCallerName = callLogs
                            //    .Where(r => !string.IsNullOrWhiteSpace(r.OtherNumber) && string.IsNullOrWhiteSpace(r.OtherCaller));

                            //if (callsWithoutCallerName.Any())
                            //{
                            //    // fetch doctor names based on phone numbers
                            //    var phoneNumbers = callsWithoutCallerName
                            //        .Select(r => r.OtherNumber)
                            //        .Union(callsWithoutCallerName.Select(r => r.OtherNumber.Right(10)))
                            //        .Distinct().ToList();

                            //    var doctors = await _doctorData.GetDoctorListByPhoneNumbersAsync(phoneNumbers);

                            //    if (doctors.Any())
                            //    {
                            //        foreach (var item in callsWithoutCallerName)
                            //        {
                            //            var contact = doctors.FirstOrDefault(p => item.OtherNumber.Contains(p.Contact.PrimaryPhone))?.Contact;

                            //            if (contact != null) item.OtherCaller = $"{contact.FirstName} {contact.LastName}";
                            //        }
                            //    }
                            //}

                            //callsWithoutCallerName = callLogs
                            //    .Where(r => !string.IsNullOrWhiteSpace(r.OtherNumber) && string.IsNullOrWhiteSpace(r.OtherCaller));

                            //if (callsWithoutCallerName.Any())
                            //{
                            //    // fetch pharmacy names based on phone numbers
                            //    var phoneNumbers = callsWithoutCallerName
                            //        .Select(r => r.OtherNumber)
                            //        .Union(callsWithoutCallerName.Select(r => r.OtherNumber.Right(10)))
                            //        .Distinct().ToList();

                            //    var pharmacies = await _pharmacyData.GetPharmacyListByPhoneNumbersAsync(phoneNumbers);

                            //    if (pharmacies.Any())
                            //    {
                            //        foreach (var item in callsWithoutCallerName)
                            //        {
                            //            var pharmacy = pharmacies.FirstOrDefault(p => item.OtherNumber.Contains(p.Contact.PrimaryPhone));

                            //            if (pharmacy != null) item.OtherCaller = pharmacy.Name;
                            //        }
                            //    }
                            //}
                            var callsWithoutCallerName = callLogsUsers
                                .Where(r => !string.IsNullOrWhiteSpace(r.OtherNumber) && string.IsNullOrWhiteSpace(r.OtherCaller));

                            var phoneNumbers = callsWithoutCallerName
                                .Select(r => r.OtherNumber)
                                .Union(callsWithoutCallerName.Select(r => r.OtherNumber.Right(10)))
                                .Distinct().ToList();

                            var patients = await _patientData.GetPatientListByPhoneNumbersAsync(phoneNumbers);
                            var doctors = await _doctorData.GetDoctorListByPhoneNumbersAsync(phoneNumbers);
                            var pharmacies = await _pharmacyData.GetPharmacyListByPhoneNumbersAsync(phoneNumbers);

                            foreach (var item in callsWithoutCallerName)
                            {
                                var patientContact = patients.FirstOrDefault(p => item.OtherNumber.Contains(p.Contact.PrimaryPhone))?.Contact;
                                var patientPharmacy = patients.FirstOrDefault(p => item.OtherNumber.Contains(p.Contact.PrimaryPhone))?.Pharmacy;
                                if (patientContact != null && adminPharmacy.Any(x => x.Name.Contains(patientPharmacy.Name)))
                                {
                                    item.OtherCaller = $"{patientContact.FirstName} {patientContact.LastName}";
                                }

                                var doctorContact = doctors.FirstOrDefault(p => item.OtherNumber.Contains(p.Contact.PrimaryPhone))?.Contact;
                                if (doctorContact != null)
                                {
                                    item.OtherCaller = $"{doctorContact.FirstName} {doctorContact.LastName}";
                                }

                                var pharmacyContact = pharmacies.FirstOrDefault(p => item.OtherNumber.Contains(p.Contact.PrimaryPhone))?.Contact;
                                if (pharmacyContact != null)
                                {
                                    item.OtherCaller = $"{pharmacyContact.FirstName} {pharmacyContact.LastName}";
                                }
                            }
                            callLogs = callLogsUsers.Where(y => y.OtherCaller != null).OrderByDescending(x => x.CallDate).ToList();
                            callLogsUsers = callLogsUsers.Union(callLogs);

                        }
                        // callLogs.Sort((x, y) => y.CallDate.CompareTo(x.CallDate));
                        //var callLogs1 = callLogs.Where(y => y.OtherCaller != null).OrderByDescending(x => x.CallDate).ToList();
                        //callLogsUsers = callLogsUsers.Union(callLogs1);
                        //}

                        callLogsUsers = callLogsUsers
                            .Where(x => x.UserName != null && x.UserName.Contains(filterValue))
                            .OrderByDescending(x => x.CallDate)
                            //.Skip(recordNumber >= 0 && pageLimit != 0 ? recordNumber : 0)
                            //.Take(recordNumber >= 0 && pageLimit != 0 ? pageLimit : int.MaxValue)
                            .ToList();


                        //callLogsUsers = callLogsUsers
                        //   .Where(x => x.UserName.Contains(filterValue))
                        //   .OrderByDescending(x => x.CallDate)
                        //   .ToList();
                        result.DataList = callLogsUsers.ToList();

                    }

                    else
                    {
                        var accountSid = _configuration["twilioAccountSid"];
                        var authToken = _configuration["twimlAuthToken"];

                        TwilioClient.Init(accountSid, authToken);

                        numberAssignedDate = user.TwilioNumberAssignedOnUtc;

                        // var callLogs = new List<TwilioCallDto>();

                        DateTime startDate = DateTime.UtcNow.Date.AddDays(1);
                        DateTime endDate = startDate.AddDays(-14);

                        // query for outbound calls

                        var calls = await CallResource.ReadAsync(new ReadCallOptions
                        {
                            From = new PhoneNumber(user.TwilioPhoneNumber),
                            StartTimeBefore = startDate,
                            EndTimeAfter = endDate
                        });

                        foreach (var c in calls)
                        {
                            if (!c.Direction.Contains("outbound")) continue;

                            if (numberAssignedDate != null && (c.DateCreated == null || c.DateCreated < numberAssignedDate)) continue;

                            callLogs.Add(new TwilioCallDto
                            {
                                Sid = c.Sid,
                                CallDate = c.DateCreated.Value,
                                CallStatus = c.Status.ToString(),
                                Direction = c.Direction,
                                DurationSeconds = c.Duration,
                                OtherNumber = c.To.StartsWith("+") ? c.To : "",
                                OtherCaller = c.CallerName
                            });
                        }

                        // query for inbound calls

                        calls = await CallResource.ReadAsync(new ReadCallOptions
                        {
                            To = new PhoneNumber(user.TwilioPhoneNumber),
                            StartTimeBefore = startDate,
                            EndTimeAfter = endDate
                        });

                        foreach (var c in calls)
                        {
                            if (!c.Direction.Contains("inbound")) continue;

                            if (numberAssignedDate != null && (c.DateCreated == null || c.DateCreated < numberAssignedDate)) continue;

                            try
                            {
                                var dto = new TwilioCallDto
                                {
                                    Sid = c.Sid,
                                    CallDate = c.DateCreated.Value,
                                    CallStatus = c.Status.ToString(),
                                    Direction = c.Direction,
                                    DurationSeconds = c.Duration,
                                    OtherNumber = c.From.StartsWith("+") ? c.From : "",
                                    OtherCaller = c.CallerName,
                                    UserName = user.FirstName + " " + user.LastName,
                                };

                                try
                                {
                                    var recordings = await RecordingResource.ReadAsync(callSid: c.Sid);

                                    if (recordings.Any()) dto.RecordingPathSid = Path.GetFileName(recordings.First()?.MediaUrl.AbsolutePath);
                                }
                                catch { }

                                callLogs.Add(dto);
                            }
                            catch { }
                        }

                        if (callLogs.Any())
                        {
                            // populate caller names, if not returned by twilio

                            var callsWithoutCallerName = callLogs
                                .Where(r => !string.IsNullOrWhiteSpace(r.OtherNumber) && string.IsNullOrWhiteSpace(r.OtherCaller));

                            if (callsWithoutCallerName.Any())
                            {
                                // fetch patient names based on phone numbers
                                var phoneNumbers = callsWithoutCallerName
                                    .Select(r => r.OtherNumber)
                                    .Union(callsWithoutCallerName.Select(r => r.OtherNumber.Right(10)))
                                    .Distinct().ToList();

                                var patients = await _patientData.GetPatientListByPhoneNumbersAsync(phoneNumbers);

                                if (patients.Any())
                                {
                                    foreach (var item in callsWithoutCallerName)
                                    {
                                        if (!item.OtherCaller.IsEmpty()) continue;

                                        var contact = patients.FirstOrDefault(p => item.OtherNumber.Contains(p.Contact.PrimaryPhone))?.Contact;

                                        if (contact != null) item.OtherCaller = $"{contact.FirstName} {contact.LastName}";
                                    }
                                }
                            }

                            callsWithoutCallerName = callLogs
                                .Where(r => !string.IsNullOrWhiteSpace(r.OtherNumber) && string.IsNullOrWhiteSpace(r.OtherCaller));

                            if (callsWithoutCallerName.Any())
                            {
                                // fetch doctor names based on phone numbers
                                var phoneNumbers = callsWithoutCallerName
                                    .Select(r => r.OtherNumber)
                                    .Union(callsWithoutCallerName.Select(r => r.OtherNumber.Right(10)))
                                    .Distinct().ToList();

                                var doctors = await _doctorData.GetDoctorListByPhoneNumbersAsync(phoneNumbers);

                                if (doctors.Any())
                                {
                                    foreach (var item in callsWithoutCallerName)
                                    {
                                        var contact = doctors.FirstOrDefault(p => item.OtherNumber.Contains(p.Contact.PrimaryPhone))?.Contact;

                                        if (contact != null) item.OtherCaller = $"{contact.FirstName} {contact.LastName}";
                                    }
                                }
                            }

                            callsWithoutCallerName = callLogs
                                .Where(r => !string.IsNullOrWhiteSpace(r.OtherNumber) && string.IsNullOrWhiteSpace(r.OtherCaller));

                            if (callsWithoutCallerName.Any())
                            {
                                // fetch pharmacy names based on phone numbers
                                var phoneNumbers = callsWithoutCallerName
                                    .Select(r => r.OtherNumber)
                                    .Union(callsWithoutCallerName.Select(r => r.OtherNumber.Right(10)))
                                    .Distinct().ToList();

                                var pharmacies = await _pharmacyData.GetPharmacyListByPhoneNumbersAsync(phoneNumbers);

                                if (pharmacies.Any())
                                {
                                    foreach (var item in callsWithoutCallerName)
                                    {
                                        var pharmacy = pharmacies.FirstOrDefault(p => item.OtherNumber.Contains(p.Contact.PrimaryPhone));

                                        if (pharmacy != null) item.OtherCaller = pharmacy.Name;
                                    }
                                }
                            }
                        }

                        callLogs = callLogs.Where(x => x.UserName != null && x.UserName.Contains(filterValue)).OrderByDescending(x => x.CallDate).ToList();

                        result.DataList = callLogs;
                    }

                }
            }
            else
            {
                if (user != null)
                {
                    var callLogs = new List<TwilioCallDto>();
                    if (role.Equals("Admin"))
                    {
                        var users = await _userData.GetAllAssignedUserForAdmin(userId);
                        var accountSid = _configuration["twilioAccountSid"];
                        var authToken = _configuration["twimlAuthToken"];

                        TwilioClient.Init(accountSid, authToken);
                        DateTime startDate = DateTime.UtcNow.Date.AddDays(1);
                        DateTime endDate = startDate.AddDays(-14);

                        foreach (User userAssign in users)
                        {
                            var calls = await CallResource.ReadAsync(new ReadCallOptions
                            {
                                From = new PhoneNumber(userAssign.TwilioPhoneNumber),
                                StartTimeBefore = startDate,
                                EndTimeAfter = endDate
                            });

                            foreach (var c in calls)
                            {
                                var dto = new TwilioCallDto
                                {
                                    Sid = c.Sid,
                                    CallDate = c.DateCreated.Value,
                                    CallStatus = c.Status.ToString(),
                                    Direction = c.Direction,
                                    DurationSeconds = c.Duration,
                                    OtherNumber = c.Direction.Contains("outbound") ? (c.To.StartsWith("+") ? c.To : "") : (c.From.StartsWith("+") ? c.From : ""),
                                    OtherCaller = c.CallerName,
                                    UserName = userAssign.FirstName + " " + userAssign.LastName
                                };
                                if (c.Direction.Contains("inbound"))
                                {
                                    try
                                    {
                                        var recordings = await RecordingResource.ReadAsync(callSid: c.Sid);

                                        if (recordings.Any()) dto.RecordingPathSid = Path.GetFileName(recordings.First()?.MediaUrl.AbsolutePath);
                                    }
                                    catch { }
                                }
                                callLogs.Add(dto);
                            }
                            callLogs = callLogs.OrderByDescending(x => x.CallDate).ToList();
                            callLogsUsers = callLogsUsers.Union(callLogs);

                            callLogs.Clear();

                        }

                        var adminPharmacy = await _userData.GetPharmacy(authorization);

                        if (callLogsUsers.Any())
                        {
                            var callsWithoutCallerName = callLogsUsers
                                .Where(r => !string.IsNullOrWhiteSpace(r.OtherNumber) && string.IsNullOrWhiteSpace(r.OtherCaller));

                            var phoneNumbers = callsWithoutCallerName
                                .Select(r => r.OtherNumber)
                                .Union(callsWithoutCallerName.Select(r => r.OtherNumber.Right(10)))
                                .Distinct().ToList();

                            var patients = await _patientData.GetPatientListByPhoneNumbersAsync(phoneNumbers);
                            var doctors = await _doctorData.GetDoctorListByPhoneNumbersAsync(phoneNumbers);
                            var pharmacies = await _pharmacyData.GetPharmacyListByPhoneNumbersAsync(phoneNumbers);

                            foreach (var item in callsWithoutCallerName)
                            {
                                var patientContact = patients.FirstOrDefault(p => item.OtherNumber.Contains(p.Contact.PrimaryPhone))?.Contact;
                                var patientPharmacy = patients.FirstOrDefault(p => item.OtherNumber.Contains(p.Contact.PrimaryPhone))?.Pharmacy;
                                if (patientContact != null && adminPharmacy.Any(x => x.Name.Contains(patientPharmacy.Name)))
                                {
                                    item.OtherCaller = $"{patientContact.FirstName} {patientContact.LastName}";
                                }

                                var doctorContact = doctors.FirstOrDefault(p => item.OtherNumber.Contains(p.Contact.PrimaryPhone))?.Contact;
                                if (doctorContact != null)
                                {
                                    item.OtherCaller = $"{doctorContact.FirstName} {doctorContact.LastName}";
                                }

                                var pharmacyContact = pharmacies.FirstOrDefault(p => item.OtherNumber.Contains(p.Contact.PrimaryPhone))?.Contact;
                                if (pharmacyContact != null)
                                {
                                    item.OtherCaller = $"{pharmacyContact.FirstName} {pharmacyContact.LastName}";
                                }
                            }
                            callLogs = callLogsUsers.Where(y => y.OtherCaller != null).OrderByDescending(x => x.CallDate).ToList();
                            callLogsUsers = callLogsUsers.Union(callLogs);
                        }
                        callLogsUsers = callLogsUsers.OrderByDescending(x => x.CallDate)
                        .Skip(recordNumber >= 0 && pageLimit != 0 ? recordNumber : 0)
                        .Take(recordNumber >= 0 && pageLimit != 0 ? pageLimit : int.MaxValue)
                        .ToList();

                        result.DataList = callLogsUsers.ToList();
                    }

                    else
                    {
                        var accountSid = _configuration["twilioAccountSid"];
                        var authToken = _configuration["twimlAuthToken"];

                        TwilioClient.Init(accountSid, authToken);

                        // var callLogs = new List<TwilioCallDto>();

                        DateTime startDate = DateTime.UtcNow.Date.AddDays(1);
                        DateTime endDate = startDate.AddDays(-14);

                        // query for outbound calls

                        var calls = await CallResource.ReadAsync(new ReadCallOptions
                        {
                            From = new PhoneNumber(user.TwilioPhoneNumber),
                            StartTimeBefore = startDate,
                            EndTimeAfter = endDate
                        });

                        callLogs.AddRange(calls.Where(c => c.Direction.Contains("outbound")).Select(c => new TwilioCallDto
                        {
                            Sid = c.Sid,
                            CallDate = c.DateCreated.Value,
                            CallStatus = c.Status.ToString(),
                            Direction = c.Direction,
                            DurationSeconds = c.Duration,
                            OtherNumber = c.To.StartsWith("+") ? c.To : "",
                            OtherCaller = c.CallerName
                        }));

                        // query for inbound calls

                        calls = await CallResource.ReadAsync(new ReadCallOptions
                        {
                            To = new PhoneNumber(user.TwilioPhoneNumber),
                            StartTimeBefore = startDate,
                            EndTimeAfter = endDate
                        });

                        foreach (var c in calls)
                        {
                            if (!c.Direction.Contains("inbound")) continue;

                            try
                            {
                                var dto = new TwilioCallDto
                                {
                                    Sid = c.Sid,
                                    CallDate = c.DateCreated.Value,
                                    CallStatus = c.Status.ToString(),
                                    Direction = c.Direction,
                                    DurationSeconds = c.Duration,
                                    OtherNumber = c.From.StartsWith("+") ? c.From : "",
                                    OtherCaller = c.CallerName,
                                    UserName = user.FirstName + " " + user.LastName,
                                };

                                try
                                {
                                    var recordings = await RecordingResource.ReadAsync(callSid: c.Sid);

                                    if (recordings.Any()) dto.RecordingPathSid = Path.GetFileName(recordings.First()?.MediaUrl.AbsolutePath);
                                }
                                catch { }

                                callLogs.Add(dto);
                            }
                            catch { }
                        }

                        if (callLogs.Any())
                        {
                            // populate caller names, if not returned by twilio

                            var callsWithoutCallerName = callLogs
                                .Where(r => !string.IsNullOrWhiteSpace(r.OtherNumber) && string.IsNullOrWhiteSpace(r.OtherCaller));

                            if (callsWithoutCallerName.Any())
                            {
                                // fetch patient names based on phone numbers
                                var phoneNumbers = callsWithoutCallerName
                                    .Select(r => r.OtherNumber)
                                    .Union(callsWithoutCallerName.Select(r => r.OtherNumber.Right(10)))
                                    .Distinct().ToList();

                                var patients = await _patientData.GetPatientListByPhoneNumbersAsync(phoneNumbers);

                                if (patients.Any())
                                {
                                    foreach (var item in callsWithoutCallerName)
                                    {
                                        if (!item.OtherCaller.IsEmpty()) continue;

                                        var contact = patients.FirstOrDefault(p => item.OtherNumber.Contains(p.Contact.PrimaryPhone))?.Contact;

                                        if (contact != null) item.OtherCaller = $"{contact.FirstName} {contact.LastName}";
                                    }
                                }
                            }

                            callsWithoutCallerName = callLogs
                                .Where(r => !string.IsNullOrWhiteSpace(r.OtherNumber) && string.IsNullOrWhiteSpace(r.OtherCaller));

                            if (callsWithoutCallerName.Any())
                            {
                                // fetch doctor names based on phone numbers
                                var phoneNumbers = callsWithoutCallerName
                                    .Select(r => r.OtherNumber)
                                    .Union(callsWithoutCallerName.Select(r => r.OtherNumber.Right(10)))
                                    .Distinct().ToList();

                                var doctors = await _doctorData.GetDoctorListByPhoneNumbersAsync(phoneNumbers);

                                if (doctors.Any())
                                {
                                    foreach (var item in callsWithoutCallerName)
                                    {
                                        var contact = doctors.FirstOrDefault(p => item.OtherNumber.Contains(p.Contact.PrimaryPhone))?.Contact;

                                        if (contact != null) item.OtherCaller = $"{contact.FirstName} {contact.LastName}";
                                    }
                                }
                            }

                            callsWithoutCallerName = callLogs
                                .Where(r => !string.IsNullOrWhiteSpace(r.OtherNumber) && string.IsNullOrWhiteSpace(r.OtherCaller));

                            if (callsWithoutCallerName.Any())
                            {
                                // fetch pharmacy names based on phone numbers
                                var phoneNumbers = callsWithoutCallerName
                                    .Select(r => r.OtherNumber)
                                    .Union(callsWithoutCallerName.Select(r => r.OtherNumber.Right(10)))
                                    .Distinct().ToList();

                                var pharmacies = await _pharmacyData.GetPharmacyListByPhoneNumbersAsync(phoneNumbers);

                                if (pharmacies.Any())
                                {
                                    foreach (var item in callsWithoutCallerName)
                                    {
                                        var pharmacy = pharmacies.FirstOrDefault(p => item.OtherNumber.Contains(p.Contact.PrimaryPhone));

                                        if (pharmacy != null) item.OtherCaller = pharmacy.Name;
                                    }
                                }
                            }
                        }


                        // callLogs.Sort((x, y) => y.CallDate.CompareTo(x.CallDate));
                        //callLogs=callLogs.Take(recordNumber >= 0 && pageLimit != 0 ? pageLimit : int.MaxValue).OrderByDescending(x => x.CallDate).ToList();
                        callLogs = callLogs.OrderByDescending(x => x.CallDate).Skip(recordNumber >= 0 && pageLimit != 0 ? recordNumber : 0)
                             .Take(recordNumber >= 0 && pageLimit != 0 ? pageLimit : int.MaxValue).ToList();

                        result.DataList = callLogs;
                    }

                }
            }



            result.Success = true;
            return result;
        }

        public async Task<Response<bool>> ToggleComplianceAsync(string userId, string complianceCode)
        {
            string complianceType = string.Empty;

            switch ((complianceCode ?? string.Empty).ToLower())
            {
                case "background":
                    complianceType = nameof(UserCompliance.BackgroundCheck);
                    break;

                case "liability":
                    complianceType = nameof(UserCompliance.LiabilityInsurance);
                    break;

                case "hippa":
                    complianceType = nameof(UserCompliance.AnnualHIPPATraining);
                    break;

                case "fraud":
                    complianceType = nameof(UserCompliance.AnnualFraudTraining);
                    break;

                case "cyber":
                    complianceType = nameof(UserCompliance.AnnualCyberTraining);
                    break;
            }

            var response = new Response<bool>();

            if (!string.IsNullOrWhiteSpace(complianceType))
            {
                response.Success = await _userData.ToggleComplianceAsync(userId, complianceType);
            }
            else
            {
                response.Message = "Invalid Compliance Code/Type";
            }

            return response;
        }

        private async Task UpdateTwilioFriendlyNameAsync(string twilioPhoneNumber, string friendlyName)
        {
            var regex = new Regex(@"^(\+1)[1-9]([0-9]{9})$");

            if (!regex.IsMatch(twilioPhoneNumber)) return;

            if (string.IsNullOrWhiteSpace(friendlyName))
                friendlyName = twilioPhoneNumber;

            var accountSid = _configuration["twilioAccountSid"];
            var authToken = _configuration["twimlAuthToken"];

            TwilioClient.Init(accountSid, authToken);

            var incomingPhoneNumbers = await IncomingPhoneNumberResource.ReadAsync(
                phoneNumber: new PhoneNumber(twilioPhoneNumber));

            if (incomingPhoneNumbers != null && incomingPhoneNumbers.FirstOrDefault() is IncomingPhoneNumberResource resource)
            {
                if (!resource.FriendlyName.Equals(friendlyName))
                {
                    var option = new UpdateIncomingPhoneNumberOptions(resource.Sid)
                    {
                        FriendlyName = friendlyName
                    };

                    await IncomingPhoneNumberResource.UpdateAsync(option);
                }
            }
        }
    }
}
