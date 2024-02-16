using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OntrackDb.Authentication;
using OntrackDb.Authorization;
using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Enums;
using OntrackDb.Filter;
using OntrackDb.Helper;
using OntrackDb.Model;
using OntrackDb.Repositories;
using OntrackDb.Service;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OntrackDb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IEmailService mailService;
        private readonly IPharmacyService _pharmacyService;
        private readonly IPatientService _patientService;
        private readonly IMedicationService _medicationService;
        readonly IJwtUtils _jwtUtils;
        readonly IPatientPdcService _patientPdcService;

        public UserController(IUserService userService, IEmailService mailService, IPharmacyService pharmacyService,
                              IPatientService patientService, IMedicationService mediccationService, IJwtUtils jwtUtils, IPatientPdcService patientPdcService)
        {
            this.userService = userService;
            this.mailService = mailService;
            _pharmacyService = pharmacyService;
            _patientService = patientService;
            _medicationService = mediccationService;
            _jwtUtils = jwtUtils;
            _patientPdcService = patientPdcService;
        }


        /// <summary>
        /// For login purpose. Role[Everyone can access]
        /// </summary>
        /// <remarks>
        /// This API Will be used for user and admin login.
        /// </remarks>
        [HttpPost]
        [Route("login")]
        [Consumes("application/json")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var result = await userService.Login(model);
            return Ok(result);
        }

        [HttpPost]
        [Route("verify-email")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> VerifyCode([FromBody] EmailVerificationModel model)
        {
            var result = await userService.VerifyCodeAsync(model.Email, model.Code, model.DeviceId);
            return Ok(result);
        }

        [HttpPost]
        [Route("resend-code")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> ResendCode([FromBody] ResendCodeModel model)
        {
            var result = await userService.ResendVerificationCodeAsync(model.Email);
            return Ok(result);
        }

        /// <summary>
        /// For register purpose. Role[Admin,Employee]
        /// </summary>
        /// <remarks>
        /// This API Will be used for user and admin register.
        /// </remarks>
        [HttpPost]
        [Route("register")]
        [Consumes("application/json")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            
            var result = await userService.Register(model);
            return Ok(result);
        }


        /// <summary>
        /// For updating purpose. Role[Admin,Employee]
        /// </summary>
        /// <remarks>
        /// This API Will be used for user and admin profile updation.
        /// </remarks>
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> EditProfile([FromBody] RegisterModel model,[FromHeader] string authorization)
        {
            var result = await userService.EditProfile(model, authorization);
            return Ok(result);
        }


        [HttpPost]
        [Route("resend/{email}")]
        public async Task<IActionResult> ResendEmail(string email)
        {
            var result = await userService.ResendEmail(email);
            return Ok(result);
        }


        /// <summary>
        /// For searching purpose. Role[Admin]
        /// </summary>
        /// <remarks>
        /// This API Will be used for searching a user.
        /// </remarks>
        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpGet]
        [Route("search")]
        public async Task<IActionResult> GetUsersList([FromQuery] PaginationFilter filter)
        {
            var response = await userService.GetUsers(filter);
            return Ok(response);

        }

        /// <summary>
        /// For Getting a userList. Role[Admin]
        /// </summary>
        /// <remarks>
        /// This API Will be used for getting a list of users.
        /// </remarks>
        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetAllUsersForAdmin([FromQuery] int recordNumber, [FromQuery] int pageLimit, [FromHeader] string authorization, [FromQuery] bool activeUser, [FromQuery] int month, [FromQuery] string keywords, [FromQuery] string sortDirection, [FromQuery] string filterType, [FromQuery] string filterValue, [FromQuery] string filterCategory)
        {
            DateTime endDate = DateTime.Now;
            DateTime startDate = UtilityHelper.GetStartDateByMonth(endDate, month);
            var parameter = "";

            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                parameter = headerValue.Parameter;

            var role =  _jwtUtils.GetRole(parameter);
            var userId =  _jwtUtils.ValidateToken(parameter);

            var response = await userService.GetAllUsersForAdmin(recordNumber, pageLimit,userId, role, activeUser, startDate,endDate,month,keywords,sortDirection,filterType,filterValue,filterCategory);
            return Ok(response);
        }

        /// <summary>
        /// For forget a password. Role[Admin,Employee]
        /// </summary>
        /// <remarks>
        /// This API Will be used for get a link a forget password .
        /// </remarks>
        [HttpPost]
        [Route("password/forgot")]
        public async Task<IActionResult> ForgotPassword([FromBody] MailRequest model)
        {
            var result = await userService.ForgotPassword(model);
            return Ok(result);
        }

        /// <summary>
        /// For invite a user. Role[Admin,Employee]
        /// </summary>
        /// <remarks>
        /// This API Will be used for invite a user by email.
        /// </remarks>
        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPost]
        [Route("invite")]
        public async Task<IActionResult> SendInvite([FromBody] InviteModel model)
        {
            var result = await userService.SendInvite(model);
            return Ok(result);
        }

        /// <summary>
        /// For resetting a password. Role[Admin,Employee]
        /// </summary>
        /// <remarks>
        /// This API Will be used for resetting a password for profile.
        /// </remarks>
        [HttpPost]
        [Route("password/reset")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            var result = await userService.ResetPassword(model);
            return Ok(result);
        }

        /// <summary>
        /// For update a password. Role[Admin,Employee]
        /// </summary>
        /// <remarks>
        /// This API Will be used for updating a password for profile.
        /// </remarks>
        [HttpPut]
        [Route("password/update")]
        public async Task<IActionResult> PassowrdUpdate([FromBody] ChangePasswordModel model,[FromHeader] string authorization)
        {

                var response = await userService.ChangePasswordAsync(model,authorization);
                return Ok(response);
            
        }

        /// <summary>
        /// For Deleting a user. Role[Admin]
        /// </summary>
        /// <remarks>
        /// This API Will be used for Deleting a user profile.
        /// </remarks>
        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpDelete]
        [Route("{Id}")]
        public async Task<IActionResult> DeleteUser(string Id)
        {
            var response = await userService.DeleteUserById(Id);
            return Ok(response);
        }

        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpDelete]
        [Route("delete/{Id}")]
        public async Task<IActionResult> DeleteUserById(string Id)
        {
            var response = await userService.DeleteUserByUserId(Id);
            return Ok(response);
        }

        /// <summary>
        /// For getting a details of a user profile. Role[Admin,Employee]
        /// </summary>
        /// <remarks>
        /// This API Will be used for getting a details of a user profile.
        /// </remarks>
        [Authorize]
        [HttpGet]
        [Route("details")]
        public async Task<IActionResult> GetSpecificUser([FromHeader] string authorization)
        {
                var response = await userService.GetSpecificUser(authorization);
                return Ok(response);
        }

        [HttpGet]
        [Route("confirmation")]
        public async Task<IActionResult> EmailConfirmation([FromQuery] string token, string email)
        {
            var response = await userService.EmailConfirmation(token.ToString(), email);
            return Ok(response);
        }

        /// <summary>
        /// For Adding new a user profile. Role[Admin]
        /// </summary>
        /// <remarks>
        /// This API Will be used for Adding new a user profile.
        /// </remarks>
        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPost]
        [Route("add")]
        [Consumes("application/json")]
        public async Task<IActionResult> AddUser([FromBody] RegisterModel model)
        {
            var response = await userService.AddUser(model);
            return Ok(response);
        }

        /// <summary>
        /// For getting a details of a user profile. Role[Admin,Employee]
        /// </summary>
        /// <remarks>
        /// This API Will be used for getting a details of a user profile.
        /// </remarks>
        [Authorize]
        [HttpGet]
        [Route("{id}/detail")]
        public async Task<IActionResult> GetUserById(string id,[FromQuery] int month)
        {
            DateTime endDate = DateTime.Now;
            DateTime startDate = UtilityHelper.GetStartDateByMonth(endDate, month);
            var response = await userService.GetUserDetailsById(id, endDate, startDate,month);
            var patients = await _patientService.GetPatientsByUserIdForEmployee(id, startDate, endDate, month, calculatePDCs: false);

            //   response.Data.Patients = patients.DataList;
            //foreach(Patient patient in patients.DataList)
            // {
            //     response.Data.RASAPDC = patient.RASAPDC;
            //     response.Data.CholestrolPDC = patient.CholestrolPDC;
            //     response.Data.DiabetesPDC = patient.DiabetesPDC;
            // }

            //  var CholestrolPdcPatients =  _patientService.GetPatientsByCondition(patients.DataList, PDC.Cholesterol.ToString());
            //  var RASAPdcPatients = _patientService.GetPatientsByCondition(patients.DataList, PDC.RASA.ToString());
            //  var DiabetesPdcPatients = _patientService.GetPatientsByCondition(patients.DataList, PDC.Diabetes.ToString());
            //  //Double CholestrolSum = 0.00;
            //  //Double RASASum = 0.00;
            //  //Double DiabetesSum = 0.00;
            //  int CholesterolPatientCount = 0;
            //  int RASAPatientCount = 0;
            //  int DiabetesPatientCount = 0;

            //  int CholesterolActualPatientCount = 0;
            //  int RASAActualPatientCount = 0;
            //  int DiabetesActualPatientCount = 0;
            //  for (int i = 0; i < CholestrolPdcPatients.Count; i++)
            //  {
            //      if(CholestrolPdcPatients[i].CholestrolPDC > 0)
            //      {
            //          CholesterolActualPatientCount++;
            //          if (CholestrolPdcPatients[i].CholestrolPDC >= (double)80)
            //          {
            //              CholesterolPatientCount++;
            //          }
            //      }
            //  }
            ////  CholestrolSum = Convert.ToDouble(String.Format("{0:0.0}", CholestrolSum));
            //  response.Data.CholestrolPDC = CholesterolActualPatientCount == 0 ? 0 : ((Double)CholesterolPatientCount / (Double)CholesterolActualPatientCount) *100;



            //  for (int i = 0; i < RASAPdcPatients.Count; i++)
            //  {
            //        if(RASAPdcPatients[i].RASAPDC > 0)
            //      {
            //          RASAActualPatientCount++;
            //          if (RASAPdcPatients[i].RASAPDC >= (double)80)
            //          {
            //              RASAPatientCount++;
            //          }
            //      }

            //  }
            ////  RASASum = Convert.ToDouble(String.Format("{0:0.0}", RASASum));
            //  response.Data.RASAPDC = RASAActualPatientCount == 0 ? 0 : ((Double)RASAPatientCount / (Double)RASAActualPatientCount) *100;

            //  for (int i = 0; i < DiabetesPdcPatients.Count; i++)
            //  {      
            //      if(DiabetesPdcPatients[i].DiabetesPDC > 0)
            //      {
            //          DiabetesActualPatientCount++;
            //          if (DiabetesPdcPatients[i].DiabetesPDC >= (double)80)
            //          {
            //              DiabetesPatientCount++;
            //          }
            //      }

            //  }
            ////  DiabetesSum = Convert.ToDouble(String.Format("{0:0.0}", DiabetesSum));
            //  response.Data.DiabetesPDC = DiabetesActualPatientCount == 0 ? 0 : ((Double)DiabetesPatientCount / (Double)DiabetesActualPatientCount) *100;
            //  response.Data.DiabetesPDC = Math.Round(response.Data.DiabetesPDC, 2);
            //  response.Data.CholestrolPDC = Math.Round(response.Data.CholestrolPDC, 2);
            //  response.Data.RASAPDC = Math.Round(response.Data.RASAPDC, 2);

            response.Data.NewPatient =   _patientService.countNewPatient(patients.DataList);
            response.Data.PatientInProgress =  _patientService.countInProgressPatient(patients.DataList);

            response.Data.DueForRefill = patients.DataList.Count(p => (p.CholesterolRefillDue || p.DiabetesRefillDue || p.RasaRefillDue)
                && p.Medications.Any(m => m.IsActive && m.RefillDue && new string[] { "Diabetes", "Cholesterol", "RASA" }.Contains(m.Condition) && Convert.ToInt32(m.RefillsRemaining) > 0));

            response.Data.NoRefillRemaining = patients.DataList.Count(p => (p.CholesterolRefillDue || p.DiabetesRefillDue || p.RasaRefillDue)
                && !p.Medications.Any(m => m.IsActive && m.RefillDue && new string[] { "Diabetes", "Cholesterol", "RASA" }.Contains(m.Condition) && Convert.ToInt32(m.RefillsRemaining) > 0));

            return Ok(response);
        }

        [Authorize]
        [HttpGet]
        [Route("{id}/userInfo")]
        public async Task<IActionResult> GetUserInfoById(string id)
        {
            var response = await userService.GetUserInfoById(id);

            return Ok(response);
        }


        [Authorize]
        [Produces("application/json")]
        [HttpGet]
        [Route("{id}/PDC")]
        public async Task<IActionResult> GetUserByIdForPDC(string id, [FromQuery] int month)
        {
            DateTime endDate = DateTime.Now;
            DateTime startDate = UtilityHelper.GetStartDateByMonth(endDate, month);
            var response = await userService.PDCCalculationForUser(id,startDate,endDate,month);
          
            return Ok(response);
        }


        /// <summary>
        /// Assigne pharmacy to user. Role[Admin]
        /// </summary>
        /// <remarks>
        /// This API will send list of pharmacy id as input and assign to given user. 
        /// </remarks>
        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPut]
        [Route("{id}/pharmacy")]
        public async Task<IActionResult> AssignPharmacy(string id, List<string> pharmacyIds)
        {
            var response = await userService.AssignPharmacy(id, pharmacyIds);
            return Ok(response);
        }


        [Authorize]
        [HttpGet]
        [Route("{userId}/{month}")]
        public async Task<IActionResult> GetUserByIdWithPdcCalculation(string userId, [FromQuery] int month)
        {
            DateTime endDate = DateTime.Now;
            DateTime startDate = UtilityHelper.GetStartDateByMonth(endDate, month);
            var response = await userService.GetUserByIdWithPdcCalculation(userId, startDate,endDate);
            return Ok(response);
        }

        /// <summary>
        /// Get All Assigned pharmacies for the user. Role[Admin, Employee]
        /// </summary>
        /// <remarks>
        /// This API will return List of all assigned pharamacy by UserID. 
        /// </remarks>
        [Authorize]
        [HttpGet]
        [Route("{userID}/pharmacy")]
        public async Task<IActionResult> GetPharmaciesByUserID(string userID)
        {
            var response = await _pharmacyService.GetPharmaciesByUserID(userID);
            return Ok(response);
        }

        //[HttpGet]
        //[Route("AssignedPharmacies")]
        //public async Task<IActionResult> GetPharmacy([FromHeader] string authorization)
        //{   
        //    var response = await userService.GetPharmacy(authorization);
        //    return Ok(response);
        //}


        /// <summary>
        /// For DashBoard Widget. Role[Employee]
        /// </summary>
        /// <remarks>
        /// This API Will be used for getting a details of a user DashBoard Widget.
        /// </remarks>
        [Authorize(Roles = "Employee")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [HttpGet]
        [Route("dashboard")]
        public async Task<IActionResult> dashboardWidget([FromHeader] string authorization)
        {
            DateTime endDate = DateTime.Now;
            DateTime startDate = UtilityHelper.GetStartDateByMonth(endDate, 6);
            string userId = userService.GetSpecificUser(authorization).Result.Data.Id;

            DashBoard dashboard = new DashBoard();

            //TODO: DateTime part to be resolved
            dashboard.AssignedPharmacy = _pharmacyService.GetPharmaciesByUserID(userId).Result.DataList.Count;

            List<Patient> patients = new List<Patient>();
            patients = _patientService.GetPatientsByUserId(userId,startDate,endDate,6).Result.DataList;

            dashboard.NewPatient =  _patientService.countNewPatient(patients);
            dashboard.PatientInProgress =  _patientService.countInProgressPatient(patients);
            List<Medication> medicationsList = new List<Medication>();
            List<Medication> medications = await _medicationService.getAllMedicationsByUserID(userId);
            medicationsList = medications.Where(p => p.Patient.IsDeleted == false && p.Condition != null).GroupBy(p => (p.Condition, p.Patient)).Select(p => p.LastOrDefault()).ToList();
            dashboard.DueForRefill = await _medicationService.countDueForRefill(medicationsList);
            dashboard.NoRefillRemaining = _medicationService.countNoRefillRemaining(medicationsList);

            return Ok(dashboard);
        }



        [Authorize]
        [HttpGet]
        [Route("dashboard/search")]
        public IActionResult dashboardWidgetSearchFunctionality([FromQuery] string search)
        {
            var patients =  userService.DashboardSearchPatient(search);
            var pharamcies =  userService.DashboardSearchPharamcy(search);
            Dictionary<string, List<object>> Pairs = new Dictionary<string, List<object>>();
            Pairs.Add("Patient", patients.Cast<object>().ToList()); 
            Pairs.Add("Pharmacy", pharamcies.Cast<object>().ToList()); 
            return Ok(Pairs);
        }


        /// <summary>
        /// For Graph user medication. Role[Admin,Employee]
        /// </summary>
        /// <remarks>
        /// This API Will be used for getting Graph of user medication situation.
        /// </remarks>
        [Authorize]
        [HttpGet]
        [Route("{userId}/graph")]
        public async Task<IActionResult> Graph([FromQuery] int month, string userId)
        {
            var cholesterolPdcList = new List<PdcModel>();
            var daibetesPdcList = new List<PdcModel>();
            var rasaPdcList = new List<PdcModel>();

            var range = _patientPdcService.GetMedicationPeriodForGraph(month);
            var startDate = range.Item1;
            var endDate = range.Item2;

            while (startDate <= endDate)
            {
                cholesterolPdcList.Add(await _patientPdcService.GetPdcForUserAsync(userId, PDC.Cholesterol.ToString(), startDate, month, PdcQueryType.ByEndDate));
                daibetesPdcList.Add(await _patientPdcService.GetPdcForUserAsync(userId, PDC.Diabetes.ToString(), startDate, month, PdcQueryType.ByEndDate));
                rasaPdcList.Add(await _patientPdcService.GetPdcForUserAsync(userId, PDC.RASA.ToString(), startDate, month, PdcQueryType.ByEndDate));

                startDate = startDate.AddMonths(1);
            }

            var pairs = new Dictionary<string, List<PdcModel>>
            {
                { "Cholesterol", cholesterolPdcList },
                { "Diabetes", daibetesPdcList },
                { "RASA", rasaPdcList }
            };

            return Ok(pairs);
        }

        /// <summary>
        /// For Update a notification. Role[Admin,Employee]
        /// </summary>
        /// <remarks>
        /// This API Will be used for Update a notification.
        /// </remarks>
        [Authorize]
        [HttpPut]
        [Route("Notification")]
        public async Task<IActionResult> UpdateNotification([FromHeader] string authorization, [FromQuery] Boolean isNotification )
        {
            var response = await userService.UpdateUserForNotification(authorization, isNotification);
            return Ok(response);
        }


        /// <summary>
        /// For getting a list of users Expiry. Role[Admin,Employee]
        /// </summary>
        /// <remarks>
        /// This API Will be used for getting a list of users Expiry.
        /// </remarks>
        [Authorize]
        [HttpGet]
        [Route("licenseExpiry")]
        public async Task<IActionResult> GetAllUsersLicenseExpiry()
        {
            var response = await userService.GetAllUsersForLicenseExpiry();
            return Ok(response);
        }

        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpGet]
        [Route("twilio-numbers")]
        [Produces("application/json")]
        public async Task<IActionResult> GetAvailableTwilioNumbers()
        {
            var response = await userService.GetAvailableTwilioNumbersAsync();
            return Ok(response);
        }

        [Authorize(Roles = "Admin,Employee")]
        [HttpGet]
        [Route("call-history")]
        [Produces("application/json")]
        public async Task<IActionResult> GetCallHistory([FromHeader] string authorization, [FromQuery] int recordNumber, [FromQuery] int pageLimit,[FromQuery] string filterType, [FromQuery] string filterValue)
        {
            var response = await userService.GetTwilioCallHistoryAsync(authorization,recordNumber,pageLimit,filterType,filterValue);
            return Ok(response);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        [Route("{userId}/toggle-compliance/{complianceCode}")]
        [Produces("application/json")]
        public async Task<IActionResult> ToggleCompliance(string userId, string complianceCode)
        {
            var response = await userService.ToggleComplianceAsync(userId, complianceCode);
            return Ok(response);
        }
    }
}
