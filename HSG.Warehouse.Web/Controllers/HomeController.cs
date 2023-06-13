using HSG.Warehouse.Common.Models;
using HSG.Warehouse.Interfaces;
using HSG.Warehouse.Interfaces.Repository;
using HSG.Warehouse.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Security.Claims;

namespace HSG.Warehouse.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISeedData _seedData;
        private readonly IReportRepository _reportRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordValidator _passwordValidator;

        public HomeController(ILogger<HomeController> logger, ISeedData seedData, IReportRepository reportRepository, IUserRepository userRepository, IPasswordValidator passwordValidator)
        {
            _logger = logger;
            _seedData = seedData;
            _reportRepository = reportRepository;
            _userRepository = userRepository;
            _passwordValidator = passwordValidator;
        }
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string? login, string password, string rememberMe)
        {
            var user = await _userRepository.GetByLonigAsync(login);
            var IP = HttpContext.Connection.RemoteIpAddress?.ToString();
            if (user == null || String.IsNullOrEmpty(password))
            {
                TempData["Error"] = "Дані не знайдені";
                // Заношу у базу спробу залогінитись
                _userRepository.LoginLog(login, IP, false);
                return View();
            }

            if (_passwordValidator.VerifyPassword(user.Password, password))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, login)
                };
                var claimIdentity = new ClaimsIdentity(claims, "Cookie");
                var claimPrincipal = new ClaimsPrincipal(claimIdentity);
                var authenticationProperties = new AuthenticationProperties()
                {
                    IsPersistent = rememberMe.IsNullOrEmpty() ? false : true
                };
                await HttpContext.SignInAsync("Cookie", claimPrincipal, authenticationProperties);
                // Заношу у базу спробу залогінитись
                _userRepository.LoginLog(login, IP, true);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["Error"] = "Не вірні дані!";
                // Заношу у базу спробу залогінитись
                _userRepository.LoginLog(login, IP, false);

                return View();
            }

        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookie");
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Index()
        {
            //Якщо пуста база, то заповнюю її стартовим набором даних
            var result = await _seedData.Seed();

            if (result != null)
            {
                return RedirectToAction("AccessDenied");
            }

            //Редірект на сторінку, яку роблю у даний час
            //return RedirectToAction("Index", "Sale");            

            var vm = new ReportViewModel();
            var list = await _reportRepository.GetSaleStat();

            //Для прикладу беру лише поточний рік і валюту - грн           
            list = list?.Where(s => s.Year == DateTime.Now.Year && s.CurrencyId == 1);

            vm.SaleStats = list;
            //var json = JsonSerializer.Serialize(vm.SaleStats);

            var month = String.Empty;
            var saleSum = String.Empty;
            var saleCount = String.Empty;

            foreach (var item in list)
            {
                month += item.Month.ToString() + ", ";
                saleSum += Convert.ToInt32(item.SaleSum).ToString() + ", ";
                saleCount += item.SaleCount.ToString() + ", ";
            }

            ViewBag.Month = month;
            ViewBag.SaleSum = saleSum;
            ViewBag.SaleCount = saleCount;

            return View(vm);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}