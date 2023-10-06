﻿using Domain.Entities;
using Domain.Entities.Feedback;

namespace Domain.DTO.Correction;

public class CorrectionUpdateRequest
{
    public string OwnerId { get; set; }
    public string ExerciseId { get; set; }
    public List<CorrectionItem> Corrections { get; set; }
}
