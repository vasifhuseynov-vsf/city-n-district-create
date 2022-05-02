using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentCar.Areas.Admin.Models.ViewModel;
using RentCar.DAL;
using RentCar.Data;
using RentCar.Tools.EmailHandler;
using RentCar.Tools.EmailHandler.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentCar.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = RoleConstants.SuperAdmin)]
    public class ContactFormController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailService _mailService;

        public ContactFormController(AppDbContext dbContext, UserManager<IdentityUser> userManager, IEmailService mailService)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _mailService = mailService;
        }
        public async Task<IActionResult> Index()
        {
            var contactForms = await _dbContext.ContactForms.ToListAsync();
            return View(contactForms);
        }

        //***** Response *****//

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FormResponse(ContactFormViewModel model)
        {

            await _mailService.SendEMailAsync(new MailRequest
            {
                ToEmail = model.Email,
                Subject = "Cavab",
                Body = model.Message
            });

            return RedirectToAction("Index", "ContactForm");
        }
    }
}