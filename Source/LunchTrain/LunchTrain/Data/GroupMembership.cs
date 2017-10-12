using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LunchTrain.Data
{
    public class GroupMembership
    {
        public int GroupMembershipID { get; set; }

        public string GroupID { get; set; }

        public string UserID { get; set; }

        public Group Group { get; set; }

        public ApplicationUser User { get; set; }
    }
}
