using CodeArena.Data.Models;
using CodeArena.Data.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

    public async Task AddAsync(Challenge challenge)
    {
        await _context.AddAsync(challenge);
        await _context.SaveChangesAsync();
    }

    public Task<int> CountAsync(Expression<Func<Challenge, bool>>? predicate = null)
    {
        return predicate is null 
            ? _context.Challenges.CountAsync()
            : _context.Challenges.CountAsync(predicate);
    }

    public async Task DeleteAsync(int id)
    {
        var challenge = await _context.Challenges.FindAsync(id);
        if (challenge is null) return;

        _context.Remove(challenge);
        await _context.SaveChangesAsync();
    }

    public IQueryable<Challenge> GetAll() => _context.Challenges.AsNoTracking();

    public async Task<Challenge?> GetByIdAsync(int id)
    {
        return await _context.Challenges
            .Include(c => c.Submissions)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task UpdateAsync(Challenge challenge)
    {
        _context.Challenges.Update(challenge);
        await _context.SaveChangesAsync();
    }
}
