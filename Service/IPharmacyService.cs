using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace OntrackDb.Service
{
    public interface IPharmacyService
    {

        Task<Response<Pharmacy>> GetPharmacies(int recordNumber, int pageLimit, DateTime startDate, DateTime endDate, int month, string keywords, string sortDirection, string filterType, string filterValue, string filterCategory);

        Task<Response<Pharmacy>> AddPharmacy(Model.PharmacyModel model, string initUserId);

        Task<Response<Pharmacy>> UpdatePharmacy(PharmacyModel pharmacy);
        Task <Response<List<string>>> DeletePharmacy(int pharmacyId);

        Task<Response<Pharmacy>> GetPharmacyById(int id, DateTime startDate, DateTime endDate,int month);
        Task<Response<Note>> AddPharmacyNote(NoteModel noteModel,int pharmacyId);
        Task<Response<Note>> GetPharmacyNoteById(int pharmacyId);
        Task<Response<Note>> UpdatePharmacyNote(NoteModel noteModel);
        Task<Response<Pharmacy>> GetPharmaciesByUserID(string userId);
        Task<PharmacyDto> CalucalateAvergaePdcForPharmacy(int pharmacyId, DateTime startDate, DateTime endDate, int month);
        Task<Pharmacy> CalucalateAvergaePdcForPharmacyGraph(Pharmacy pharmacy, DateTime startDate, DateTime endDate);
        Task<Response<Pharmacy>> GetPharmacyByIdForGraph(int id, DateTime startDate, DateTime endDate);
     
        Task<Response<PharmacyDto>> GetPharmacyByIdForPDCForDto(int id, DateTime startDate, DateTime endDate, int month);
        Task<Pharmacy> GetPharmacyByNpiNumber(string npiNumber);

        Task<Response<Pharmacy>> GetPharmaciesByUserIDWithPagination(int pageRecords, int pageLimit, string userId, DateTime startDate, DateTime endDate, int month,string keywords, string sortDirection, string filterType, string filterValue, string filterCategory);
        Task<Response<Pharmacy>> GetAllPharmacyNames();
        Task DeleteTwilioConversations(List<string> contactId);
    }
}
