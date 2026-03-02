using CodeArena.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Data.Repositories.Contracts;

public interface IChallengeRepository
{
    Task<IEnumerable<Challenge>> GetChallengesAsync();
    Task<Challenge?> GetByIdAsync(int id);
    Task AddAsync(Challenge challenge);
    Task UpdateAsync(Challenge challenge);
    Task DeleteAsync(int id);
}
