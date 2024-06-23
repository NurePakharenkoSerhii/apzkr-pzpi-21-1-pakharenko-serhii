namespace API.Core.Entities;

public class Guard : BaseEntity
{
    public string Name { get; set; }
    
    public string Department { get; set; }
    
    public string Email { get; set; }
    
    public string Password { get; set; }
    
    public string Salt { get; set; }
    
    public List<Assignment> Assignments { get; set; }
}