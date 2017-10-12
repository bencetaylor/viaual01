using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LunchTrain.Data
{
    public enum StatusFlag
    {
        WaitingForAnswer = 0,
        ReadyToGo = 1,
        CannotGo = 2
    }

    public class GroupMemberFlag
    {
        public int GroupMemberFlagID { get; set; }

        public StatusFlag Status { get; set; }

        public string GroupID { get; set; }

        public string UserID { get; set; }

        public Group Group { get; set; }

        public ApplicationUser User { get; set; }
    }
}
