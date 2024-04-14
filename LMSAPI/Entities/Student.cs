using System;
using System.Collections.Generic;

namespace LMSAPI.Entities;

public partial class Student
{
    public int Id { get; set; }

    public string? IdNumber { get; set; }

    public string? FirstName { get; set; }

    public string? MiddleName { get; set; }

    public string? LastName { get; set; }

    public string? Category { get; set; }

    public string? Sex { get; set; }

    public string? MobileNumber { get; set; }
}
