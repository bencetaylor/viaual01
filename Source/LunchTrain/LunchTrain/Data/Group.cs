using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LunchTrain.Data
{
    public class Group
    {
        [Key]
        public string Name { get; set; }

        public string Description { get; set; }

        public string OwnerID { get; set; }

        public ApplicationUser Owner { get; set; }
    }
}
