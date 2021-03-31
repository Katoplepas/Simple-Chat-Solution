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

        public static chatRoom GetChatRoomByName(List<chatRoom> __listChatRoom, string __chatRoomName)
        {
            var results = __listChatRoom.FirstOrDefault(x => x.name == __chatRoomName);
            return results;
        }

        public static void MoveUserToRoom(ref chatRoom oldChatRoom, ref chatRoom newChatRoom, userStateObj __userObj)
        {
            oldChatRoom.removeUser(__userObj);
            newChatRoom.insertUser(__userObj);
        }


        public static List<chatRoom> InsertUserIntoRoom(List<chatRoom> __listChatRoom, string __chatRoomName, userStateObj __userObj)
        {
            var __chatToInsertUser = GetChatRoomByName(__listChatRoom, __chatRoomName);
            var __chatToRemoveUser = GetUserChatRoom(__listChatRoom, __userObj);
            if (__chatToInsertUser == null)
            {
                __chatToInsertUser = new chatRoom(__chatRoomName);
                __listChatRoom.Add(__chatToInsertUser);
            }
            MoveUserToRoom(ref __chatToRemoveUser, ref __chatToInsertUser, __userObj);
            return __listChatRoom;
        }

    }
}
