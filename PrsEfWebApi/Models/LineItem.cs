using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace PrsEfWebApi.Models;

[Table("LineItem")]
[Index("RequestId", "ProductId", Name = "UQ_LineItem_Req_Prod", IsUnique = true)]
public partial class LineItem
{
    [Key]
    public int Id { get; set; }

    public int RequestId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }


    [ForeignKey("ProductId")]
    [InverseProperty("LineItems")]
    public virtual Product? Product { get; set; } = null!;

    [ForeignKey("RequestId")]

  
    [InverseProperty("LineItems")]
    public virtual Request? Request { get; set; } = null!;
}
