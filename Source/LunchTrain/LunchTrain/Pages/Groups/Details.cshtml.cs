using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LunchTrain.Data;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LunchTrain.Pages.Groups
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DetailsModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Group Group { get; set; }
        public ApplicationUser User { get; set; }

        public ApplicationUser currentUser { get; set; }
        public List<ApplicationUser> Users { get; set; }
        public List<GroupMemberFlag> Flags { get; set; }
        public List<ApplicationUser> Applications { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        

        public async Task<IActionResult> OnPostAsync(string id)
        {
            var user = await _userManager.FindByNameAsync(Input.Email);
            
            if (user == null || (await _userManager.IsEmailConfirmedAsync(user)))
            {
                Console.WriteLine(Input.Email);
                // Don't reveal that the user does not exist or is not confirmed
                return RedirectToPage("./Index");
            }
            else
            {
                // Add user to group
                Group = await _context.Groups.Include(x => x.Owner).SingleOrDefaultAsync(m => m.Name == id);

                await AddUserToGroup(Group.Name, user.Id);

                // Ha egyb�l visszaadom az oldalt valami�rt nem fejezi be az adatb�zis m�veleteket �s az OnGet-n�l elsz�ll
                return RedirectToPage("./Index");
               // return Page();
            }
        }

        private async Task AddUserToGroup(string groupId, string userId)
        {
            await _context.GroupMemberships.AddAsync(new GroupMembership
            {
                GroupID = groupId,
                UserID = userId
            });
            await _context.GroupMemberFlags.AddAsync(new GroupMemberFlag
            {
                GroupID = groupId,
                UserID = userId,
                Status = StatusFlag.WaitingForAnswer
            });
            await _context.SaveChangesAsync();
        }

        private async Task RemoveUserFromGroup(string groupId, string userId)
        {
            var user = _context.GroupMemberships.SingleOrDefault(x => x.GroupID == groupId && x.UserID == userId);
            if (user != null) _context.GroupMemberships.Remove(user);
            var flag = _context.GroupMemberFlags.SingleOrDefault(x => x.GroupID == groupId && x.UserID == userId);
            if (flag != null) _context.GroupMemberFlags.Remove(flag);
            await _context.SaveChangesAsync();
        }

        private async Task<bool> RemoveUserApplicationFromGroup(string groupId, string userId)
        {
            var app = _context.GroupApplications.SingleOrDefault(x => x.UserID == userId && x.GroupID == groupId);
            if (app == null) return false;
            _context.GroupApplications.Remove(app);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IActionResult> OnGetAsync(string id, string accept, string ignore, string drop)
        {
            if (id == null)
            {
                return NotFound();
            }

            Group = await _context.Groups.Include(x => x.Owner).SingleOrDefaultAsync(m => m.Name == id);
            currentUser = await _userManager.GetUserAsync(HttpContext.User);

            if (Group == null)
            {
                return NotFound();
            }
            
            if (Group.OwnerID == currentUser.Id)
            {                
                if (!string.IsNullOrWhiteSpace(accept))
                {
                    var existed = await RemoveUserApplicationFromGroup(id, accept);
                    if (existed) await AddUserToGroup(id, accept);
                    // yes I know this is ugly but the id parameter gets eaten by the route thingy if I use the normal redirectToAction
                    return new LocalRedirectResult($"~/Groups/Details?id={id}");
                }
                if(!string.IsNullOrWhiteSpace(ignore))
                {
                    await RemoveUserApplicationFromGroup(id, ignore);
                    // yes I know this is ugly but the id parameter gets eaten by the route thingy if I use the normal redirectToAction
                    return new LocalRedirectResult($"~/Groups/Details?id={id}");
                }
                if (!string.IsNullOrWhiteSpace(drop))
                {
                    await RemoveUserFromGroup(id, drop);
                    // yes I know this is ugly but the id parameter gets eaten by the route thingy if I use the normal redirectToAction
                    return new LocalRedirectResult($"~/Groups/Details?id={id}");
                }
            }

            Users = new List<ApplicationUser>();

            foreach (var member in _context.GroupMemberships)
            {
                if (member.GroupID == Group.Name)
                {
                    Users.Add(await _userManager.FindByIdAsync(member.UserID));
                }
            }

            //Flags = _context.GroupMemberFlags.ToList();

            Flags = new List<GroupMemberFlag>();

            foreach (var member in _context.GroupMemberFlags)
            {
                if (member.GroupID == Group.Name)
                {
                    Flags.Add(await _context.GroupMemberFlags.Include(x => x.User).SingleOrDefaultAsync(m => m.GroupMemberFlagID == member.GroupMemberFlagID));
                }
            }

            Applications = new List<ApplicationUser>();

            foreach (var member in _context.GroupApplications)
            {
                if (member.GroupID == Group.Name)
                {
                    Applications.Add(await _userManager.FindByIdAsync(member.UserID));
                }
            }

            return Page();
        }
    }
}
