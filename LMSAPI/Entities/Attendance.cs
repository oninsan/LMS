using System;
using System.Collections.Generic;

namespace LMSAPI.Entities;

public partial class Attendance
{
    public int Id { get; set; }

    public string? IdNumber { get; set; }

    public DateTime? AttendanceDate { get; set; }

    public TimeSpan? TimeIn { get; set; }

    public TimeSpan? TimeOut { get; set; }

    public string? Location { get; set; }
}
