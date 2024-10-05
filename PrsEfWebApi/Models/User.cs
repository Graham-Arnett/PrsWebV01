using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace PrsEfWebApi.Models;

[Table("User")]
[Index("Email", Name = "UQ_User_Email", IsUnique = true)]
[Index("FirstName", "LastName", "PhoneNumber", Name = "UQ_User_Person", IsUnique = true)]
[Index("Username", Name = "UQ_User_Username", IsUnique = true)]
public partial class User
{
    [Key]
    public int Id { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string Username { get; set; } = null!;

    [StringLength(10)]
    [Unicode(false)]
    public string Password { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string FirstName { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string LastName { get; set; } = null!;

    [StringLength(12)]
    [Unicode(false)]
    public string PhoneNumber { get; set; } = null!;

    [StringLength(75)]
    [Unicode(false)]
    public string Email { get; set; } = null!;

    [Column("REVIEWER")]
    public bool Reviewer { get; set; }

    [Column("ADMIN")]
    public bool Admin { get; set; }
    [JsonIgnore]
    [InverseProperty("User")]
    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
}
