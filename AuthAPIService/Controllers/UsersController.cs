using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.IdentityModel.Tokens.Jwt;
using Auth.Helpers;
using Microsoft.Extensions.Options;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Auth.Services;
using Auth.Dtos;
using Auth.Entities;

namespace Auth.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;
        private ITokenService _tokenService;
        private IOAuth2TokenService _OAuth2TokenService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public UsersController(
            ITokenService tokenService,
            IOAuth2TokenService OAuth2TokenService,
            IUserService userService,
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            _mapper = mapper;
            _tokenService = tokenService;
            _OAuth2TokenService = OAuth2TokenService;
            _appSettings = appSettings.Value;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]UserDto userDto)
        {
            var user = _userService.Authenticate(userDto.Username, userDto.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            string tokenString;

            try
            {
                if (_tokenService.GetTokenByUID(user.UserId) != null)
                {
                    tokenString = _tokenService.GetTokenByUID(user.UserId);
                    return Ok(new
                    {
                        UserId = user.UserId,
                        Username = user.Username,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Token = tokenString
                    });
                }
            }
            catch
            {
                return BadRequest();
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserId.ToString())
                }),
                Expires = DateTime.Now.AddMinutes(211),  //????
                Issuer = "HotelApp",
                Audience = "HotelApp",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            tokenString = tokenHandler.WriteToken(token);

            //adding token to db

            _tokenService.AddToken(tokenString, user.UserId);



            // return basic user info (without password) and token to store client side
            return Ok(new {
                UserId = user.UserId,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = tokenString
            });
        }

        [AllowAnonymous]
        [HttpPost("get-oauth2-token")]
        public IActionResult GetOAuth2Token([FromBody]CurUser userModel)
        {

            string tokenString;

            try
            {
                if (_OAuth2TokenService.GetTokenByUID(userModel.UserId) != null)
                {
                    tokenString = _OAuth2TokenService.GetTokenByUID(userModel.UserId);
                    return Ok(new
                    {
                        Token = tokenString
                    });
                }
            }
            catch
            {
                return BadRequest();
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, userModel.Token.ToString())
                }),
                Expires = DateTime.Now.AddMinutes(211),  //????
                Issuer = "HotelApp",
                Audience = "HotelApp",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            tokenString = tokenHandler.WriteToken(token);

            //adding token to db
            _OAuth2TokenService.AddToken(tokenString, userModel.UserId);


            // return basic user info (without password) and token to store client side
            return Ok(new
            {
                Token = tokenString
            });
        }

        [AllowAnonymous]
        [HttpPost("get-info-by-oauth2")]
        public IActionResult GetInfoByOAuth2Token([FromBody] OnlyToken token)
        {
            try
            {
                var userid = _OAuth2TokenService.GetUIDByToken(token.Token);
                var User = _userService.GetById(userid);

                return Ok(new
                {
                    UserId = User.UserId,
                    Username = User.Username,
                    FirstName = User.FirstName,
                    LastName = User.LastName
                });
            }
            catch
            {
                return BadRequest();
            }

        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody]UserDto userDto)
        {

            Guid id = Guid.NewGuid();
            userDto.UserId = id;

            // map dto to entity
            var user = _mapper.Map<User>(userDto);

            try 
            {
                // save 
                _userService.Create(user, userDto.Password);
                return Ok();
            } 
            catch(AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var users =  _userService.GetAll();
            var userDtos = _mapper.Map<IList<UserDto>>(users);
            return Ok(userDtos);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var user =  _userService.GetById(id);
            var userDto = _mapper.Map<UserDto>(user);
            return Ok(userDto);
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody]UserDto userDto)
        {
            // map dto to entity and set id
            var user = _mapper.Map<User>(userDto);
            user.UserId = id;

            try 
            {
                // save 
                _userService.Update(user, userDto.Password);
                return Ok();
            } 
            catch(AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            _userService.Delete(id);
            return Ok();
        }
    }
}
