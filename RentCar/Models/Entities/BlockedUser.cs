using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentCar.Models.Entities
{
    public class BlockedUser : BaseEntity
    {
        public string UserID { get; set; }
    }
}
