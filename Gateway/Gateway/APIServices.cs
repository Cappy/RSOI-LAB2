﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gateway
{
    public class APIServices
    {
        public string customersAPI = "http://localhost:9001/api/customers";
        public string roomsAPI = "http://localhost:9003/api/rooms";
        public string bookingsAPI = "http://localhost:9005/api/bookings";
        public string authAPI = "http://localhost:8030/o"; //main
        public string gatewayAPI = "http://localhost:4314/api";
        //public string authAPI = "http://localhost:2833/api/users";  //test

    }
}
