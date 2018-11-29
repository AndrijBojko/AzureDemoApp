using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AzureAppExapmle.Models;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;

namespace AzureAppExapmle.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        ExampleDBContext _context = new ExampleDBContext();

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View(_context.Product.FirstOrDefault());
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact(string name)
        {
            string azureBaseUrl = "https://exampleappfunc.azurewebsites.net/api/HttpTrigger1"; // url to azure function
            string urlQueryStringParams = $"?code=...&name={name}"; // can be found on azure portal


            if (name != null) {
                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage res = client.GetAsync($"{azureBaseUrl}{urlQueryStringParams}").GetAwaiter().GetResult())
                    {
                        using (HttpContent content = res.Content)
                        {
                            string data = content.ReadAsStringAsync().GetAwaiter().GetResult();
                            if (data != null)
                            {
                                ViewData["Message"] = data;
                            }
                            else
                                ViewData["Message"] = "";
                        }
                    }
                }
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
