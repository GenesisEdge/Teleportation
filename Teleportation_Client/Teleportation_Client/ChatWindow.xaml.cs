using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Teleportation_Client
{
    /// <summary>
    /// ChatWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ChatWindow : Window
    {
        public string splitSignalString = "WWWWW！@#￥%QQQQQQQ";
        public string splitSendString = "@@@";
        public string Username;
        public string Password;
        private string Server_IP = "127.0.0.1";
        private int Server_Port = 52557;
        private BinaryReader br;
        private BinaryWriter bw;
        public TcpClient client;
        public ChatWindow()
        {
            InitializeComponent();
            InitializeComponent();
            string[] LoginData = File.ReadAllLines(System.AppDomain.CurrentDomain.BaseDirectory + @"\Client_Data\Login_Data.WANGQI");
            if (LoginData.Length == 1)
            {
                string[] LoginDataContent = LoginData[0].Split('@');
                Username = LoginDataContent[0].Split('#')[1];
                Password = LoginDataContent[1].Split('#')[1];
            }
            client = new TcpClient();
            try
            {
                client.Connect(Server_IP, Server_Port);
                this.textBlock_ChatInfo.Text += "\n" + DateTime.Now.ToString("[yyyy年MM月dd日HH时mm分ss秒]") + "\n成功与服务器取得联系！\n";
                this.scrollViewer.ScrollToEnd();
            }
            catch
            {
                this.textBlock_ChatInfo.Text += "\n" + DateTime.Now.ToString("[yyyy年MM月dd日HH时mm分ss秒]") + "\n无法与服务器取得联系，请重试！\n";
                this.scrollViewer.ScrollToEnd();
            }
            if (client.Connected)
            {
                NetworkStream networkStream = client.GetStream();
                //将网络流作为二进制读写对象
                br = new BinaryReader(networkStream);
                bw = new BinaryWriter(networkStream);

                Thread threadReceive = new Thread(ReceiveData);
                threadReceive.SetApartmentState(ApartmentState.STA);
                threadReceive.IsBackground = true;
                threadReceive.Start();

            }
        }

        private void button_Exit_Click(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void button_Send_Click(object sender, RoutedEventArgs e)
        {
            string SendInfo = this.textBox_SendInfo.Text;
            string[] splitString = Regex.Split(SendInfo, splitSendString, RegexOptions.None);
            if(splitString.Length == 1)
            {
                SendMessage("公开聊天" + splitSignalString + Username + splitSignalString + Password + splitSignalString + "All" + splitSignalString + splitString[0]);
                this.textBlock_ChatInfo.Text += "\n" + DateTime.Now.ToString("[yyyy年MM月dd日HH时mm分ss秒]") + "\n[公开聊天]你[" + Username + "]说：\n" + splitString[0] + "\n";
                this.scrollViewer.ScrollToEnd();
            }
            else if(splitString.Length == 2)
            {
                SendMessage("私密聊天" + splitSignalString + Username + splitSignalString + Password + splitSignalString + splitString[1] + splitSignalString + splitString[0]);
                this.textBlock_ChatInfo.Text += "\n" + DateTime.Now.ToString("[yyyy年MM月dd日HH时mm分ss秒]") + "\n[私密聊天]你[" + Username + "]对用户[" + splitString[1] + "]说：\n" + splitString[0] + "\n";
                this.scrollViewer.ScrollToEnd();
            }
            else
            {
                this.textBlock_ChatInfo.Text += "\n" + DateTime.Now.ToString("[yyyy年MM月dd日HH时mm分ss秒]") + "\n[发送信息语法格式有误]必须以格式【聊天内容@@@私密聊天对象】或者【聊天内容】发送信息，请重试。\n";
                this.scrollViewer.ScrollToEnd();
            }
        }
        private void SendMessage(string message)
        {

            try
            {
                //将字符串写入网络流，此方法会自动附加字符串长度前缀
                bw.Write(message);
                bw.Flush();
            }
            catch
            {

                this.Dispatcher.Invoke(() =>
                {
                    this.textBlock_ChatInfo.Text += "\n" + DateTime.Now.ToString("[yyyy年MM月dd日HH时mm分ss秒]") + "\n聊天信息请求未能发送到服务器！\n";
                    this.scrollViewer.ScrollToEnd();
                });
            }
        }
        private void ReceiveData()
        {


            while (true)
            {
                string receiveString = null;
                try
                {
                    //从网络流中读出字符串
                    //此方法会自动判断字符串长度前缀，并根据长度前缀读出字符串
                    receiveString = br.ReadString();

                    string[] splitString = Regex.Split(receiveString, splitSignalString, RegexOptions.None);
                    string command = splitString[0];

                    switch (command)
                    {
                        case "服务器信息":
                            if (splitString[2].Equals("ChatPassword"))
                            {
                                if(splitString[3].Equals("All"))
                                {
                                    this.Dispatcher.Invoke(() =>
                                    {
                                        this.textBlock_ChatInfo.Text += "\n" + DateTime.Now.ToString("[yyyy年MM月dd日HH时mm分ss秒]") + "\n[公开聊天]用户：" + splitString[1] + "说：\n" + splitString[4] + "\n";
                                        this.scrollViewer.ScrollToEnd();
                                    });
                                }
                                else if(splitString[3].Equals("You"))
                                {
                                    this.Dispatcher.Invoke(() =>
                                    {
                                        this.textBlock_ChatInfo.Text += "\n" + DateTime.Now.ToString("[yyyy年MM月dd日HH时mm分ss秒]") + "\n[私密聊天]用户：" + splitString[1] + "[悄悄对你]说：\n" + splitString[4] + "\n";
                                        this.scrollViewer.ScrollToEnd();
                                    });
                                }

                            }
                            break;

                        default:
                            break;
                    }
                }
                catch
                {
                    ;
                }
            }
        }
    }
}
