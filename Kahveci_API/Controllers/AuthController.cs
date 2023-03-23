using Azure;
using Kahveci_API.Data;
using Kahveci_API.Dto;
using Kahveci_API.Entities;
using Kahveci_API.Services;
using Kahveci_API.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace Kahveci_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ApiResponse _apiResponse;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private string secretKey;
        public AuthController(AppDbContext db, IConfiguration configuration, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
            _apiResponse = new ApiResponse();
            _userManager = userManager;
            _roleManager = roleManager;
        }
        //[HttpPost("login")]
        //public async Task<IActionResult> Login([FromBody]LoginRequestDto model)
        //{
        //    AppUser user = _db.AppUsers.FirstOrDefault(x => x.UserName.ToLower() == model.UserName.ToLower());
        //    bool isValid = await _userManager.CheckPasswordAsync(user, model.Password);
        //    if (isValid == false)
        //    {
        //        _apiResponse.Result = new LoginResponseDto();
        //        _apiResponse.StatusCode = HttpStatusCode.BadRequest;
        //        _apiResponse.IsSuccess = false;
        //        _apiResponse.ErrorMessages.Add("Kullanıcı Adı veya Şifre yanlış");
        //        return BadRequest(_apiResponse);
        //    }
        //    //Generate JWT Token
        //    var roles = await _userManager.GetRolesAsync(user);
        //    JwtSecurityTokenHandler tokenHandler = new();
        //    byte[] key = Encoding.ASCII.GetBytes(secretKey);

        //    SecurityTokenDescriptor tokenDescriptor = new()
        //    {
        //        Subject = new ClaimsIdentity(new Claim[]
        //        {
        //            new Claim("fullName", user.Name),
        //            new Claim("id", user.Id.ToString()),
        //            new Claim(ClaimTypes.Email, user.Email.ToString()),
        //            new Claim(ClaimTypes.Role, roles.FirstOrDefault()), // tek bir role sahip olabilir
        //        }),
        //        Expires = DateTime.UtcNow.AddDays(3),
        //        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        //    };
        //    SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

        //    LoginResponseDto loginResponse = new()
        //    {
        //        Email = user.Email,
        //        Token = tokenHandler.WriteToken(token),
        //    };
        //    if (loginResponse.Email == null || string.IsNullOrEmpty(loginResponse.Token))
        //    {
        //        _apiResponse.StatusCode = HttpStatusCode.BadRequest;
        //        _apiResponse.IsSuccess = false;
        //        _apiResponse.ErrorMessages.Add("Kullanıcı Adı veya Şifre Yanlış");
        //        return BadRequest(_apiResponse);
        //    }
        //    _apiResponse.StatusCode = HttpStatusCode.OK;
        //    _apiResponse.IsSuccess = true;
        //    return Ok(_apiResponse);
        //}
        //[HttpPost("register")]
        //public async Task<IActionResult> Register([FromBody] RegisterRequestDto model)
        //{
        //    AppUser user = _db.AppUsers.FirstOrDefault(x => x.UserName.ToLower() == model.UserName.ToLower());
        //    if (user != null)
        //    {
        //        _apiResponse.StatusCode = HttpStatusCode.BadRequest;
        //        _apiResponse.IsSuccess = false;
        //        _apiResponse.ErrorMessages.Add("Kullanıcı Adı Daha Önce Alınmış");
        //        return BadRequest(_apiResponse);
        //    }
        //    AppUser newUser = new()
        //    {
        //        UserName = model.UserName,
        //        Email = model.UserName,
        //        NormalizedEmail = model.UserName.ToUpper(),
        //        Name = model.Name,
        //    };
        //    try
        //    {
        //        var res = await _userManager.CreateAsync(newUser, model.Password);
        //        if (res.Succeeded)
        //        {
        //            if (!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
        //            {
        //                //CREATE ROLES
        //                await _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin));
        //                await _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer));
        //            }
        //            if (model.Role.ToLower() == SD.Role_Admin)
        //                await _userManager.AdDtoRoleAsync(newUser, SD.Role_Admin);
        //            else
        //                await _userManager.AdDtoRoleAsync(newUser, SD.Role_Customer);
        //            _apiResponse.StatusCode = HttpStatusCode.OK;
        //            _apiResponse.IsSuccess = true;
        //            return Ok(_apiResponse);
        //        }
        //    }
        //    catch (Exception e)
        //    {

        //    }
        //    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
        //    _apiResponse.IsSuccess = false;
        //    _apiResponse.ErrorMessages.Add("Kayıt Sırasında Hata");
        //    return BadRequest(_apiResponse);

        //}
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            AppUser userFromDb = _db.AppUsers
                    .FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());

            bool isValid = await _userManager.CheckPasswordAsync(userFromDb, model.Password);

            if (isValid == false)
            {
                _apiResponse.Result = new LoginResponseDto();
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("Username or password is incorrect");
                return BadRequest(_apiResponse);
            }

            //we have to generate JWT Token
            var roles = await _userManager.GetRolesAsync(userFromDb);
            JwtSecurityTokenHandler tokenHandler = new();
            byte[] key = Encoding.ASCII.GetBytes(secretKey);

            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("fullName", userFromDb.Name),
                    new Claim("id", userFromDb.Id.ToString()),
                    new Claim(ClaimTypes.Email, userFromDb.UserName.ToString()),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault()),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            LoginResponseDto loginResponse = new()
            {
                Email = userFromDb.Email,
                Token = tokenHandler.WriteToken(token)
            };

            if (loginResponse.Email == null || string.IsNullOrEmpty(loginResponse.Token))
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("Username or password is incorrect");
                return BadRequest(_apiResponse);
            }

            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.IsSuccess = true;
            _apiResponse.Result = loginResponse;
            return Ok(_apiResponse);

        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto model)
        {
            AppUser userFromDb = _db.AppUsers
                .FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());

            if (userFromDb != null)
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("Username already exists");
                return BadRequest(_apiResponse);
            }

            AppUser newUser = new()
            {
                UserName = model.UserName,
                Email = model.UserName,
                NormalizedEmail = model.UserName.ToUpper(),
                Name = model.Name
            };

            try
            {
                var result = await _userManager.CreateAsync(newUser, model.Password);
                if (result.Succeeded)
                {
                    if (!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
                    {
                        //create roles in database
                        await _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin));
                        await _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer));
                    }
                    if (model.Role.ToLower() == SD.Role_Admin)
                    {
                        await _userManager.AddToRoleAsync(newUser, SD.Role_Admin);
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(newUser, SD.Role_Customer);
                    }

                    _apiResponse.StatusCode = HttpStatusCode.OK;
                    _apiResponse.IsSuccess = true;
                    return Ok(_apiResponse);
                }
            }
            catch (Exception)
            {

            }
            _apiResponse.StatusCode = HttpStatusCode.BadRequest;
            _apiResponse.IsSuccess = false;
            _apiResponse.ErrorMessages.Add("Error while registering");
            return BadRequest(_apiResponse);

        }
    }
}
