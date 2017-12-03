using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LunchTrain.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace LunchTrain.Pages.Groups
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DeleteModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
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

                        Group = await _context.Groups.FindAsync(id);

            if (Group != null && Group.OwnerID == (await _userManager.GetUserAsync(HttpContext.User)).Id)
            {
                foreach(var i in _context.GroupMemberships)
                {
                    if(i.GroupID == Group.Name)
                    {
                        _context.GroupMemberships.Remove(i);
                    }
                }

                foreach(var i in _context.GroupMemberFlags)
                {
                    if(i.GroupID == Group.Name)
                    {
                        _context.GroupMemberFlags.Remove(i);
                    }
                }

                _context.Groups.Remove(Group);
                await _context.SaveChangesAsync();

                HttpContext.Session.SetString(IndexModel.SessionKeyMessege, "Group deleted successfully!");
                HttpContext.Session.SetString(IndexModel.SessionKeyMessegeType, "success");
            }
            else
            {
                HttpContext.Session.SetString(IndexModel.SessionKeyMessege, "Can't delete group!");
                HttpContext.Session.SetString(IndexModel.SessionKeyMessegeType, "danger");
            }

            return RedirectToPage("./Index");
        }
    }
}
