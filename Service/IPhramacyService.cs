using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OntrackDb.Service
{
    public interface IPhramacyService
    {

        Task<Response<Pharmacy>> GetPharmacies(DateTime startDate, DateTime endDate);

        Task<Response<Pharmacy>> AddPharmacy(Model.PharmacyModel model);

        Task<Response<Pharmacy>> UpdatePharmacy(PharmacyModel pharmacy);
        Task <Response<Pharmacy>> DeletePharmacy(int pharmacyId);

        Task<Response<Pharmacy>> GetPharmacyById(int id, DateTime startDate, DateTime endDate);
        Task<Response<Note>> AddPharmacyNote(NoteModel noteModel,int pharmacyId);
        Task<Response<Note>> GetPharmacyNoteById(int pharmacyId);
        Task<Response<Note>> UpdatePharmacyNote(NoteModel noteModel);
        Task<Response<Pharmacy>> GetPharmaciesByUserId(string userId, DateTime startDate, DateTime endDate);
        Task<Pharmacy> CalucalateAvergaePdcForPharmacy(Pharmacy pharmacy, DateTime startDate, DateTime endDate);
    }
}
