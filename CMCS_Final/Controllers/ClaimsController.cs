using CMCS_Final.Data;
using CMCS_Final.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CMCS_Final.Controllers
{
    [Authorize(Roles = "Lecturer")]
    public class ClaimsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ClaimsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult Claims()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Claims(Claims claim)
        {
            if (ModelState.IsValid)
            {
                var file = Request.Form.Files["DocumentFileName"];
                if (file != null && file.Length > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    claim.DocumentFileName = fileName;
                    claim.DocumentFilePath = filePath;
                }

                if (claim.HoursWorked < 0 || claim.RatePerHour < 0)
                {
                    ModelState.AddModelError("", "Hours Worked and Rate Per Hour must be positive values.");
                }
                else
                {
                    claim.TotalAmount = claim.HoursWorked * claim.RatePerHour;
                    claim.Status = "Verifying and Approving...";
                    claim.Lecturer_ID = User.FindFirstValue(ClaimTypes.NameIdentifier); 
                    _context.Add(claim);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = true;
                    return RedirectToAction(nameof(AllClaims));
                }
            }

            return View(claim);
        }

        [Authorize(Roles = "Academic Manager, Programme Coordinator")] 
        public async Task<IActionResult> AllClaims()
        {
            var claims = await _context.Claims.ToListAsync();
            return View(claims);
        }

        [HttpPost]
        [Authorize(Roles = "Academic Manager")] 
        public async Task<IActionResult> ApproveClaim(int id)
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim == null) return NotFound();

            claim.Status = "Approved";
            _context.Update(claim);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(AllClaims));
        }

        [HttpPost]
        [Authorize(Roles = "Academic Manager")]
        public async Task<IActionResult> RejectClaim(int id)
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim == null) return NotFound();

            claim.Status = "Rejected";
            _context.Update(claim);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(AllClaims));
        }

        public async Task<IActionResult> ViewMyClaims()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var claims = await _context.Claims
                .Where(c => c.Lecturer_ID == userId)
                .ToListAsync();

            return View(claims);
        }
    }
}