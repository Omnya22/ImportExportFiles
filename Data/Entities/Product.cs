using System.ComponentModel.DataAnnotations;

namespace ImportExportFiles.Data.Entities;
public class Product
{
    [Key]
    [Required]
    public int Id { get; set; }
    [Required]
    public string BandNumber { get; set; }
    [Required]
    public string CategoryCode { get; set; }
    [Required]
    public string Manufacturer { get; set; }
    [Required]
    public string PartSku { get; set; }
    [Required]
    public string ItemDescription { get; set; }
    [Required]
    public decimal ListPrice { get; set; }
    [Required, Range(0, 100)]
    public decimal MinDiscount { get; set; }
    [Required]
    public decimal DiscountPrice { get; set; }
}
