using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LunchTrain.Data;
using Microsoft.AspNetCore.Identity;

namespace LunchTrain.Pages.Groups
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<Group> Group { get;set; }
        public ApplicationUser user { get; set; }
        public IList<Group> UserMemberGroups { get; set; }
        public IList<Group> UserApplicationGroup { get; set; }

        [BindProperty]
        public Group Input { get; set; }

        public async Task<IActionResult> OnPostAsync(string id)
        {

            if (id == null)
            {
                return NotFound();
            }

            Input = await _context.Groups.FindAsync(id);
            var currentuser = await _userManager.GetUserAsync(HttpContext.User);

            if (Input != null)
            {

                GroupApplication groupApplication = new GroupApplication();
                groupApplication.Group = Input;
                groupApplication.User = currentuser;
                groupApplication.GroupID = Input.Name;
                groupApplication.UserID = currentuser.Id;

                _context.GroupApplications.Add(groupApplication);

                await _context.SaveChangesAsync();

            }

            return RedirectToPage("./Index");
        }

        public async Task OnGetAsync()
        {
            Group = await _context.Groups.Include(x => x.Owner).ToListAsync();
            user = await _userManager.GetUserAsync(HttpContext.User);
            
            UserMemberGroups = new List<Group>();
            foreach (var member in _context.GroupMemberships) {
                if (member.UserID == user.Id) {
                    UserMemberGroups.Add(await _context.Groups.Include(x => x.Owner).SingleOrDefaultAsync(m => m.Name == member.GroupID));
                }
            }

            UserApplicationGroup = new List<Group>();
            foreach (var member in _context.GroupApplications)
            {
                if (member.UserID == user.Id)
                {
                    UserApplicationGroup.Add(await _context.Groups.Include(x => x.Owner).SingleOrDefaultAsync(m => m.Name == member.GroupID));
                }
            }
        }
    }
}
