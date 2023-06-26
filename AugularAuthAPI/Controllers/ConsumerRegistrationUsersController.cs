using AugularAuthAPI.Context;
using AugularAuthAPI.Helpers;
using AugularAuthAPI.Models;
using AugularAuthAPI.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Formats.Asn1;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace AugularAuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsumerRegistrationUsersController: ControllerBase
    {
        private readonly AppDbContext _authContext;
        public ConsumerRegistrationUsersController(AppDbContext appDbContext)
        {
            _authContext = appDbContext;
        }
        [HttpPost("Consumer-Registration-User")]
        public async Task<IActionResult> ConsumerRegistrationUserPost([FromBody] ConsumerRegistrationUsers ConsumerRegistrationUsersObj)
        {
            try
            {
                // check here weather object has data or not
                if (ConsumerRegistrationUsersObj == null)
                {
                    return BadRequest();
                }
                // ckecking here Consumer Fullname Existed or blank
                if (ConsumerRegistrationUsersObj.ConsumerFullName == "")
                {
                    return BadRequest(new { Message = "Consumer Full Name Should not be Blank!!!" });
                }
     

                //checking here Consumer Mobile Number Existed or blank
                if (ConsumerRegistrationUsersObj.ConsumerMobileNumber == "")
                {
                    return BadRequest(new { Message = "Consumer Mobile Number Should not be Blank!!!" });
                }
                if (await CheckConsumerMobileNumberExistAsync(ConsumerRegistrationUsersObj.ConsumerMobileNumber))
                {
                    return BadRequest(new { Message = "Consumer Mobile Number Already Existed Found!!!" });
                }
                // checking Consuer Mobile number 10 digit or not
                string CheckConsumerMobileNumberDigitTen = CheckConsumerMobileNumberDigitTenExistAsync(ConsumerRegistrationUsersObj.ConsumerMobileNumber);
    
                if (CheckConsumerMobileNumberDigitTen.Length > 10)
                {
                    return BadRequest(new { Message = CheckConsumerMobileNumberDigitTen.ToString() });
                }
                if (ConsumerRegistrationUsersObj.ConsumerPassword == "")
                {
                    return BadRequest(new { Message = "Consumer Password Should not be Blank!!!" });
                }
                var CheckConsumerPasswordLengthStrength = CheckConsumerPasswordLengthStrengthAsync(ConsumerRegistrationUsersObj.ConsumerPassword);
                
                if (!string.IsNullOrEmpty(CheckConsumerPasswordLengthStrength))
                {
                    return BadRequest(new {Message = CheckConsumerPasswordLengthStrength.ToString() });
                }
                // hashing ConsumerPassword Before Saving into DB
                ConsumerRegistrationUsersObj.ConsumerPassword = PasswordHasher.HashPassword(ConsumerRegistrationUsersObj.ConsumerPassword);
                ConsumerRegistrationUsersObj.ConsumerRole = "ConsumerRole";
                ConsumerRegistrationUsersObj.CousumerToken = "";

                //inserting details in DB Table
                await _authContext.ConsumerRegistrationUsers.AddAsync(ConsumerRegistrationUsersObj);
                await _authContext.SaveChangesAsync();
                // Return a successful response indicating successful user registration
                return Ok(new
                {
                    Message = "Consumer Successfull Registered!"
                });
            }
            catch (Exception ex) 
            {
                throw ex;
            }
        }

        private async Task<bool> CheckConsumerMobileNumberExistAsync(string ConsumerMobileNumber)
        {
            return await _authContext.ConsumerRegistrationUsers.AnyAsync(x => x.ConsumerMobileNumber == ConsumerMobileNumber);
        }

        private string CheckConsumerMobileNumberDigitTenExistAsync(string consumerMobileNumber)
        {
            StringBuilder stringBuilder = new StringBuilder();

            if (!Regex.IsMatch(consumerMobileNumber.ToString(), "^[0-9]+$"))
            {
                stringBuilder.Append("Consumer Mobile Number should only contain numeric characters." + Environment.NewLine);
            }

            if (consumerMobileNumber.ToString().Length < 10)
            {
                stringBuilder.Append("Minimum Consumer Mobile Number length should be at least 10 digits.");
            }

            return stringBuilder.ToString();
        }
        private string CheckConsumerPasswordLengthStrengthAsync (string ConsumerPassword)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (ConsumerPassword.Length < 8)
                stringBuilder.Append("Minimum password length should be at least 8 characters." + Environment.NewLine);

            if (!Regex.IsMatch(ConsumerPassword, "[a-z]"))
                stringBuilder.Append("Password should contain at least one lowercase letter." + Environment.NewLine);

            if (!Regex.IsMatch(ConsumerPassword, "[A-Z]"))
                stringBuilder.Append("Password should contain at least one uppercase letter." + Environment.NewLine);

            if (!Regex.IsMatch(ConsumerPassword, "[0-9]"))
                stringBuilder.Append("Password should contain at least one digit." + Environment.NewLine);

            if (!Regex.IsMatch(ConsumerPassword, "[^a-zA-Z0-9]"))
                stringBuilder.Append("Password should contain at least one special character." + Environment.NewLine);
            else
            {
                int specialCharCount = 0;

                // Loop through each character in the password
                foreach (char c in ConsumerPassword)
                {
                    // Check if the character is a special character (not a letter or digit)
                    if (!char.IsLetterOrDigit(c))
                        specialCharCount++;
                }

                // Check if the number of special characters is less than 2
                if (specialCharCount < 2)
                    stringBuilder.Append("Password should contain at least two special characters." + Environment.NewLine);
            }

            return stringBuilder.ToString();
        }
        [HttpPost("Consumer-Registration-User-Authenticate")]
        public async Task<IActionResult> ConsumerRegistrationUserAuthenticate([FromBody] ConsumerRegistrationUsers consumerRegistrationUsersObj)
        {
            try
            {
                Console.WriteLine(consumerRegistrationUsersObj);
                if (consumerRegistrationUsersObj == null)
                {
                    return BadRequest();
                }
                var consumerRegisteredId = await _authContext.ConsumerRegistrationUsers.FirstOrDefaultAsync(x => x.ConsumerMobileNumber == consumerRegistrationUsersObj.ConsumerMobileNumber);
                Console.WriteLine(consumerRegisteredId);
                // If no user is found, return a "User Not Found" response
                if (consumerRegisteredId == null)
                {
                    return NotFound(new { message = "Consumer User Not Found!" });
                }

                if (!PasswordHasher.VerifyPassword(consumerRegistrationUsersObj.ConsumerPassword, consumerRegisteredId.ConsumerPassword))
                {
                    return BadRequest(new { Message = "Consumer Password is Incorrect" });
                }

                consumerRegisteredId.CousumerToken = CreateJwt(consumerRegisteredId);

                // If the user is found, return a successful response indicating successful login
                return Ok(new
                {
                    Token = consumerRegisteredId.CousumerToken,
                    Message = "Login Successfully with ConsumerRegistered UserID " + consumerRegisteredId.ConsumerMobileNumber
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("This is a invalid Token");
            return principal;
        }
        private string CreateRefreshToken()
        {
            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var refreshToken = Convert.ToBase64String(tokenBytes);
            var tokenInUser = _authContext.ConsumerRegistrationUsers.Any(a => a.RefreshToken == refreshToken);
            if (tokenInUser)
            {
                return CreateRefreshToken();
            }
            return refreshToken;

        }
        private string CreateJwt(ConsumerRegistrationUsers consumerRegistrationUsers)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("veryverysecret.....");
            var identity = new ClaimsIdentity(new Claim[]
            {
                //new Claim(ClaimTypes.Role, consumerRegistrationUsers.ConsumerRole),
                new Claim(ClaimTypes.Role, consumerRegistrationUsers.ConsumerRole),
                new Claim(ClaimTypes.MobilePhone, consumerRegistrationUsers.ConsumerMobileNumber),

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
        [HttpGet]
        public async Task<ActionResult<ConsumerRegistrationUsers>> GetAllUsers()
        {
            return Ok(await _authContext.ConsumerRegistrationUsers.ToListAsync());
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
            Console.WriteLine(username);
            var user = await _authContext.ConsumerRegistrationUsers.FirstOrDefaultAsync(u => u.ConsumerMobileNumber == username);
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
