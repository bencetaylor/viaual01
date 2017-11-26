using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LunchTrain.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LunchTrain.Views.Shared.Components.GroupMemberComponent
{

    public class GroupMemberViewModel
    {
        public string UserId { get; set; }

        public List<Group> UserMemberGroup { get; set; }
    }

    [ViewComponent(Name = "GroupMemberComponent")]
    public class GroupMemberComponent : ViewComponent
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public GroupMemberComponent(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var currentUserId = (await _userManager.GetUserAsync(HttpContext.User)).Id;

            List<Group> memberGroups = new List<Group>();
            foreach (var member in _context.GroupMemberships)
            {
                if (member.UserID == currentUserId)
                {
                    memberGroups.Add(await _context.Groups.Include(x => x.Owner).SingleOrDefaultAsync(m => m.Name == member.GroupID));
                }
            }

            var vm = new GroupMemberViewModel {
                UserId = currentUserId,
                UserMemberGroup = memberGroups
            };
            return View(vm);
        }
    }
}
