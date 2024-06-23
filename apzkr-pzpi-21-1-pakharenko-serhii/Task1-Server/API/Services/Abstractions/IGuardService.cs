using API.Core.DTOs.Guard;
using API.Core.Entities;

namespace API.Services.Abstractions;

public interface IGuardService
{
    Task<IEnumerable<Guard>> GetGuardsAsync();
    Task<Guard?> GetGuardAsync(int id);
    Task CreateGuardAsync(CreateGuardDto guard);
    Task UpdateGuardAsync(int id, UpdateGuardDto guard);
    Task<Guard?> LogInAsync(string email, string password);
    Task<bool> SignUpAsync(GuardSignUpDto guardSignUpDto);
}
