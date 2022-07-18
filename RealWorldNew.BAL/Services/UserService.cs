using RealWorldNew.Common;
using RealWorldNew.DAL.Entities;
using RealWorldNewAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using RealWorldNew.DAL.Repositories;
using RealWorldNew.Common.DtoModels;
using AutoMapper;
using Common;
using RealWorldNew.DAL.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace RealWorldNew.BAL.Services
{
    public class UserService : IUserService
    {
        private readonly AuthenticationSettings _authenticationSettings;
        private readonly IUserRepositorie _userRepositorie;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly UserManager<User> _userManager;
        private readonly IPackingService _packingService;
        private readonly ILogger _logger; 


        public UserService(AuthenticationSettings authenticationSettings, 
                           IUserRepositorie userRepositorie,
                           IMapper mapper,
                           IPasswordHasher<User> passwordHasher,
                           UserManager<User> userManager,
                           IPackingService packingService,
                           ILogger<UserService> logger)
        {
            _authenticationSettings = authenticationSettings;
            _userRepositorie = userRepositorie;
            _mapper = mapper;
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
                throw new BadHttpRequestException("Username already taken.");
            }
            var user = _packingService.UnpackRegisterUser(userPack);
            var result = await _userManager.CreateAsync(user, userPack.user.password);
            if (!result.Succeeded)
            {
                throw new BadHttpRequestException("Invalid username or password.");
            }
            return user;
        }

        public async Task<User> Login(LoginUserPack modelPack)
        {
            var user = await _userManager.FindByEmailAsync(modelPack.user.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, modelPack.user.Password))
            {
                return user;
            }
            else
            {
                _logger.LogInformation("About page visited at {DT}", DateTime.UtcNow.ToLongTimeString());
                throw new BadHttpRequestException("Invalid username or password.");
            }
        }

        public string GetToken(User user)
        {
            var userRoles = _userManager.GetRolesAsync(user);
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
            var user = await _userRepositorie.GetById(Id);
            if(user == null) return null;

            var userResponse = new UserResponse()
            {
                email = user.Email,
                username = user.UserName,
                bio = user.ShortBio,
                image = user.UrlProfile,
                token = "looooool"
            };

            var userResponseContainer = new UserResponseContainer()
            {
                user = userResponse
            };

            return userResponseContainer;
        }

        public async Task ChangeUser(User user, ChangeProfile settings)
        {
            var hashPassword = _passwordHasher.HashPassword(user, settings.password);
            await _userRepositorie.ChangeUser(user.Id, settings.username, settings.bio, settings.image, settings.email, hashPassword);
        }

        public async Task<bool> IsFollowedUser(string id, string username)
        {
            var user = await _userManager.FindByIdAsync(id);
            return await _userRepositorie.IsFollowedUser(user, username);
        }

        public async Task<ProfileViewContainer> FollowUser(User user, User userToFollow)
        {
            await _userRepositorie.AddFollow(user, userToFollow);
            var ProfileView = new ProfileView()
            {
                bio = userToFollow.ShortBio,
                following = await IsFollowedUser(user.Id, userToFollow.UserName),
                image = userToFollow.UrlProfile,
                username = userToFollow.UserName
            };

            var container = new ProfileViewContainer()
            {
                profile = ProfileView
            };

            return container;
        }

    }
}