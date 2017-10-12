using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace LunchTrain.Data
{
    public class Group
    {
        [Key]
        [Remote("GroupNameAvailable", "Groups")]
        public string Name { get; set; }

        public string Description { get; set; }

        public string OwnerID { get; set; }

        public ApplicationUser Owner { get; set; }
    }
}
