using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gateway.Controllers;
using Gateway.Models;
using Gateway;


namespace UnitTests
{
    [TestClass]
    public class GatewayTests
    {
        [TestMethod]
        public async Task GetCustomer_ShouldFindCustomer()
        {
            CustomersController cc = new CustomersController();
            await cc.GetCustomer(new Guid("095de47e-f427-4ad8-b5be-1038fa2b0f82"));

        }

        private List<Customer> GetTestCustomers()
        {
            var testCustomers = new List<Customer>();
            testCustomers.Add(new Customer { CustomerId = new Guid("095de47e-f427-4ad8-b5be-1038fa2b0f82"), Name = "Anton", Surname = "Strebkov", PhoneNumber = 2284523 });
            testCustomers.Add(new Customer { CustomerId = new Guid("dbe35e54-9d2a-405c-addb-b1d2ddfc0711"), Name = "Pavel", Surname = "Kiselev", PhoneNumber = 123456789 });


            return testCustomers;
        }

    }
}
