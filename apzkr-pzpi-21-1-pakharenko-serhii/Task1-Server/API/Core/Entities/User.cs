namespace API.Core.Entities;

public class User : BaseEntity
{
    public string Fullname { get; set; }
    
    public string Password { get; set; }
    
    public string Salt { get; set; }
    
    public string Email { get; set; }
    
    public List<Assignment> Assignments { get; set; }
}