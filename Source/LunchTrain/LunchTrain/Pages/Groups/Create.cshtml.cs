using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using LunchTrain.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace LunchTrain.Pages.Groups
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CreateModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Group Group { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            Group.OwnerID = (await _userManager.GetUserAsync(HttpContext.User)).Id;

            if (!ModelState.IsValid)
            {
                return Page();
            }
            GroupMembership groupMembership = new GroupMembership();
            groupMembership.Group = Group;
            groupMembership.User = Group.Owner;
            groupMembership.GroupID = Group.Name;
            groupMembership.UserID = Group.OwnerID;

            GroupMemberFlag groupMemberFlag = new GroupMemberFlag();
            groupMemberFlag.Group = Group;
            groupMemberFlag.GroupID = Group.Name;
            groupMemberFlag.User = Group.Owner;
            groupMemberFlag.UserID = Group.OwnerID;
            groupMemberFlag.Status = StatusFlag.WaitingForAnswer;

            _context.GroupMemberships.Add(groupMembership);
            _context.GroupMemberFlags.Add(groupMemberFlag);
            _context.Groups.Add(Group);
            await _context.SaveChangesAsync();

            HttpContext.Session.SetString(IndexModel.SessionKeyMessege, Group.Name + " group created!");
            HttpContext.Session.SetString(IndexModel.SessionKeyMessegeType, "success");

            return RedirectToPage("./Index");
        }
    }
}