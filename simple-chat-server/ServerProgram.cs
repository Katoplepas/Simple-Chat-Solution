using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using Util;
using Util.Commands;

namespace simple_chat_server
{
    class ServerProgram
    {
        static bool __isrun = false; //variavel de controle para o Loop central
        //static Dictionary<string, Socket> __listClient = new Dictionary<string, Socket>(); //lista de clients ativos no servidor.
        static List<userStateObj> __listClient = new List<userStateObj>();
        static ManualResetEvent __alldone = new ManualResetEvent(true);
        static Dictionary<string, List<userStateObj>> __listRoom = new Dictionary<string, List<userStateObj>>();
        static List<chatRoom> __listChatRoom = new List<chatRoom>();
        static void Main(string[] args)
        {

            Socket __server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint __ip = new IPEndPoint(IPAddress.Any, 1300);

            __server.Bind(__ip);
            __server.Listen(1000);
            __isrun = true;
            Console.WriteLine("Server listening");
            __listRoom.Add("Main", __listClient);    

            __listChatRoom.Add(new chatRoom("Main"));

            while (__isrun)
            {
                __alldone.Reset();
                __server.BeginAccept(new AsyncCallback(_acceptClient), (Socket)__server);
                __alldone.WaitOne();
            }

        }

        static void _acceptClient(IAsyncResult iasync)
        {

            Socket __server = (Socket)iasync.AsyncState;
            Socket __listener = __server.EndAccept(iasync);
            string __adress = __listener.RemoteEndPoint.ToString();

            userStateObj __userState = new userStateObj();
            __userState.__socket = __listener;
            __userState.__address = __adress;
            Random randomNumber = new Random();
            __userState.__nickName = "newUser" + randomNumber.Next(0, 9999); //TODO adicionar um gerador seguro de nickname
            
            //lista de todos os clientes do servidor.
            __listClient.Add(__userState);

            //todo usuario será adicionado a sala principal.
            chatRoom __mainChatRoom = chatRoomController.GetChatRoomByName(__listChatRoom,"Main");
            __mainChatRoom.insertUser(__userState);

            Console.WriteLine("Client <{0}> connected", __adress);

            __mainChatRoom.broadcastMessage(String.Format("User {0} connected!", __userState.__nickName));

            __alldone.Set();
            __listener.BeginReceive(__userState.__buffer, 0, userStateObj.__buffersize, SocketFlags.None, new AsyncCallback(_recieveMessage), __userState);

            //espera de 100ms para garantir que as mensagens sejam enviadas separadas.
            __alldone.WaitOne(500, false);
            __alldone.Set();
            ServerSendMessage(__userState, "#nickChanged " + __userState.__nickName);
            //__alldone.Set();
            //__listener.BeginReceive(__userState.__buffer, 0, userStateObj.__buffersize, SocketFlags.None, new AsyncCallback(_recieveMessage), __userState);
        }

        static void _recieveMessage(IAsyncResult iasync)
        {
            userStateObj __userState = (userStateObj)iasync.AsyncState;
            Socket __handler = __userState.__socket; //socket do usuario;

            try
            {
                //Variavel I recebe o tamanho da informação a ser recebida, para serir de parametro na operação de getString abaixo.
                int i = __handler.EndReceive(iasync);
                string __messageRecieved = Encoding.UTF8.GetString(__userState.__buffer, 0, i);
                string __data = string.Format("{0}->{1}", __userState.__nickName, __messageRecieved);

                string __commandFound = commandController.ValidateCommands(__messageRecieved);

                if (!string.IsNullOrWhiteSpace(__commandFound))
                {

                    //decidindo qual comando executar
                    switch (__commandFound)
                    {
                        case logout.invokeCommand:
                            try
                            {
                                __userState.__socket.Shutdown(SocketShutdown.Both);
                            }
                            finally
                            {
                                __listClient.Remove(__userState);
                                __userState.__socket.Close();
                            }
                            break;
                        case changeNickname.invokeCommand:
                            string __oldNick = __userState.__nickName;
                            var __nickNameToChange = changeNickname.getFirstArgument(__messageRecieved, __commandFound);
                            if (__oldNick != __nickNameToChange)
                            {
                                __userState.__nickName = __nickNameToChange;
                                string __messageToPlot = String.Format("UserNick Change from {0} -> to {1}", __oldNick, __nickNameToChange);
                                Console.WriteLine(__messageToPlot);
                                
                                ServerSendMessage(__userState, "#nickChanged " + __userState.__nickName);

                                __alldone.WaitOne(200, false);
                                var __userChatRoom = chatRoomController.GetUserChatRoom(__listChatRoom, __userState);
                                __userChatRoom.broadcastMessage(__messageToPlot);
                           
                            }
                            break;
                        case wispUser.invokeCommand:
                            var __userSenderNickname = __userState.__nickName;
                            var __userRecipientNickname = wispUser.getFirstArgument(__messageRecieved, __commandFound);
                            var __messageToSend = wispUser.getSecondArgument(__messageRecieved, __commandFound, __userRecipientNickname);

                            var __userRecipient = __listClient.FirstOrDefault(x => x.__nickName == __userRecipientNickname);
                            var __userSender = __userState;
                            if (!string.IsNullOrWhiteSpace(__userRecipientNickname) && !string.IsNullOrWhiteSpace(__messageToSend) && __userRecipient != null)
                            {
                                string __messageToRecipientUser = String.Format("Message from [{0}]: {1}", __userSenderNickname, __messageToSend);//mesagem a ser enviada ao destinatário
                                string __messageToSenderUser = String.Format("Message delivered to [{0}]: {1}", __userRecipientNickname,__messageToSend);//mensagem a ser enviada ao remetente.
                                ServerSendMessage(__userRecipient, __messageToRecipientUser);
                                ServerSendMessage(__userSender, __messageToSenderUser);
                            }
                            break;

                        case joinChatRoom.invokeCommand:
                            var __joinChatRoomName = joinChatRoom.getFirstArgument(__messageRecieved, __commandFound);
                            __listChatRoom = chatRoomController.InsertUserIntoRoom(__listChatRoom, __joinChatRoomName, __userState);
                            var __userNewChatRoom = chatRoomController.GetUserChatRoom(__listChatRoom, __userState);
                            __userNewChatRoom.broadcastMessage(String.Format("User {0} enter the room {1}",__userState.__nickName, __joinChatRoomName));
                            break;

                        default:
                            break;
                    }
                    if (__userState.SocketIsConnected())
                    {
                        __handler.BeginReceive(__userState.__buffer, 0, userStateObj.__buffersize, SocketFlags.None, new AsyncCallback(_recieveMessage), __userState);
                    }
                }
                else
                {
                    //caso a mensagem não contenha nenhum comando, o fluxo segue normalmente.
                    if (__userState.SocketIsConnected())
                    {
                        //Escreve no Console do Servidor
                        Console.WriteLine("Recieve from {0}", __data);
                        __handler.BeginReceive(__userState.__buffer, 0, userStateObj.__buffersize, SocketFlags.None, new AsyncCallback(_recieveMessage), __userState);
                    }
                    else
                    {
                        Console.WriteLine("User {0} disconnected.", __userState.__nickName);
                    }

                    List<userStateObj> __RoomFound = __listRoom.FirstOrDefault(x => x.Key == "Main").Value;

                    var __userChatRoom = chatRoomController.GetUserChatRoom(__listChatRoom, __userState);
                    __userChatRoom.broadcastMessage(__data);
                }

            }
            catch (SocketException)
            {
                //infornando e retirando o client da lista de Clients ativos.
                Console.WriteLine("Client <{0}> disconnected", __userState.__address);
                __listClient.Remove(__userState);
            }
            catch (Exception __e)
            {
                Console.WriteLine(__e.ToString());
            }
        }

        private static void ServerSendMessage(userStateObj __userToSend, string __messageToSend)
        {
            //este metodo envia mensagens independente das chatRooms, em caso de Wisp por exemplo.
            if (__userToSend != null)
            {
                __userToSend.__socket.Send(Encoding.UTF8.GetBytes(__messageToSend));
            }
        }

        private static void  ServerSendBroadcast(string __messageToSend)
        {
            //mensagem para todos os usuarios do sistema.
            foreach (userStateObj __state in __listClient)
            {
                __state.__socket.Send(Encoding.UTF8.GetBytes(__messageToSend));
            }
        }
    }
}
