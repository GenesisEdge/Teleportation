using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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

namespace Teleportation_Server
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public struct LoginDataContent
        {
            public string Username;
            public string Password;
        };
        public LoginDataContent LoginDataContentTemp;
        public List<LoginDataContent> ServerLoginDataContent = new List<LoginDataContent>();
        public int ServerLoginDataContentCount = 0;
        public MainWindow()
        {
            InitializeComponent();
            //王琪：服务器端初始化代码
            //王琪：导入之前保存的用户登录数据
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
            //启动TCP监听初始化进程
            this.textBlock_ServerInfo.Text = "\n" + DateTime.Now.ToString("[yyyy年MM月dd日HH时mm分ss秒]") + "\n本服务器端初始化完成！\n";
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //王琪：使得鼠标左键按下时能拖动程序窗口
            this.DragMove();
        }

        private void button_ExitServer_Click(object sender, RoutedEventArgs e)
        {
            SaveServerChatData();
            System.Environment.Exit(0);
        }
        private void SaveServerChatData()
        {
            File.AppendAllText(System.AppDomain.CurrentDomain.BaseDirectory + @"\Server_Data\Chat_Record.WANGQI", this.textBlock_ServerInfo.Text);
        }

        private void button_EditUserLoginData_Click(object sender, RoutedEventArgs e)
        {
            Process process_EditUserLoginData = new Process();
            process_EditUserLoginData.StartInfo.FileName = "notepad.exe";
            process_EditUserLoginData.StartInfo.Arguments = System.AppDomain.CurrentDomain.BaseDirectory + @"\Server_Data\Login_Data.WANGQI";
            process_EditUserLoginData.Start();
        }

        private void button_OpenUserChatRecord_Click(object sender, RoutedEventArgs e)
        {
            Process process_OpenUserChatData = new Process();
            process_OpenUserChatData.StartInfo.FileName = "notepad.exe";
            process_OpenUserChatData.StartInfo.Arguments = System.AppDomain.CurrentDomain.BaseDirectory + @"\Server_Data\Chat_Record.WANGQI";
            process_OpenUserChatData.Start();
        }

        private void button_RestartServer_Click(object sender, RoutedEventArgs e)
        {
            SaveServerChatData();
            Process process_Restart = new Process();
            process_Restart.StartInfo.FileName = System.AppDomain.CurrentDomain.BaseDirectory + "心灵传输：服务器端.exe";
            process_Restart.StartInfo.UseShellExecute = false;
            process_Restart.Start();
            Application.Current.Shutdown();
        }
    }
}
