using CodeArena.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Data.Repositories.Contracts;

public interface IChallengeRepository
{
    IQueryable<Challenge> GetAll(bool includeDeleted = false);
    IQueryable<Challenge> GetAllTracked();
    Task<Challenge?> GetByIdAsync(int id, bool includeDeleted = false);
    Task<Challenge?> GetBySlugAsync(string slug);
    Task AddAsync(Challenge challenge);
    Task UpdateAsync(Challenge challenge);
    Task DeleteAsync(Challenge challenge);
    Task RestoreAsync(Challenge challenge); 
    Task<int> CountAsync(Expression<Func<Challenge, bool>>? predicate = null);
    Task<bool> AnyAsync(Expression<Func<Challenge, bool>>? predicate = null);
    Task SaveChangesAsync();
    Task<HashSet<string>> GetExistingSlugsAsync();

}
