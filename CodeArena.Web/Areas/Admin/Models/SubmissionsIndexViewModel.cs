using CodeArena.Services.DTOs.Admin.Submission;

namespace CodeArena.Web.Areas.Admin.Models;

public class SubmissionsIndexViewModel
{
    public IEnumerable<SubmissionDisplayDto> Submissions {  get; set; } 
        = new List<SubmissionDisplayDto>();
}
