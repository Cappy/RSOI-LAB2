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

namespace Gateway.Controllers
{
    [Route("")]
    public class CustomersController : Controller
    {

        public HttpClient client = new HttpClient();
        public APIServices services = new APIServices();

        //List<Customer> customers = new List<Customer>();

        //public CustomersController() { }
        //public CustomersController(List<Customer> customers)
        //{
        //    this.customers = customers;
        //}


        [HttpGet("customers")]
        public async Task<IActionResult> GetCustomers(int? page, int? size)
        {
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

            return Ok(customers);
        }

        [HttpGet("customers/{id}")]
        public async Task<IActionResult> GetCustomer(Guid id)
        {
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
            
            return Ok(customer);
        }

        [HttpPut("customers/{id}")]
        public async Task<IActionResult> PutCustomer(Guid id, [FromBody] Customer customerModel)
        {
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

            //return Ok(client.GetStringAsync(customersAPI + $"/{id}"));
            return Ok(customer);
        }

        [HttpPost("customers")]
        public async Task<IActionResult> PostCustomer([FromBody] Customer customerModel)
        {
            HttpResponseMessage customer;

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
            return Ok(customer);

        }

        //удаляет покупателя и все его брони
        [HttpDelete("customers/{id}")]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
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