using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OntrackDb.Authentication;
using OntrackDb.Context;
using OntrackDb.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace OntrackDb.Repositories
{
    public class PatientMailListData : IPatientMailListData
    {
        private readonly ApplicationDbContext _applicationDbcontext;
        private readonly UserManager<User> _userManager;
        public PatientMailListData(ApplicationDbContext applicationDbcontext, UserManager<User> userManager)
        {
            _applicationDbcontext = applicationDbcontext;    
            _userManager = userManager; 
        }

        public async Task<PatientMailList> AddPatientMailList(PatientMailList patientMailList)
        {
            var result = await _applicationDbcontext.PatientMailLists.AddAsync(patientMailList);
            await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }


        public async Task<List<User>> GetAdminUsersListByPharmacyId(int id)
        {
            List<User> users = new List<User>();
            var result = await _applicationDbcontext.PharmacyUsers.Where(p=>p.PharmacyId == id ).Select(p=>p.User).ToListAsync();
           
            foreach(User user in result)
            {
                user.RoleList = await _userManager.GetRolesAsync(user);
                if (user.RoleList.Contains("Admin"))
                {
                    users.Add(user);
                }
            }

            return users;
        }

        public async Task<List<int>> GetPharmacyIdsByUserId(string userId)
        {
            
            var result = await _applicationDbcontext.PharmacyUsers.Where(p => p.User.Id == userId).Select(p => p.Pharmacy.Id).ToListAsync();

            return result;
        }


        public async Task<List<PatientMailList>> GetPatientMailLists(string keywords,string sortDirection, string filterType, string filterValue, string filterCategory)
        {
            List<PatientMailList> patientMailLists = null;
            bool condition = false;
            DateTime searchDateOfBirth;
            bool isDateOfBirthValid = DateTime.TryParseExact(keywords, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out searchDateOfBirth);
            if (sortDirection == "asc")
            {
                condition = true;
            }
            if (!string.IsNullOrEmpty(filterType))
            {
                switch (filterType)
                {
                    case "CMR":
                        patientMailLists = await _applicationDbcontext.PatientMailLists.Include(p => p.Address)
                       .Where(p => !p.IsDeleted && p.SentType == "Mail" && p.type == "CMR")
                       .ToListAsync(); 
                        break;
                    case "MedRec":
                        patientMailLists = await _applicationDbcontext.PatientMailLists.Include(p => p.Address)
                   .Where(p => !p.IsDeleted  && p.SentType == "Mail" && p.type == "MedRec")
                   .ToListAsync();
                        break;
                    case "Organization":
                        patientMailLists =  _applicationDbcontext.PatientMailLists.Include(p => p.Address).AsEnumerable()
                    .Where(p => !p.IsDeleted && p.SentType == "Mail" && (p.PharmacyName.ToLower().Contains(filterValue.ToLower())))
                    .ToList();
                        break;
                }
                return patientMailLists;
            }
            else
            {
                 patientMailLists = await _applicationDbcontext.PatientMailLists.Include(p => p.Address)
                .Where(p => !p.IsDeleted && p.SentType == "Mail" && (keywords == null || keywords == string.Empty || p.PatientName.ToLower().Contains(keywords.ToLower())) || (isDateOfBirthValid && p.DateOfBirth.Date == searchDateOfBirth.Date))
                .OrderBy(p => condition ? p.PatientName : null)
                .ThenByDescending(p => condition ? null : p.PatientName).ToListAsync();
                return patientMailLists;
            }
            
        }

        public async Task<PatientMailList> GetPatientMailListById(int id)
        {

            var patientMailList = await _applicationDbcontext.PatientMailLists.Include(x => x.Address).
            SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted);


            return patientMailList;
        }

        public async Task<PatientMailList> GetPatientMailListForCompletedDate(int id, string type)
        {

            //var patientMailList = await _applicationDbcontext.PatientMailLists.Include(x => x.Address).
            //Where(x => x.PatientId == id  && x.type == type).OrderByDescending(x=>x.CreatedDate).FirstOrDefaultAsync();

            var patientMailList = await _applicationDbcontext.PatientMailLists.Include(x => x.Address).
            Where(x => x.PatientId == id && x.type == type).OrderByDescending(x => x.Id).FirstOrDefaultAsync();

            return patientMailList;
        }

        public async Task<PatientMailList> UpdatePatientMailList(PatientMailList patientMailList)
        {

            var result = _applicationDbcontext.PatientMailLists.Update(patientMailList);
            await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }

    }
}
