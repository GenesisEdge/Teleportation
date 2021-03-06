# Teleportation WANGQI DESIGN
Design and implementation of C/S mode remote chat software based on TCP (based on C#)
# 心灵传输 王琪设计与制作
基于TCP的C/S模式远程聊天软件设计与实现（基于C#实现）
# 最终版本：V10.0，Copyright © 王琪  2021
Final Version: V10.0
# 版本V10.0的Windows平台的客户端
/* 编写者：王琪，学号：U201713824，华中科技大学，光学与电子信息学院，电子1702班，QQ：1910652892
 * 本软件是王琪的毕业设计《基于TCP/IP的远程聊天软件设计与实现》的实现软件作品，作品名称：“心灵传输”
 * 2021年5月3日星期一：完成版本V10.0的Windows平台的客户端编写，全面升级心灵传输C/S端的可用性，是毕业设计的最终发布版本，为方便发布测试引入匿名通用密码，与注册用户安全区别隔离，安全可靠
 * Copyright © 王琪  2021
 */
 # 版本V10.0的Android平台的客户端
 /* 编写者：王琪，学号：U201713824，华中科技大学，光学与电子信息学院，电子1702班，QQ：1910652892
 * 本软件是王琪的毕业设计《基于TCP/IP的远程聊天软件设计与实现》的实现软件作品，作品名称：“心灵传输”
 * 2021年5月3日星期一：完成版本V10.0的Android平台的客户端编写，全面升级心灵传输C/S端的可用性，是毕业设计的最终发布版本，为方便发布测试引入匿名通用密码，与注册用户安全区别隔离，安全可靠
 * Copyright © 王琪  2021
 */
 # 版本V10.0的服务器端
 /* 编写者：王琪，学号：U201713824，华中科技大学，光学与电子信息学院，电子1702班，QQ：1910652892
 * 本软件是王琪的毕业设计《基于TCP/IP的远程聊天软件设计与实现》的实现软件作品，作品名称：“心灵传输”
 * 2021年5月3日星期一：完成版本V10.0的服务器端编写，全面升级心灵传输C/S端的可用性，是毕业设计的最终发布版本，为方便发布测试引入匿名通用密码，与注册用户安全区别隔离，安全可靠
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
 * //王琪：11【已废弃】读取客户端登录账户数据从纯文本格式的自定义后缀名WANGQI的文件，利用string.Split()分割字符串，缺点是不支持中文且特殊字符支持不完全
 * //王琪：12【已升级】采用了特殊自定义字符串作为分割字符串的标志字符串，引入using System.Text.RegularExpressions;极大的提升了程序稳定性
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
 * //王琪：41删除用户具体操作：1先删除userList中的user项userList.Remove(user);
 * //王琪：42删除用户具体操作：2再关闭class user，调用close()函数：user.Close();
 * //王琪：43调用scrolltoend函数确保每次更新textBlock时都能使得自动显示最后一行：this.scrollViewer.ScrollToEnd();
 * //王琪：44尝试绑定newClient与myListener.AcceptTcpClient()，如果失败则置为null
 * //王琪：45尝试初始化user，使用(User)userState方法
 * //王琪：46若isExit为false，始终执行该循环，若放在主线程会导致卡死，所以启动新线程
 * //王琪：47执行异步操作IAsyncResult类型的result，用BeginInvoke方法
 * //王琪：48启动Thread方法中的Sleep函数，单位为毫秒，会挂起阻塞该线程
 * //王琪：49若receiveString == null，说明用户已经失联，执行RemoveUser(user);操作
 * //王琪：50调用switch方法判断splitString[0]的内容匹配，得益于C#的字符串比较可以直接用==判断
 * //王琪：51调用binaryreader方法，读string，自动封装与判断长度
 * //王琪：52调用binarywriter方法，刷新缓冲区写入数据，自动封装与判断长度
 * //王琪：53通过for循环判断发送信息与接收信息不为同一个客户端，防止“信息共振”
 * //王琪：54调用List类型的Add方法，自动添加同类型子变量进入List类型的末端ServerLoginDataContent.Add(LoginDataContentTemp);
 * //王琪：55调用process的StartInfo.UseShellExecute置为false，使得重新启动服务器端的命令行窗口不可见process_Restart.StartInfo.UseShellExecute = false;
 * //王琪：56调用系统环境退出指令，强制结束所有与本程序相关的程序和子进程System.Environment.Exit(0);
 * //王琪：57调用异步发送信息给client函数，发送给指定客户端AsyncSendToClient();
 * //王琪：58调用异步发送信息给所有client函数（不包括自己）AsyncSendToAllClient();
 * //王琪：59AsyncSendToAllClient实际上是AsyncSendToClient的封装，封装形式为for循环遍历加判断if (user.userName != userList[i].userName)
 * //王琪：60【新升级】读取客户端登录账户数据从纯文本格式的自定义后缀名WANGQI的文件，利用Regex.Split代替string.Split()分割字符串，有效的解决了支持中文且特殊字符支持的问题
 * //王琪：61【新升级】更改process的StartInfo为启动cmd，同时引用参数/c，即执行完后立即关闭，预生成一个BAT文件，有效的解决了服务器的重启问题
 * //王琪：62【新升级】为方便发布测试引入匿名通用密码，与注册用户安全区别隔离，安全可靠
 * Copyright © 王琪  2021
 */
