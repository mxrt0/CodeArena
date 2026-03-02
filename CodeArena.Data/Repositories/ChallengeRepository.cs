using CodeArena.Data.Models;
using CodeArena.Data.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Data.Repositories;

public class ChallengeRepository : IChallengeRepository
{
    private readonly ApplicationDbContext _context;

    public ChallengeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task AddAsync(Challenge challenge)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<Challenge?> GetByIdAsync(int id)
    {
        return await _context.Challenges
            .AsNoTracking()
            .Include(c => c.Submissions)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Challenge>> GetChallengesAsync()
    {
        var query = _context.Challenges
            .AsNoTracking()
            .Include(c => c.Submissions)
            .OrderBy(c => c.Difficulty);

        return await query.ToListAsync();
    }

    public Task UpdateAsync(Challenge challenge)
    {
        throw new NotImplementedException();
    }
}
