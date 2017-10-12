using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LunchTrain.Data;
using Microsoft.AspNetCore.Mvc;

namespace LunchTrain.Controllers
{
    public class GroupsController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public GroupsController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool GroupNameAvailable([Bind(Prefix = "Group.Name")] string groupName)
        {
            return !_dbContext.Groups.Any(x => x.Name == groupName);
        }
    }
}
