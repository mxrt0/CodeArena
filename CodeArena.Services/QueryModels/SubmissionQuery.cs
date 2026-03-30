using CodeArena.Common.Enums;
using CodeArena.Data.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Services.QueryModels;

public class SubmissionQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public SubmissionLanguage? Language { get; set; }
    public SubmissionStatus? Status { get; set; }
}
