using AugularAuthAPI.Context;
using AugularAuthAPI.Helpers;
using AugularAuthAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Formats.Asn1;
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
                //if (await CheckConsumerFullNameExistAsync(ConsumerRegistrationUsersObj.ConsumerFullName))
                //{
                //    return BadRequest(new { Message = "Consumer FullName Alread Existed Found!!!" });
                //}

                //checking here Consumer Mobile Number Existed or blank
                if (ConsumerRegistrationUsersObj.ConsumerMobileNumber == 0)
                {
                    return BadRequest(new { Message = "Consumer Mobile Number Should not be Blank!!!" });
                }
                if (await CheckConsumerMobileNumberExistAsync(ConsumerRegistrationUsersObj.ConsumerMobileNumber))
                {
                    return BadRequest(new { Message = "Consumer Mobile Number Already Existed Found!!!" });
                }
                // checking Consuer Mobile number 10 digit or not
                var CheckConsumerMobileNumberDigitTen = CheckConsumerMobileNumberDigitTenExistAsync(Convert.ToUInt32(ConsumerRegistrationUsersObj.ConsumerMobileNumber).ToString());
                if (!string.IsNullOrEmpty(CheckConsumerMobileNumberDigitTen))
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
        //private async Task<bool> CheckConsumerFullNameExistAsync(string ConsumerFullName)
        //{
        //    return await _authContext.ConsumerRegistrationUsers.AnyAsync(x => x.ConsumerFullName == ConsumerFullName);
        //}
        private async Task<bool> CheckConsumerMobileNumberExistAsync(int ConsumerMobileNumber)
        {
            return await _authContext.ConsumerRegistrationUsers.AnyAsync(x => x.ConsumerMobileNumber == ConsumerMobileNumber);
        }
        private string CheckConsumerMobileNumberDigitTenExistAsync (string ConsumerMobileNumber)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (!Regex.IsMatch(ConsumerMobileNumber, "[0-9]"))
            {
                stringBuilder.Append("Consumer Mobile Number Should be Integer Characters" + Environment.NewLine);
            }
            if (ConsumerMobileNumber.Length < 10)
            {
                stringBuilder.Append("Minimum Consumer Mobile Number length should be at least 10 Integer");
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
    }
}
