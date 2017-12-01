using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LunchTrain.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LunchTrain.Pages.Groups
{
    public class QuitModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public QuitModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
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

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            var user = _context.GroupMemberships.SingleOrDefault(x => x.GroupID == id && x.UserID == currentUser.Id);
            if (user != null) _context.GroupMemberships.Remove(user);
            var flag = _context.GroupMemberFlags.SingleOrDefault(x => x.GroupID == id && x.UserID == currentUser.Id);
            if (flag != null) _context.GroupMemberFlags.Remove(flag);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
