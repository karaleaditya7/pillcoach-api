using OntrackDb.Entities;
using System.Threading.Tasks;

namespace OntrackDb.Repositories
{
    public interface IReactionData
    {
        Task<Reaction> AddReaction(Reaction reaction,bool commitChanges);
      
        Task<Reaction> GetReactionByName(string PatientReaction);
    }
}
