using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using Util;
using Util.Commands;

namespace simple_chat_client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Socket __client = null;
        string __ip = string.Empty;
        int __port = 1200;
        bool __isConneced = false;
        static List<string> __clientCommands = new List<string>() { "/exit", "#nickChanged", "#join" };

        private void btnConnect_Click(object sender, EventArgs e)
        {
            
            __ip = tbIp.Text;
            int.TryParse(tbPort.Text, out __port);
            try
            {
                if (!__isConneced)
                {
                    __client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    __client.Connect(__ip, __port);
                    insertOnRtbHistory("Connected on server.");
                    __isConneced = true;
                    userStateObj __state = new userStateObj();
                    __state.__socket = __client;
                    __client.BeginReceive(__state.__buffer, 0, userStateObj.__buffersize, SocketFlags.None, new AsyncCallback(_recieveMessage), __state);
                    btnConnect.Text = "Disconnect";

                }
                else
                {
                    ProcessSendingMessage(logout.invokeCommand);
                    btnConnect.Text = "Connected";
                }

            }
            catch (Exception __e)
            {
                MessageBox.Show(__e.ToString());
            }
        }
        private void _recieveMessage(IAsyncResult iasync)
        {
            userStateObj __state = (userStateObj)iasync.AsyncState;
            Socket __handler = __state.__socket;
            try
            {

                //o metodo abaixo avalia se o socket do usuario permanece conectado ao servidor.
                if (__state.SocketIsConnected())
                {
                    int i = __handler.EndReceive(iasync);
                    string __messageReceived = Encoding.UTF8.GetString(__state.__buffer, 0, i);
                    ProcessRecievingMessage(__messageReceived);
                    __client.BeginReceive(__state.__buffer, 0, userStateObj.__buffersize, SocketFlags.None, new AsyncCallback(_recieveMessage), __state);
                }
                else
                {
                    throw new SocketException();
                }
            }
            catch (SocketException)
            {
                insertOnRtbHistory("Disconected from server.");
                __isConneced = false;
            }
            catch (Exception __e)
            {
                insertOnRtbHistory("Disconected from server.");
                __isConneced = false;
                MessageBox.Show(__e.ToString());
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string __message = tbMessage.Text;
            if (!string.IsNullOrWhiteSpace(__message))
            {
                if (__isConneced)
                {
                    ProcessSendingMessage(__message);
                }
                else
                {
                    insertOnRtbHistory("Client Not Connecetd on Server.");
                }
            }
            this.Invoke(new MethodInvoker(delegate ()
            {
                //limpa a textBox de mensagem
                this.tbMessage.Text = "";
            }));
        }

        private void ProcessSendingMessage(string __message)
        {
            //validando se a mensagem contem um comando, caso sim, get comando.
            string __commandFound = commandController.ValidateCommands(__message);

            //caso o comando seja encontrado, é testado se este é um comando ClientSide ou não.
            bool isClientCommand = false;
            if (!string.IsNullOrWhiteSpace(__message))
            {
                isClientCommand = __clientCommands.Any(__message.StartsWith);
            }
            if (isClientCommand)
            {
                //decidindo qual comando executar
                switch (__commandFound)
                {
                    case exitClient.invokeCommand:
                        Application.Exit();
                        break;
                }
            }
            else
            {
                __client.Send(Encoding.UTF8.GetBytes(__message));
            }
            
        }

        private void ProcessRecievingMessage(string __messageReceived)
        {
            //TODO melhorar esta lógica/tomada de decisão.
            //validando se a mensagem contem um comando, caso sim, get comando.
            string __commandFound = commandController.ValidateCommands(__messageReceived,"client");

            //caso o comando seja encontrado, é testado se este é um comando ClientSide ou não.
            bool isClientCommand = false;
            if (!string.IsNullOrWhiteSpace(__messageReceived))
            {
                isClientCommand = __clientCommands.Any(__messageReceived.StartsWith);
            }
            if (isClientCommand)
            {
                //decidindo qual comando executar
                switch (__commandFound)
                {
                    case "#nickChanged":
                        string __newNickname = changeNickname.getFirstArgument(__messageReceived, __commandFound);
                        this.Invoke(new MethodInvoker(delegate ()
                        {
                            this.tbNickname.Text = __newNickname;
                        }));
                        break;
                }
            }
            else
            {
                insertOnRtbHistory(__messageReceived);
            }

        }
        public void insertOnRtbHistory(string __message)
        {
            this.Invoke(new MethodInvoker(delegate ()
            {
                this.rtbHistory.AppendText(__message + Environment.NewLine);
            }));
        }

        private void tbnChangeNick_Click(object sender, EventArgs e)
        {
            string __message = "/changeNick " + tbNickname.Text;
            if (!string.IsNullOrWhiteSpace(__message))
            {
                if (__isConneced)
                {
                    ProcessSendingMessage(__message);
                }
                else
                {
                    insertOnRtbHistory("Client Not Connecetd on Server.");
                }
            }
        }
    }
}
