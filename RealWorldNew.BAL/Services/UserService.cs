using RealWorldNew.Common;
using RealWorldNew.DAL.Entities;
using System.Text;
using System.Security.Claims;
using RealWorldNew.Common.DtoModels;
using Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using RealWorldNew.Common.Exceptions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace RealWorldNew.BAL.Services
{
    public class UserService : IUserService
    {

        private readonly AuthenticationSettings _authenticationSettings;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly UserManager<User> _userManager;
        private readonly IPackingService _packingService;
        private readonly ILogger _logger; 


        public UserService(AuthenticationSettings authenticationSettings, 
                           IPasswordHasher<User> passwordHasher,
                           UserManager<User> userManager,
                           IPackingService packingService,
                           ILogger<UserService> logger)
        {
            _authenticationSettings = authenticationSettings;
            _passwordHasher = passwordHasher;
            _userManager = userManager;
            _packingService = packingService;
            _logger = logger;
        }

        public async Task<User> Register(RegisterUserPack userPack)
        {
            var userExists = await _userManager.FindByNameAsync(userPack.user.username);
            if (userExists != null)
            {
                _logger.LogError("Trying to add existing user");
                throw new UserException("Username already taken.");
            }
            var user = _packingService.UnpackRegisterUser(userPack);
            var result = await _userManager.CreateAsync(user, userPack.user.password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(x => x.Description);
                _logger.LogError(string.Join(" ", errors));
                throw new UserException("Wrong input");
            }
            return user;
        }

        public async Task<User> Login(LoginUserPack modelPack)
        {
            var user = await _userManager.FindByEmailAsync(modelPack.user.Email);
            var CheckPassword = await _userManager.CheckPasswordAsync(user, modelPack.user.Password);
            if (user != null && CheckPassword)
            {
                return user;
            }
            else
            {
                _logger.LogError("Invalid username or password");
                throw new UserException("Invalid username or password.");
            }
        }

        public string GetToken(User user)
        {
            if(user == null)
            {
                _logger.LogError("Can't generate token");
                throw new UserException("Can't generate token");
            }

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.Id),
                new Claim(ClaimTypes.Email, $"{user.Email}")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtKey));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(_authenticationSettings.JwtExpireDays);

            var token = new JwtSecurityToken(_authenticationSettings.JwtIssuer,
                _authenticationSettings.JwtIssuer,
                claims,
                expires: expires,
                signingCredentials: cred);

            var tokenHandler = new JwtSecurityTokenHandler();

            return tokenHandler.WriteToken(token).ToString();
        }

        public async Task<UserResponseContainer> GetMyInfo(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            if(user == null)
            {
                _logger.LogError("Can not find active user in database.");
                throw new UserException("Can not find active user in database.");
            }

            var userResponseContainer = new UserResponseContainer()
            {
                user = new UserResponse()
                {
                    email = user.Email,
                    username = user.UserName,
                    bio = user.ShortBio,
                    image = user.UrlProfile,
                    token = "looooool"
                }
            };

            return userResponseContainer;
        }

        public async Task UpdateUser(string id, ChangeProfile settings)
        {
            var user = await _userManager.FindByIdAsync(id);
            if(user == null)
            {
                _logger.LogError("No user found logged in.");
                throw new UserException("No user found logged in.");
            }

            var hashPassword = _passwordHasher.HashPassword(user, settings.password);

            user.UserName = settings.username;
            user.UrlProfile = settings.image;
            user.ShortBio = settings.bio;
            user.Email = settings.email;
            user.PasswordHash = hashPassword;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                _logger.LogError("Can't update user");
                throw new UserException("Can't update user");
            }
        }

        public async Task<ProfileViewContainer> LoadProfile(string username, string id)
        {
            User user = await _userManager.FindByNameAsync(username);
            
            if(user == null)
            {
                _logger.LogError($"Can not find user {username}");
                throw new UserException($"Can not find user {username}");
            }

            var activeUser = await _userManager.FindByIdAsync(id);
            var isFollowed = false;
            if(activeUser != null)
            {
                isFollowed = activeUser.FollowedUsers.Contains(user);
            }

            var packProfile = _packingService.PackUserToProfileView(user, isFollowed);
            return _packingService.PackProfileView(packProfile);
        }

    }
}