using BlogProjectPrac7.Data;
using BlogProjectPrac7.Models;
using BlogProjectPrac7.Models.ViewModel;
using BlogProjectPrac7.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using X.PagedList;

namespace BlogProjectPrac7.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IBlogEmailSender _emailSender;
        private readonly IBlogService _blogService;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IBlogEmailSender emailSender, IBlogService blogService)
        {
            _logger = logger;
            _context = context;
            _emailSender = emailSender;
            _blogService = blogService;
        }

        public async Task<IActionResult> Index(int? page)
        {
            var pageNumber = page ?? 1;
            var pageSize = 6; //how many records we want on each page
            var posts = _context.Posts
                                      .Where(p => p.ReadyStatus == Enums.ReadyStatus.ProductionReady)
                                      .OrderByDescending(p => p.Created)
                                      .Include(p => p.BlogUser)
                                      .ToPagedListAsync(pageNumber, pageSize);

            ViewData["HeaderImage"] = Url.Content("~/img/home-bg.jpg");
            ViewData["MainText"] = "My Coding Journal";
            ViewData["SubText"] = "Browse the below blogs to learn more about my coding journey";
            //ViewData["CategoryIds"] = new SelectList(await _blogService.GetCategoryListAsync(), "Id", "Name");
            return View(await posts);
        }



        public IActionResult Contact()
        {
            ViewData["HeaderImage"] = Url.Content("~/img/home-bg.jpg");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Contact(ContactMe model)
        {
            //this is Where we'll be emailing.
            model.Message = $"{model.Message} <hr/> Phone: {model.Phone}";
            await _emailSender.SendContactEmailAsync(model.Email!, model.Name!, model.Subject!, model.Message);
            return RedirectToAction("Index", "Home");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
