using OntrackDb.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OntrackDb.Authentication;
using OntrackDb.Filter;
using OntrackDb.Dto;
using OntrackDb.Entities;

namespace OntrackDb.Service
{
   public interface IUserService
    {
        Task<Response<User>> GetUsers(PaginationFilter filter);
        Task<Response<User>> Register(RegisterModel model);
        Task<Response<User>> EditProfile(RegisterModel model,string authorization);
        Task<Response<User>> EditProfileAdmin(RegisterModel model); 
        Task<Response<User>> AddUser(RegisterModel model);
        Task<User> GetUserByUserName(string userName);
        Task<Response<User>> Login(LoginModel model);
        Task<Response<User>> ForgotPassword(MailRequest model);
        Task<Response<User>> ResetPassword(ResetPasswordModel model);
        Task<Response<User>> GetAllUsers();
        Task<Response<User>> DeleteUserById(String Id);
        Task<Response<User>> SendInvite(InviteModel model);
        Task<Response<User>> ChangePasswordAsync(ChangePasswordModel model, string authorization);
        Task<Response<User>> GetSpecificUser(string parameter);
        Task<Response<User>> UpdateUserImageName(string imageName, string authorization);
        Task<Response<User>> GetUserById(string id);
        Task<Response<User>> GetUserDetailsById(string id, DateTime startDate, DateTime endDate, int month);
        Task<Response<User>> AssignPharmacy(string userId, List<string> pharmacyIds);
        Task<Response<Pharmacy>> GetPharmacy(string authorization);
        List<PdcModel> GetGraphDataByStartDateAndEndDate(DateTime startDate, DateTime endDate);
        Task<Response<User>> UpdateUserForNotification(string authorizatioin, Boolean isNotification);

        Task<Response<User>> GetAllUsersForLicenseExpiry();

        List<Patient> DashboardSearchPatient(string search);

        List<Pharmacy> DashboardSearchPharamcy(string searchText);

        Task<Response<User>> GetUserByIdWithPdcCalculation(string userId, DateTime startDate, DateTime endDate);
        Task<Response<UserDto>> GetAllUsersForAdmin(int recordNumber, int pageLimit,string userId, string userRole, bool activeUser, DateTime startDate, DateTime endDate, int month, string keywords, string sortDirection, string filterType, string filterValue, string filterCategory);
        Task<Response<UserPDCDto>> GetUserByIdForPDC(string id);
        Task<Response<User>> EmailConfirmation(string code, string email);
        Task<Response<User>> DeleteUserByUserId(String Id);

        Task<Response<User>> ResendEmail(string email);

        Task<Response<UserPDCDto>> PDCCalculationForUser(string id, DateTime startDate, DateTime endDate, int month);

        Task<Response<User>> getUserListByPatientId(int patientId);

        Task<Response<string>> GetAvailableTwilioNumbersAsync();

        Task<Response<TwilioCallDto>> GetTwilioCallHistoryAsync(string authorization,int recordNumber, int pageLimit, string filterType, string filterValue);

        Task<Response<User>> VerifyCodeAsync(string userEmail, string code, string deviceId);

        Task<Response<bool>> ResendVerificationCodeAsync(string userEmail);

        Task<Response<bool>> ToggleComplianceAsync(string userId, string complianceCode);

        Task<Response<UserDto>> GetUserInfoById(string id);
    }
}
