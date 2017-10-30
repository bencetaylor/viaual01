using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LunchTrain.Data;

namespace LunchTrain.Pages.Groups
{
    public class InviteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public InviteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Group Group { get; set; }

        public ApplicationUser User { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
