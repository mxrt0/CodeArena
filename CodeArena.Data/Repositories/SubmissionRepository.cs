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

public class SubmissionRepository : ISubmissionRepository
{
    private readonly ApplicationDbContext _context;

    public SubmissionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Submission submission)
    {
        _context.Submissions.Add(submission);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> AnyAsync(Expression<Func<Submission, bool>> predicate)
    {
        return await _context.Submissions.AnyAsync(predicate);
    }

    public async Task<Submission?> FirstOrDefaultAsync(Expression<Func<Submission, bool>> predicate)
    {
        return await _context.Submissions.FirstOrDefaultAsync(predicate);
    }

    public IQueryable<Submission> GetAll() => _context.Submissions.AsNoTracking();

    public async Task RemoveAsync(Submission submission)
    {
        _context.Submissions.Remove(submission);
        await _context.SaveChangesAsync();
    }
}
