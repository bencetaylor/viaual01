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

                _context.GroupMemberships.Add(groupMembership);

                await _context.SaveChangesAsync();

                // Ha egybõl visszaadom az oldalt valamiért nem fejezi be az adatbázis mûveleteket és az OnGet-nél elszáll
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

            return Page();
        }
    }
}
