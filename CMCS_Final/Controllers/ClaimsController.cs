using CMCS_Final.Data;
using CMCS_Final.Models;
using Microsoft.AspNetCore.Authorization;
using CMCS_Final.Data;
using CMCS_Final.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Security.Claims;

namespace CMCS_Final.Controllers
{
    [Authorize]
    public class ClaimsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        // Constructor
        public ClaimsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // HTTP GET for displaying the claim submission form
        [Authorize(Roles = "Lecturer")]
        [HttpGet]
        public IActionResult Claims()
        {
            return View();
        }

        // HTTP POST for processing the claim submission form
        [HttpPost]
        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> Claims(Claims claim)
        {
            if (ModelState.IsValid)
            {
                // Handle file upload
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

                // Check if HoursWorked and RatePerHour are valid
                if (claim.HoursWorked < 0 || claim.RatePerHour < 0)
                {
                    ModelState.AddModelError("", "Hours Worked and Rate Per Hour must be positive values.");
                }
                else
                {
                    claim.TotalAmount = claim.HoursWorked * claim.RatePerHour;
                    claim.Status = "Verifying and Approving...";
                    _context.Add(claim);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = true; // Indicate success
                    return RedirectToAction(nameof(AllClaims));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(claim);
        }
        // HTTP GET for viewing all claims
        [Authorize(Roles = "Programme Co-ordinator")]
        public async Task<IActionResult> AllClaims()
        {
            var claims = await _context.Claims.ToListAsync();
            return View(claims);
        }


        // HTTP POST for approving a claim
        [Authorize(Roles = "Programme Co-ordinator")]
        [HttpPost]
        public async Task<IActionResult> ApproveClaim(int id)
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim == null) return NotFound();

            claim.Status = "Approved"; // Update status
            _context.Update(claim);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(AllClaims));
        }

        // HTTP POST for rejecting a claim
        [Authorize(Roles = "Programme Co-ordinator")]
        [HttpPost]
        public async Task<IActionResult> RejectClaim(int id)
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim == null) return NotFound();

            claim.Status = "Rejected"; // Update status
            _context.Update(claim);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(AllClaims));
        }

        [Authorize(Roles = "Lecturer")]
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


