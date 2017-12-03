using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LunchTrain.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace LunchTrain.Pages.Groups
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EditModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Group Group { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Group = await _context.Groups.Include(x => x.Owner).SingleOrDefaultAsync(m => m.Name == id);

            if (Group == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Group != null && Group.OwnerID == (await _userManager.GetUserAsync(HttpContext.User)).Id) { 
                _context.Attach(Group).State = EntityState.Modified;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                
            }

            HttpContext.Session.SetString(IndexModel.SessionKeyMessege, "Saved changes for " + Group.Name + " group!");
            HttpContext.Session.SetString(IndexModel.SessionKeyMessegeType, "success");

            return RedirectToPage("./Index");
        }
    }
}
