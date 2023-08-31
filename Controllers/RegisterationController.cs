using JWTGenerator.Models;
using JWTGenerator.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWTGenerator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterationController : ControllerBase
    {
        public readonly HelperMethods _helperMethods;
        public readonly DataContext _dataContext;
        public readonly IConfiguration _configuration;
        public RegisterationController(HelperMethods helperMethods, DataContext dataContext, 
            IConfiguration configuration)
        {
            _helperMethods = helperMethods;
            _dataContext = dataContext;
            _configuration = configuration;
        }
        /// <summary>
        /// Register Methods
        /// </summary>
        /// <param name="userModel"></param>
        /// <returns></returns>
        [HttpPost("Register")]
        public async Task<ActionResult<string>> Register(UserModel userModel)
        {
            if (userModel.UserName != null && userModel.Password != null)
            {
                _helperMethods.CreateHasedPassword(userModel.Password, out byte[] hasehedPassword, out byte[] passwordSalt);

                _dataContext.Users.Add(new User
                {
                    UserName = userModel.UserName,
                    HashedPassword = hasehedPassword,
                    PasswordSalt = passwordSalt
                });
                _dataContext.SaveChanges();
                return Ok("Successfully Registered");
            }
            else
            {
                return BadRequest("userRegisterationModel doesn't have values");
            }
        }

        [HttpPost("Login")]
        public async Task<string> Login(UserModel userModel)
        {
            User user = _dataContext.Users.Where(u => u.UserName == userModel.UserName).FirstOrDefault();

            if (user == null)
            {
                return "User not found";
            }
            if (_helperMethods.VerifyHasedPassword(userModel.Password, user.HashedPassword, user.PasswordSalt))
            {
                return "Password Incorrect";
            }
             return CreateToken(userModel);
        }

        private string CreateToken(UserModel userModel)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, userModel.UserName)
            };
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("SecretKey").Value));

            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            JwtSecurityToken token = new JwtSecurityToken(
                                        claims: claims,
                                        expires: DateTime.Now.AddDays(1),
                                        signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
