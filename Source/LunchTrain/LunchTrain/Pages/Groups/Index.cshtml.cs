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
        public bool alreadyUsed = false;

        public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<Group> Group { get;set; }
        public ApplicationUser user { get; set; }
        public IList<Group> UserMemberGroups { get; set; }
        public IList<Group> UserApplicationGroup { get; set; }
        public IList<GroupMemberFlag> Flags { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public string Name { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {

            if (Input.Name == null)
            {
                return NotFound();   
            }

            var currentuser = await _userManager.GetUserAsync(HttpContext.User);
            Group group = await _context.Groups.Include(x => x.Owner).SingleOrDefaultAsync(m => m.Name == Input.Name);

            if (group != null)
            {

                GroupApplication groupApplication = new GroupApplication();
                groupApplication.Group = group;
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

            Flags = new List<GroupMemberFlag>();

            foreach (var member in _context.GroupMemberFlags)
            {
                if (member.UserID == user.Id)
                {
                    Flags.Add(await _context.GroupMemberFlags.Include(x => x.User).SingleOrDefaultAsync(m => m.GroupMemberFlagID == member.GroupMemberFlagID));
                }
            }

        }
    }
}
