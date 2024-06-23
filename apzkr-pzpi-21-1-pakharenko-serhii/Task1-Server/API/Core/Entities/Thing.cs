namespace API.Core.Entities;

public class Thing : BaseEntity
{
    public string Name { get; set; }

    public string Description { get; set; }

    public string Effect { get; set; }

    public string Interactions { get; set; }

    public DateTime ExpirationDate { get; set; }

    public List<Order> Orders { get; set; }
}