using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LunchTrain.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LunchTrain.Views.Shared.Components.GroupStatusComponent
{
    public class GroupStatusViewModel
    {
        public string GroupName { get; set; }

        public bool IsCurrentUserOwner { get; set; }

        public bool IsCurrentUserStatusSet { get; set; }

        public StatusFlag CurrentUserStatus { get; set; }

        public List<GroupStatusMemberViewModel> GroupMembers { get; set; }

        public string CanGoButtonAction => $"/groups/{GroupName}/signal/ready";

        public string CannotGoButtonAction => $"/groups/{GroupName}/signal/cantgo";

        public string ForceButtonAction => $"/groups/{GroupName}/force";
    }

    public class GroupStatusMemberViewModel
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        public StatusFlag MemberStatus { get; set; }
    }

    [ViewComponent(Name = "GroupStatusComponent")]
    public class GroupStatusComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public GroupStatusComponent(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string groupName)
        {
            var group = _context.Groups.Single(x => x.Name == groupName);

            // this should be fine as we're only rendering this component when logged in
            var currentUserId = (await _userManager.GetUserAsync(HttpContext.User)).Id;

            var members = _context.GroupMemberships.Where(x => x.GroupID == groupName);
            var flags = members.Select(x => _context.GroupMemberFlags.Single(y => y.UserID == x.UserID && y.GroupID == groupName));

            var vm = new GroupStatusViewModel()
            {
                GroupName = groupName,
                IsCurrentUserOwner = group.OwnerID == currentUserId,
                IsCurrentUserStatusSet = flags.Single(x => x.UserID == currentUserId).Status != StatusFlag.WaitingForAnswer,
                CurrentUserStatus = flags.Single(x => x.UserID == currentUserId).Status,
                GroupMembers = flags.Where(f => f.UserID != currentUserId).Select(x => new GroupStatusMemberViewModel
                {
                    UserId = x.UserID,
                    UserName = x.User.FullName,
                    MemberStatus = x.Status
                }).ToList()
            };

            return View(vm);
        }

    }
}
