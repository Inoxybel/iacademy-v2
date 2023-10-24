﻿using CrossCutting.Enums;

namespace Domain.Entities.Contents;

public class SubcontentHistory
{
    public string Content { get; set; }
    public TextGenre TextGenre { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime DisabledDate { get; set; }
}