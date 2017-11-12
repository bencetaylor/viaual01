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

        public async Task OnGetAsync()
        {
            Group = new List<Group>();
            user = await _userManager.GetUserAsync(HttpContext.User);
            foreach (var member in _context.GroupMemberships) {
                if (member.UserID == user.Id) {
                    Group.Add(await _context.Groups.Include(x => x.Owner).SingleOrDefaultAsync(m => m.Name == member.GroupID));
                }
            }
        }
    }
}
