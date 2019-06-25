using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HslCommunication.Core;

namespace HslCommunication
{
    /// <summary>
    /// 一个工业物联网的底层架构框架，专注于底层的技术通信及跨平台，跨语言通信功能，实现各种主流的PLC数据读写，实现modbus的各种协议读写等等，
    /// 支持快速搭建工业上位机软件，组态软件，SCADA软件，工厂MES系统，助力企业工业4.0腾飞，实现智能制造，智慧工厂的目标。
    /// <br /><br />
    /// 本组件免费开源，使用之前请认真的阅读本API文档，对于本文档中警告部分的内容务必理解，部署生产之前请详细测试，如果在测试的过程中，
    /// 发现了BUG，或是有问题的地方，欢迎联系作者进行修改，或是直接在github上进行提问。统一声明：对于操作设备造成的任何损失，作者概不负责。
    /// <br /><br />
    /// 官方网站：<a href="http://www.hslcommunication.cn/">http://www.hslcommunication.cn/</a>，包含组件的在线API地址以及一个MES DEMO的项目展示。
    /// <br /><br />
    /// <note type="important">
    /// 本组件的目标是集成一个框架，统一所有的设备读写方法，抽象成统一的接口<see cref="IReadWriteNet"/>，对于上层操作只需要关注地址，读取类型即可，另一个目标是使用本框架轻松实现C#后台+C#客户端+web浏览器+android手机的全方位功能实现。
    /// </note>
    /// <br /><br />
    /// 本库提供了C#版本和java版本和python版本，java，python版本的使用和C#几乎是一模一样的，都是可以相互通讯的。
    /// </summary>
    /// <remarks>
    /// 本软件著作权归Richard.Hu所有，开源项目地址：<a href="https://github.com/dathlin/HslCommunication">https://github.com/dathlin/HslCommunication</a>  开源协议：LGPL-3.0
    /// <br />
    /// 博客地址：<a href="https://www.cnblogs.com/dathlin/p/7703805.html">https://www.cnblogs.com/dathlin/p/7703805.html</a>
    /// <br />
    /// 打赏请扫码：<br />
    /// <img src="https://raw.githubusercontent.com/dathlin/HslCommunication/master/imgs/support.png" />
    /// </remarks>
    /// <revisionHistory>
    ///     <revision date="2017-10-21" version="3.7.10" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>正式发布库到互联网上去。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2017-10-21" version="3.7.11" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>添加xml文档</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2017-10-31" version="3.7.12" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>重新设计西门子的数据读取机制，提供一个更改类型的方法。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2017-11-06" version="3.7.13" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>提供一个ModBus的服务端引擎。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2017-11-07" version="3.7.14" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>紧急修复了西门子批量访问时出现的BUG。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2017-11-12" version="3.7.15" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>完善CRC16校验码功能，完善数据库辅助类方法。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2017-11-13" version="3.7.16" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>西门子访问类，提供一个批量bool数据写入，但该写入存在安全隐患，具体见博客。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2017-11-21" version="4.0.0" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>与3.X版本不兼容，谨慎升级。如果要升级，主要涉及的代码包含PLC的数据访问和同步数据通信。</item>
    ///             <item>删除了2个类，OperateResultBytes和OperateResultString类，提供了更加强大方便的泛型继承类，多达10个泛型参数。地址见http://www.cnblogs.com/dathlin/p/7865682.html</item>
    ///             <item>将部分类从HslCommunication命名空间下移动到HslCommunication.Core下面。</item>
    ///             <item>提供了一个通用的ModBus TCP的客户端类，方便和服务器交互。</item>
    ///             <item>完善了HslCommunication.BasicFramework.SoftBaisc下面的辅助用的静态方法，提供了一些方便的数据转化，在上面进行公开。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2017-11-24" version="4.0.1" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>更新了三菱的读取接口，提供了一个额外的字符串表示的方式，OperateResult&lt;byte[]&gt; read =  melsecNet.ReadFromPLC("M100", 5);</item>
    ///             <item>更新了西门子的数据访问类和modbus tcp类提供双模式运行，按照之前版本的写法是默认模式，每次请求重新创建网络连接，新增模式二，在代码里先进行连接服务器方法，自动切换到模式二，每次请求都共用一个网络连接，内部已经同步处理，加速数据访问，如果访问失败，自动在下次请求是重新连接，如果调用关闭连接服务器，自动切换到模式一。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2017-11-25" version="4.0.2" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>修复Modbus tcp批量写入寄存器时，数据解析异常的BUG。</item>
    ///             <item>三菱访问器新增长连接模式。</item>
    ///             <item>三菱访问器支持单个M写入，在数组中指定一个就行。</item>
    ///             <item>三菱访问器提供了float[]数组写入的API。</item>
    ///             <item>三菱访问器支持F报警器，B链接继电器，S步进继电器，V边沿继电器，R文件寄存器读写，不过还需要大面积测试。</item>
    ///             <item>三菱访问器的读写地址支持字符串形式传入。</item>
    ///             <item>其他的细节优化。</item>
    ///             <item>感谢 hwdq0012 网友的测试和建议。</item>
    ///             <item>感谢 吃饱睡好 好朋友的测试</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2017-11-27" version="4.0.3" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>三菱，西门子，Modbus tcp客户端内核优化重构。</item>
    ///             <item>三菱，西门子，Modbus tcp客户端提供统一的报文测试方法，该方法也是通信核心，所有API都是基于此扩展起来的。</item>
    ///             <item>三菱，西门子，Modbus tcp客户端提供了一些便捷的读写API，详细参见对应博客。</item>
    ///             <item>三菱的地址区分十进制和十六进制。</item>
    ///             <item>优化三菱的位读写操作。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2017-11-28" version="4.1.0" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>修复西门子读取的地址偏大会出现异常的BUG。</item>
    ///             <item>完善统一了所有三菱，西门子，modbus客户端类的读写方法，已经更新到博客。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2017-12-02" version="4.1.1" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>完善日志记录，提供关键字记录操作。</item>
    ///             <item>三菱，西门子，modbus tcp客户端提供自定义数据读写。</item>
    ///             <item>modbus tcp服务端提供数据池功能，并支持数据订阅操作。</item>
    ///             <item>提供一个纵向的进度控件。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2017-12-04" version="4.1.2" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>完善Modbus tcp服务器端的数据订阅功能。</item>
    ///             <item>进度条控件支持水平方向和垂直方向两个模式。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2017-12-05" version="4.1.3" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>进度条控件修复初始颜色为空的BUG。</item>
    ///             <item>进度条控件文本锯齿修复。</item>
    ///             <item>按钮控件无法使用灰色按钮精灵破解。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2017-12-13" version="4.1.4" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>modbus tcp提供读取short数组的和ushort数组方法。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2017-12-13" version="4.1.5" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>修复流水号生成器无法生成不带日期格式的流水号BUG。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2017-12-18" version="4.1.6" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>OperateResult成功时，消息为成功。</item>
    ///             <item>数据库辅助类API添加，方便的读取聚合函数。</item>
    ///             <item>日志类分析工具界面，显示文本微调。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2017-12-25" version="4.1.7" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>进度条控件新增一个新的属性对象，是否使用动画。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2017-12-27" version="4.1.8" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>新增一个饼图控件。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2017-12-28" version="4.1.9" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>饼图显示优化，新增是否显示百分比的选择。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2017-12-31" version="4.2.0" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>新增一个仪表盘控件。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-01-03" version="4.2.1" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>饼图控件新增一个是否显示占比很小的信息文本。</item>
    ///             <item>新增一个旋转开关控件。</item>
    ///             <item>新增一个信号灯控件。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-01-05" version="4.2.2" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>修复modbus tcp客户端读取 float, int, long,的BUG。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-01-08" version="4.2.3" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>修复modbus tcp客户端读取某些特殊设备会读取不到数据的BUG。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-01-15" version="4.2.4" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>双模式的网络基类中新增一个读取超时的时间设置，如果为负数，那么就不验证返回。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-01-24" version="4.3.0" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>信号灯控件显示优化。</item>
    ///             <item>Modbus Tcp服务端类修复内存暴涨问题。</item>
    ///             <item>winfrom客户端提供一个曲线控件，方便显示实时数据，多曲线数据。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-02-05" version="4.3.1" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>优化modbus tcp客户端的访问类，支持服务器返回错误信息。</item>
    ///             <item>优化曲线控件，支持横轴文本显示，支持辅助线标记，详细见对应博客。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-02-22" version="4.3.2" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>曲线控件最新时间显示BUG修复。</item>
    ///             <item>Modbus tcp错误码BUG修复。</item>
    ///             <item>三菱访问类完善long类型读写。</item>
    ///             <item>西门子访问类支持1500系列，支持读取订货号。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-03-05" version="4.3.3" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>曲线控件增加一个新的属性，图标标题。</item>
    ///             <item>Modbus tcp服务器端的读写BUG修复。</item>
    ///             <item>西门子访问类重新支持200smart。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-03-07" version="4.3.4" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>Json组件更新至11.0.1版本。</item>
    ///             <item>紧急修复日志类的BeforeSaveToFile事件在特殊情况的触发BUG。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-03-19" version="4.3.5" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>修复Modbus-tcp服务器接收异常的BUG。</item>
    ///             <item>修复SoftBasic.ByteTo[U]ShortArray两个方法异常。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-04-05" version="5.0.0" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>网络核心层重新开发，完全的基于异步IO实现。</item>
    ///             <item>所有双模式客户端类进行代码重构，接口统一。</item>
    ///             <item>完善并扩充OperateResult对象的类型支持。</item>
    ///             <item>提炼一些基础的更加通用的接口方法，在SoftBasic里面。</item>
    ///             <item>支持欧姆龙PLC的数据交互。</item>
    ///             <item>支持三菱的1E帧数据格式。</item>
    ///             <item>不兼容升级，谨慎操作。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-04-10" version="5.0.1" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>OperateResult静态方法扩充。</item>
    ///             <item>文件引擎提升缓存空间到100K，加速文件传输。</item>
    ///             <item>三菱添加读取单个bool数据。</item>
    ///             <item>Modbus-tcp客户端支持配置起始地址不是0的服务器。</item>
    ///             <item>其他代码优化。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-04-14" version="5.0.2" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>ComplexNet服务器代码精简优化，移除客户端的在线信息维护代码。</item>
    ///             <item>西门子访问类第一次握手信号18字节改为0x02。</item>
    ///             <item>更新JSON组件到11.0.2版本。</item>
    ///             <item>日志存储类优化，支持过滤存储特殊关键字的日志。</item>
    ///             <item>Demo项目新增控件介绍信息。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-04-20" version="5.0.3" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>修复Modbus-Tcp服务器的空异常。</item>
    ///             <item>修复西门子类写入float，double，long数据异常。</item>
    ///             <item>修复modbus-tcp客户端读写字符串颠倒异常。</item>
    ///             <item>修复三菱多读取数据字节的问题。</item>
    ///             <item>双模式客户端新增异形客户端模式，变成了三模式客户端。</item>
    ///             <item>提供异形modbus服务器和客户端Demo方便测试。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-04-25" version="5.0.4" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>Modbus-tcp服务器同时支持RTU数据交互。</item>
    ///             <item>异形客户端新增在线监测，自动剔除访问异常设备。</item>
    ///             <item>modbus-tcp支持读取输入点。</item>
    ///             <item>所有客户端设备的连接超时判断增加休眠，降低CPU负载。</item>
    ///             <item>西门子批量读取上限为19个数组。</item>
    ///             <item>其他小幅度的代码优化。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-04-30" version="5.0.5" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>Modbus相关的代码优化。</item>
    ///             <item>新增Modbus-Rtu客户端模式，配合服务器的串口支持，已经可以实现电脑本机的通讯测试了。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-05-04" version="5.0.6" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>提炼数据转换基类，优化代码，修复WordReverse类对字符串的BUG，相当于修复modbus和omron读写字符串的异常。</item>
    ///             <item>新增一个全新的功能类，数据的推送类，轻量级的高效的订阅发布数据信息。具体参照Demo。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-05-07" version="5.0.7" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>Modbus服务器提供在线客户端数量属性。</item>
    ///             <item>所有服务器基类添加端口缓存。</item>
    ///             <item>双模式客户端完善连接失败，请求超时的消息提示。</item>
    ///             <item>修复双模式客户端某些特殊情况下的头子节NULL异常。</item>
    ///             <item>修复三菱交互类的ASCII协议下的写入数据异常。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-05-12" version="5.0.8" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>新增一个埃夫特机器人的数据访问类。</item>
    ///             <item>双模式客户端的长连接支持延迟连接操作，通过一个新方法完成。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-05-21" version="5.0.9" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>优化ComplexNet客户端的代码。</item>
    ///             <item>更新埃夫特机器人的读取机制到最新版。</item>
    ///             <item>Modbus Rtu及串口基类支持接收超时时间设置，不会一直卡死。</item>
    ///             <item>Modbus Tcp及Rtu都支持带功能码输入，比如读取100地址，等同于03X100。（注意：该多功能地址仅仅适用于Read及相关的方法</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-05-22" version="5.0.10" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>Modbus Tcp及Rtu支持手动更改站号。也就是支持动态站号调整。</item>
    ///             <item>修复上个版本遗留的Modbus在地址偏移情况下会多减1的BUG。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-06-05" version="5.1.0" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>Modbus服务器支持串口发送数据时也会触发消息接收。</item>
    ///             <item>IReadWriteNet接口新增Read(string address,ushort length)方法。</item>
    ///             <item>提炼统一的设备基类，支持Read方法及其扩展的子方法。</item>
    ///             <item>修复埃夫特机器人的读取BUG。</item>
    ///             <item>三菱PLC支持读取定时器，计数器的值，地址格式为"T100"，"C100"。</item>
    ///             <item>新增快速离散的傅立叶频谱变换算法，并在Demo中测试三种周期信号。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-06-16" version="5.1.1" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>修复西门子fetch/write协议对db块，定时器，计数器读写的BUG。</item>
    ///             <item>埃夫特机器人修复tostring()的方法。</item>
    ///             <item>modbus客户端新增两个属性，指示是否字节颠倒和字符串颠倒，根据不同的服务器配置。</item>
    ///             <item>IReadWriteNet接口补充几个数组读取的方法。</item>
    ///             <item>新增一个全新的连接池功能类，详细请参见 https://www.cnblogs.com/dathlin/p/9191211.html </item>
    ///             <item>其他的小bug修复，细节优化。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-06-27" version="5.1.2" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>IByteTransform接口新增bool[]数组转换的2个方法。</item>
    ///             <item>Modbus Server类新增离散输入数据池和输入寄存器数据池，可以在服务器端读写，在客户端读。</item>
    ///             <item>Modbus Tcp及Modbus Rtu及java的modbus tcp支持富地址表示，比如"s=2;100"为站号2的地址100信息。</item>
    ///             <item>Modbus Server修复一个偶尔出现多次异常下线的BUG。</item>
    ///             <item>其他注释修正。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-07-13" version="5.1.3" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>Modbus服务器新增数据大小端配置。</item>
    ///             <item>Modbus服务器支持数据存储本地及从本地加载。</item>
    ///             <item>修复modbus服务器边界读写bug。</item>
    ///             <item>ByteTransformBase的double转换bug修复。</item>
    ///             <item>修复ReverseWordTransform批量字节转换时隐藏的一些bug。</item>
    ///             <item>SoftBasic移除2个数据转换的方法。</item>
    ///             <item>修复modbus写入单个寄存器的高地位倒置的bug。</item>
    ///             <item>修复串口通信过程中字节接收不完整的异常。包含modbus服务器和modbus-rtu。</item>
    ///             <item>添加了.net 4.5项目，并且其他项目源代码引用该项目。添加了单元测试，逐步新增测试方法。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-07-27" version="5.2.0" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>项目新增api文档，提供离线版和在线版，文档提供了一些示例代码。</item>
    ///             <item>modbus-rtu新增批量的数组读取方法。</item>
    ///             <item>modbus-rtu公开ByteTransform属性，方便的进行数据转换。</item>
    ///             <item>SoftMail删除发送失败10次不能继续发送的机制。</item>
    ///             <item>modbus server新增站号属性，站号不对的话，不响应rtu反馈。</item>
    ///             <item>modbus server修复读取65524和65535地址提示越界的bug。</item>
    ///             <item>Demo项目提供了tcp/ip的调试工具。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-08-08" version="5.2.1" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>API文档中西门子FW协议示例代码修复。</item>
    ///             <item>modbus-rtu修复读取线圈和输入线圈的值错误的bug。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-08-23" version="5.2.2" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>Demo中三菱A-1E帧，修复bool读取显示失败的BUG。</item>
    ///             <item>数据订阅类客户端连接上服务器后，服务器立即推送一次。</item>
    ///             <item>串口设备基类代码提炼，提供了多种数据类型的读写支持。</item>
    ///             <item>仪表盘新增属性IsBigSemiCircle，设置为true之后，仪表盘可显示大于半圆的视图。</item>
    ///             <item>提供了一个新的三菱串口类，用于采集FX系列的PLC，MelsecFxSerial</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-08-24" version="5.2.3" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>修复双模式基类的一个bug，支持不接受反馈数据。</item>
    ///             <item>修复三菱串口类的读写bug，包括写入位，和读取字和位。</item>
    ///             <item>相关代码重构优化。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-09-08" version="5.3.0" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>串口基类接收数据优化，保证接收一次完整的数据内容。</item>
    ///             <item>新增一个容器罐子的控件，可以调整背景颜色。</item>
    ///             <item>OperateResult成功时的错误码调整为0。</item>
    ///             <item>修复modbus-tcp及modbus-rtu读取coil及discrete的1个位时解析异常的bug。</item>
    ///             <item>授权类公开一个属性，终极秘钥的属性，感谢 洛阳-LYG 的建议。</item>
    ///             <item>修复transbool方法在特殊情况下的bug</item>
    ///             <item>NetworkDeviceBase 写入的方法设置为了虚方法，允许子类进行重写。</item>
    ///             <item>SoftBasic: 新增三个字节处理的方法，移除前端字节，移除后端字节，移除两端字节。</item>
    ///             <item>新增串口应用的LRC校验方法。还未实际测试。</item>
    ///             <item>Siemens的s7协议支持V区自动转换，方便数据读取。</item>
    ///             <item>新增ab plc的类AllenBradleyNet，已测试读写，bool写入仍存在一点问题。</item>
    ///             <item>新增modbus-Ascii类，该类库还未仔细测试。</item>
    ///             <item>埃夫特机器人更新，适配最新版本数据采集。</item>
    ///             <item>其他的代码优化，重构精简</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-09-10" version="5.3.1" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>修复埃夫特机器人读取数据的bug，已测试通过。</item>
    ///             <item>ByteTransform数据转换层新增一个DataFormat属性，可选ABCD,BADC,CDAB,DCBA</item>
    ///             <item>三个modbus协议均适配了ByteTransform并提供了直接修改的属性，默认ABCD</item>
    ///             <item>注意：如果您的旧项目使用的Modbus类，请务必重新测试适配。给你带来的不便，敬请谅解。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-09-21" version="5.3.2" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>所有显示字符串支持中英文，支持切换，默认为系统语言。</item>
    ///             <item>Json组件依赖设置为不依赖指定版本。</item>
    ///             <item>modbus-ascii类库测试通过。</item>
    ///             <item>新增松下的plc串口读写类，还未测试。</item>
    ///             <item>西门子s7类写入byte数组长度不受限制，原先大概250个字节左右。</item>
    ///             <item>demo界面进行了部分的中英文适配。</item>
    ///             <item>OperateResult类新增了一些额外的构造方法。</item>
    ///             <item>SoftBasic新增了几个字节数组操作相关的通用方法。</item>
    ///             <item>其他大量的细节的代码优化，重构。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-09-27" version="5.3.3" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>DeviceNet层添加异步的API，支持async+await调用。</item>
    ///             <item>java修复西门子的写入成功却提示失败的bug。</item>
    ///             <item>java代码重构，和C#基本保持一致。</item>
    ///             <item>python版本发布，支持三菱，西门子，欧姆龙，modbus，数据订阅，同步访问。</item>
    ///             <item>其他的代码优化，重构精简。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-10-20" version="5.4.0" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>python和java的代码优化，完善，添加三菱A-1E类。</item>
    ///             <item>修复仪表盘控件，最大值小于0会产生的特殊Bug。</item>
    ///             <item>NetSimplifyClient: 提供高级.net的异步版本方法。</item>
    ///             <item>serialBase: 新增初始化和结束的保护方法，允许重写实现额外的操作。</item>
    ///             <item>softBuffer: 添加一个线程安全的buffer内存读写。</item>
    ///             <item>添加西门子ppi协议类，针对s7-200，需要最终测试。</item>
    ///             <item>Panasonic: 修复松下plc的读取读取数据异常。</item>
    ///             <item>修复fx协议批量读取bool时意外的Bug。</item>
    ///             <item>NetSimplifyClient: 新增带用户int数据返回的读取接口。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-10-24" version="5.4.1" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>新增一个温度采集模块的类，基于modbus-rtu实现，阿尔泰科技发展有限公司的DAM3601模块。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-10-25" version="5.4.2" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>三菱的mc协议新增支持读取ZR文件寄存器功能。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-10-30" version="5.4.3" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>修复AB PLC的bool和byte写入失败的bug，感谢 北京-XLang 提供的思路。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-11-1" version="5.5.0" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>新增西门子PPI通讯类库，支持200，200smart等串口通信，感谢 合肥-加劲 和 江阴-  ∮溪风-⊙_⌒ 的测试</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-11-5" version="5.5.1" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>新增三菱计算机链接协议通讯库，支持485组网，有效距离达50米，感谢珠海-刀客的测试。</item>
    ///             <item>串口协议的基类提供了检测当前串口是否处于打开的方法接口。</item>
    ///             <item>西门子S7协议新增槽号为3的s7-400的PLC选项，等待测试。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-11-9" version="5.5.2" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>西门子PPI写入bool方法名重载到了Write方法里。</item>
    ///             <item>松下写入bool方法名重载到了Write方法里。</item>
    ///             <item>修复CRC16验证码在某些特殊情况下的溢出bug。</item>
    ///             <item>西门子类添加槽号和机架号属性，只针对400PLC有效，初步测试可读写。</item>
    ///             <item>ab plc支持对数组的读写操作，支持数组长度为0-246，超过246即失败。</item>
    ///             <item>三菱的编程口协议修复某些特殊情况读取失败，却提示成功的bug。</item>
    ///             <item>串口基类提高缓存空间到4096，并在数据交互时捕获COM口的异常。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-11-16" version="5.6.0" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>修复欧姆龙的数据格式错误，修改为CDAB。</item>
    ///             <item>新增一个瓶子的控件。</item>
    ///             <item>新增一个管道的控件。</item>
    ///             <item>初步新增一个redis的类，初步实现了读写关键字。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-11-21" version="5.6.1" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>AB PLC读取数组过长时提示错误信息。</item>
    ///             <item>正式发布redis客户端，支持一些常用的操作，并提供一个浏览器。博客：https://www.cnblogs.com/dathlin/p/9998013.html </item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-11-24" version="5.6.2" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>曲线控件的曲线支持隐藏其中的一条或是多条曲线，可以用来实现手动选择显示曲线的功能。</item>
    ///             <item>Redis功能块代码优化，支持通知服务器进行数据快照保存，包括同步异步。</item>
    ///             <item>Redis新增订阅客户端类，可以实现订阅一个或是多个频道数据。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-11-30" version="5.6.3" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>串口数据接收的底层机制重新设计。</item>
    ///             <item>串口底层循环验证缓冲区是否有数据的间隔可更改，默认20ms。</item>
    ///             <item>串口底层新增一个清除缓冲区数据的方法。</item>
    ///             <item>串口底层新增一个属性，用于配置是否在每次读写前清除缓冲区的脏数据。</item>
    ///             <item>新增了一个SharpList类，用于超高性能的管理固定长度的数组。博客：https://www.cnblogs.com/dathlin/p/10042801.html </item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-12-3" version="5.6.4" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>Networkbase: 接收方法的一个多余对象删除。</item>
    ///             <item>修复UserDrum控件的默认的text生成，及复制问题。</item>
    ///             <item>UserDrum修复属性在设计界面没有注释的bug。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-12-5" version="5.6.5" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>修复Demo程序在某些特殊情况下无法在线更新的bug。</item>
    ///             <item>修复曲线控件隐藏曲线时在某些特殊情况的不隐藏的bug。</item>
    ///             <item>modbus协议无论读写都支持富地址格式。</item>
    ///             <item>修复连接池清理资源的一个bug，感谢 泉州-邱蕃金</item>
    ///             <item>修复java的modbus代码读取线圈异常的操作。</item>
    ///             <item>Demo程序新增免责条款。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-12-11" version="5.6.6" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>修复redis客户端对键值进行自增自减指令操作时的类型错误bug。</item>
    ///             <item>修复redis客户端对哈希值进行自增自减指令操作时的类型错误bug。</item>
    ///             <item>推送的客户端可选委托或是事件的方式，方便labview调用。</item>
    ///             <item>推送的客户端修复当服务器的关键字不存在时连接未关闭的Bug。</item>
    ///             <item>Demo程序里，欧姆龙测试界面新增数据格式功能。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-12-19" version="5.6.7" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>ByteTransfer数据转换类新增了一个重载的构造方法。</item>
    ///             <item>Redis客户提供了一个写键值并发布订阅的方法。</item>
    ///             <item>AB-PLC支持槽号选择，默认为0。</item>
    ///             <item>PushNet推送服务器新增一个配置，可用于设置是否在客户端刚上线的时候推送缓存数据。</item>
    ///             <item>PushNet推送服务器对客户端的上下限管理的小bug修复。</item>
    ///             <item>本版本开始，组件将使用强签名。</item>
    ///             <item>本版本开始，组件的控件库将不再维护更新，所有的控件在新的控件库重新实现和功能增强，VIP群将免费使用控件库。</item>
    ///             <item>VIP群的进入资格调整为赞助200Rmb，谢谢支持。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-12-27" version="5.7.0" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>修复modbus服务器地址写入的bug，之前写入地址数据后无效，必须带x=3;100才可以。</item>
    ///             <item>修复极少数情况内核对象申请失败的bug，之前会引发资源耗尽的bug。</item>
    ///             <item>SoftBasic的ByteToBoolArray新增一个转换所有位的重载方法，不需要再传递位数。</item>
    ///             <item>埃夫特机器人新增旧版的访问类对象，达到兼容的目的。</item>
    ///             <item>Demo程序新增作者简介。</item>
    ///             <item>修复Demo程序的redis订阅界面在设置密码下无效的bug。</item>
    ///             <item>Demo程序的免责界面新增demo在全球的使用情况。</item>
    ///             <item>VIP群将免费使用全新的控件库，谢谢支持。地址：https://github.com/dathlin/HslControlsDemo </item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2018-12-31" version="5.7.1" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>修复modbus服务器地址读取的bug，之前读取地址数据后无效，必须带x=3;100才可以。</item>
    ///             <item>NetPush功能里，当客户端订阅关键字时，服务器即使没有该关键字，也成功。</item>
    ///             <item>三菱的通讯类支持所有的字读取。例如读取M100的short数据表示M100-M115。</item>
    ///             <item>VIP群将免费使用全新的控件库，谢谢支持。地址：https://github.com/dathlin/HslControlsDemo </item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2019-1-15" version="5.7.2" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>修复三菱A-1E协议的读取数据的BUG错误，给大家造成的不便，非常抱歉。</item>
    ///             <item>VIP群将免费使用全新的控件库，谢谢支持。地址：https://github.com/dathlin/HslControlsDemo </item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2019-2-7" version="5.7.3" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>欧姆龙读写机制更改，报警的异常不再视为失败，仍然可以解析数据。</item>
    ///             <item>Modbus地址优化，Modbus服务器的地址读写优化。</item>
    ///             <item>新增一个数据池类，SoftBuffer，主要用来缓存字节数组内存的，支持BCL数据类型读写。</item>
    ///             <item>Modbus服务器的数据池更新，使用了最新的数据池类SoftBuffer。</item>
    ///             <item>SoftBasic类新增一个GetEnumFromString方法，支持从字符串直接生成枚举值，已通过单元测试。</item>
    ///             <item>新增一个机器人的读取接口信息IRobotNet，统一化所有的机器人的数据读取。</item>
    ///             <item>Demo程序中增加modbus的服务器功能。</item>
    ///             <item>VIP群将免费使用全新的控件库，谢谢支持。地址：https://github.com/dathlin/HslControlsDemo </item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2019-2-13" version="5.7.4" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>日志存储的线程号格式化改为D3，也即三位有效数字。</item>
    ///             <item>日志存储事件BeforeSaveToFile里允许设置日志Cancel属性，强制当前的记录不存储。</item>
    ///             <item>JSON库更新到12.0.1版本。</item>
    ///             <item>SoftBasic新增一个GetTimeSpanDescription方法，用来将时间差转换成文本的方法。</item>
    ///             <item>调整日志分析控件不随字体变化而变化。</item>
    ///             <item>其他的代码精简优化。</item>
    ///             <item>VIP群将免费使用全新的控件库，谢谢支持。地址：https://github.com/dathlin/HslControlsDemo </item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2019-2-21" version="5.8.0" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>SoftBasic修复AddArrayData方法批量添加数据异常的bug，导致曲线控件显示异常。</item>
    ///             <item>提炼一个公共的欧姆龙辅助类，准备为串口协议做基础的通用支持。</item>
    ///             <item>RedisHelper类代码优化精简，提炼部分的公共逻辑到NetSupport。</item>
    ///             <item>SoftBuffer: 新增读写单个的位操作，通过位的与或非来实现。</item>
    ///             <item>SiemensS7Server：新增一个s7协议的服务器，可以模拟PLC，进行通讯测试或是虚拟开发。</item>
    ///             <item>其他的代码精简优化。</item>
    ///             <item>VIP群将免费使用全新的控件库，谢谢支持。地址：https://github.com/dathlin/HslControlsDemo </item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2019-3-4" version="6.0.0" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>西门子虚拟PLC的ToString()方法重新实现。</item>
    ///             <item>埃夫特机器人的json格式化修正换行符。</item>
    ///             <item>IReadWriteNet接口添加Write(address, bytes)的方法。</item>
    ///             <item>Modbus虚拟服务器修复写入位操作时影响后面3个位的bug。</item>
    ///             <item>SoftBuffer内存数据池类的SetValue(byte,index)的bug修复。</item>
    ///             <item>西门子虚拟PLC和Modbus服务器新增客户端管理，关闭时也即断开所有连接。</item>
    ///             <item>三菱编程口协议的读取结果添加错误说明，显示原始返回信号，便于分析。</item>
    ///             <item>三菱MC协议新增远程启动，停止，读取PLC型号的接口。</item>
    ///             <item>新增三菱MC协议的串口的A-3C协议支持，允许读写三菱PLC的数据。</item>
    ///             <item>新增欧姆龙HostLink协议支持，允许读写PLC数据。</item>
    ///             <item>新增基恩士PLC的MC协议支持，包括二进制和ASCII格式，支持读写PLC的数据。</item>
    ///             <item>所有PLC的地址说明重新规划，统一在API文档中查询。</item>
    ///             <item>注意：三菱PLC的地址升级，有一些地址格式进行了更改，比如定时器和计数器，谨慎更新，详细地址参考最新文档。</item>
    ///             <item>如果有公司使用了本库并愿意公开logo的，将在官网及git上进行统一显示，有意愿的联系作者。</item>
    ///             <item>VIP群将免费使用全新的控件库，谢谢支持。地址：https://github.com/dathlin/HslControlsDemo </item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2019-3-10" version="6.0.1" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>修复代码注释上的一些bug，三菱的注释修复。</item>
    ///             <item>调整三菱和基恩士D区数据和W区数据的地址范围，原来只支持到65535。</item>
    ///             <item>SoftIncrementCount: 修复不持久化的序号自增类的数据复原的bug，并添加totring方法。</item>
    ///             <item>IRobot接口更改。针对埃夫特机器人进行重新实现。</item>
    ///             <item>RedisClient: 修复redis类在带有密码的情况下锁死的bug。</item>
    ///             <item>初步添加Kuka机器人的通讯类，等待测试。</item>
    ///             <item>西门子的s7协议读写字符串重新实现，根据西门子的底层存储规则来操作。</item>
    ///             <item>Demo的绝大多的界面进行重构。更友好的支持英文版的显示风格。</item>
    ///             <item>如果有公司使用了本库并愿意公开logo的，将在官网及git上进行统一显示，有意愿的联系作者。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2019-3-21" version="6.0.2" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>修复西门子s7协议读写200smart字符串的bug。</item>
    ///             <item>重构优化NetworkBase及NetwordDoubleBase网络类的代码。</item>
    ///             <item>新增欧姆龙的FinsUdp的实现，DA1【PLC节点号】在配置Ip地址的时候自动赋值，不需要额外配置。</item>
    ///             <item>FinsTcp类的DA1【PLC节点号】在配置Ip地址的时候自动赋值，不需要额外配置。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2019-3-28" version="6.0.3" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>NetPushServer推送服务器修复某些情况下的推送卡死的bug。</item>
    ///             <item>SoftBuffer内存数据类修复Double转换时出现的错误bug。</item>
    ///             <item>修复Kuka机器人读写数据错误的bug，已通过测试。</item>
    ///             <item>修复三菱的MelsecMcAsciiNet类写入bool值及数组会导致异常的bug，已通过单元测试。</item>
    ///             <item>SoftBasic新增从字符串计算MD5码的方法。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2019-4-4" version="6.0.4" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>修复java的NetPushClient掉线重复连接的bug。</item>
    ///             <item>发布java的全新测试Demo。</item>
    ///             <item>Kuka机器人Demo修改帮助链接。</item>
    ///             <item>西门子新增s200的以太网模块连接对象。</item>
    ///             <item>修复文件引擎在上传文件时意外失败，服务器仍然识别为成功的bug。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2019-4-17" version="6.1.0" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>修复日志存储自身异常时，时间没有初始化的bug。</item>
    ///             <item>NetworkBase: 新增UseSynchronousNet属性，默认为true，通过同步的网络进行读写数据，异步手动设置为false。</item>
    ///             <item>修复西门子的读写字符串的bug。</item>
    ///             <item>添加KeyenceNanoSerial以支持基恩士Nano系列串口通信。</item>
    ///             <item>其他的代码优化。</item>
    ///             <item>发布一个基于xamarin的安卓测试demo。</item>
    ///             <item>发布官方论坛： http://bbs.hslcommunication.cn/ </item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2019-4-24" version="6.1.1" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>修复基恩士MC协议读取D区数据索引不能大于100000的bug。</item>
    ///             <item>修复基恩士串口协议读写bool数据的异常bug。</item>
    ///             <item>修复数据推送服务器在客户端异常断开时的奔溃bug，界面卡死bug。</item>
    ///             <item>SoftNumericalOrder类新增数据重置和，最大数限制 。</item>
    ///             <item>ModbusTcp客户端公开属性SoftIncrementCount，可以强制消息号不变，或是最大值。</item>
    ///             <item>NetworkBase: 异步的方法针对Net451及standard版本重写。</item>
    ///             <term>modbus服务器的方法ReadFromModbusCore( byte[] modbusCore )设置为虚方法，可以继承重写，实现自定义返回。</term>
    ///             <item>串口基类serialbase的初始化方法新增多个重载方法，方便VB和labview调用。</item>
    ///             <item>NetworkBase: 默认的机制任然使用异步实现，UseSynchronousNet=false。</item>
    ///             <item>发布官方论坛： http://bbs.hslcommunication.cn/ </item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2019-4-25" version="6.1.2" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>紧急修复在NET451和Core里的异步读取的bug。</item>
    ///             <item>紧急修复PushNetServer的发送回调bug。</item>
    ///             <item>发布官方论坛： http://bbs.hslcommunication.cn/ </item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2019-5-6" version="6.2.0" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>SoftBuffer缓存类支持bool数据的读写，bool数组的读写，并修复double读写的bug。</item>
    ///             <item>Modbus虚拟服务器代码重构实现，继承自NetworkDataServerBase类。</item>
    ///             <item>新增韩国品牌LS的Fast Enet协议</item>
    ///             <item>新增韩国品牌LS的Cnet协议</item>
    ///             <item>新增三菱mc协议的虚拟服务器，仅支持二进制格式的机制。</item>
    ///             <item>LogNet支持写入任意的字符串格式。</item>
    ///             <item>其他的注释添加及代码优化。</item>
    ///             <item>发布官方论坛： http://bbs.hslcommunication.cn/ </item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2019-5-9" version="6.2.1" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>修复三菱读写PLC位时的bug。</item>
    ///             <item>修复Modbus读写线圈及离散的变量bug。</item>
    ///             <item>强烈建议更新，不能使用6.2.0版本！或是回退更低的版本。</item>
    ///             <item>有问题先上论坛： http://bbs.hslcommunication.cn/ </item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2019-5-10" version="6.2.2" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>修复上个版本modbus的致命bug，已通过单元测试。</item>
    ///             <item>新增松下的mc协议，demo已经新增，等待测试。</item>
    ///             <item>github源代码里的支持的型号需要大家一起完善。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2019-5-31" version="6.2.3" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>Ls的Fast Enet协议问题修复，感谢来自埃及朋友。</item>
    ///             <item>Ls的CEnet协议问题修复，感谢来自埃及朋友。</item>
    ///             <item>Ls新增虚拟的PLC服务器，感谢来自埃及朋友。</item>
    ///             <item>改进了机器码获取的方法，获取实际的硬盘串号。</item>
    ///             <item>日志的等级为None的情况，不再格式化字符串，原生写入日志。</item>
    ///             <item>IReadWriteNet接口测试西门子的写入，没有问题。</item>
    ///             <term>三菱及松下，基恩士的地址都调整为最大20亿长度，实际取决于PLC本身。</term>
    ///             <item>松下MC协议修复LD数据库的读写bug。</item>
    ///             <item>Redis的DEMO界面新增删除key功能。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2019-6-3" version="6.2.4" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>Redis新增读取服务器的时间接口，可用于客户端的时间同步。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2019-6-6" version="6.2.5" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>西门子的SiemensS7Net类当读取PLC配置长度的DB块数据时，将提示错误信息。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2019-6-22 " version="7.0.0" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>新增安川机器人通信类，未测试。</item>
    ///             <item>西门子的多地址读取的长度不再限制为19个，而是无限制个。</item>
    ///             <item>NetworkDoubleBase: 实现IDispose接口，方便手动释放资源。</item>
    ///             <item>SerialBase: 实现IDispose接口，方便手动释放资源。</item>
    ///             <item>NetSimplifyClient:新增一个async...await方法。</item>
    ///             <item>NetSimplifyClient:新增读取字符串数组。</item>
    ///             <item>ModbusServer:新增支持账户密码登录，用于构建安全的服务器，仅支持hsl组件的modbus安全访问。</item>
    ///             <item>NetSimplifyServer:新增支持账户密码登录。</item>
    ///             <item>新增永宏PLC的编程口协议。</item>
    ///             <item>新增富士PLC的串口通信，未测试。</item>
    ///             <item>新增欧姆龙PLC的CIP协议通讯。</item>
    ///             <item>初步添加OpenProtocol协议，还未完成，为测试。</item>
    ///             <item>MelsecMcNet:字单位的批量读取长度突破960长度的限制，支持读取任意长度。</item>
    ///             <item>MelsecMcAsciiNet:字单位的批量读取长度突破480长度的限制，支持读取任意长度。</item>
    ///             <item>AllenBradleyNet:读取地址优化，支持读取数组任意起始位置，任意长度，支持结构体嵌套读取。</item>
    ///             <item>其他大量的代码细节优化。</item>
    ///         </list>
    ///     </revision>
    ///     <revision date="2019-6-25" version="7.0.1" author="Richard.Hu">
    ///         <list type="bullet">
    ///             <item>IReadWriteNet完善几个忘记添加的Write不同类型参数的重载方法。</item>
    ///             <item>IReadWriteNet新增ReadBool方法，Write(string address, bool value)方法，是否支持操作需要看plc是否支持，不支持返回操作不支持的错误。</item>
    ///             <item>OmronFinsNet:新增一个属性，IsChangeSA1AfterReadFailed，当设置为True时，通信失败后，就会自动修改SA1的值，这样就能快速链接上PLC了。</item>
    ///             <item>OmronFinsNet:新增读写E区的能力，地址示例E0.0，EF.100，E12.200。</item>
    ///             <item>新增HslDeviceAddress特性类，现在支持直接基于对象的读写操作，提供了一种更加便捷的读写数据的机制，详细的关注后续的论坛。</item>
    ///         </list>
    ///     </revision>
    /// </revisionHistory>
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute( )]
    public class NamespaceDoc
    {

    }


    // 工作的备忘录
    // 1. 三菱的代码提炼，重构，抽象，将MC协议核心提取，适配不同的格式要求。       =================== 基本差不多实现。已完成对三菱3C协议的适配
    // 2. redis的协议在Python上的实现，并且测试。                                     =================== 已实现，通过单元测试
    // 3. python新增对串口的支持并测试。
    // 4. python新增对ab plc的支持。
    // 5. .net端对安川机器人的支持，已经有协议文档。
    // 6. .net端对库卡机器人的支持，http://blog.davidrobot.com/2014/09/kukavarproxy_index.html?tdsourcetag=s_pctim_aiomsg   ===== 已实现，等待测试。
    // 7. .net端对三菱Qna兼容3C帧协议的支持。                                         ===================== 已实现，通过基本的测试
    // 8. .net端对欧姆龙的Fins串口协议的支持                                           ==================== 已实现，正在最后的测试
    // 9. .net端对基恩士PLC的串口支持 https://china.keyence.com/support/user/plc/sample-program/index.jsp            ============== 已实现，基本测试通过

    // 组件之外的计划
    // 1. 研究MQTT协议的通讯                                                           ===================== 研究完成在C#的服务器构建和客户端的数据推送操作
    // 2. 研究 ML.NET 的机器学习的平台
    // 3. 工业网关的深入集成
    // 4. HslCommunication官网集成项目发布接收及案例展示平台
    // 5. 研究PyQt的界面开发和实现



    // bugs
    // IReadWriteNet接口可以读取数据可以，Write方法写入西门子数据不管赋值什么值写入都是0，强制转换成simenss7net 又可以正常写入




    //git checkout A
    //git log
    //找出要合并的commit ID :
    //例如
    //0128660c08e325d410cb845616af355c0c19c6fe
    //然后切换到B分支上
    //git checkout B
    //git cherry-pick  0128660c08e325d410cb845616af355c0c19c6fe

    //然后就将A分支的某个commit合并到了B分支了
}
