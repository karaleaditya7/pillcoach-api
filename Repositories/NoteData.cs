using System;
using System.Threading.Tasks;
using OntrackDb.Context;
using OntrackDb.Entities;

namespace OntrackDb.Repositories
{
    public class NoteData:INoteData
    {
       
        private readonly ApplicationDbContext _applicationDbcontext;

        public NoteData(ApplicationDbContext applicationDbcontext)
        {
            _applicationDbcontext = applicationDbcontext;
        }

        public async Task<Note> AddPatientNote(Note note)
        {
            var result = await _applicationDbcontext.Notes.AddAsync(note);
            await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<Note> AddPharmacyNote(Note note)
        {
            var result = await _applicationDbcontext.Notes.AddAsync(note);
            await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }


        public async Task<Note> GetNoteById(int noteId)
        {
            var noteExists = await _applicationDbcontext.Notes.FindAsync(noteId);
            return noteExists;
        }


        public async Task<Note> GetPatientNoteById(int noteId)
        {
            var noteExists = await _applicationDbcontext.Notes.FindAsync(noteId);
            return noteExists;
        }

        public async Task<Note> GetPharmacyNoteById(int noteId)
        {
            var noteExists = await _applicationDbcontext.Notes.FindAsync(noteId);
            return noteExists;
        }

        public async Task<Note> UpdateNote(Note note)
        {
            var result = _applicationDbcontext.Notes.Update(note);
            await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<int> UpdatePharmacyNote(Note note)
        {
            var result = await _applicationDbcontext.SaveChangesAsync();
            return result;
        }

    }
}
