using RentCar.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentCar.Areas.Admin.Models.ViewModel
{
    public class ContactFormViewModel
    {
        public string Email { get; set; }
        public string Message { get; set; }
    }
}
