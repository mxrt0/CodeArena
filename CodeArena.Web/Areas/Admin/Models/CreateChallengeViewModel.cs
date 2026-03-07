using CodeArena.Services.DTOs.Admin.Challenge;

namespace CodeArena.Web.Areas.Admin.Models;

public class CreateChallengeViewModel
{
    public CreateChallengeDto Challenge { get; set; } = null!;
}
