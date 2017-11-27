using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LunchTrain.Data;
using LunchTrain.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LunchTrain.Controllers
{
    public class GroupsController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public GroupsController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public bool GroupNameAvailable([Bind(Prefix = "Group.Name")] string groupName)
        {
            return !_dbContext.Groups.Any(x => x.Name == groupName);
        }

        private StatusFlag? ParseFlag(string signal)
        {
            switch (signal)
            {
                case "cantgo":
                    return StatusFlag.CannotGo;
                case "ready":
                    return StatusFlag.ReadyToGo;
                default:
                    return null;
            }
        }

        [HttpGet("/groups/{groupName}/signal/{signal}")]
        public async Task<IActionResult> SignalStatus(string groupName, string signal)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            if (currentUser == null)
            {
                // user not logged in
                return RedirectToPage("Index");
            }

            var cid = currentUser.Id;

            var groupMembership = _dbContext.GroupMemberships.FirstOrDefault(x => x.UserID == cid && x.GroupID == groupName);

            if (groupMembership == null)
            {
                // user is not a member of group
                return RedirectToPage("Index");
            }

            var gmFlag = _dbContext.GroupMemberFlags.SingleOrDefault(x => x.UserID == cid && x.GroupID == groupName);

            if (gmFlag == null)
            {
                // create flag, weird
                _dbContext.GroupMemberFlags.Add(new GroupMemberFlag
                {
                    GroupID = groupName,
                    Status = ParseFlag(signal) ?? StatusFlag.WaitingForAnswer,
                    UserID = cid
                });
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                var flag = ParseFlag(signal);
                if (flag.HasValue)
                {
                    gmFlag.Status = flag.Value;
                    await _dbContext.SaveChangesAsync();
                }
            }

            await CheckSendNotifications(groupName);

            return new RedirectResult("/groups/");
        }

        private async Task CheckSendNotifications(string groupName)
        {
            var memberships = _dbContext.GroupMemberships.Where(x => x.GroupID == groupName);
            var flags = _dbContext.GroupMemberFlags.Where(x => x.GroupID == groupName);
            if (!memberships.All(x => flags.Any(y => y.UserID == x.UserID && y.Status != StatusFlag.WaitingForAnswer))) return;

            foreach (var groupMemberFlag in flags)
            {
                groupMemberFlag.Status = StatusFlag.WaitingForAnswer;
                var user = await _userManager.FindByIdAsync(groupMemberFlag.UserID);
                if (user == null) continue; // this shouldn't happen ever, but who knows...
                await _emailSender.SendEmailAsync(user.Email, $"LunchTrain {groupName} is ready to go", $"Your LunchTrain group {groupName} is ready to go, as everyone set their status!");
            }
            await _dbContext.SaveChangesAsync();
        }
    }
}
