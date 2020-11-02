using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using StrM;//advoid deserialization can not find the assembly
namespace Chat_Client
{
    public partial class MainForm : Form
    {
        //Client is responsible for receiving data messages sent by the server thread
        //客户端负责接收服务端发来的数据消息的线程  
        Thread threadClient = null;
        //Create a client socket, responsible for connecting to the server
        //创建客户端套接字，负责连接服务器  
        Socket socketClient = null;

        public MainForm()
        {
            //  
            // The InitializeComponent() call is required for Windows Forms designer support.  
            //  
            InitializeComponent();
            //Turn off checking for cross-thread text box actions
            //关闭对文本框跨线程操作的检查  
            TextBox.CheckForIllegalCrossThreadCalls = false;
            //  
            // TODO: Add constructor code after the InitializeComponent() call.  
            //  
        }

        //connect to the server
        //连接服务器  
        void BtnConnectClick(object sender, EventArgs e)
        {
            //Obtain the IP address object in the text box
            //获得文本框中的IP地址对象  
            IPAddress address = IPAddress.Parse(txtIP.Text.Trim());
            //Create a network node object that contains IPs and ports
            //创建包含IP和端口的网络节点对象  
            IPEndPoint endpoint = new IPEndPoint(address, int.Parse(txtPort.Text.Trim()));
            //Create a client socket, responsible for connecting to the server
            //创建客户端套接字，负责连接服务器  
            socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                //Client-side connect to the Server
                //客户端连接到服务器  
                socketClient.Connect(endpoint);
            }
            catch (SocketException ex)
            {
                ShowMsg("SocketException:" + ex.Message);
            }
            catch (Exception ex)
            {
                ShowMsg("Exception:" + ex.Message);
            }

            threadClient = new Thread(ReceiveMsg);
            threadClient.IsBackground = true;
            threadClient.Start();
        }
        //Send a text message to the server
        //向服务器发送文本消息  
        void BtnSendMsgClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(listMember.Text))
            {
                MessageBox.Show("Please select a friend to send this message.");
            }
            else
            {
                StrMessage strMsg = new StrMessage();
                strMsg.ToMessage = txtMsgSend.Text.Trim();
                strMsg.ToTopic = txtTpcSend.Text.Trim();
                strMsg.ToName = listMember.Text;
                byte[] arrMsg = SerObject.SerializeObject(strMsg);
                byte[] arrMsgSend = new byte[arrMsg.Length + 1];
                arrMsgSend[0] = 0;//设置标识位，0代表发送的是文字  
                Buffer.BlockCopy(arrMsg, 0, arrMsgSend, 1, arrMsg.Length);
                try
                {
                    socketClient.Send(arrMsgSend);
                    ShowMsg(string.Format("#{0}# I speak to {1}:{2}", strMsg.ToTopic, strMsg.ToName, strMsg.ToMessage));
                    //清空发送消息文本框中的消息  
                    this.txtMsgSend.Text = "";
                }
                catch (SocketException ex)
                {
                    ShowMsg("SocketException:" + ex.Message);
                }
                catch (Exception ex)
                {
                    ShowMsg("Exception:" + ex.Message);
                }
            }
        }

        //The client-side sends a nickname to the server
        //客户端向服务器发送昵称
        void BtnConfirmClick(object sender, EventArgs e)
        {
            string strMsg = txtNickname.Text.Trim();
            //Convert strings to binary arrays for easy web delivery
            //将字符串转成方便网络传送的二进制数组  
            byte[] arrMsg = Encoding.UTF8.GetBytes(strMsg);
            byte[] arrMsgSend = new byte[arrMsg.Length + 1];
            arrMsgSend[0] = 1;//Set the flag, 1 is sent to the nickname 设置标识位，1代表发送的是昵称  
            Buffer.BlockCopy(arrMsg, 0, arrMsgSend, 1, arrMsg.Length);//从arrMsg的第0位开始复制到arrMsgSend的第1位开始，共复制arrMsg.Length位
            try
            {
                socketClient.Send(arrMsgSend);

                ShowMsg(string.Format("You successful change the nickname to {0}.", strMsg));
                //Empty the message in the Send Message text box
                //清空发送消息文本框中的消息  
                this.txtMsgSend.Text = "";
            }
            catch (SocketException ex)
            {
                ShowMsg("SocketException:" + ex.Message);
            }
            catch (Exception ex)
            {
                ShowMsg("Exception:" + ex.Message);
            }
        }

        void ShowMsg(string msg)
        {
            txtMsg.AppendText(msg + "\r\n");
        }

        /// <summary>  
        /// Listen to the message sent by the server监听服务端发来的消息  
        /// </summary>  
        void ReceiveMsg()//0 is an ordinary message, 1 is a user list 0是普通消息，1是用户列表
        {
            while (true)
            {
                //Define a receive byte array buffer (2M size)
                //定义一个接收消息用的字节数组缓冲区（2M大小）  
                byte[] arrMsgRev = new byte[1024 * 1024 * 2];
                //Save the received data to arrMsgRev and return the length of the data actually received
                //将接收到的数据存入arrMsgRev,并返回真正接收到数据的长度  
                int length = -1;
                try
                {
                    length = socketClient.Receive(arrMsgRev);
                }
                catch (SocketException ex)
                {
                    ShowMsg("SocketException:" + ex.Message);
                    break;
                }
                catch (Exception ex)
                {
                    ShowMsg("Exception:" + ex.Message);
                    break;
                }
                if (arrMsgRev[0] == 0) //Judge the client sent the first element of the data is 0, on behalf of the text 判断客户端发送过来数据的第一位元素是0，代表文字  
                {
                    //At this point all the elements of the array (each byte) are converted to a string, and really received only the server sent a few characters
                    //此时是将数组的所有元素（每个字节）都转成字符串，而真正接收到只有服务端发来的几个字符  
                    //string strMsgReceive = Encoding.UTF8.GetString(arrMsgRev, 1, length - 1);
                    byte[] strMsgRev = new byte[arrMsgRev.Length - 1];
                    Buffer.BlockCopy(arrMsgRev, 1, strMsgRev, 0, arrMsgRev.Length - 1);
                    StrMessage strMsgReceive = (StrMessage)SerObject.DeserializeObject(strMsgRev);
                    ShowMsg(string.Format("#{0}# {1} speak to me {2}",strMsgReceive.ToTopic, strMsgReceive.ToName, strMsgReceive.ToMessage));
                }
                else if (arrMsgRev[0] == 1)//1 is the online user list 1是在线用户列表
                {
                    byte[] strMsgRev = new byte[arrMsgRev.Length - 1];
                    Buffer.BlockCopy(arrMsgRev, 1, strMsgRev, 0, arrMsgRev.Length - 1);
                    List<string> listname = (List<String>)SerObject.DeserializeObject(strMsgRev);
                   
                    listMember.Items.Clear(); //Empty the text 清空文本
                    foreach (string name in listname)
                        listMember.Items.Add(name);
                }
            }
        }

    }
    static class Program
    {
        /// <summary>
        /// The main entry point for the application 应用程序的主入口点
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }











}
