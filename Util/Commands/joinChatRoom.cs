namespace Util.Commands
{
    public class joinChatRoom
    {
        public const string invokeCommand = "/join"; // comando em constante é necessario para ser usado no Case.

        public static string getFirstArgument(string __message, string __commandFound)
        {
            return commandController.getFirstArgument(__message, __commandFound);
        }

    }
}
