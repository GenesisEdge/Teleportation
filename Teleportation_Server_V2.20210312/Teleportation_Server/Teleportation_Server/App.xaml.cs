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
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Teleportation_Server
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
    }
}
