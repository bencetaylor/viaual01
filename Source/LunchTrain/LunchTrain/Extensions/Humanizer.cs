using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LunchTrain.Data;

namespace LunchTrain.Extensions
{
    public static class Humanizer
    {
        public static string HumanizeStatusFlag(this StatusFlag flag, bool isFirstPerson)
        {
            switch (flag)
            {
                case StatusFlag.WaitingForAnswer:
                    return isFirstPerson ? "You haven't answered yet." : "No answer yet.";
                case StatusFlag.ReadyToGo:
                    return isFirstPerson ? "You already said you're ready to go." : "Ready to go.";
                case StatusFlag.CannotGo:
                    return isFirstPerson ? "You already said you can't go." : "Can't go.";
                default:
                    return "";
            }
        }
    }
}
