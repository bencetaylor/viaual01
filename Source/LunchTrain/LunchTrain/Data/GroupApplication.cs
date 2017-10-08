using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LunchTrain.Data
{
    public class GroupApplication
    {
        [Key, Column(Order = 1)]
        public string GroupID { get; set; }

        [Key, Column(Order = 2)]
        public string UserID { get; set; }

        [ForeignKey("GroupID")]
        public Group Group { get; set; }
        [ForeignKey("UserID")]
        public ApplicationUser ApplicationUser { get; set; }
    }
}
