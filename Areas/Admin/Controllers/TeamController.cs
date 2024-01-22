using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Praktikabitdi.Areas.Admin.ViewModels;
using Praktikabitdi.DAL;
using Praktikabitdi.Models;
using Praktikabitdi.Utilities.Extension;

namespace Praktikabitdi.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="Admin")]
    public class TeamController : Controller
    {
        private readonly AppDbcontext _context;
        private readonly IWebHostEnvironment _env;

        public TeamController(AppDbcontext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<Team> teams = await _context.Teams.ToListAsync();

            return View(teams);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateTeamVM teamVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            bool result = await _context.Teams.AnyAsync(t => t.Name.Trim().ToLower() == teamVM.Name.Trim().ToLower());
            if (result)
            {
                ModelState.AddModelError("Name", "This name is exists");
                return View();
            }
            if (!teamVM.Photo.ValidateType("image/"))
            {
                ModelState.AddModelError("Photo", "Photo type unavailable");
                return View();
            }
            if (!teamVM.Photo.ValidateSize(2*1024))
            {
                ModelState.AddModelError("Photo", "Photo size unavailable");
                return View();
            }
            string filename = await teamVM.Photo.CreateFileAsync(_env.WebRootPath, "assets", "images");
            Team team = new Team
            {
                Image= filename,
                Name = teamVM.Name,
                Description = teamVM.Description,
            };
            await _context.Teams.AddAsync(team);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Update(int id)
        {
            if (id<=0)
            {
                return BadRequest();
            }
            Team existed= await _context.Teams.FirstOrDefaultAsync(t=>t.Id== id);
            if (existed==null)
            {
                return NotFound();
            }
            UpdateTeamVM updateTeamVM = new UpdateTeamVM
            {
                Name = existed.Name,
                Description = existed.Description,
                Image = existed.Image,
            };
           return View(updateTeamVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id,UpdateTeamVM teamVM)
        {
            if (!ModelState.IsValid)
            {
                return View(teamVM);
            }

            bool result= await _context.Teams.AnyAsync(t=>t.Name.Trim().ToLower()==teamVM.Name.Trim().ToLower()&& t.Id!=id);
           
            if (result)
            {
                ModelState.AddModelError("Name", "Bu adda movcuddur");
                return View (teamVM);
            }
            Team existed= await _context.Teams.FirstOrDefaultAsync(t=> t.Id== id);
            if (existed is null)
            {
                return NotFound();
            }
            if (teamVM.Photo is not null)
            {
                if (!teamVM.Photo.ValidateType("image/"))
                {
                    ModelState.AddModelError("Photo", "Photo type unavailable");
                    return View();
                }
                if (!teamVM.Photo.ValidateSize(2 * 1024))
                {
                    ModelState.AddModelError("Photo", "Photo size unavailable");
                    return View();
                }
                string newImage = await teamVM.Photo.CreateFileAsync(_env.WebRootPath, "assets", "images");
                teamVM.Image.DeleteFile(_env.WebRootPath, "assets", "images");
                existed.Image = newImage;
            }
            existed.Name = teamVM.Name;
            existed.Description = teamVM.Description;

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int id)
        {
            if (id<=0)
            {
                return BadRequest();
            }
            Team existed = await _context.Teams.FirstOrDefaultAsync(t => t.Id == id);
            if (existed is null)
            {
                return NotFound();
            }
            existed.Image.DeleteFile(_env.WebRootPath, "assets", "images");

            _context.Teams.Remove(existed);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");

        }
    }
}
