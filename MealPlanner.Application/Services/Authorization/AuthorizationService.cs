using MealPlanner.Application.DomainModel;
using MealPlanner.Shared.DTO.Authorization;
using MealPlanner.Infrastructure.Extensions;
using MealPlanner.Infrastructure.Security;
using Microsoft.AspNetCore.Identity;
using MealPlanner.Application.Data;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace MealPlanner.Application.Services.Authorization;

public class AuthorizationService : IAuthorizationService
{
    private readonly ApplicationDbContext _appContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthorizationService(UserManager<ApplicationUser> userManager, ApplicationDbContext appContext)
    {
        _appContext = appContext;
        _userManager = userManager;
    }

    public async Task<ApplicationUser?> Login(LoginRequest request)
    {
        if(request == null)
        {
            throw new ArgumentException("Request is not valid");
        }

        if (request.Email.IsNullOrWhiteSpace())
        {
            throw new ArgumentNullException("Email is not valid.");
        }

        if (request.Password.IsNullOrWhiteSpace())
        {
            throw new ArgumentException("Password is not valid");
        }

        try
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email!);

            if (existingUser == null)
            {
                throw new ArgumentException("User does not exist.");
            }

            var salt = existingUser.Salt;

            var encryption = new Encryption();

            var userPasswordToValidate = GeneratePasswordHash(encryption, request.Password!, salt);

            if (!userPasswordToValidate.Equals(existingUser.PasswordHash))
            {
                throw new ArgumentException("Password is not correct.");
            }

            return existingUser;
        }
        catch (Exception ex)
        {
            Console.Write(ex.Message);
        }

        return null;
    }

    public ApplicationUser? CheckIfUserExists(string cookie)
    {
        var user = _userManager.Users
            .FirstOrDefault(x => x.SecurityStamp == cookie);

        if(user == null)
        {
            return null;
        }

        return user;
    }

    public async Task<ApplicationUser?> Register(RegisterRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException("Request is not valid.");
        }

        if (request.Email.IsNullOrWhiteSpace())
        {
            throw new ArgumentNullException("Email is not valid.");
        }

        if (request.Password.IsNullOrWhiteSpace())
        {
            throw new ArgumentException("Password is not valid");
        }

        if(request.Username.IsNullOrWhiteSpace())
        {
            throw new ArgumentException("Username is not valid");
        }
        try
        {
            var existingUser = await _userManager.FindByNameAsync(request.Username!);

            if (existingUser != null)
            {
                throw new ArgumentException("Email already exists");
            }

            var encryption = new Encryption();

            byte[] salt = encryption.GenerateSalt();

            var password = GeneratePasswordHash(encryption, request.Password!, salt);

            var user = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.Username,
                PasswordHash = password,
                DateOfBirth = request.DateOfBirth,
                Salt = salt
            };

            await _userManager.CreateAsync(user);

            await _appContext.SaveChangesAsync();

            return user;
        }
        catch (Exception ex)
        {
            Console.Write(ex.Message);
        }

        return null;
    }

    internal string GeneratePasswordHash(Encryption encryption, string password, byte[] salt)
    {
        byte[] hashedPasswordBytes = encryption.HashPassword(password, salt);

        var hashedPassword = BitConverter.ToString(hashedPasswordBytes);

        return hashedPassword;
    }
}
