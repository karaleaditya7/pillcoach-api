using Microsoft.AspNetCore.Mvc;
using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace OntrackDb.Service
{
    public interface IPatientMailListService
    {
        Task<Response<PatientMailList>> AddPatientMailList(PatientMailListModel model);

        Task<Response<PatientMailList>> GetPatientMailLists(int recordNumber, int pageLimit, string authorizatioin,string keywords,string sortDirection, string filterType, string filterValue, string filterCategory);

        Task<Response<PatientMailList>> UpdatePatientMailList(PatientMailListModel model);

        Task<Response<PatientMailList>> DeletePatientMailListById(int id);

        Task<Response<PatientMailList>> GetPatientMailListForLatestCompletedDate(int id, string type);

        Task<Response<PatientMailList>> GetPatientMailListById(int id);
    }
}
