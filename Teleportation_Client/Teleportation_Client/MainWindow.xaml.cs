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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Teleportation_Client
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public string splitSignalString = "WWWWW！@#￥%QQQQQQQ";
        public string Username;
        public string Password;
        public string RememberPassword;
        public string AutoLogin;
        private string Server_IP = "127.0.0.1";
        private int Server_Port = 52557;
        private BinaryReader br;
        private BinaryWriter bw;
        public TcpClient client;
        public MainWindow()
        {
            InitializeComponent();
            string[] LoginData = File.ReadAllLines(System.AppDomain.CurrentDomain.BaseDirectory + @"\Client_Data\Login_Data.WANGQI");
            if (LoginData.Length == 1)
            {
                string[] LoginDataContent = LoginData[0].Split('@');
                Username = LoginDataContent[0].Split('#')[1];
                Password = LoginDataContent[1].Split('#')[1];
                RememberPassword = LoginDataContent[2].Split('#')[1];
                AutoLogin = LoginDataContent[3].Split('#')[1];
                //王琪：开始载入之前的登录信息
                this.textBox_Username.Text = Username;
                this.textBlock_LoginInfo.Text = "初始化完成！";
                if (RememberPassword == "true")
                {
                    this.checkBox_RememberPassword.IsChecked = true;
                    this.textBox_Password.Text = Password;
                }
                if (AutoLogin == "true")
                {
                    this.checkBox_AutoLogin.IsChecked = true;
                    this.button_Login_Click(null, null);
                }
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void button_Exit_Click(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void button_Login_Click(object sender, RoutedEventArgs e)
        {
            this.textBlock_LoginInfo.Text = "登录中……";
            //王琪：点击登录按钮后禁用与登录有关的操作
            this.button_Login.IsEnabled = false;
            this.checkBox_AutoLogin.IsEnabled = false;
            this.checkBox_RememberPassword.IsEnabled = false;
            this.textBox_Username.IsEnabled = false;
            this.textBox_Password.IsEnabled = false;
            //王琪：保存登录信息至本地文件
            Username = this.textBox_Username.Text;
            Password = this.textBox_Password.Text;
            RememberPassword = "false";
            if (this.checkBox_RememberPassword.IsChecked == true)
            {
                RememberPassword = "true";
            }
            AutoLogin = "false";
            if (this.checkBox_AutoLogin.IsChecked == true)
            {
                AutoLogin = "true";
            }
            string LoginDataContent = "Username" + "#" + Username + "@" + "Password" + "#" + Password + "@"
                + "RememberPassword" + "#" + RememberPassword + "@" + "AutoLogin" + "#" + AutoLogin;
            string[] LoginData = { LoginDataContent };
            File.WriteAllLines(System.AppDomain.CurrentDomain.BaseDirectory + @"\Client_Data\Login_Data.WANGQI", LoginData);
            //王琪：开始登录
            client = new TcpClient();
            try
            {
                client.Connect(Server_IP, Server_Port);
                this.textBlock_LoginInfo.Text = "成功与服务器取得联系！";
            }
            catch
            {
                this.textBlock_LoginInfo.Text = "无法与服务器取得联系，请重试！";
                this.button_Login.IsEnabled = true;
                this.checkBox_AutoLogin.IsEnabled = true;
                this.checkBox_RememberPassword.IsEnabled = true;
                this.textBox_Username.IsEnabled = true;
                this.textBox_Password.IsEnabled = true;
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

                SendMessage("身份核验" + splitSignalString + Username + splitSignalString + Password);
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
                    this.textBlock_LoginInfo.Text = "身份核验请求未能发送到服务器！";
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
                            if (splitString[1].Equals(Username) && splitString[2].Equals("CorrectPassword"))
                            {
                                this.Dispatcher.Invoke(() =>
                                {
                                    this.textBlock_LoginInfo.Text = "身份核验成功！正在登录……";
                                });
                                



                                this.Dispatcher.Invoke(() =>
                                {
                                    ChatWindow ChatWin = new ChatWindow();
                                    ChatWin.Show();
                                    this.Close();
                                });
                                
                            }
                            else if(splitString[1].Equals(Username) && splitString[2].Equals("WrongPassword"))
                            {
                                this.Dispatcher.Invoke(() =>
                                {
                                    this.textBlock_LoginInfo.Text = "用户名或密码错误，请重试！";
                                    this.button_Login.IsEnabled = true;
                                    this.checkBox_AutoLogin.IsEnabled = true;
                                    this.checkBox_RememberPassword.IsEnabled = true;
                                    this.textBox_Username.IsEnabled = true;
                                    this.textBox_Password.IsEnabled = true;
                                });

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
