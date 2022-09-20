using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmployeeManager.Data;
using EmployeeManager.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.Graph;

namespace EmployeeManager.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly ChallengeContext _context;
        

        public EmployeesController(ChallengeContext context)
        {
            _context = context;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Employee objUser)
        {
            if (ModelState.IsValid)
            {
               
                {
                    var obj = _context.Employees.Where(a => a.Email.Equals(objUser.Email) && a.Password.Equals(objUser.Password)).FirstOrDefault();

                    if (obj != null)
                    {
                        HttpContext.Session.SetInt32("ID", obj.Id);
                        HttpContext.Session.SetString("FirstName", obj.Firstname);
                        HttpContext.Session.SetString("LastName", obj.LastName);
                        HttpContext.Session.SetString("Email", obj.Email);

                        
                        return RedirectToAction("index");
                    }
                }
            }
            return View(objUser);
        }


        //public async Task<IActionResult> Login(string Email, string Password)
        //{
        //    if (Email == null)
        //    {
        //        return NotFound();
        //    }

        //    var employee = await _context.Employees
        //        .FirstOrDefaultAsync(m => m.Email == Email & m.Password == Password);
        //    if (employee == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(employee);
        //}
        // GET: Employees
        public async Task<IActionResult> Index()
        {
            if (ViewBag.ID = HttpContext.Session.GetInt32("ID") != null)
            {
                ViewBag.ID = HttpContext.Session.GetInt32("ID");
                ViewBag.FirstName = HttpContext.Session.GetString("FirstName");
                ViewBag.LastName = HttpContext.Session.GetString("LastName");
                ViewBag.Email = HttpContext.Session.GetString("Email");
                return View(await _context.Employees.ToListAsync());
            }
            return RedirectToAction("Login");
        }
        public ActionResult LogOut()
        {
           
                HttpContext.Session.Clear();
                return  RedirectToAction("Login");
           
        }
        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (ViewBag.ID = HttpContext.Session.GetInt32("ID") == null)
            {
                return RedirectToAction("Login");
            }
                if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Firstname,LastName,Email,Gender,Password")] Employee employee)
        {
            var emp = await _context.Employees
                .FirstOrDefaultAsync(m => m.Email == employee.Email);

            if (ModelState.IsValid & emp==null)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login");
            }
            return View();
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (ViewBag.ID = HttpContext.Session.GetInt32("ID") == null)
            {
                return RedirectToAction("Login");
            }
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Firstname,LastName,Email,Gender,Password")] Employee employee)
        {
            if (ViewBag.ID = HttpContext.Session.GetInt32("ID") == null)
            {
                return RedirectToAction("Login");
            }
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
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
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (ViewBag.ID = HttpContext.Session.GetInt32("ID") == null)
            {
                return RedirectToAction("Login");
            }
            if (id == null)
            {
                return NotFound();
            }
            if (id == HttpContext.Session.GetInt32("ID"))
            {
                return RedirectToAction(nameof(Index));
            }
            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }
           
            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            _context.Employees.Remove(employee);
           
            await _context.SaveChangesAsync();
            if (_context.Employees.Count() == 0)
            {
                return RedirectToAction("Login");
            }
            
            return RedirectToAction(nameof(Index));
        }

        private IActionResult BadRequestResult()
        {
            return BadRequest(); 
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
    }
}
