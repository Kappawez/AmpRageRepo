using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AmpRageRepo.Models;

namespace AmpRageRepo.Controllers
{
    public class UsersController : Controller
    {
        private readonly AmpContext _context;

        public UsersController(AmpContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.Include(x => x.UserCars).ToListAsync());
        }

        public async Task<IActionResult> Login(UserViewModel UserViewModel)
        {
            var user = await _context.Users.Where(x => x.Name == UserViewModel.Name && x.Password == UserViewModel.Password).FirstOrDefaultAsync();
            return RedirectToAction("CreatePath", "Path", user);
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(x => x.UserCars)
                .ThenInclude(y => y.Car)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
        public async Task<IActionResult> AddCarToUser(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            var userCarViewModel = new UserCarViewModel()
            {
                UserId = user.Id,
                AllCarBrands = LicensePlateSearcher.GetAllBrands().Select(x => new SelectListItem
                {
                    Text = x,
                    Value = x.ToString()
                }),
                AllCarModels = LicensePlateSearcher.GetAllModels().Select(x => new SelectListItem
                {
                    Text = x,
                    Value = x.ToString()
                })
            };

            return View(userCarViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddCarToUser(UserCarViewModel userCarViewModel)
        {
            try
            {
                var user = await _context.Users.Include(x => x.UserCars).FirstOrDefaultAsync(m => m.Id == userCarViewModel.UserId);
                //var car = LicensePlateSearcher.CheckForCarInDatabase(userCarViewModel.CarBrand, userCarViewModel.CarMake);
                var newMake = userCarViewModel.CarMake.ToString().Split('-')[0].Trim().Replace(' ', ';');
                var car = await _context.Cars.FirstOrDefaultAsync(c => c.Make == newMake);

                if(user != null && car != null)
                {
                    var userCar = new UserCar
                    {
                        User = user,
                        Car = car
                    };

                    user.UserCars.Add(userCar);

                    _context.Add(userCar);

                    await _context.SaveChangesAsync();

                    return RedirectToAction("CreatePath", "Path", user);

                }
                else
                {
                    throw new Exception("NULL");
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return RedirectToAction("CreatePath", "Path");
        }
        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserViewModel userViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Name = userViewModel.Name,
                    Phone = userViewModel.Phone,
                    Email = userViewModel.Email,
                    Password = userViewModel.Password
                };

                _context.Add(user);

                await _context.SaveChangesAsync();
                return RedirectToAction("CreatePath", "Path", user);
            }

            return View();
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Phone,Email,Password")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
