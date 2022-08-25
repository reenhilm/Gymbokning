using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Gymbokning.Data;
using Gymbokning.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Gymbokning.Models.ViewModels;
using AutoMapper;

namespace Gymbokning.Controllers
{
    public class GymClassesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public GymClassesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            this.mapper = mapper;
        }

        // GET: GymClasses
        public async Task<IActionResult> Index(bool showHistory, bool hasBooking)
        {
            var retGymClassesVM = new IndexViewModel();

            retGymClassesVM.GymClasses = await mapper.ProjectTo<IndexGymClassViewModel>(_context.GymClass
                .AsNoTracking()
                .Include(g => g.ApplicationUserGymClasses)
                .WhereIf(!showHistory, g => g.StartTime > DateTime.Now)
                .WhereIf(hasBooking, g => g.ApplicationUserGymClasses.Count > 0)
                ).ToListAsync();
            retGymClassesVM.ShowHistory = showHistory;
            retGymClassesVM.HasBooking = hasBooking;
            return View(retGymClassesVM);
        }

        [Authorize]
        public async Task<IActionResult> BookingToggle(int? id, bool showHistory, bool hasBooking)
        {
            if (id == null)
                return BadRequest();

            var loggedInUser = await _userManager.GetUserAsync(HttpContext.User);
            if (loggedInUser == null)
                return NotFound();

            var SelectedGymClassForLoggedInUser = await _context.ApplicationUserGymClass
                .AsTracking()
                .Include(ugc => ugc.ApplicationUser)
                .Include(ugc => ugc.GymClass)
                .Where(ugc => ugc.ApplicationUser.Id == loggedInUser.Id && ugc.GymClass.Id == id)
                .ToListAsync();

            //has user not attended? then add user to that gymClass a.k.a. new row in junction-table ApplicationUserGymClass, else remove all rows in junction-table ApplicationUserGymClass
            if (SelectedGymClassForLoggedInUser.Count == 0)
                _context.Add(new ApplicationUserGymClass() { ApplicationUserId = loggedInUser.Id, GymClassId = id.GetValueOrDefault()! });
            else
                _context.ApplicationUserGymClass.RemoveRange(SelectedGymClassForLoggedInUser);
            
            await _context.SaveChangesAsync();

            var routeValues = new RouteValueDictionary {
              { "showHistory", showHistory },
              { "hasBooking", hasBooking }
            };
            return RedirectToAction(nameof(Index), routeValues);
        }

        // GET: GymClasses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return BadRequest();

            var gymClass = await mapper.ProjectTo<GymClassDetailsViewModel>(_context.GymClass.AsNoTracking().Where(m => m.Id == id)).FirstOrDefaultAsync();

            if (gymClass == null)
                return NotFound();

            return View(gymClass);
        }

        // GET: GymClasses/Create
        [Authorize(Roles = RoleNames.AdminRole)]
        public IActionResult Create() => View();

        // POST: GymClasses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = RoleNames.AdminRole)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,StartTime,Duration,Description")] GymClass gymClass)
        {
            if (ModelState.IsValid)
            {
                _context.Add(gymClass);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(gymClass);
        }

        // GET: GymClasses/Edit/5
        [Authorize(Roles = RoleNames.AdminRole)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return BadRequest();

            var gymClass = await _context.GymClass.FindAsync(id);
            if (gymClass == null)
                return NotFound();

            return View(gymClass);
        }

        // POST: GymClasses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = RoleNames.AdminRole)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,StartTime,Duration,Description")] GymClass gymClass)
        {
            if (id != gymClass.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gymClass);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GymClassExists(gymClass.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(gymClass);
        }

        // GET: GymClasses/Delete/5
        [Authorize(Roles = RoleNames.AdminRole)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return BadRequest();

            var gymClass = await _context.GymClass
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gymClass == null)
                return NotFound();

            return View(gymClass);
        }

        // POST: GymClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = RoleNames.AdminRole)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gymClass = await _context.GymClass.FindAsync(id);
            if (gymClass != null)
                _context.GymClass.Remove(gymClass);
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GymClassExists(int id) => (_context.GymClass?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}
