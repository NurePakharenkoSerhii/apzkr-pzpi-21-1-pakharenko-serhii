using API.Core.Contexts;
using API.Core.DTOs.Guard;
using API.Core.Entities;
using API.Services.Abstractions;
using API.Utils;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Implementations;

public class GuardService(GuardicDbContext context) : IGuardService
{
    public async Task<IEnumerable<Guard>> GetGuardsAsync()
    {
        return await context.Guards.ToListAsync();
    }

    public async Task<Guard?> GetGuardAsync(int id)
    {
        return await context.Guards.FindAsync(id);
    }

    public async Task CreateGuardAsync(CreateGuardDto guard)
    {
        if (guard == null)
        {
            throw new ArgumentException(nameof(guard));
        }
        
        var (hashedPassword, salt) = PasswordHelper.HashPassword(guard.Password);
        
        await context.Guards.AddAsync(new Guard
        {
            Email = guard.Email,
            Password = hashedPassword,
            Salt = salt, 
            Name= guard.Name,
            Department = guard.Department
        });
        
        await context.SaveChangesAsync();
    }

    public async Task UpdateGuardAsync(int id, UpdateGuardDto guard)
    {
        if (guard == null)
        {
            throw new ArgumentException(nameof(guard));
        }

        var guardDb = await context.FindAsync<Guard>(id);
        
        if (guardDb == null)
        {
            throw new ArgumentException(nameof(guard));
        }

        guardDb.Email = guard.Email;
        guardDb.Name = guard.Name;
        guardDb.Department = guard.Department;
        
        var (hashedPassword, salt) = PasswordHelper.HashPassword(guard.Password);

        guardDb.Password = hashedPassword;
        guardDb.Salt = salt;
        
        context.Guards.Update(guardDb);
        
        await context.SaveChangesAsync();
    }

    public async Task<Guard?> LogInAsync(string email, string password)
    {
        var guard = await context.Guards.FirstOrDefaultAsync(u => u.Email == email);
        
        if (guard != null && PasswordHelper.IsPasswordValid(password, guard.Password, guard.Salt))
        {
            return guard;
        }

        return null;
    }

    public async Task<bool> SignUpAsync(GuardSignUpDto guardSignUpDto)
    {
        var existingGuard = await context.Users.FirstOrDefaultAsync(u => u.Email == guardSignUpDto.Email);

        if (existingGuard != null)
        {
            return false;
        }
        
        var (hashedPassword, salt) = PasswordHelper.HashPassword(guardSignUpDto.Password);
        
        var newGuard = new Guard
        {
            Email = guardSignUpDto.Email,
            Password = hashedPassword,
            Salt = salt,
            Name = guardSignUpDto.Name,
            Department = guardSignUpDto.Department
        };

        context.Guards.Add(newGuard);
        await context.SaveChangesAsync();

        return true;
    }
}
