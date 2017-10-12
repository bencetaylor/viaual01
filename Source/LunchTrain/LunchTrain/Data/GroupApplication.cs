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
        public int GroupApplicationID { get; set; }

        public string GroupID { get; set; }

        public string UserID { get; set; }

        public Group Group { get; set; }

        public ApplicationUser User { get; set; }
    }
}
