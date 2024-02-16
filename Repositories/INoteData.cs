using System;
using System.Threading.Tasks;
using OntrackDb.Entities;

namespace OntrackDb.Repositories
{
    public interface INoteData
    {

        Task<Note> AddPatientNote(Note note);
        Task<Note> UpdateNote(Note note);
        Task<Note> GetPatientNoteById(int noteId);
        Task<Note> GetNoteById(int noteId);

        Task<Note> AddPharmacyNote(Note note);
        Task<Note> GetPharmacyNoteById(int noteId);
        Task<int> UpdatePharmacyNote(Note note);
    }
}
