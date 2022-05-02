using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentCar.DAL;
using RentCar.Models.Entities;
using RentCar.Models.ViewModels;
using RentCar.Models.ViewModels.Car;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentCar.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _dbContext;

        public HomeController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            return View(new HomeIndexViewModel { 
                CarModels = await _dbContext.CarModels.Where(c => !c.IsDeleted)
                .Include(c => c.Car)
                .Include(c => c.District).ThenInclude(d => d.City)
                .Include(c => c.CarImages.Where(i => i.IsMain))
                .Where(c => c.Rating > 3).Take(4).ToListAsync()
            });
        }

        //***** LoadMore *****//
        public async Task<IActionResult> LoadMore(int skipCount)
        {
            var carModels = await _dbContext.CarModels.Where(c => !c.IsDeleted)
                .Skip(skipCount).Take(4).Include(c => c.Car)
                .Include(c => c.District).ThenInclude(d => d.City)
                .Include(c => c.CarImages.Where(i => i.IsMain))
                .ToListAsync();

            return PartialView("_CarModelPartial", new HomeIndexViewModel { CarModels = carModels});
        }

        //***** Search *****//
        public async Task<IActionResult> SearchCar(CarSearchViewModel model)
        {

            var carModels = await _dbContext.CarModels.Where(c => !c.IsDeleted && c.ModelName == model.CarModelName
            && c.District.City.Name == model.CityName && (c.CurrentPrice > model.MinPrice && c.CurrentPrice < model.MaxPrice))
                .Include(c => c.Car)
                .Include(c => c.District).ThenInclude(d => d.City)
                .Include(c => c.CarImages.Where(i => i.IsMain))
                .ToListAsync();

            List<CarModel> searchedCarList = new List<CarModel>();

            var rentedCars = await _dbContext.RentedCars.ToListAsync();


            if (model.StartDate > model.EndDate || model.StartDate < DateTime.Now || model.EndDate < DateTime.Now)
            {
                ViewBag.Error = "Tarix düzgün göstərilməyib";
            }
            else
            {
                if (carModels == null || !carModels.Any())
                {
                    ViewBag.NoResult = "Axtarışa uyğun nəticə tapılmadı";
                }
                else
                {
                    for (int i = 0; i < carModels.Count; i++)
                    {
                        for (int j = 0; j < rentedCars.Count; j++)
                        {
                            if (carModels[i].Id == rentedCars[j].CarModelId)
                            {
                                if ((model.StartDate < rentedCars[j].StartDate && model.EndDate < rentedCars[j].StartDate)
                                       || (model.StartDate > rentedCars[j].EndDate && model.EndDate > rentedCars[j].EndDate))
                                {
                                    searchedCarList.Add(carModels[i]);
                                }
                            }
                            else
                            {
                                searchedCarList.Add(carModels[i]);
                            }
                        }
                    }
                }
                
            }

            
            return View(searchedCarList);

        }

    }
}
