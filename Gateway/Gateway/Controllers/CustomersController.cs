using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gateway.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using Newtonsoft.Json;
using Microsoft.Extensions.Primitives;

namespace Gateway.Controllers
{
    [Route("api/")]
    public class CustomersController : Controller
    {

        public HttpClient client = new HttpClient();
        public APIServices services = new APIServices();
        public OAuthController OA = new OAuthController();


        public string GetTokenFromHeader(HttpRequest request)
        {
            var headers = request.Headers;
            if (headers != null)
            {
                StringValues headerValues;
                if (headers.TryGetValue("Authorization", out headerValues))
                {
                    return headerValues.FirstOrDefault().Substring(7);
                }
            }
            return "";
        }

        public async Task<bool> ValidateToken()
        {
            var token = GetTokenFromHeader(Request);
            HttpResponseMessage introspect;
            try
            {
                var stringContent = new StringContent(string.Empty);
                string url = string.Format("http://localhost:4314/api/o/introspect/?token={0}", token);
                introspect = await client.GetAsync(url);
            }
            catch
            {
                return false;
            }

            var Introspect = await introspect.Content.ReadAsAsync<OAuthController.IntrospectResponse>();
            if (Introspect == null)
            {
                return false;
            }
            else if (Introspect.active == true)
            {
                return true;
            }
            else
            {
                return false;
            }

        }


        [HttpGet("customers")]
        public async Task<IActionResult> GetCustomers(int? page, int? size)
        {
            bool checktoken = await ValidateToken();
            if(!checktoken)
            {
                return Unauthorized();
            }

            string customers = null;

            try
            {
                customers = await client.GetStringAsync(services.customersAPI + $"?page={page}&size={size}");
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (customers == null)
            {
                return NotFound();
            }

            var Customers = JsonConvert.DeserializeObject<List<Customer>>(customers);

            return Ok(Customers);
        }

        [HttpGet("customers/{id}")]
        public async Task<IActionResult> GetCustomer(Guid id)
        {
            bool checktoken = await ValidateToken();
            if (!checktoken)
            {
                return Unauthorized();
            }

            string customer = null;

            try
            {
                customer = await client.GetStringAsync(services.customersAPI + $"/{id}");
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (customer == null)
            {
                return NotFound();
            }

            var Customer = JsonConvert.DeserializeObject<Customer>(customer);

            return Ok(Customer);
        }

        [HttpPut("customers/{id}")]
        public async Task<IActionResult> PutCustomer(Guid id, [FromBody] Customer customerModel)
        {
            bool checktoken = await ValidateToken();
            if (!checktoken)
            {
                return Unauthorized();
            }

            HttpResponseMessage customer;

            try
            {
                customer = await client.PutAsJsonAsync(services.customersAPI + $"/{id}", customerModel);
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (customer == null)
            { 
                return NotFound();
            }

            return Ok();
        }

        [HttpPost("customers")]
        public async Task<IActionResult> PostCustomer([FromBody] Customer customerModel)
        {
            bool checktoken = await ValidateToken();
            if (!checktoken)
            {
                return Unauthorized();
            }

            HttpResponseMessage customer;

            Guid id = Guid.NewGuid();
            customerModel.CustomerId = id;

            try
            {
                customer = await client.PostAsJsonAsync(services.customersAPI, customerModel);
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (customer == null)
            {
                return NotFound();
            }

            //return Ok(client.GetStringAsync(customersAPI + $"/{customerModel.CustomerId}"));
            return Ok(customerModel);

        }

        //удаляет покупателя и все его брони
        [HttpDelete("customers/{id}")]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            bool checktoken = await ValidateToken();
            if (!checktoken)
            {
                return Unauthorized();
            }

            HttpResponseMessage customer;

            try
            {
                customer = await client.DeleteAsync(services.customersAPI + $"/{id}");
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (customer == null)
            {
                return NotFound();
            }

            string bookings = null;

            try
            {
                bookings = await client.GetStringAsync(services.bookingsAPI);
            }
            catch
            {

            }

            var bk = JsonConvert.DeserializeObject<List<Booking>>(bookings);

            try
            {
                foreach (Booking entr in bk)
                {
                    if (entr.CustomerId == id)
                    {

                        HttpResponseMessage booking = await client.DeleteAsync(services.bookingsAPI + $"/{entr.BookingId}");

                    }
                }
            }
            catch
            {

            }

            return Ok(customer);
        }
    }
}