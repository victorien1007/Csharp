using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Net; //IP,IPAdress(port)  
using System.Net.Sockets;
using System.Threading;
using System.IO;
using StrM;//advoid deserialization can not find the assembly
namespace Chat_Server
{
        public partial class MainForm 
        {
            public MainForm()
            { 
                InitializeComponent();
            //Turn off the checking for cross-thread text box actions
            //关闭对文本框跨线程操作的检查  
                TextBox.CheckForIllegalCrossThreadCalls = false;
            }

            Thread threadWatch = null;//The thread responsible for listening for client requests 负责监听客户端请求的线程  
            Socket socketWatch = null;//Responsible for listening server socket 负责监听服务端的套接字  

        //Save all the server-side nickname and the corresponding socket
        //保存服务器端所有的昵称和对应的套接字
            Array_String clientlist = new Array_String();
            
            //Socket socketConnection = null;//The socket responsible for communicating with the client 负责和客户端通信的套接字  
            //Saves all server-side sockets for client communication
            //保存了服务器端所有和客户端通信的套接字  
            Array_Type<Socket> dict = new Array_Type<Socket>();

            //Saves all the server-side threads responsible for calling the Receive socket of the communication socket
            //保存了服务器端所有负责调用通信套接字的Receive方法的线程  
            Array_Type<Thread> dictThread = new Array_Type<Thread>();

            //Turn on service
            //开启服务  
            void BtnBeginListenClick(object sender, EventArgs e)
            {
                //Create the socket responsible for snooping, use IP4 addressing protocol for parameters, use streaming connection, use TCP protocol to transfer data
                //创建负责监听的套接字，参数使用IP4寻址协议，使用流式连接，使用TCP协议传输数据  
                socketWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //Obtain the IP address object in the text box
                //获得文本框中的IP地址对象  
                IPAddress address = IPAddress.Parse(txtIP.Text.Trim());
                //Create a network node object that contains IPs and ports
                //创建包含IP和port的网络节点对象  
                IPEndPoint endpoint = new IPEndPoint(address, int.Parse(txtPort.Text.Trim()));
                //Bind the socket responsible for snooping to a unique IP and port
                //将负责监听的套接字绑定到唯一的IP和端口上  
                try
                {
                    socketWatch.Bind(endpoint);
                }
                catch (SocketException ex)
                {
                    ShowMsg("SocketException:" + ex.Message);
                    return;
                }
                catch (Exception ex)
                {
                    ShowMsg("Exception:" + ex.Message);
                    return;
                }

                //Set the length of the listening queue
                //设置监听队列的长度  
                socketWatch.Listen(10);

                //Create the thread responsible for snooping and pass in the snooping method
                //创建负责监听的线程，并传入监听方法  
                threadWatch = new Thread(WatchConnection);
                threadWatch.IsBackground = true;//Set as the background thread 设置为后台线程  
                threadWatch.Start();//Open the thread 开启线程  

                ShowMsg("Server start listening successfully!");
            }

            //Send a message to the client
            //发送消息到客户端  
            void BtnSendClick(object sender, EventArgs e)
            {
                if (string.IsNullOrEmpty(listMember.Text))
                {
                    MessageBox.Show("Please select a friend to send the message.");
                }
                else
                {
                    StrMessage strMsg = new StrMessage();
                    strMsg.ToMessage = txtMsgSend.Text.Trim();
                    strMsg.ToTopic = txtTpcSend.Text.Trim();
                    strMsg.ToName = "The server end";
                //set the string which to be sent into UTF-8 corresponding byte array
                //将要发送的字符串转成UTF-8对应的字节数组  
                byte[] arrMsg = SerObject.SerializeObject(strMsg);
                    //noname================================================================================
                    byte[] arrMsgSend = new byte[arrMsg.Length + 1];
                    arrMsgSend[0] = 0;//设置标识位，0代表发送的是文字  
                    Buffer.BlockCopy(arrMsg, 0, arrMsgSend, 1, arrMsg.Length);
                    //Get the list of selected remote IP Key
                    //获得列表中选中的远程IP的Key  
                    String strCli = listMember.Text;
                    string strClientKey = clientlist.toK(strCli);
                    try
                    {
                    //Through the key to find the dictionary set corresponding to a client communication socket Send method to send data to each other
                    //通过key找到字典集合中对应的某个客户端通信的套接字，用Send方法发送数据给对方 
                        Socket s = dict[strClientKey];
                        s.Send(arrMsgSend);

                        ShowMsg(string.Format("#{0}#I speak to {1} : {2}",strMsg.ToTopic, listMember.Text, strMsg.ToMessage));
                        //Clear the message in the send box
                        //清空发送框中的消息  
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
            
            //Send a message to each client
            //群发消息给每个客户端
            void BtnSendToAllClick(object sender, EventArgs e)
            {
                StrMessage strMsg = new StrMessage();
                strMsg.ToMessage = txtMsgSend.Text.Trim();
                strMsg.ToTopic = txtTpcSend.Text.Trim();
                strMsg.ToName = "The server end";
                byte[] arrMsg = SerObject.SerializeObject(strMsg);
                byte[] arrMsgSend = new byte[arrMsg.Length + 1];
                arrMsgSend[0] = 0;//Set the flag, 0 on behalf of the text sent 设置标识位，0代表发送的是文字  
                Buffer.BlockCopy(arrMsg, 0, arrMsgSend, 1, arrMsg.Length);
                for (int i=0; i < dict.Size();i++)
                {

                    try
                    {
                        Socket s = dict[i];
                        s.Send(arrMsgSend);
                        ShowMsg(string.Format("#{0}# I speak to All : {1}",strMsg.ToTopic,strMsg.ToMessage));
                    }
                    catch (SocketException se)
                    {
                        ShowMsg("SocketException:" + se.Message);
                        break;
                    }
                    catch (Exception ex)
                    {
                        ShowMsg("SocketException:" + ex.Message);
                        break;
                    }
                }
            }
            

            void sendNewClientList()
        {
            //Send new user list to all users
            //发送新的用户列表给所有用户
            List<String> list = new List<String>();
            list.Add("The server end");
            for (int i = 0; i < clientlist.Size(); i++)
                list.Add(clientlist[i]);

            byte[] arrMsg = SerObject.SerializeObject(list);
            byte[] arrMsgSend = new byte[arrMsg.Length + 1];
            arrMsgSend[0] = 1;//Set the flag, 1 represents the list of users sent 设置标识位，1代表发送的用户列表 
            Buffer.BlockCopy(arrMsg, 0, arrMsgSend, 1, arrMsg.Length);
            for (int i = 0; i < dict.Size(); i++)
            {
                try
                {
                    Socket s = dict[i];
                    s.Send(arrMsgSend);
                }
                catch (SocketException se)
                {
                    ShowMsg("SocketException:" + se.Message);
                    break;
                }
                catch (Exception ex)
                {
                    ShowMsg("SocketException:" + ex.Message);
                    break;
                }
            }
        }
            /// Method to listen for client requests 监听客户端请求的方法  
            void WatchConnection()
            {
                //Continuous monitoring of the client's new connection request
                //持续不断的监听客户端的新的连接请求  
                while (true)
                {
                    Socket socketConnection = null;
                    try
                    {
                        //Start listening request, return a new socket responsible for the connection, is responsible for communicating with the client
                        //开始监听请求，返回一个新的负责连接的套接字，负责和该客户端通信  
                        //Note: Accept method will block the current thread!
                        //注意：Accept方法会阻断当前线程！  
                        socketConnection = socketWatch.Accept();
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

                    //When a new socket is connected to the server IP will be added to the online list, as the client's unique identifier
                    //当有新的socket连接到服务端时就将IP添加到在线列表中,作为客户端的唯一标识                  
                    listMember.Items.Add(socketConnection.RemoteEndPoint.ToString());
                    
                    //Add IP into the client list
                    //添加IP进入客户列表
                    clientlist.Add(socketConnection.RemoteEndPoint.ToString(),socketConnection.RemoteEndPoint.ToString());
                    //Save each newly generated socket to the key-value Dict collection with the client IP: port as the key
                    //将每个新产生的套接字存起来，装到键值对Dict集合中，以客户端IP:端口作为key  
                dict.Add(socketConnection.RemoteEndPoint.ToString(), socketConnection);

                    //Create a separate communication thread for each server-side communication socket, call the Receive method of the communication socket, and listen to the data sent by the client
                    //为每个服务端通信套接字创建一个单独的通信线程，负责调用通信套接字的Receive方法，监听客户端发来的数据  
                    //Create a communication thread
                    //创建通信线程  
                    Thread threadCommunicate = new Thread(ReceiveMsg);
                    threadCommunicate.IsBackground = true;
                    threadCommunicate.Start(socketConnection);//The thread with incoming parameters 有传入参数的线程  
                    
                    dictThread.Add(socketConnection.RemoteEndPoint.ToString(), threadCommunicate);

                    ShowMsg(string.Format("{0} is online. ", socketConnection.RemoteEndPoint.ToString()));

                sendNewClientList();


            }
            }

            /// Server-side monitoring client-side sends data 服务端监听客户端发来的数据  
            /// </summary>  
            void ReceiveMsg(object socketClientPara)//0 is normal message, 1 is the user's nickname 0普通消息，1是用户昵称
            {
                Socket socketClient = socketClientPara as Socket;
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
                        ShowMsg("Exception:" + ex.Message + "RemoteEndPoint: " + socketClient.RemoteEndPoint.ToString());
                        clientlist.Remove(socketClient.RemoteEndPoint.ToString());
                        sendNewClientList();
                        //Removes the communication socket that was disconnected from the communication socket combination
                        //从通信套接字结合中删除被中断连接的通信套接字  
                        dict.Remove(socketClient.RemoteEndPoint.ToString());
                        //Removes the disconnected communication thread object from the set of communication threads
                        //从通信线程集合中删除被中断连接的通信线程对象
                        dictThread.Remove(socketClient.RemoteEndPoint.ToString());
                        //Remove the disconnected IP: Port from the display list
                        //从显示列表中移除被中断连接的IP:Port
                        listMember.Items.Remove(socketClient.RemoteEndPoint.ToString());
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
                        byte[] strMsgRev= new byte[arrMsgRev.Length - 1];
                        Buffer.BlockCopy(arrMsgRev, 1, strMsgRev, 0, arrMsgRev.Length - 1);
                        StrMessage strMsgReceive = (StrMessage)SerObject.DeserializeObject(strMsgRev);
                        
                        if(strMsgReceive.ToName== "The server end")
                        {
                         ShowMsg(string.Format("#{0}# {1} speak to me :{2}", strMsgReceive.ToTopic, clientlist[socketClient.RemoteEndPoint.ToString()], strMsgReceive.ToMessage));
                        }
                    else
                    {
                        StrMessage strMsg = new StrMessage();
                        strMsg.ToMessage = strMsgReceive.ToMessage;
                        strMsg.ToTopic = strMsgReceive.ToTopic;
                        strMsg.ToName = clientlist[socketClient.RemoteEndPoint.ToString()];
                        //set the string which to be sent into UTF-8 corresponding byte array
                        //将要发送的字符串转成UTF-8对应的字节数组  
                        byte[] arrMsg = SerObject.SerializeObject(strMsg);
                        byte[] arrMsgSend = new byte[arrMsg.Length + 1];
                        arrMsgSend[0] = 0;//Set the flag, 0 on behalf of the text sent 设置标识位，0代表发送的是文字  
                        Buffer.BlockCopy(arrMsg, 0, arrMsgSend, 1, arrMsg.Length);
                        //Get the list of selected remote IP Key
                        //获得列表中选中的远程IP的Key  
                        String strCli = strMsgReceive.ToName;
                        string strClientKey = clientlist.toK(strCli);
                        try
                        {
                            //Through the key to find the dictionary set corresponding to a client communication socket Send method to send data to each other
                            //通过key找到字典集合中对应的某个客户端通信的套接字，用Send方法发送数据给对方 
                            Socket s = dict[strClientKey];
                            s.Send(arrMsgSend);

                            ShowMsg(string.Format("#{0}# {1} speak to {2} :{3}", strMsgReceive.ToTopic, strMsgReceive.ToName, clientlist[socketClient.RemoteEndPoint.ToString()],  strMsgReceive.ToMessage));
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
                    else if (arrMsgRev[0] == 1)//1 for the nickname 1代表昵称
                    {
                        String nickname= Encoding.UTF8.GetString(arrMsgRev, 1, length - 1);
                        clientlist[socketClient.RemoteEndPoint.ToString()] = nickname;
                        listMember.Items.Clear(); //Empty the text 清空文本
                       for (int i = 0; i < clientlist.Size(); i++)
                            listMember.Items.Add(clientlist[i]);

                    sendNewClientList();
                }
                }
            }

            void ShowMsg(string msg)
            {
                txtMsg.AppendText(msg + "\n");
            }


        
    }


static class Program
{
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new MainForm());
    }
}
}
