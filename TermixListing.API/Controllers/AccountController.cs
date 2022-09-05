using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TermixListing.API.Core.Contracts;
using TermixListing.API.Core.Models.Users;

namespace TermixListing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthManager _authManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAuthManager authManager, ILogger<AccountController> logger)
        {
            this._authManager = authManager;
            this._logger = logger;
        }

        // POST: api/Account/register
        [HttpPost]
        [Route("register")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Register([FromBody] ApiUserDto apiUserDto)
        {
            _logger.LogInformation($"Regestration Attempt for {apiUserDto.Email}");
            try
            {
                var errors = await _authManager.Register(apiUserDto);

                if (errors.Any())
                {
                    foreach (var error in errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return BadRequest(ModelState);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something Went Wrong In The {nameof(Register)} - User Regestration attempt for {apiUserDto.Email}");
                return Problem($"Something Went Wrong In The {nameof(Register)}", statusCode: 500);
            }


        }
        // POST: api/Account/Login
        [HttpPost]
        [Route("Login")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            _logger.LogInformation($"Login Attempt for {loginDTO.Email}");
            try
            {
                var authResponse = await _authManager.Login(loginDTO);

                if (authResponse == null)
                {

                    return Unauthorized();
                }
                return Ok(authResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something Went Wrong In The {nameof(Register)} - Email Address OR Paaword not match for {loginDTO.Email} - Please Try Again, OR Regester First!");
                return Problem($"Something Went Wrong In The {nameof(Register)} - Email Address OR Paaword not match for {loginDTO.Email} - Please Try Again, OR Regester First!", statusCode: 500);
            }

        }
        // POST: api/Account/refreshtoken
        [HttpPost]
        [Route("refreshtoken")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> RefreshToken([FromBody] AuthResponseDto request)
        {
            var authResponse = await _authManager.VerifyRefreshToken(request);

            if (authResponse == null)
            {

                return Unauthorized();
            }
            return Ok(authResponse);
        }
    }
}
