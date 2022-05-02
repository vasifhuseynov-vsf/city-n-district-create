using Microsoft.AspNetCore.Identity;
using RentCar.Data;
using RentCar.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentCar.Areas.Admin.Models.ViewModel
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string[] Roles { get; set; }
        public List<BlockedUser> BlockedUsers { get; set; }
    }
}
