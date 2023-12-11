namespace POS.Application.Models.Entities;

public class CategoryVM : IEntityVM
{
    public long Id { get; set; }
    public string CategoryName { get; set; } = String.Empty;
    public string CategoryDescription { get; set; } = String.Empty;
}