﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.DTO
{
    public class CustomerDto
    {

        public int CustomerId;
        public string Code;
        public string Contact;
        public string Address;
        public string Name;
        public double Latitude;
        public double Longitude;
        public string Email;
        public bool Status;
        public List<String> Tags;
        public string Description;
        public bool IsFacturable { get; set; }

        public CustomerDto()
        {

            Tags = new List<string>();
        }
    
    }

    public class CustomerWithVarioDto : CustomerDto
    {
        public CustomerWithVarioDto()
        {
            Tags = new List<string>();
            isVario = false;
        }

        public bool isVario { get; set; }
    }
}