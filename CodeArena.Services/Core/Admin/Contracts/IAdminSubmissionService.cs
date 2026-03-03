using CodeArena.Services.DTOs.Admin.Submission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Services.Core.Admin.Contracts;

public interface IAdminSubmissionService
{
    Task<IEnumerable<AdminSubmissionReviewDto>> GetPendingSubmissionsAsync();
}
