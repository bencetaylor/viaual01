using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LunchTrain.Data;
using Microsoft.EntityFrameworkCore;

namespace LunchTrain.Pages.Groups
{
    public class QuitModel : PageModel
    {
        public void OnGet()
        {
        }
        private readonly ApplicationDbContext _context;

        public QuitModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Group Group { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Group = await _context.Groups.Include(x => x.Owner).SingleOrDefaultAsync(m => m.Name == id);

            if (Group == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            /*if (id == null)
            {
                return NotFound();
            }*/

            //TODO quit from group logic
            
            return RedirectToPage("./Index");
        }
    }
}
