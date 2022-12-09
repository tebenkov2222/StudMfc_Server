using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.EMMA;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ISTU_MFC.Models;
using ISTU_MFC.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ModelsData;
using Repository;

namespace ISTU_MFC.Controllers
{
    public class LogInController : Controller
    {
        private readonly ILogger<LogInController> _logger;

        public LogInController(ILogger<LogInController> logger, UserContext context, IRepository repository)
        {
            _logger = logger;
            db = context;
            _repository = repository;
        }
        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        private UserContext db;
        private readonly IRepository _repository;

        public IActionResult Home()
        {
            var logedin = HttpContext.User.Claims;
            if (logedin.Count()==0)
                return RedirectToAction("Login");
            else
            {
                var userId = Int32.Parse(HttpContext.User.FindFirst(ClaimsIdentity.DefaultNameClaimType).Value);
                bool isStudent = _repository.CheckStudentExistence(userId);
                if (isStudent)
                    return RedirectToAction("Home", "Students");
                else
                    return RedirectToAction("WorkWithDoc", "Employees");
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                HttpClient client = new HttpClient();
                LoginModel loginData = new LoginModel()
                {
                    email = model.email,
                    password = model.password
                };
                string jsonRequest = JsonSerializer.Serialize(loginData);
                using HttpContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                using HttpResponseMessage response = await client.PostAsync("https://istu.ru/api/mobile/login", content).ConfigureAwait(false);
                string response_result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                LoginModel result = null;
                try
                {
                    result = JsonSerializer.Deserialize<LoginModel>(response_result);
                }
                catch
                {
                    result = new LoginModel()
                    {
                        message = "Unauthorized"
                    };
                }

                if (result.message == null)
                {
                    if (!_repository.CheckUserExistence(result.user_id))
                    {
                        var request = new HttpRequestMessage(HttpMethod.Get, "https://istu.ru/api/mobile/user");
                        request.Headers.Authorization = new AuthenticationHeaderValue(result.token_type, result.access_token);
                        HttpResponseMessage response_1 = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                        
                        var apiString = await response_1.Content.ReadAsStringAsync();
                        var student = JsonSerializer.Deserialize<StudentModelForAddToDB>(apiString);
                        _repository.CreateStudent(student);
                    }
                    bool isStudent = _repository.CheckStudentExistence(result.user_id);
                    await Authenticate(result.user_id.ToString(), isStudent ? "Student" : "Employee"); // аутентификация
                    if(isStudent)
                        return RedirectToAction("Home", "Students");
                    else
                        return RedirectToAction("WorkWithDoc", "Employees");
                }
                else
                { 
                    user user = await db.users.FirstOrDefaultAsync(u => u.login == model.email && u.password == model.password);
                    if (user != null)
                    {
                        bool isStudent = _repository.CheckStudentExistence(user.id);
                        await Authenticate(user.id.ToString(), isStudent ? "Student" : "Employee"); // аутентификация
                        if(isStudent)
                            return RedirectToAction("Home", "Students");
                        else
                            return RedirectToAction("WorkWithDoc", "Employees");
                    } 
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }

        private async Task Authenticate(string userName, string userRole)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, userRole),
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
 
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Login");
        }
    }
}
