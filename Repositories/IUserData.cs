using OntrackDb.Authentication;
using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Filter;
using OntrackDb.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OntrackDb.Repositories
{
    public interface IUserData
    {
        Task<List<User>> GetUsers(PaginationFilter filter);
        Task<Response<User>> Register(User user, string password);
        Task<Response<User>> AddUser(User user, string password);
        Task<Response<User>> UpdateUser(User user);
        Task<User> GetUserByUserName(string userName);
        Task<User> GetUserByUserId(string Id);
        Task<User> GetUserByEmail(string email);
        Task<User> GetUser(string userName);
        Task<User> GetUserById(string id);
        Task<bool> GetUserByUserAndPassword(User user, string password);
        Task<string> GeneratePasswordResetToken(User user);
        Task<Response<User>> ResetPasswordAsync(User user, string token, string password);
        Task<List<User>> GetAllUsers();
        Task<List<UserDto>> GetAllUsersForAdmin(int recordNumber, int pageLimit, string userId, string userRole, bool activeUser, string keywords, string sortDirection, string filterType, string filterValue, string filterCategory);
        Task<List<User>> GetAdminUsers();
        Task<User> UpdateLastLoginForUser(User user, string deviceId);
        Task<Response<User>> DeleteUserByUserId(User user);
        Task<Response<User>> ChangePasswordAsync(ChangePasswordModel model, string authorization);

        Task<bool> AssignPharmacies(string id, List<string> pharmacyIds, bool replaceExisting = true);
        Task<List<Pharmacy>> GetPharmacy(string authorization);

        Task<Licenses> GetLicenseById(int id);
        Task<Address> GetAddressById(int id);
        Task<WebSocket> AddWebSocket(WebSocket webSocket);
        Task<WebSocket> UpdateWebSocket(WebSocket webSocket);
        Task<WebSocket> GetWebSocketByUser(string userId);
        Task<UserPDCDto> GetUserByIdForPDC(string id);
        Task DeleteUserByUserIdForPermanent(User user);

        Task<List<AdminNotification>> GetAdminNotificationByUserIdForDelete(string userId);
        //  Task DeleteNotificationForById(int notificationId);

        Task<List<User>> getUserListByPatientId(int patientId);

        Task<List<string>> GetAssignedTwilioNumbersAsync();
        Task<User> GetUserByTwilioNumberAsync(string number);

        Task<bool> SetVerificationCodeAsync(string userId, string code);
        Task<bool> SetVerifiedDeviceAsync(string userId, string deviceId);

        Task<bool> ToggleComplianceAsync(string userId, string complianceType);

        Task<UserDto> GetUserInfoById(string id);

        Task<List<User>> GetAdminUsersByPharmacyId(int pharmacyId);
        Task<List<User>> GetAdminUsersByUserId(string userId);

        Task<List<User>> GetAllAssignedUserForAdmin(string userId);
    }
}
