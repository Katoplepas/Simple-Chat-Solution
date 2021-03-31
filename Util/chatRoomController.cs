using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util
{
    public class chatRoomController
    {

        public static chatRoom GetUserChatRoom(List<chatRoom> __listChatRoom, userStateObj __userObj)
        {

            var results = __listChatRoom.FirstOrDefault(l => l.__listUser != null && l.__listUser.Any(u => u.Equals(__userObj)));

            return results;
        }


        public static void MoveUserToRoom(ref chatRoom oldChatRoom, ref chatRoom newChatRoom, userStateObj __userObj)
        {
            oldChatRoom.removeUser(__userObj);
            newChatRoom.insertUser(__userObj);
        }

    }
}
