/* 编写者：王琪，学号：U201713824，华中科技大学，光学与电子信息学院，电子1702班，QQ：1910652892
 * 本软件是王琪的毕业设计《基于TCP/IP的远程聊天软件设计与实现》的实现软件作品，作品名称：“心灵传输”
 * 2021年3月12日星期五：完成版本V2.0的服务器端编写，增加了TCP监听初始化进程的功能，并对服务器端软件进行BUG修复
 * //王琪：1定义了一个结构体LoginDataContent
 * //王琪：2采用List型变长数组定义了一个List<LoginDataContent>型的变量ServerLoginDataContent
 * //王琪：3导入之前保存的用户登录数据
 * //王琪：4启动TCP监听初始化进程（已完成）
 * //王琪：5使得鼠标左键按下时能拖动程序窗口
 * //王琪：6使得点击[安全退出服务器端]按钮时先保存聊天数据再完全退出
 * //王琪：7保存聊天记录至纯文本格式的自定义后缀名WANGQI的文件，利用File.AppendAllText函数
 * //王琪：8修改客户端登录账户数据从纯文本格式的自定义后缀名WANGQI的文件，利用Process.Start()传参启动notepad.exe系统自带记事本程序
 * //王琪：9打开聊天记录从纯文本格式的自定义后缀名WANGQI的文件，利用Process.Start()传参启动notepad.exe系统自带记事本程序
 * //王琪：10重新启动服务器端，利用Process.Start()传参启动自身程序
 * //王琪：11读取客户端登录账户数据从纯文本格式的自定义后缀名WANGQI的文件，利用string.Split()分割字符串，缺点是不支持中文且特殊字符支持不完全
 * //王琪：12采用了特殊自定义字符串作为分割字符串的标志字符串，引入using System.Text.RegularExpressions;极大的提升了程序稳定性
 * //王琪：13引入TCP相关变量，定义了一个类，名称为：User
 * //王琪：14一个类，名称为：User，其中定义了一个public的成员TcpClient变量：client
 * //王琪：15一个类，名称为：User，其中定义了一个public的成员BinaryReader变量：br
 * //王琪：16一个类，名称为：User，其中定义了一个public的成员BinaryWriter变量：bw
 * //王琪：17一个类，名称为：User，其中定义了一个public的成员string变量：userName
 * //王琪：18一个类，名称为：User，其中定义了一个public的成员User函数：public User(TcpClient client)
 * //王琪：19一个类，名称为：User，其中定义了一个public的成员Close函数：public void Close()
 * //王琪：20采用List型变长数组定义了一个List<User>型的变量userList，用于储存连接的客户端用户相关数据传输的标识信息
 * //王琪：21本服务器所在计算机的IP地址，用IPAddress类定义变量localAddress
 * //王琪：22本服务器所在计算机的IP地址对应的端口号，用const int类定义变量port = 52557，其中52557指我爱王王其（谐音，即：我爱自己）
 * //王琪：23定义了一个private的成员TcpListener变量：myListener
 * //王琪：24定义了一个private的成员bool变量：isExit，初始值为false来指示程序是否退出
 * //王琪：25启动线程myThread，用于监听客户端连接请求
 * //王琪：26函数private void ListenClientConnect()，用于监听客户端连接请求
 * //王琪：27使用轮询方式来判断异步操作是否完成
 * //王琪：28获取Begin 方法的返回值和所有输入/输出参数
 * //王琪：29每接受一个客户端连接，就创建一个对应的线程循环接收该客户端发来的信息
 * //王琪：30在自定义函数中如果要修改WPF界面程序，必须调用this.Dispatcher.Invoke(() =>{});方法
 * //王琪：31接受挂起的客户端连接请求
 * //王琪：32在用户失联后调用RemoveUser(user);删除指定已离线用户
 * //王琪：33利用splitSignalString分割字符串，获得指令参数信息
 * //王琪：34无论指令参数信息如何，必须验证发送此消息的客户端的特征用户名与密码是否正确，防止别有用心的人恶意攻击致使服务器信息被破解
 * //王琪：35接收客户端发来的信息
 * //王琪：36移除用户
 * //王琪：37异步发送message给user
 * //王琪：38发送message给user
 * //王琪：39将字符串写入网络流，此方法会自动附加字符串长度前缀
 * //王琪：40异步发送信息给所有客户
 * Copyright © 王琪  2021
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace Teleportation_Server
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //王琪：12采用了特殊自定义字符串作为分割字符串的标志字符串，引入using System.Text.RegularExpressions;极大的提升了程序稳定性
        public string splitSignalString = "WWWWW！@#￥%QQQQQQQ";
        //王琪：1定义了一个结构体LoginDataContent
        public struct LoginDataContent
        {
            public string Username;
            public string Password;
        };
        public LoginDataContent LoginDataContentTemp;
        //王琪：2采用List型变长数组定义了一个List<LoginDataContent>型的变量ServerLoginDataContent
        public List<LoginDataContent> ServerLoginDataContent = new List<LoginDataContent>();
        public int ServerLoginDataContentCount = 0;
        //王琪：13引入TCP相关变量，定义了一个类，名称为：User
        private class User
        {
            //王琪：14一个类，名称为：User，其中定义了一个public的成员TcpClient变量：client
            public TcpClient client { get; private set; }
            //王琪：15一个类，名称为：User，其中定义了一个public的成员BinaryReader变量：br
            public BinaryReader br { get; private set; }
            //王琪：16一个类，名称为：User，其中定义了一个public的成员BinaryWriter变量：bw
            public BinaryWriter bw { get; private set; }
            //王琪：17一个类，名称为：User，其中定义了一个public的成员string变量：userName
            public string userName { get; set; }
            //王琪：18一个类，名称为：User，其中定义了一个public的成员User函数：public User(TcpClient client)
            public User(TcpClient client)
            {
                this.client = client;
                NetworkStream networkStream = client.GetStream();
                br = new BinaryReader(networkStream);
                bw = new BinaryWriter(networkStream);
            }
            //王琪：19一个类，名称为：User，其中定义了一个public的成员Close函数：public void Close()
            public void Close()
            {
                br.Close();
                bw.Close();
                client.Close();
            }
        }
        //王琪：20采用List型变长数组定义了一个List<User>型的变量userList，用于储存连接的客户端用户相关数据传输的标识信息
        private List<User> userList = new List<User>();
        //王琪：21本服务器所在计算机的IP地址，用IPAddress类定义变量localAddress
        IPAddress localAddress = IPAddress.Parse("127.0.0.1");
        //王琪：22本服务器所在计算机的IP地址对应的端口号，用const int类定义变量port = 52557，其中52557指我爱王王其（谐音，即：我爱自己）
        private const int port = 52557;//王琪：我爱王王其（谐音，即：我爱自己）
        //王琪：23定义了一个private的成员TcpListener变量：myListener
        private TcpListener myListener;
        //王琪：24定义了一个private的成员bool变量：isExit，初始值为false来指示程序是否退出
        bool isExit = false;
        public MainWindow()
        {
            InitializeComponent();
            //王琪：服务器端初始化代码
            //王琪：3导入之前保存的用户登录数据
            string[] UserLoginData = File.ReadAllLines(System.AppDomain.CurrentDomain.BaseDirectory + @"\Server_Data\Login_Data.WANGQI");
            if (UserLoginData.Length != 0)
            {
                int LoadPos_ServerLoginData = 0;
                for (LoadPos_ServerLoginData = 0; LoadPos_ServerLoginData < UserLoginData.Length; LoadPos_ServerLoginData++)
                {
                    LoginDataContentTemp.Username = UserLoginData[LoadPos_ServerLoginData].Split('@')[0].Split('#')[1];
                    LoginDataContentTemp.Password = UserLoginData[LoadPos_ServerLoginData].Split('@')[1].Split('#')[1];
                    ServerLoginDataContent.Add(LoginDataContentTemp);
                    ServerLoginDataContentCount++;
                }
            }
            //王琪：4启动TCP监听初始化进程（已完成）
            myListener = new TcpListener(localAddress, port);
            myListener.Start();
            this.textBlock_ServerInfo.Text = DateTime.Now.ToString("[yyyy年MM月dd日HH时mm分ss秒]") + "\n开始在" + localAddress + ":" + port + "监听客户端……\n";
            this.scrollViewer.ScrollToEnd();
            //王琪：25启动线程myThread，用于监听客户端连接请求
            Thread myThread = new Thread(ListenClientConnect);
            myThread.Start();
            this.textBlock_ServerInfo.Text += "\n" + DateTime.Now.ToString("[yyyy年MM月dd日HH时mm分ss秒]") + "\n本服务器端初始化完成！\n";
            this.scrollViewer.ScrollToEnd();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //王琪：5使得鼠标左键按下时能拖动程序窗口
            this.DragMove();
        }
        //王琪：6使得点击[安全退出服务器端]按钮时先保存聊天数据再完全退出
        private void button_ExitServer_Click(object sender, RoutedEventArgs e)
        {
            SaveServerChatData();
            System.Environment.Exit(0);
        }
        //王琪：7保存聊天记录至纯文本格式的自定义后缀名WANGQI的文件，利用File.AppendAllText函数
        private void SaveServerChatData()
        {
            File.AppendAllText(System.AppDomain.CurrentDomain.BaseDirectory + @"\Server_Data\Chat_Record.WANGQI", this.textBlock_ServerInfo.Text);
        }
        //王琪：8修改客户端登录账户数据从纯文本格式的自定义后缀名WANGQI的文件，利用Process.Start()传参启动notepad.exe系统自带记事本程序
        private void button_EditUserLoginData_Click(object sender, RoutedEventArgs e)
        {
            Process process_EditUserLoginData = new Process();
            process_EditUserLoginData.StartInfo.FileName = "notepad.exe";
            process_EditUserLoginData.StartInfo.Arguments = System.AppDomain.CurrentDomain.BaseDirectory + @"\Server_Data\Login_Data.WANGQI";
            process_EditUserLoginData.Start();
        }
        //王琪：9打开聊天记录从纯文本格式的自定义后缀名WANGQI的文件，利用Process.Start()传参启动notepad.exe系统自带记事本程序
        private void button_OpenUserChatRecord_Click(object sender, RoutedEventArgs e)
        {
            Process process_OpenUserChatData = new Process();
            process_OpenUserChatData.StartInfo.FileName = "notepad.exe";
            process_OpenUserChatData.StartInfo.Arguments = System.AppDomain.CurrentDomain.BaseDirectory + @"\Server_Data\Chat_Record.WANGQI";
            process_OpenUserChatData.Start();
        }
        //王琪：10重新启动服务器端，利用Process.Start()传参启动自身程序
        private void button_RestartServer_Click(object sender, RoutedEventArgs e)
        {
            SaveServerChatData();
            Process process_Restart = new Process();
            process_Restart.StartInfo.FileName = System.AppDomain.CurrentDomain.BaseDirectory + "心灵传输：服务器端.exe";
            process_Restart.StartInfo.UseShellExecute = false;
            process_Restart.Start();
            Application.Current.Shutdown();
        }
        //王琪：26函数private void ListenClientConnect()，用于监听客户端连接请求
        private void ListenClientConnect()
        {
            TcpClient newClient = null;
            while (true)
            {
                ListenClientDelegate d = new ListenClientDelegate(ListenClient);
                IAsyncResult result = d.BeginInvoke(out newClient, null, null);
                //王琪：27使用轮询方式来判断异步操作是否完成
                while (result.IsCompleted == false)
                {
                    if (isExit)
                        break;
                    Thread.Sleep(250);
                }
                //王琪：28获取Begin 方法的返回值和所有输入/输出参数
                d.EndInvoke(out newClient, result);
                if (newClient != null)
                {
                    //王琪：29每接受一个客户端连接，就创建一个对应的线程循环接收该客户端发来的信息
                    User user = new User(newClient);
                    Thread threadReceive = new Thread(ReceiveData);
                    threadReceive.Start(user);
                    userList.Add(user);
                    //王琪：30在自定义函数中如果要修改WPF界面程序，必须调用this.Dispatcher.Invoke(() =>{});方法
                    this.Dispatcher.Invoke(() =>
                    {
                        this.textBlock_ServerInfo.Text += "\n" + DateTime.Now.ToString("[yyyy年MM月dd日HH时mm分ss秒]") + "\n[" + newClient.Client.RemoteEndPoint.ToString() + "]进入。\n";
                        this.textBlock_ServerInfo.Text += "\n" + DateTime.Now.ToString("[yyyy年MM月dd日HH时mm分ss秒]") + "\n当前连接用户数：" + userList.Count.ToString() + "。\n";
                        this.scrollViewer.ScrollToEnd();
                    });
                }
                else
                {
                    break;
                }
            }
        }
        private delegate void ListenClientDelegate(out TcpClient client);
        /// <summary>
        /// //王琪：31接受挂起的客户端连接请求
        /// </summary>
        /// <param name="newClient"></param>
        private void ListenClient(out TcpClient newClient)
        {
            try
            {
                newClient = myListener.AcceptTcpClient();
            }
            catch
            {
                newClient = null;
            }
        }
        private void ReceiveData(object userState)
        {
            User user = (User)userState;
            TcpClient client = user.client;
            while (!isExit)
            {
                string receiveString = null;
                ReceiveMessageDelegate d = new ReceiveMessageDelegate(ReceiveMessage);
                IAsyncResult result = d.BeginInvoke(user, out receiveString, null, null);
                //王琪：27使用轮询方式来判断异步操作是否完成
                while (!result.IsCompleted)
                {
                    if (isExit)
                        break;
                    Thread.Sleep(250);
                }
                //王琪：28获取Begin 方法的返回值和所有输入/输出参数
                d.EndInvoke(out receiveString, result);
                if (receiveString == null)
                {
                    if (!isExit)
                    {
                        //王琪：30在自定义函数中如果要修改WPF界面程序，必须调用this.Dispatcher.Invoke(() =>{});方法
                        this.Dispatcher.Invoke(() =>
                        {
                            this.textBlock_ServerInfo.Text += "\n" + DateTime.Now.ToString("[yyyy年MM月dd日HH时mm分ss秒]") + "\n[用户失联]与" + client.Client.RemoteEndPoint.ToString() + "失去联系，已终止接收该用户信息。\n";
                            this.scrollViewer.ScrollToEnd();
                        });
                        //王琪：32在用户失联后调用RemoveUser(user);删除指定已离线用户
                        RemoveUser(user);
                    }
                    break;
                }
                //王琪：33利用splitSignalString分割字符串，获得指令参数信息
                string[] splitString = Regex.Split(receiveString, splitSignalString, RegexOptions.None);
                switch (splitString[0])
                {
                    case "身份核验":
                        LoginDataContent unknownUserLogin;
                        unknownUserLogin.Username = splitString[1];
                        unknownUserLogin.Password = splitString[2];
                        //王琪：34无论指令参数信息如何，必须验证发送此消息的客户端的特征用户名与密码是否正确，防止别有用心的人恶意攻击致使服务器信息被破解
                        if (ServerLoginDataContent.Contains(unknownUserLogin) == true)
                        {
                            user.userName = splitString[1];
                            AsyncSendToAllClient(user, "[该用户已上线]");
                            this.Dispatcher.Invoke(() =>
                            {
                                this.textBlock_ServerInfo.Text += "\n" + DateTime.Now.ToString("[yyyy年MM月dd日HH时mm分ss秒]") + "\n[用户上线]用户名为：" + user.userName.ToString() + "已经上线。\n";
                                this.scrollViewer.ScrollToEnd();
                            });
                        }                      
                        break;
                    case "退出聊天":
                        LoginDataContent unknownUserExit;
                        unknownUserExit.Username = splitString[1];
                        unknownUserExit.Password = splitString[2];
                        //王琪：34无论指令参数信息如何，必须验证发送此消息的客户端的特征用户名与密码是否正确，防止别有用心的人恶意攻击致使服务器信息被破解
                        if (ServerLoginDataContent.Contains(unknownUserExit) == true)
                        {
                            user.userName = splitString[1];
                            AsyncSendToAllClient(user, "[该用户已下线]");
                            this.Dispatcher.Invoke(() =>
                            {
                                this.textBlock_ServerInfo.Text += "\n" + DateTime.Now.ToString("[yyyy年MM月dd日HH时mm分ss秒]") + "\n[用户下线]用户名为：" + user.userName.ToString() + "已经下线。\n";
                                this.scrollViewer.ScrollToEnd();
                            });
                            //王琪：32在用户失联后调用RemoveUser(user);删除指定已离线用户
                            RemoveUser(user);
                        }
                        return;
                    case "聊天信息":
                        LoginDataContent unknownUserChat;
                        unknownUserChat.Username = splitString[1];
                        unknownUserChat.Password = splitString[2];
                        //王琪：34无论指令参数信息如何，必须验证发送此消息的客户端的特征用户名与密码是否正确，防止别有用心的人恶意攻击致使服务器信息被破解
                        if (ServerLoginDataContent.Contains(unknownUserChat) == true)
                        {

                            user.userName = splitString[1];
                            AsyncSendToAllClient(user, splitString[3]);
                            this.Dispatcher.Invoke(() =>
                            {
                                this.textBlock_ServerInfo.Text += "\n" + DateTime.Now.ToString("[yyyy年MM月dd日HH时mm分ss秒]") + "\n[聊天信息]用户名为：" + user.userName.ToString() + "发送信息如下：" + splitString[3] + "\n";
                                this.scrollViewer.ScrollToEnd();
                            });
                        }                       
                        break;
                    default:
                        break;
                }
            }
        }
        delegate void ReceiveMessageDelegate(User user, out string receiveMessage);
        /// <summary>
        /// //王琪：35接收客户端发来的信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="receiveMessage"></param>
        private void ReceiveMessage(User user, out string receiveMessage)
        {
            try
            {
                receiveMessage = user.br.ReadString();
            }
            catch
            {
                receiveMessage = null;
            }
        }
        /// <summary>
        /// //王琪：36移除用户
        /// </summary>
        /// <param name="user"></param>
        private void RemoveUser(User user)
        {
            userList.Remove(user);
            user.Close();
            this.Dispatcher.Invoke(() =>
            {
                this.textBlock_ServerInfo.Text += "\n" + DateTime.Now.ToString("[yyyy年MM月dd日HH时mm分ss秒]") + "\n当前连接用户数：" + userList.Count.ToString() + "。\n";
                this.scrollViewer.ScrollToEnd();
            });
            //AddItemToListBox(string.Format("当前连接用户数：{0}", userList.Count));
        }
        /// <summary>
        /// //王琪：37异步发送message给user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        private void AsyncSendToClient(User user, string message)
        {
            SendToClientDelegate d = new SendToClientDelegate(SendToClient);
            IAsyncResult result = d.BeginInvoke(user, message, null, null);
            while (result.IsCompleted == false)
            {
                if (isExit)
                    break;
                Thread.Sleep(250);
            }
            d.EndInvoke(result);
        }

        private delegate void SendToClientDelegate(User user, string message);
        /// <summary>
        /// //王琪：38发送message给user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        private void SendToClient(User user, string message)
        {
            try
            {
                //王琪：39将字符串写入网络流，此方法会自动附加字符串长度前缀
                user.bw.Write(message);
                user.bw.Flush();
            }
            catch
            {
                ;
            }
        }
        /// <summary>
        /// //王琪：40异步发送信息给所有客户
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        private void AsyncSendToAllClient(User user, string message)
        {
            for (int i = 0; i < userList.Count; i++)
            {
                    AsyncSendToClient(userList[i], "服务器信息" + splitSignalString + user.userName + splitSignalString + "CorrectPassword" + splitSignalString + message);
            }
        }
    }
}
