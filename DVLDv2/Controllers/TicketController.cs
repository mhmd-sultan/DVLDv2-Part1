using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DVLD.Data;
using DVLD.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using DVLD.Models.Tickets;
using DVLDv2.Data;
using DVLD.Models.Car;
using DVLD.Models.Driver;

namespace DVLD.Controllers
{
    [Authorize]
    public class TicketController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private dynamic carsListItems;
        private dynamic driversListItems;

        public TicketController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        #region Actions

        // GET: Ticket
        public async Task<IActionResult> Index()
        {
            List<TicketVM> ticketViewModels = await _context
                                            .Tickets
                                            .Include(ticket => ticket.Driver)
                                            .Include(ticket => ticket.Car)
                                            .Select(ticket => _mapper.Map<TicketVM>(ticket))
                                            .ToListAsync();

            return View(ticketViewModels);
        }

        // GET: Ticket/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context
                                .Tickets
                                .Include(ticket => ticket.Driver)
                                .Include(ticket => ticket.Car)
                                .FirstOrDefaultAsync(m => m.Id == id);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync()
        {



            List<Driver> drivers = await _context
                                        .Drivers
                                        .ToListAsync();

            List<DriverVM> driverVMs = _mapper.Map<List<Driver>, List<DriverVM>>(drivers);

            SelectList driversListItems = new SelectList(driverVMs, "Id", "FullName");

            ViewBag.DriversListItems = driversListItems;



            List<Car> Cars = await _context
                                        .Cars
                                        .ToListAsync();


            List<CarVM> CarVMs = _mapper.Map<List<Car>, List<CarVM>>(Cars);

            SelectList carsListItems = new SelectList(CarVMs, "Id", "Manu");

            ViewBag.CarsListItems = carsListItems;


            return View();
        }

        // GET: Ticket/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.DriversListItems = driversListItems;

            ViewBag.CarsListItems = carsListItems;

            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context
                                    .Tickets
                                    .Include(ticket => ticket.Driver)
                                    .Include(ticket => ticket.Car)
                                    .FirstOrDefaultAsync(m => m.Id == id);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Ticket/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IssueDate,VehicleOwnerName,VehicleModel,LicensePlate,Location,Reason,Amount,OfficerName")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.Id))
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
            return View(ticket);
        }

        // GET: Ticket/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Ticket/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Private Functions

        private bool TicketExists(int id)
        {
            return _context.Tickets.Any(e => e.Id == id);
        }

        private async Task<SelectList> GetCarsListItems()
        {
            List<Car> cars = await _context
                                    .Cars
                                    .ToListAsync();

            SelectList carsListItems = new SelectList(cars, "Id", "LicensePlate");

            return carsListItems;
        }

        private async Task<SelectList> GetDriversListItems()
        {
            List<Driver> drivers = await _context
                                            .Drivers
                                            .ToListAsync();

            SelectList driversListItems = new SelectList(drivers, "Id", "FullName");

            return driversListItems;
        }

        #endregion
    }
}
