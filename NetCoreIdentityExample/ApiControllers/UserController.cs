using FreeCourse.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NetCoreIdentityExample.DTO.ApiViewModels;
using NetCoreIdentityExample.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using static IdentityServer4.IdentityServerConstants;

namespace NetCoreIdentityExample.ApiControllers
{
    [Authorize(LocalApi.PolicyName)]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public UserController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost]
        public async Task<IActionResult> Signup(SignupVM data)
        {
            var user = new AppUser() { UserName = data.Username, Email = data.Email, City = data.City };
            var result = await _userManager.CreateAsync(user, data.Password).ConfigureAwait(false);
            if (!result.Succeeded) { 
                return BadRequest(Response<NoContent>.Fail(result.Errors.Select(x=> x.Description).ToList(), 500));
            }
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub);
            if(userIdClaim == null)
            {
                return BadRequest();
            }

            var user = await _userManager.FindByIdAsync(userIdClaim.Value).ConfigureAwait(false);
            if(user == null)
            {
                return BadRequest();
            }

            var userDto = new AppUser()
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                City = user.City,
            };

            return Ok(userDto);
        }
    }
}
