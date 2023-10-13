using MealPlanner.Application.DomainModel;
using MealPlanner.Application.Services.Authorization;
using MealPlanner.Common.Authorization;
using MealPlanner.Shared.DTO.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MealPlanner.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthorizationController : ControllerBase
{
    private readonly IAuthorizationService _authorizationService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;

    internal const string RefreshTokenCookieKey = "cookie";
    internal const string RefreshTokenSessionkey = "session";

    public AuthorizationController(IAuthorizationService authorizationService, IConfiguration configuration, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _authorizationService = authorizationService;
        _configuration = configuration;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(LoginRequest request)
    {
        var user = await _authorizationService.Login(request);

        if(user == null)
        {
            return NotFound();
        }

        var token = await GenerateJwt(user);

        if (request.RememberMe)
        {
            AppendRefreshTokenCookie(user, HttpContext.Response.Cookies);
        }
        else
        {
            AppendRefreshTokenToSession(user, HttpContext.Session);
        } 

        return Ok(token);
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterRequest request)
    {
        var result = await _authorizationService.Register(request);

        if (result == null)
        {
            return BadRequest();
        }

        return Ok();
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<string>> RefreshToken()
    {
        var cookie = HttpContext.Request.Cookies[RefreshTokenCookieKey];
        var session = HttpContext.Session.GetString(RefreshTokenSessionkey);
        string? token;

        if (cookie != null)
        {
            var user = _authorizationService.CheckIfUserExists(cookie);

            if (user == null)
            {
                return NoContent();
            }

            token = await GenerateJwt(user);

            return Ok(token);
        }
        else if (session != null)
        {
            var user = _authorizationService.CheckIfUserExists(session);

            if (user == null)
            {
                return NoContent();
            }

            token = await GenerateJwt(user);

            return Ok(token);
        }

        return BadRequest();
    }

    [HttpPost("logout")]
    public ActionResult Logout()
    {
        var cookie = HttpContext.Request.Cookies[RefreshTokenCookieKey];
        var session = HttpContext.Session.GetString(RefreshTokenSessionkey);

        if(cookie != null)
        {
            var user = _authorizationService.CheckIfUserExists(cookie);

            if (user == null)
            {
                return NoContent();
            }

            HttpContext.Response.Cookies.Delete(RefreshTokenCookieKey);

            return Ok();
        }
        else if(session != null)
        {
            var user = _authorizationService.CheckIfUserExists(session);

            if (user == null)
            {
                return NoContent();
            }

            HttpContext.Session.Remove(RefreshTokenSessionkey);

            return Ok();
        }

        return NoContent();
    }

    private async Task<string> GenerateJwt(ApplicationUser user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");

        var secretkey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
        var credentials = new SigningCredentials(secretkey, SecurityAlgorithms.HmacSha256);

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles
            .FirstOrDefault()!
            .ToString();

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString()),
            new Claim(AuthorizationPolicyRegistration.AuthorizationClaimType, role)
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static void AppendRefreshTokenCookie(ApplicationUser user, IResponseCookies cookies)
    {
        var options = new CookieOptions();
        options.HttpOnly = true;
        options.Secure = true;
        options.SameSite = SameSiteMode.Strict;
        options.Expires = DateTime.Now.AddMinutes(60);
        cookies.Append(RefreshTokenCookieKey, user.SecurityStamp!, options);
    }

    private static void AppendRefreshTokenToSession(ApplicationUser user, ISession session)
    {
        session.SetString(RefreshTokenSessionkey, user.SecurityStamp!);
    }
}
