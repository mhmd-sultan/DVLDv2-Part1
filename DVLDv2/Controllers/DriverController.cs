using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DVLD.Data;
using DVLD.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using DVLD.Models.Car;
using AutoMapper;
using DVLD.Models.Driver;

namespace DVLD.Controllers
{
    [Authorize]
    public class DriverController : Controller
    {
        #region Data and Constructor

        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public DriverController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        #endregion

        #region Actions

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            List<Driver> drivers = await _context
                                  .Drivers
                                  .ToListAsync();

            List<DriverVM> driverVMs = _mapper.Map<List<Driver>, List<DriverVM>>(drivers);

            return View(driverVMs);
        }

        [AllowAnonymous]
        public async Task<IActionResult> DriverCars(int id)
        {
            string driverName = await _context
                                    .Drivers
                                    .Where(driver => driver.Id == id)
                                    .Select(driver => $"{driver.FirstName} {driver.LastName}")
                                    .SingleAsync();


            List<Car> driverCars = await _context
                                            .Cars
                                            .Include(car => car.Driver)
                                            .Where(car => car.Driver.Id == id)
                                            .ToListAsync();

            List<CarVM> carVMs = _mapper.Map<List<Car>, List<CarVM>>(driverCars);

            return View(carVMs);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Driver driver = await _context
                                        .Drivers
                                        .FirstOrDefaultAsync(m => m.Id == id);

            if (driver == null)
            {
                return NotFound();
            }

            DriverVM driverVM = _mapper.Map<Driver, DriverVM>(driver);

            return View(driverVM);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DriverVM DriverVM)
        {
            if (ModelState.IsValid)
            {
                Driver driver = _mapper.Map<DriverVM, Driver>(DriverVM);

                _context.Add(driver);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(DriverVM);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Driver driver = await _context.Drivers.FindAsync(id);

            if (driver == null)
            {
                return NotFound();
            }

            DriverVM driverVM = _mapper.Map<Driver, DriverVM>(driver);

            return View(driverVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DriverVM DriverVM)
        {
            if (id != DriverVM.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Driver driver = _mapper.Map<DriverVM, Driver>(DriverVM);

                    _context.Update(driver);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DriverExists(DriverVM.Id))
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

            return View(DriverVM);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var driver = await _context.Drivers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (driver == null)
            {
                return NotFound();
            }

            return View(driver);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var driver = await _context.Drivers.FindAsync(id);
            _context.Drivers.Remove(driver);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Private Methods

        private bool DriverExists(int id)
        {
            return _context.Drivers.Any(e => e.Id == id);
        }

        #endregion

    }
}
