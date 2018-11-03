using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gateway;
using Gateway.Controllers;
using Gateway.Models;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace GatewayTests
{
    [TestClass]
    public class GatewayTests
    {
        [TestMethod]
        public void GetCustomerTestMethod_ShouldNotBeNull()
        {

            var controller = new CustomersController();

            var result = controller.GetCustomer(new Guid("095de47e-f427-4ad8-b5be-1038fa2b0f82"));
            Assert.IsNotNull(result);

        }

        [TestMethod]
        public void GetAllCustomersTestMethod()
        {
            var controller = new CustomersController();

            var result = controller.GetCustomers(null,null);

            Assert.IsTrue(result.Result is StatusCodeResult);
        }

        [TestMethod]
        public void GetBookingTestMethod_BadRequestResult()
        {

            var controller = new BookingsController();

            var result = controller.GetBooking(new Guid("095de47e-f427-4ad8-b5be-1038fa2b0f82"));

            Assert.IsFalse(result.Result is OkObjectResult);

        }

        [TestMethod]
        public void GetAllBookingsTestMethod_ShouldNotBeNull()
        {

            var controller = new BookingsController();

            var result = controller.GetBookings(null, null);

            Assert.IsNotNull(result.Result);

        }

        //[TestMethod]
        //public void GetAllCustomersTestMethod()
        //{
        //    var controller = new CustomersController();

        //    var result = controller.GetCustomers(null, null);

        //    Assert.IsTrue(result.Result is StatusCodeResult);
        //}


    }
}
