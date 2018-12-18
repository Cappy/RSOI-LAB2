using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Gateway.Models;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using System.Collections.Specialized;
using Microsoft.Extensions.Primitives;
using System.Net.Http.Headers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Gateway.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {

        public HttpClient client = new HttpClient();
        public APIServices services = new APIServices();

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody]UserDto userDto)
        {
            HttpResponseMessage user;
            try
            {
                user = await client.PostAsJsonAsync(services.authAPI + "/" + "authenticate", userDto);
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (!user.IsSuccessStatusCode)
            {
                var message = user.Content.ReadAsAsync<ErrorMessage>().Result;
                return BadRequest(message);
            }

            var User = await user.Content.ReadAsAsync<CurUser>();

            return Ok(User);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]UserDto userDto)
        {
            HttpResponseMessage user;
            try
            {
                user = await client.PostAsJsonAsync(services.authAPI + "/" + "register", userDto);
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (user.IsSuccessStatusCode)
            {
                return Ok();
            }
            else
            {
                var message = user.Content.ReadAsAsync<ErrorMessage>().Result;
                return BadRequest(message);
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {

            var re = Request;
            var headers = re.Headers;
            string token = null;
            StringValues headerValues;
            if (headers.TryGetValue("Authorization", out headerValues))
            {
                token = headerValues.FirstOrDefault().Substring(7);
            }

            HttpResponseMessage users;
            try
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                users = await client.GetAsync(services.authAPI);

                if (users.IsSuccessStatusCode)
                {
                    var User = await users.Content.ReadAsAsync<List<User>>();
                    return Ok(User);
                }
                else
                {
                    return Unauthorized();
                }

            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {

            var re = Request;
            var headers = re.Headers;
            string token = null;
            StringValues headerValues;
            if (headers.TryGetValue("Authorization", out headerValues))
            {
                token = headerValues.FirstOrDefault().Substring(7);
            }

            HttpResponseMessage users;
            try
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                users = await client.DeleteAsync(services.authAPI + "/" + id);

                if (users.IsSuccessStatusCode)
                {
                    return Ok();
                }
                else
                {
                    return Unauthorized();
                }

            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

        }
    }
}
