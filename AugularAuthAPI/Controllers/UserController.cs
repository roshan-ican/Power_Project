using AugularAuthAPI.Context;
using AugularAuthAPI.Helpers;
using AugularAuthAPI.Models;
using AugularAuthAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace AugularAuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _authContext;

        // Constructor to inject the AppDbContext instance
        public UserController(AppDbContext appDbContext)
        {
            _authContext = appDbContext;
        }

        // API endpoint for user authentication
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] User userObj)
        {
            // Check if the request body contains a User object
            if (userObj == null)
                return BadRequest();
            // Try to find a user in the database that matches the provided username and password
            var user = await _authContext.Users.FirstOrDefaultAsync(x => x.Username == userObj.Username);

            // If no user is found, return a "User Not Found" response
            if (user == null)
                return NotFound(new { message = "User Not Found!" });

            if (!PasswordHasher.VerifyPassword(userObj.Password, user.Password))
            {
                return BadRequest(new {Message = "Password is incorrect"});
            }

            user.Token = CreateJwt(user);
            var newAccessToken = user.Token;
            var newRefreshToken = CreateRefreshToken();
            user.RefreshToken = newAccessToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(1); 
            await _authContext.SaveChangesAsync();
            // If the user is found, return a successful response indicating successful login
            return Ok(new TokenApiDto()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }

        // API endpoint for user registration
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] User userObj)
        {
            // Check if the request body contains a User object
            if (userObj == null)
                return BadRequest();

            //Check if username exists in the db table
            if(await CheckUserNameExistAsync(userObj.Username))
                return BadRequest(new {Message = " Username already exists"});

            //Check if email exists in the db table
            if (await CheckEmailExistAsync(userObj.Email))
                return BadRequest(new { Message = " Email already exists" });

            //Check if password is strong or not
            var pass = CheckPasswordStrength(userObj.Password);
            if(!string.IsNullOrEmpty(pass))
                return BadRequest(new {  message = pass.ToString() });



            //Hash The Password before saving
            userObj.Password = PasswordHasher.HashPassword(userObj.Password);
            userObj.Role = "User";
            userObj.Token = "";

            // Add the user to the database
            await _authContext.Users.AddAsync(userObj);
            await _authContext.SaveChangesAsync();

            // Return a successful response indicating successful user registration
            return Ok(new
            {
                Message = "User Registered!"
            });
        }

        private async Task<bool> CheckUserNameExistAsync(string username)
        {
            return await _authContext.Users.AnyAsync(x => x.Username == username);
        }

        private async Task<bool> CheckEmailExistAsync(string email)
        {
            return await _authContext.Users.AnyAsync(x => x.Email == email);
        }

        private string CheckPasswordStrength(string password)
        {
            StringBuilder sb = new StringBuilder();

            if (password.Length < 8)
                sb.Append("Minimum password length should be at least 8 characters." + Environment.NewLine);

            if (!Regex.IsMatch(password, "[a-z]"))
                sb.Append("Password should contain at least one lowercase letter." + Environment.NewLine);

            if (!Regex.IsMatch(password, "[A-Z]"))
                sb.Append("Password should contain at least one uppercase letter." + Environment.NewLine);

            if (!Regex.IsMatch(password, "[0-9]"))
                sb.Append("Password should contain at least one digit." + Environment.NewLine);

            if (!Regex.IsMatch(password, "[^a-zA-Z0-9]"))
                sb.Append("Password should contain at least one special character." + Environment.NewLine);
            else
            {
                int specialCharCount = 0;

                // Loop through each character in the password
                foreach (char c in password)
                {
                    // Check if the character is a special character (not a letter or digit)
                    if (!char.IsLetterOrDigit(c))
                        specialCharCount++;
                }

                // Check if the number of special characters is less than 2
                if (specialCharCount < 2)
                    sb.Append("Password should contain at least two special characters." + Environment.NewLine);
            }

            return sb.ToString();
        }

        private string CreateRefreshToken()
        {
            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var refreshToken = Convert.ToBase64String(tokenBytes);
            var tokenInUser = _authContext.Users.Any(a => a.RefreshToken == refreshToken);
            if (tokenInUser)
            {
                return CreateRefreshToken();
            }
            return refreshToken;

        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var key = Encoding.ASCII.GetBytes("veryverysecret.....");
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = false,
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters,out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("This is a invalid Token");
            return principal;
        }

        private string CreateJwt(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("veryverysecret.....");
            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Name, $"{user.Username}"),
            });

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddSeconds(10),
                SigningCredentials = credentials,
            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<User>> GetAllUsers()
        {
            return Ok (await _authContext.Users.ToListAsync());
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(TokenApiDto tokenApiDto)
        {
            if (tokenApiDto == null)
                return BadRequest("Invalid Client Request");
            string accessToken = tokenApiDto.AccessToken;
            string refreshToken = tokenApiDto?.RefreshToken;
            var principal = GetPrincipalFromExpiredToken(accessToken);
            var username = principal.Identity.Name;
            var user = await _authContext.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                return BadRequest("Invalid Request");
            var newAccessToken = CreateJwt(user);
            var newRefreshToken = CreateRefreshToken();
            user.RefreshToken = newRefreshToken;
            await _authContext.SaveChangesAsync();
            return Ok(new TokenApiDto()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
            });
        }



    }
}
