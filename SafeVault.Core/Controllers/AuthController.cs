using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SafeVault.Core.Models;

namespace SafeVault.Core.Controllers
{
    public class AuthController(SafeVaultContext context) : Controller
    {
        private readonly SafeVaultContext _context = context;

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Submit()
        {
            return View("Submit"); // Loads Submit.cshtml from Views/Auth/
        }

        [HttpGet]
        public IActionResult Success()
        {
            return View("Success");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Submit(UserInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); // Returns the form with validation errors
            }

            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Role = model.Role
            };

            // Check if the username already exists
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == model.Username);

            if (existingUser != null)
            {
                ModelState.AddModelError("Username", "Username already exists.");
                return View("Submit", model); // Returns the form with validation errors
            }

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Process valid data (e.g., save to database)
            return RedirectToAction("Success"); // Redirects to the Success.cshtml view
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View("Register");
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Register", model); // Returns the form with validation errors
            }
            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password)
            };

            // Check if the username already exists
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == model.Username);

            if (existingUser != null)
            {
                ModelState.AddModelError("Username", "Username already exists.");
                return View("Register", model); // Returns the form with validation errors
            }

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Success"); // Redirects to the Success.cshtml view
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View("Login");
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginInputModel model)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Model state is invalid");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                }
                return View("Login", model); // Returns the form with validation errors
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == model.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
            {
                Console.WriteLine("Invalid username or password.");
                ModelState.AddModelError("", "Invalid username or password.");
                return View("Login", model); // Returns the form with validation errors
            }

            // Create claims for the user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role) // Add the role claim
            };

            // Create the identity and principal
            var identity = new ClaimsIdentity(claims, "MyCookieAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("MyCookieAuth", principal); // Store username in session  
            // Perform login logic (e.g., set session, redirect to dashboard)
            Console.WriteLine("Model state is good");
            return RedirectToAction("Success"); // Redirects to the Success.cshtml view
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> UserList()
        {
            var users = await GetAllUsers();

            if (users == null)
                Console.WriteLine("The user list is null");

            else
                Console.WriteLine("The user is there, but razor page is too dumb for this shit");

            var model = new UserListViewModel { Users = users };
            return View(model);
        }

        public async Task<List<User>?> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            return users; // Returns the list of users to UserList.cshtml
        }

        /*
        [HttpPost]
        public IActionResult ProcessForm(UserInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Submit", model); // Returns the form with validation errors
            }

            return RedirectToAction("Submit"); // Redirects to the Submit.cshtml view
        }
        */
    }
}
