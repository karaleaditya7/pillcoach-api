using Microsoft.EntityFrameworkCore;
using OntrackDb.Context;
using OntrackDb.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace OntrackDb.Repositories
{
    public class ReactionData :IReactionData
    {
        private readonly ApplicationDbContext _applicationDbcontext;
        public ReactionData(ApplicationDbContext applicationDbcontext)
        {
            _applicationDbcontext = applicationDbcontext;   
        }
        public async Task<Reaction> AddReaction(Reaction reaction,bool commitChanges)
        {
            var result = await _applicationDbcontext.Reactions.AddAsync(reaction);
            if(commitChanges) await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }


        public async Task<Reaction> GetReactionByName(string PatientReaction)
        {
            var reaction = await _applicationDbcontext.Reactions.Where(m => m.Name == PatientReaction).
                                            FirstOrDefaultAsync();

            return reaction;
        }
    }
}
