using CodeArena.Data.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Services.DTOs.Admin.Challenge;

public sealed record EditChallengeDto(
    int Id,
    string Title,
    string Description,
    Difficulty Difficulty,
    string Tags);

