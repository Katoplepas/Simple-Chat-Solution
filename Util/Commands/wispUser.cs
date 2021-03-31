using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util.Commands
{
    public class wispUser
    {
        public const string invokeCommand = "/w"; // comando em constante é necessario para ser usado no Case.
        public static string getFirstArgument(string __message, string __commandFound)
        {
            return commandController.getFirstArgument(__message, __commandFound);
        }

        public static string getSecondArgument(string __message, string __commandFound, string __firstArgument = "")
        {
            return commandController.getSecondArgument(__message, __commandFound, __firstArgument);
        }

    }
}
