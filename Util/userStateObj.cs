using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Util
{
    //class userStateObj
    //{
    //}

    public class userStateObj
    {
        public Socket __socket = null;
        public const int __buffersize = 1024;
        public byte[] __buffer = new byte[__buffersize];
        public string __address = string.Empty;
        public string __nickName = string.Empty;

        public bool SocketIsConnected()
        {
            try
            {
                return !(this.__socket.Poll(1, SelectMode.SelectRead) && this.__socket.Available == 0);
            }
            catch (SocketException) { return false; }
            catch (ObjectDisposedException) { return false;}
        }
            
    
        public void changeUserNick(userStateObj userToChange, Dictionary<string, Socket> userList)
        {


        }
    
    
    }






    

}
