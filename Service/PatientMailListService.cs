using Aspose.Pdf.Annotations;
using OntrackDb.Authentication;
using OntrackDb.Context;
using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Model;
using OntrackDb.Repositories;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net;
using System.Threading.Tasks;
using OntrackDb.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Globalization;
using Microsoft.AspNetCore.SignalR;
using OntrackDb.Hub;

namespace OntrackDb.Service
{
    public class PatientMailListService : IPatientMailListService
    {
        private readonly IPatientMailListData _patientMailListData;
        private readonly IServiceTakeawayInformationData _serviceTakeawayInformationData;
        private readonly IServiceTakeAwayMedReconciliationData _serviceTakeAwayMedReconciliationData;
        private readonly IUserData _userData;
        private readonly INotificationData _notificationData;
        private readonly IJwtUtils _jwtUtils;
        private readonly UserManager<User> _userManager;
        private readonly IHubContext<ChatHub> _hubContext;

        public PatientMailListService(IPatientMailListData patientMailListData, IServiceTakeAwayMedReconciliationData serviceTakeAwayMedReconciliationData, IServiceTakeawayInformationData serviceTakeawayInformationData, UserManager<User> userManager, IJwtUtils jwtUtils, IUserData userData, INotificationData notificationData, IHubContext<ChatHub> hubContext)
        {
            _patientMailListData = patientMailListData;
            _userData = userData;
            _notificationData = notificationData;
            _jwtUtils = jwtUtils;
            _userManager = userManager;
            _serviceTakeawayInformationData = serviceTakeawayInformationData;
            _serviceTakeAwayMedReconciliationData = serviceTakeAwayMedReconciliationData;
            _hubContext = hubContext;
        }

        public async Task<Response<PatientMailList>> AddPatientMailList(PatientMailListModel model)
        {
            Response<PatientMailList> response = new Response<PatientMailList>();

            Address address = new Address();
            address.AddressLineOne = model.Address.AddressLineOne;
            address.AddressLineTwo = model.Address.AddressLineTwo;
            address.City = model.Address.City;
            address.State= model.Address.State; 
            address.ZipCode= model.Address.ZipCode;

            PatientMailList patientMailList = new PatientMailList
            {
                PatientName = model.PatientName,
                PatientId = model.PatientId,
                PharmacyName = model.PharmacyName,
                Address = address,
                PhoneNumber = model.PhoneNumber,
                type = model.type,
                SentType =model.SentType,
                CreatedDate= model.CreatedDate, 
                CreatedUser= model.CreatedUser,
                DeletedDate= model.DeletedDate,
                DateOfBirth= model.DateOfBirth,
                Email= model.Email, 
                IsDeleted= model.IsDeleted,
                ModifiedUser = model.ModifiedUser,
                PharmacyId= model.PharmacyId,   
                LastUpdated = model.LastUpdated,
                ContactId= model.ContactId, 
                CreatedBy= model.CreatedBy,
            };

            var result = await _patientMailListData.AddPatientMailList(patientMailList);

            if (result.SentType == "Mail")
            {
                var mailUser = await _userData.GetUserById(result.CreatedUser);

                var adminUsers = await _patientMailListData.GetAdminUsersListByPharmacyId(result.PharmacyId);

                foreach (User user in adminUsers)
                {
                    Notification notification = new Notification()
                    {
                        User = user,
                        NotificationType = "Mail Request:" + " " + mailUser.FirstName + " " + mailUser.LastName,
                        SendDateTime = DateTime.Now,
                        Status = "Success"
                    };

                    await _notificationData.AddNotification(notification);

                    try
                    {
                        await _hubContext.Clients.Groups(user.Id).SendAsync("SendNotification", notification);
                    }
                    catch { }
                }
            }

            response.Success = true;
            response.Message = "PatientMailList Added Successfully!";
            response.Data = result;

            return response;
        }

        public async Task<Response<PatientMailList>> GetPatientMailLists(int recordNumber, int pageLimit, string authorizatioin, string keywords,string sortDirection, string filterType, string filterValue, string filterCategory)
        {
            List<PatientMailList> patientMailLists1= new List<PatientMailList>();   
            Response<PatientMailList> response = new Response<PatientMailList>();
            var parameter = "";
            if (AuthenticationHeaderValue.TryParse(authorizatioin, out var headerValue))
            {
                parameter = headerValue.Parameter;
            }
            string userId =  _jwtUtils.ValidateToken(parameter);
            User user = await _userData.GetUserById(userId);
            var roleList = await _userManager.GetRolesAsync(user);
            var pharmacyIds = await _patientMailListData.GetPharmacyIdsByUserId(userId);
            var patientMailLists = await _patientMailListData.GetPatientMailLists(keywords,sortDirection,filterType,filterValue,filterCategory);
            if(roleList.Contains("Admin"))
            {
                DateTime searchDateOfBirth;
                bool isDateOfBirthValid = DateTime.TryParseExact(keywords, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out searchDateOfBirth);
                foreach (PatientMailList patientMailList in patientMailLists)
                {
                    if (pharmacyIds.Contains(patientMailList.PharmacyId))
                    {
                        patientMailLists1.Add(patientMailList);
                    }
                }
                if(!string.IsNullOrEmpty(filterType))
                {
                    response.Success = true;
                    response.DataList = patientMailLists1.Skip(recordNumber).Take(pageLimit).ToList() ;
                    return response;
                }
                else
                {
                    patientMailLists1 = patientMailLists1.Skip(recordNumber >= 0 && pageLimit != 0 ? recordNumber : 0)
               .Take(recordNumber >= 0 && pageLimit != 0 ? pageLimit : int.MaxValue).ToList();
                    response.Success = true;
                    response.DataList = patientMailLists1;
                    return response;
                }
               
            }
            else
            {
                if (!string.IsNullOrEmpty(filterType))
                {
                    response.Success = true;
                    response.DataList = patientMailLists;
                    return response;
                }
                else
                {
                    patientMailLists = patientMailLists.Skip(recordNumber >= 0 && pageLimit != 0 ? recordNumber : 0)
                .Take(recordNumber >= 0 && pageLimit != 0 ? pageLimit : int.MaxValue).ToList();
                    response.Success = true;
                    response.DataList = patientMailLists;
                    return response;
                }
                
            }
           

           
        }

        public async Task<Response<PatientMailList>> UpdatePatientMailList(PatientMailListModel model)
        {
            Response<PatientMailList> response = new Response<PatientMailList>();
            var patientMailList = await _patientMailListData.GetPatientMailListById(model.Id);
            if (patientMailList == null)
            {
                response.Success = false;
                response.Message = "PatientMailList Not Found";
                return response;
            }
            patientMailList.PhoneNumber = model.PhoneNumber;
            patientMailList.Email = model.Email;
            patientMailList.LastUpdated = DateTime.Now;
            patientMailList.ModifiedUser = model.ModifiedUser;
            var result = await _patientMailListData.UpdatePatientMailList(patientMailList);

           

            response.Success = true;
            response.Message = "Patient Updated successfully!";
            response.Data= result;  
            return response;
        }


        public async Task<Response<PatientMailList>> DeletePatientMailListById(int id )
        {
            Response<PatientMailList> response = new Response<PatientMailList>();
            
            var patientMailList = await _patientMailListData.GetPatientMailListById(id);
            if (patientMailList == null)
            {
                response.Success = false;
                response.Message = "PatientMailList Not Found";
                return response;
            }
            patientMailList.IsDeleted =true;
            var result = await _patientMailListData.UpdatePatientMailList(patientMailList);



            response.Success = true;
            response.Message = "PatientMailList Deleted successfully!";
            response.Data = result;
            return response;
        }

        public async Task<Response<PatientMailList>> GetPatientMailListForLatestCompletedDate(int id,string type)
        {
            Response<PatientMailList> response = new Response<PatientMailList>();
            var patientMailList = await _patientMailListData.GetPatientMailListForCompletedDate(id,type);
            if (patientMailList == null)
            {
                response.Success = false;
                response.Message = "PatientMailList Not Found";
                return response;
            }
            if (type == "CMR")
            {
                var takeawayVerify = await _serviceTakeawayInformationData.GetTakeawayVerifyByPatientId(id);
                if(takeawayVerify!=null)
                {
                    patientMailList.LastAccessed = takeawayVerify.LastModified;
                    await _patientMailListData.UpdatePatientMailList(patientMailList);
                }
               
            }

            if (type == "MedRec")
            {
                var takeawayVerify = await _serviceTakeAwayMedReconciliationData.GetTakeawayVerifyByPatientId(id);
                if (takeawayVerify != null)
                {
                    patientMailList.LastAccessed = takeawayVerify.LastModified;
                    await _patientMailListData.UpdatePatientMailList(patientMailList);
                }
                  
            }
            response.Success = true;
            response.Data = patientMailList;
            return response;
        }

        public async Task<Response<PatientMailList>> GetPatientMailListById(int id)
        {
            Response<PatientMailList> response = new Response<PatientMailList>();
            var patientMailList = await _patientMailListData.GetPatientMailListById(id);

            response.Success = true;
            response.Data = patientMailList;
            return response;
        }


    }
}
