using System;
using System.Collections.Generic;

namespace LMSAPI.Entities;

public partial class User
{
    public int Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? IdNumber { get; set; }

    public int? YearLevel { get; set; }

    public string? Rfid { get; set; }

    public string? Role { get; set; }

    public string? Key { get; set; }

    public string? Sex { get; set; }

    public string? MobileNumber { get; set; }
}
