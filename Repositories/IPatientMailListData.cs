using OntrackDb.Authentication;
using OntrackDb.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OntrackDb.Repositories
{
    public interface IPatientMailListData
    {
        Task<PatientMailList> AddPatientMailList(PatientMailList patientMailList);
        Task<List<PatientMailList>> GetPatientMailLists(string keywords, string sortDirection, string filterType, string filterValue, string filterCategory);
        Task<PatientMailList> GetPatientMailListById(int id);
        Task<PatientMailList> UpdatePatientMailList(PatientMailList patientMailList);
        Task<PatientMailList> GetPatientMailListForCompletedDate(int id, string type);

        Task<List<User>> GetAdminUsersListByPharmacyId(int id);
        Task<List<int>> GetPharmacyIdsByUserId(string userId);



    }
}
