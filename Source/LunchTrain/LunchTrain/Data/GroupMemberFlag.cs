using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LunchTrain.Data
{
    public class GroupMemberFlag
    {
        [Key, Column(Order = 1)]
        public string GroupID { get; set; }
        [Key, Column(Order = 2)]
        public string UserID { get; set; }
        public int Status { get; set; }

        [ForeignKey("GroupID")]
        public virtual Group Group { get; set; }
        [ForeignKey("UserID")]
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
