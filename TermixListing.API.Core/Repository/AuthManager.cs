using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TermixListing.API.Core.Contracts;
using TermixListing.API.Data;
using TermixListing.API.Core.Models.Users;

namespace TermixListing.API.Core.Repository
{
    public class AuthManager : IAuthManager
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApiUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthManager> _logger;
        private ApiUser _user;

        private const string _loginProvider = "HotelListingApi";
        private const string _refreshToken = "RefreshToken";

        public AuthManager(IMapper mapper, UserManager<ApiUser> userManager, IConfiguration
            configuration, ILogger<AuthManager> logger)
        {
            this._mapper = mapper;
            this._userManager = userManager;
            this._configuration = configuration;
            this._logger = logger;
        }

        public async Task<string> CreateRefreshToken()
        {
            await _userManager.RemoveAuthenticationTokenAsync
                (_user, _loginProvider, _refreshToken);
            var newRefreshToken = await _userManager.GenerateUserTokenAsync
                (_user, _loginProvider, _refreshToken);
            var result = await _userManager.SetAuthenticationTokenAsync
                (_user, _loginProvider, _refreshToken, newRefreshToken);
            return newRefreshToken;
            //return result;
        }
        public async Task<AuthResponseDto> VerifyRefreshToken(AuthResponseDto request)
        {
            var JwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var tokenContent = JwtSecurityTokenHandler.ReadJwtToken(request.Token);
            var userName = tokenContent.Claims.ToList().FirstOrDefault(q => q.Type ==
            JwtRegisteredClaimNames.Email)?.Value;
            _user = await _userManager.FindByNameAsync(userName);

            if (_user == null || _user.Id != request.UserID)
            {
                return null;
            }

            var isValidRefreshToken = await _userManager.VerifyUserTokenAsync
                (_user, _loginProvider, _refreshToken, request.RefreshToken);
            if (isValidRefreshToken)
            {
                var token = await GenerateToken();
                return new AuthResponseDto
                {
                    Token = token,
                    UserID = _user.Id,
                    RefreshToken = await CreateRefreshToken()
                };
            }

            await _userManager.UpdateSecurityStampAsync(_user);
            return null;
        }
        public async Task<AuthResponseDto> Login(LoginDTO loginDTO)
        {
            _logger.LogInformation($"Looking for user with Email {loginDTO.Email}");
            _user = await _userManager.FindByEmailAsync(loginDTO.Email);
            bool isValidCredentials = await _userManager.CheckPasswordAsync(_user, loginDTO.Password);

            if (_user == null || isValidCredentials == false)
            {
                _logger.LogWarning($"User with Email {loginDTO.Email} Was Not Found!");
                return null;
            }

            var token = await GenerateToken();
            _logger.LogInformation($"Token Generated for User with Email: {loginDTO.Email} | Token: {token}");
            return new AuthResponseDto
            {
                Token = token,
                UserID = _user.Id,
                RefreshToken = await CreateRefreshToken()
            };

        }
        public async Task<IEnumerable<IdentityError>> Register(ApiUserDto userDto)
        {
            _user = _mapper.Map<ApiUser>(userDto);
            _user.UserName = userDto.Email;

            var result = await _userManager.CreateAsync(_user, userDto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(_user, "User");
            }

            return result.Errors;

        }
        private async Task<string> GenerateToken()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration
            ["JwtSettings:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var roles = await _userManager.GetRolesAsync(_user);
            var roleClaimes = roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();
            var userClaims = await _userManager.GetClaimsAsync(_user);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, _user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, _user.Email),
                new Claim("uid", _user.Email)
            }.Union(userClaims).Union(roleClaimes);
            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToInt32(_configuration
                ["JwtSettings:DurationInMinutes"])),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
