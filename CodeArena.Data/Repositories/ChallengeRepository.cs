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

    public async Task<bool> AnyAsync(Expression<Func<Challenge, bool>>? predicate = null)
    {
        return predicate is null
            ? await _context.Challenges.AnyAsync()
            : await _context.Challenges.AnyAsync(predicate);
    }

    public Task<int> CountAsync(Expression<Func<Challenge, bool>>? predicate = null)
    {
        return predicate is null 
            ? _context.Challenges.CountAsync()
            : _context.Challenges.CountAsync(predicate);
    }

    public async Task DeleteAsync(Challenge challenge)
    { 
        challenge.IsDeleted = true;
        await _context.SaveChangesAsync();
    }

    public IQueryable<Challenge> GetAll(bool includeDeleted = false) => includeDeleted
        ? _context.Challenges.IgnoreQueryFilters().AsNoTracking()
        : _context.Challenges.AsNoTracking();

    public IQueryable<Challenge> GetAllTracked() => _context.Challenges;

    public async Task<Challenge?> GetByIdAsync(int id, bool includeDeleted = false)
    {
        var query = includeDeleted
        ? _context.Challenges.IgnoreQueryFilters()
        : _context.Challenges;

        return await query
            .Include(c => c.Submissions)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Challenge?> GetBySlugAsync(string slug)
    {
        return await _context.Challenges
            .Include(c => c.Submissions)
            .FirstOrDefaultAsync(c => c.Slug.ToLower() == slug.ToLower());
    }

    public async Task<HashSet<string>> GetExistingSlugsAsync()
    {
        var slugs = await GetAll()
                            .Where(c => !string.IsNullOrWhiteSpace(c.Slug))
                            .Select(c => c.Slug)
                            .ToListAsync();

        return new HashSet<string>(slugs);
    }

    public async Task RestoreAsync(Challenge challenge)
    {
        challenge.IsDeleted = false;
        await _context.SaveChangesAsync();
    }

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

    public async Task UpdateAsync(Challenge challenge)
    {
        _context.Challenges.Update(challenge);
        await _context.SaveChangesAsync();
    }
}
