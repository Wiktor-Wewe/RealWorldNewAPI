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

namespace RealWorldNew.BAL.Services
{
    public class UserService : IUserService
    {
        private readonly AuthenticationSettings _authenticationSettings;
        private readonly IUserRepositorie _userRepositorie;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly UserManager<User> _userManager;


        public UserService(AuthenticationSettings authenticationSettings, 
                           IUserRepositorie userRepositorie,
                           IMapper mapper,
                           IPasswordHasher<User> passwordHasher,
                           UserManager<User> userManager)
        {
            _authenticationSettings = authenticationSettings;
            _userRepositorie = userRepositorie;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _userManager = userManager;
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

        public async Task AddUser(RegisterUserDto userDto)
        {
            var user = _mapper.Map<User>(userDto);
            var hashPassword = _passwordHasher.HashPassword(user, userDto.password);
            user.PasswordHash = hashPassword;
            await _userRepositorie.AddUser(user);
        }

        public async Task<UserResponseContainer> GetMyInfo(ClaimsPrincipal claims)
        {
            var user = await _userRepositorie.GetById(claims.Identity.Name);
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