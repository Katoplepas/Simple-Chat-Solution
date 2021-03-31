using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Util
{
    public class chatRoom
    {
        public string name { get; set; }
        public List<userStateObj> __listUser { get;}

        public chatRoom(string name)
        {
            this.name = name;
            this.__listUser = new List<userStateObj>();
        }

        public void insertUser(userStateObj userToInsert)
        {
            this.__listUser.Add(userToInsert);
        }

        public void removeUser (userStateObj userToRemove)
        {
            this.__listUser.Remove(userToRemove);
        }

        public void broadcastMessage(string messageToBroadcast)
        {
            foreach (userStateObj __user in this.__listUser)
            {
                var __socket = __user.__socket;
                __socket.Send(Encoding.UTF8.GetBytes(messageToBroadcast));
            }
        }

        public void sendPrivateMessageToUser(string privateMessageToSend, userStateObj userToRecieve)
        {
            var userToRecievefound = this.__listUser.FirstOrDefault(x => x.__nickName == userToRecieve.__nickName);

            if(userToRecievefound != null)
            {
                userToRecievefound.__socket.Send(Encoding.UTF8.GetBytes(privateMessageToSend));
            }

        }
    }
}

