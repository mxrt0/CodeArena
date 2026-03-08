using CodeArena.Services.DTOs.Submission;

namespace CodeArena.Web.Models.Submission;

public class SubmissionsIndexViewModel
{
    public IEnumerable<SubmissionDisplayDto> Submissions { get; set; }
                = new List<SubmissionDisplayDto>();
}
