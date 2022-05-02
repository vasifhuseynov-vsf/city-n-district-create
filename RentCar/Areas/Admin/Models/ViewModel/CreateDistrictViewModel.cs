using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentCar.Areas.Admin.Models.ViewModel
{
    public class CreateDistrictViewModel
    {
        public int CityId { get; set; }
        public string DistrictName { get; set; }
    }
}
