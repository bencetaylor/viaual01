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

                GroupMembership groupMembership = new GroupMembership();
                groupMembership.Group = Group;
                groupMembership.User = user;
                groupMembership.GroupID = Group.Name;
                groupMembership.UserID = user.Id;

                // Create ready-to-go flag for user
                GroupMemberFlag groupMemberFlag = new GroupMemberFlag();
                groupMemberFlag.Group = Group;
                groupMemberFlag.GroupID = Group.Name;
                groupMemberFlag.User = user;
                groupMemberFlag.UserID = user.Id;
                groupMemberFlag.Status = StatusFlag.WaitingForAnswer;

                _context.GroupMemberFlags.Add(groupMemberFlag);

                _context.GroupMemberships.Add(groupMembership);

                await _context.SaveChangesAsync();

                // Ha egyb�l visszaadom az oldalt valami�rt nem fejezi be az adatb�zis m�veleteket �s az OnGet-n�l elsz�ll
                return RedirectToPage("./Index");
               // return Page();
            }
        }

        public async Task<IActionResult> OnGetAsync(string id)
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

            Users = new List<ApplicationUser>();

            foreach (var member in _context.GroupMemberships)
            {
                if (member.GroupID == Group.Name)
                {
                    Users.Add(await _userManager.FindByIdAsync(member.UserID));
                }
            }

            Flags = _context.GroupMemberFlags.ToList();

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
