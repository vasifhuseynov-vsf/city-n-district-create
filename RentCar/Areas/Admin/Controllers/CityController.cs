using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentCar.Areas.Admin.Models.ViewModel;
using RentCar.DAL;
using RentCar.Data;
using RentCar.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentCar.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = RoleConstants.SuperAdmin)]
    public class CityController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppDbContext _dbContext;

        public CityController(AppDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        //***** Index *****//
        public async Task<IActionResult> Index()
        {
            var cities = await _dbContext.Cities.Include(c => c.Districts).ToListAsync();
            return View(cities);
        }

        //***** Create District *****//

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDistrict(CreateDistrictViewModel model)
        {
            var city = await _dbContext.Cities.Where(c => c.Id == model.CityId).FirstOrDefaultAsync();

            District district = new District
            {
                Name = model.DistrictName,
                City = city
            };

            await _dbContext.Districts.AddAsync(district);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index", "City");
        }

        //***** Create City *****//
        public IActionResult CreateCity()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCity(CreateCityViewModel model)
        {
            City city = new City
            {
                Name = model.CityName
            };

            await _dbContext.Cities.AddAsync(city);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index", "City");
        }

    }
}
