using AuthCustomDb.Db;
using AuthCustomDb.Enums;
using AuthCustomDb.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthCustomDb.Controllers;

[Route("[controller]")]
public class SimpleController : Controller
{
    private readonly ITokenGenerator _tokenGenerator;
    private readonly UserDb _userDb;

    public SimpleController(ITokenGenerator tokenGenerator, UserDb userDb)
    {
        _userDb = userDb;
        _tokenGenerator = tokenGenerator;
    }

    [Authorize(Roles = "Customer")]
    [HttpGet("test")]
    public IActionResult AuthEndPoint()
    {
        return Ok();
    }

    [HttpPost("CreateUser")]
    public async Task<IActionResult> Post([FromQuery] string email, [FromQuery] string password)
    {
        if (_userDb.Users.Any(w => w.Email == email.ToLower()))
        {
            return BadRequest();
        }

        _userDb.Users.Add(new Db.User
        {
            Email = email.ToLower(), Password = BCrypt.Net.BCrypt.HashPassword(password),
            UserRoles = new List<UserRole>() { new UserRole { RoleId = (int)Roles.Customer } }
        });
        await _userDb.SaveChangesAsync();

        return Created();
    }

    [HttpGet]
    public async Task<IActionResult> Auth([FromQuery] string email, [FromQuery] string password)
    {
        var user = await _userDb.Users.FirstOrDefaultAsync(w => w.Email == email.ToLower());
        if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
        {
            return BadRequest();
        }

        var roles = user.UserRoles.Select(w => w.Role.Name).ToArray();
        return Ok(new
            { Token = _tokenGenerator.BuildToken(new TokenGeneratorClaims(email, Guid.NewGuid(), roles)) });
    }
}