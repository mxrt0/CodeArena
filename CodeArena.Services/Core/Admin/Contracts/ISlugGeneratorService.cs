using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Services.Core.Admin.Contracts;

public interface ISlugGeneratorService
{
    Task<string> GenerateUniqueSlugAsync(string input);
}
