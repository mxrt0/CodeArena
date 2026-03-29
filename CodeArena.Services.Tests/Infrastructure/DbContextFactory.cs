using CodeArena.Data;
using CodeArena.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Services.Tests.Infrastructure;

public static class DbContextFactory
{
    public static ApplicationDbContext Create()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new(options);
    }
    public static async Task<ApplicationDbContext> CreateWithDataAsync<T>(IEnumerable<T> data) 
        where T : class
    {
        var context = Create();
        var entityType = context.Model.FindEntityType(typeof(T));
        if (entityType is null)
        {
            throw new InvalidOperationException($"{typeof(T).Name} is not part of the EF model.");
        }
        await context.Set<T>().AddRangeAsync(data);
        await context.SaveChangesAsync();
        return context;
    }
}
