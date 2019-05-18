using Microsoft.Win32.SafeHandles;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;

namespace HslCommunication.BasicFramework
{
    /// <summary>
    /// 软件授权类
    /// </summary>
    public class SoftAuthorize : SoftFileSaveBase
    {
        #region Constructor

        /// <summary>
        /// 实例化一个软件授权类
        /// </summary>
        /// <param name="UseAdmin">是否使用管理员模式</param>
        public SoftAuthorize(bool UseAdmin=false)
        {
            machine_code = GetInfo(UseAdmin);
            LogHeaderText = "SoftAuthorize";
        }

        #endregion

        #region Static Members

        /// <summary>
        /// 注册码描述文本
        /// </summary>
        public static readonly string TextCode = "Code";

        #endregion

        #region Private Members

        /// <summary>
        /// 最终的注册秘钥信息，注意是只读的。
        /// </summary>
        /// <remarks>
        /// 时间：2018年9月1日 23:01:54，来自 洛阳-LYG 的建议，公开了本属性信息，只读。
        /// </remarks>
        public string FinalCode { get; private set; } = "";
        /// <summary>
        /// 是否正式发行版，是的话就取消授权
        /// </summary>
        public bool IsReleaseVersion { get; set; } = false;
        /// <summary>
        /// 指示是否加载过文件信息
        /// </summary>
        private bool HasLoadByFile { get; set; } = false;


        private string machine_code = "";

        #endregion

        #region Public Members


        /// <summary>
        /// 指示系统是否处于试用运行
        /// </summary>
        public bool IsSoftTrial { get; set; } = false;



        #endregion

        #region Public Method


        /// <summary>
        /// 获取本机的机器码
        /// </summary>
        /// <returns>机器码字符串</returns>
        public string GetMachineCodeString()
        {
            return machine_code;
        }

        /// <summary>
        /// 获取需要保存的数据内容
        /// </summary>
        /// <returns>实际保存的内容</returns>
        public override string ToSaveString()
        {
            JObject json = new JObject
            {
                { TextCode, new JValue(FinalCode) }
            };
            return json.ToString();
        }
        /// <summary>
        /// 从字符串加载数据
        /// </summary>
        /// <param name="content">文件存储的数据</param>
        public override void LoadByString(string content)
        {
            JObject json = JObject.Parse(content);
            FinalCode = SoftBasic.GetValueFromJsonObject(json, TextCode, FinalCode);
            HasLoadByFile = true;
        }
        /// <summary>
        /// 使用特殊加密算法加密数据
        /// </summary>
        public override void SaveToFile()
        {
            SaveToFile(m => SoftSecurity.MD5Encrypt(m));
        }
        /// <summary>
        /// 使用特殊解密算法解密数据
        /// </summary>
        public override void LoadByFile()
        {
            LoadByFile(m => SoftSecurity.MD5Decrypt(m));
        }



        /// <summary>
        /// 检查该注册码是否是正确的注册码
        /// </summary>
        /// <param name="code">注册码信息</param>
        /// <param name="encrypt">数据加密的方法，必须用户指定</param>
        /// <returns>是否注册成功</returns>
        public bool CheckAuthorize(string code, Func<string, string> encrypt)
        {
            if (code != encrypt(GetMachineCodeString()))
            {
                return false;
            }
            else
            {
                FinalCode = code;
                SaveToFile();
                return true;
            }
        }

        /// <summary>
        /// 检测授权是否成功
        /// </summary>
        /// <param name="encrypt">数据加密的方法，必须用户指定</param>
        /// <returns>是否成功授权</returns>
        public bool IsAuthorizeSuccess(Func<string, string> encrypt)
        {
            if (IsReleaseVersion) return true;

            if (encrypt(GetMachineCodeString()) == FinalCode)
            {
                return true;
            }
            else
            {
                FinalCode = "";
                SaveToFile();
                return false;
            }
        }


        #endregion

        #region Static Method


        /// <summary>
        /// 获取本计算机唯一的机器码  
        /// </summary>
        /// <returns>字符串形式的机器码</returns>
        public static string GetInfo(bool UseAdmin)
        {
            //说明 这个方法所有的获取hwid的行为均在Ring3模式下获取 方法前半部分为WMI 后半部分为S.M.A.R.T.(DeviceIoControl())
            //程序依赖kernel32.dll不能运行在wince下 纯c#解决方案几乎不可能在Ring0模式下获取 如果有更高的要求建议加密狗
            string unique = "";
            //bios名称
            unique += HWID.BIOS + "|";
            //cpu信息
            unique += HWID.CPU + "|";
            //硬盘信息
            unique += HWID.HDD + "|";
            //主板信息
            unique += HWID.BaseBoard + "|";
            //mac信息 群主建议取消mac计算 这里建议放弃 mac太玄学了
            //unique += HWID.MAC + "|";
            //是否存在scsi 
            if (HWID.IsServer)
            {
                unique += HWID.SCSI+ "|";
            }

            //获取系统盘ID 新增 较为稳定
            string systempath = Environment.GetEnvironmentVariable("systemdrive");//获取当前系统盘
            string win32_logicaldisk = "\"" + systempath + "\"";
            ManagementObject dsk = new ManagementObject(@"win32_logicaldisk.deviceid=" + win32_logicaldisk);
            dsk.Get();
            unique += dsk["VolumeSerialNumber"].ToString();
            unique += "|";

            //获取SMBIOS的id https://docs.microsoft.com/zh-cn/windows/desktop/CIMWin32Prov/win32-computersystemproduct
            ManagementClass cimobject3 = new ManagementClass("Win32_ComputerSystemProduct");
            ManagementObjectCollection moc3 = cimobject3.GetInstances();
            foreach (ManagementObject mo in moc3)
            {
                unique += (string)mo.Properties["UUID"].Value;
                break;
            }
            unique += "|";
            //如果启用了管理员模式 则读取hwid
            if (UseAdmin)
            {
                WindowsIdentity current = WindowsIdentity.GetCurrent();
                WindowsPrincipal windowsPrincipal = new WindowsPrincipal(current);
                bool IsAdmin = windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
                //硬盘物理id
                if (IsAdmin)
                {
                    var HddInfo = GetHddInfo();
                    unique += HddInfo.SerialNumber;
                }
                else
                {
                    //创建启动对象
                    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                    //设置运行文件
                    startInfo.FileName = System.Windows.Forms.Application.ExecutablePath;
                    //设置启动动作,确保以管理员身份运行
                    startInfo.Verb = "runas";
                    //如果不是管理员，则启动UAC
                    System.Diagnostics.Process.Start(startInfo);
                    //退出
                    System.Windows.Forms.Application.Exit();
                }
            }
          
            SHA1CryptoServiceProvider SHA1 = new SHA1CryptoServiceProvider();
            var md5= SoftBasic.ByteToHexString(SHA1.ComputeHash(Encoding.Unicode.GetBytes(unique)));
            return md5.Substring(0, 25);
        }


        #endregion
        #region HWID
        private class HWID
        {
            public  static String BIOS { get {return GetWMIIdent("Win32_BIOS", "Manufacturer", "SerialNumber", "SMBIOSBIOSVersion", "IdentificationCode"); }}
            public static String CPU { get { return GetWMIIdent("Win32_Processor", "ProcessorId", "UniqueId", "Name"); } }
            public static String HDD { get { return GetWMIIdent("Win32_DiskDrive", "Model", "TotalHeads"); } }
            public static String GPU { get { return GetWMIIdent("Win32_VideoController", "DriverVersion", "Name"); } }
            public static String MAC { get { return GetWMIIdent("Win32_NetworkAdapterConfiguration", "MACAddress"); } }
            public static String OS { get { return GetWMIIdent("Win32_OperatingSystem", "SerialNumber", "Name"); } }
            public static String SCSI { get { return GetWMIIdent("Win32_SCSIController", "DeviceID", "Name"); } }
            public static String BaseBoard { get { return GetWMIIdent("Win32_BaseBoard", "SerialNumber", "PartNumber"); } }
            public static Boolean IsServer { get { return HDD.Contains("SCSI"); } }
        }
        private static String GetWMIIdent(String Class, String Property)
        {
            var ident = "";
            var objCol = new ManagementClass(Class).GetInstances();
            foreach (var obj in objCol)
            {
                if ((ident = obj.GetPropertyValue(Property) as String) != "")
                    break;
            }
            return ident;
        }

        private static String GetWMIIdent(String Class, params String[] Propertys)
        {
            var ident = "";
            Array.ForEach(Propertys, prop => ident += GetWMIIdent(Class, prop) + " ");
            return ident;
        }

        /// <summary>
        /// 获得硬盘信息
        /// </summary>
        /// <param name="driveIndex">硬盘序号</param>
        /// <returns>硬盘信息</returns>
        /// <remarks>
        /// by sunmast for everyone
        /// thanks lu0 for his great works
        /// 在Windows Array8/ME中，S.M.A.R.T并不缺省安装，请将SMARTVSD.VXD拷贝到%SYSTEM%＼IOSUBSYS目录下。
        /// 在Windows 2000/2003下，需要Administrators组的权限。
        /// </remarks>
        /// <example>
        /// AtapiDevice.GetHddInfo()
        /// </example>
        public static HardDiskInfo GetHddInfo(byte driveIndex=0)
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32Windows:
                    return GetHddInfoArrayx(driveIndex);
                case PlatformID.Win32NT:
                    return GetHddInfoNT(driveIndex);
                case PlatformID.Win32S:
                    throw new NotSupportedException("Win32s is not supported.");
                case PlatformID.WinCE:
                    throw new NotSupportedException("WinCE is not supported.");
                default:
                    throw new NotSupportedException("Unknown Platform.");
            }
        }
        private static HardDiskInfo GetHardDiskInfo(IdSector phdinfo)
        {
            HardDiskInfo hddInfo = new HardDiskInfo();

            ChangeByteOrder(phdinfo.sModelNumber);
            hddInfo.ModuleNumber = Encoding.ASCII.GetString(phdinfo.sModelNumber).Trim();

            ChangeByteOrder(phdinfo.sFirmwareRev);
            hddInfo.Firmware = Encoding.ASCII.GetString(phdinfo.sFirmwareRev).Trim();

            ChangeByteOrder(phdinfo.sSerialNumber);
            hddInfo.SerialNumber = Encoding.ASCII.GetString(phdinfo.sSerialNumber).Trim();

            hddInfo.Capacity = phdinfo.ulTotalAddressableSectors / 2 / 1024;

            return hddInfo;
        }
        private static HardDiskInfo GetHddInfoArrayx(byte driveIndex)
        {
            GetVersionOutParams vers = new GetVersionOutParams();
            SendCmdInParams inParam = new SendCmdInParams();
            SendCmdOutParams outParam = new SendCmdOutParams();
            uint bytesReturned = 0;

            IntPtr hDevice = CreateFile(@"＼＼.＼Smartvsd", FileAccess.ReadWrite, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, System.IO.FileAttributes.Normal, IntPtr.Zero);
            if (hDevice == IntPtr.Zero)
            {
                throw new Exception("Open smartvsd.vxd failed.");
            }
            if (0 == DeviceIoControl(
                hDevice,
                DFP_GET_VERSION,
                IntPtr.Zero,
                0,
                ref vers,
                (uint)Marshal.SizeOf(vers),
                ref bytesReturned,
                IntPtr.Zero))
            {
                CloseHandle(hDevice);
                throw new Exception("DeviceIoControl failed:DFP_GET_VERSION");
            }
            // If IDE identify command not supported, fails
            if (0 == (vers.fCapabilities & 1))
            {
                CloseHandle(hDevice);
                throw new Exception("Error: IDE identify command not supported.");
            }
            if (0 != (driveIndex & 1))
            {
                inParam.irDriveRegs.bDriveHeadReg = 0xb0;
            }
            else
            {
                inParam.irDriveRegs.bDriveHeadReg = 0xa0;
            }
            if (0 != (vers.fCapabilities & (16 >> driveIndex)))
            {
                // We don’t detect a ATAPI device.
                CloseHandle(hDevice);
                throw new Exception(string.Format("Drive {0} is a ATAPI device, we don’t detect it", driveIndex + 1));
            }
            else
            {
                inParam.irDriveRegs.bCommandReg = 0xec;
            }
            inParam.bDriveNumber = driveIndex;
            inParam.irDriveRegs.bSectorCountReg = 1;
            inParam.irDriveRegs.bSectorNumberReg = 1;
            inParam.cBufferSize = 512;
            if (0 == DeviceIoControl(
                hDevice,
                DFP_RECEIVE_DRIVE_DATA,
                ref inParam,
                (uint)Marshal.SizeOf(inParam),
                ref outParam,
                (uint)Marshal.SizeOf(outParam),
                ref bytesReturned,
                IntPtr.Zero))
            {
                CloseHandle(hDevice);
                throw new Exception("DeviceIoControl failed: DFP_RECEIVE_DRIVE_DATA");
            }
            CloseHandle(hDevice);

            return GetHardDiskInfo(outParam.bBuffer);
        }
        private static HardDiskInfo GetHddInfoNT(byte driveIndex)
        {
            GetVersionOutParams vers = new GetVersionOutParams();
            SendCmdInParams inParam = new SendCmdInParams();
            SendCmdOutParams outParam = new SendCmdOutParams();
            uint bytesReturned = 0;

            // We start in NT/Win2000
            // open a handle to PhysicalDriveN instead
            IntPtr hDevice = CreateFile(@"\\.\PhysicalDrive0", FileAccess.ReadWrite, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, System.IO.FileAttributes.Normal, IntPtr.Zero);
            //判断句柄是否无效
            if (hDevice == IntPtr.Zero)
            {
                throw new Exception("CreateFile faild.");
            }
            if (0 == DeviceIoControl(
                hDevice,
                DFP_GET_VERSION,
                IntPtr.Zero,
                0,
                ref vers,
                (uint)Marshal.SizeOf(vers),
                ref bytesReturned,
                IntPtr.Zero))
            {
                CloseHandle(hDevice);
                throw new Exception(string.Format("Drive {0} may not exists.", driveIndex + 1));
            }
            // If IDE identify command not supported, fails
            if (0 == (vers.fCapabilities & 1))
            {
                CloseHandle(hDevice);
                throw new Exception("Error: IDE identify command not supported.");
            }
            // Identify the IDE drives
            if (0 != (driveIndex & 1))
            {
                inParam.irDriveRegs.bDriveHeadReg = 0xb0;
            }
            else
            {
                inParam.irDriveRegs.bDriveHeadReg = 0xa0;
            }
            if (0 != (vers.fCapabilities & (16 >> driveIndex)))
            {
                // We don’t detect a ATAPI device.
                CloseHandle(hDevice);
                throw new Exception(string.Format("Drive {0} is a ATAPI device, we don’t detect it.", driveIndex + 1));
            }
            else
            {
                inParam.irDriveRegs.bCommandReg = 0xec;
            }
            inParam.bDriveNumber = driveIndex;
            inParam.irDriveRegs.bSectorCountReg = 1;
            inParam.irDriveRegs.bSectorNumberReg = 1;
            inParam.cBufferSize = 512;

            if (0 == DeviceIoControl(
                hDevice,
                DFP_RECEIVE_DRIVE_DATA,
                ref inParam,
                (uint)Marshal.SizeOf(inParam),
                ref outParam,
                (uint)Marshal.SizeOf(outParam),
                ref bytesReturned,
                IntPtr.Zero))
            {
                CloseHandle(hDevice);
                throw new Exception("DeviceIoControl failed: DFP_RECEIVE_DRIVE_DATA");
            }
            CloseHandle(hDevice);

            return GetHardDiskInfo(outParam.bBuffer);
        }
        private static void ChangeByteOrder(byte[] charArray)
        {
            byte temp;
            for (int i = 0; i < charArray.Length; i += 2)
            {
                temp = charArray[i];
                charArray[i] = charArray[i + 1];
                charArray[i + 1] = temp;
            }
        }
        /// <summary>
        /// 执行打开/建立资源的功能。
        /// </summary>
        /// <param name="lpFileName">指定要打开的设备或文件的名称。</param>
        /// <param name="dwDesiredAccess">
        /// <para>Win32 常量，用于控制对设备的读访问、写访问或读/写访问的常数。内容如下表：
        /// <p><list type="table">
        /// <listheader>
        /// <term>名称</term>
        /// <description>说明</description>
        /// </listheader>
        /// <item>
        /// <term>GENERIC_READ</term><description>指定对设备进行读取访问。</description>
        /// </item>
        /// <item>
        /// <term>GENERIC_WRITE</term><description>指定对设备进行写访问。</description>
        /// </item>
        /// <item><term><b>0</b></term><description>如果值为零，则表示只允许获取与一个设备有关的信息。</description></item>
        /// </list></p>
        /// </para>
        /// </param>
        /// <param name="dwShareMode">指定打开设备时的文件共享模式</param>
        /// <param name="lpSecurityAttributes"></param>
        /// <param name="dwCreationDisposition">Win32 常量，指定操作系统打开文件的方式。内容如下表：
        /// <para><p>
        /// <list type="table">
        /// <listheader><term>名称</term><description>说明</description></listheader>
        /// <item>
        /// <term>CREATE_NEW</term>
        /// <description>指定操作系统应创建新文件。如果文件存在，则抛出 <see cref="IOException"/> 异常。</description>
        /// </item>
        /// <item><term>CREATE_ALWAYS</term><description>指定操作系统应创建新文件。如果文件已存在，它将被改写。</description></item>
        /// </list>
        /// </p></para>
        /// </param>
        /// <param name="dwFlagsAndAttributes"></param>
        /// <param name="hTemplateFile"></param>
        /// <returns>使用函数打开的设备的句柄。</returns>
        /// <remarks>
        /// 本函数可以执行打开或建立文件、文件流、目录/文件夹、物理磁盘、卷、系统控制的缓冲区、磁带设备、
        /// 通信资源、邮件系统和命名管道。
        /// </remarks>
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr CreateFile(
          string lpFileName,
          [MarshalAs(UnmanagedType.U4)] FileAccess dwDesiredAccess,
          [MarshalAs(UnmanagedType.U4)] FileShare dwShareMode,
          IntPtr lpSecurityAttributes,
          [MarshalAs(UnmanagedType.U4)] FileMode dwCreationDisposition,
          [MarshalAs(UnmanagedType.U4)] System.IO.FileAttributes dwFlagsAndAttributes,
          IntPtr hTemplateFile);

        /// <summary>
        /// 关闭一个指定的指针对象指向的设备。。
        /// </summary>
        /// <param name="hObject">要关闭的句柄 <see cref="IntPtr"/> 对象。</param>
        /// <returns>成功返回 <b>0</b> ，不成功返回非零值。</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int CloseHandle(IntPtr hObject);

        /// <summary>
        /// 对设备执行指定的操作。
        /// </summary>
        /// <param name="hDevice">要执行操作的设备句柄。</param>
        /// <param name="dwIoControlCode">Win32 API 常数，输入的是以 <b>FSCTL_</b> 为前缀的常数，定义在
        /// <b>WinIoCtl.h</b> 文件内，执行此重载方法必须输入 <b>SMART_GET_VERSION</b> 。</param>
        /// <param name="lpInBuffer">当参数为指针时，默认的输入值是 <b>0</b> 。</param>
        /// <param name="nInBufferSize">输入缓冲区的字节数量。</param>
        /// <param name="lpOutBuffer">一个 <b>GetVersionOutParams</b> ，表示执行函数后输出的设备检查。</param>
        /// <param name="nOutBufferSize">输出缓冲区的字节数量。</param>
        /// <param name="lpBytesReturned">实际装载到输出缓冲区的字节数量。</param>
        /// <param name="lpOverlapped">同步操作控制，一般不使用，默认值为 <b>0</b> 。</param>
        /// <returns>非零表示成功，零表示失败。</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int DeviceIoControl(IntPtr hDevice, uint dwIoControlCode, IntPtr lpInBuffer,
        uint nInBufferSize, ref GetVersionOutParams lpOutBuffer,
        uint nOutBufferSize, ref uint lpBytesReturned,
        [Out] IntPtr lpOverlapped);

        /// <summary>
        /// 对设备执行指定的操作。
        /// </summary>
        /// <param name="hDevice">要执行操作的设备句柄。</param>
        /// <param name="dwIoControlCode">Win32 API 常数，输入的是以 <b>FSCTL_</b> 为前缀的常数，定义在
        /// <b>WinIoCtl.h</b> 文件内，执行此重载方法必须输入 <b>SMART_SEND_DRIVE_COMMAND</b> 或 <b>SMART_RCV_DRIVE_DATA</b> 。</param>
        /// <param name="lpInBuffer">一个 <b>SendCmdInParams</b> 结构，它保存向系统发送的查询要求具体命令的数据结构。</param>
        /// <param name="nInBufferSize">输入缓冲区的字节数量。</param>
        /// <param name="lpOutBuffer">一个 <b>SendCmdOutParams</b> 结构，它保存系统根据命令返回的设备相信信息二进制数据。</param>
        /// <param name="nOutBufferSize">输出缓冲区的字节数量。</param>
        /// <param name="lpBytesReturned">实际装载到输出缓冲区的字节数量。</param>
        /// <param name="lpOverlapped">同步操作控制，一般不使用，默认值为 <b>0</b> 。</param>
        /// <returns>非零表示成功，零表示失败。</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int DeviceIoControl(IntPtr hDevice, uint dwIoControlCode, ref SendCmdInParams lpInBuffer,
        uint nInBufferSize, ref SendCmdOutParams lpOutBuffer,
        uint nOutBufferSize, ref uint lpBytesReturned,
        [Out] IntPtr lpOverlapped);

        const uint DFP_GET_VERSION = 0x00074080;
        const uint DFP_SEND_DRIVE_COMMAND = 0x0007c084;
        const uint DFP_RECEIVE_DRIVE_DATA = 0x0007c088;

        const uint GENERIC_READ = 0x80000000;
        const uint GENERIC_WRITE = 0x40000000;
        const uint FILE_SHARE_READ = 0x00000001;
        const uint FILE_SHARE_WRITE = 0x00000002;
        const uint CREATE_NEW = 1;
        const uint OPEN_EXISTING = 3;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct SendCmdInParams
        {
            public uint cBufferSize;
            public IdeRegs irDriveRegs;
            public byte bDriveNumber;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] bReserved;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public uint[] dwReserved;
            public byte bBuffer;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct DriverStatus
        {
            public byte bDriverError;
            public byte bIDEStatus;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] bReserved;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public uint[] dwReserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct SendCmdOutParams
        {
            public uint cBufferSize;
            public DriverStatus DriverStatus;
            public IdSector bBuffer;
        }
        #endregion
        #region Internal Structs
        [Serializable]
        public struct HardDiskInfo
        {
            /// <summary>
            /// 型号
            /// </summary>
            public string ModuleNumber;
            /// <summary>
            /// 固件版本
            /// </summary>
            public string Firmware;
            /// <summary>
            /// 序列号
            /// </summary>
            public string SerialNumber;
            /// <summary>
            /// 容量，以M为单位
            /// </summary>
            public uint Capacity;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct GetVersionOutParams
        {
            public byte bVersion;
            public byte bRevision;
            public byte bReserved;
            public byte bIDEDeviceMap;
            public uint fCapabilities;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public uint[] dwReserved; // For future use.
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct IdeRegs
        {
            public byte bFeaturesReg;
            public byte bSectorCountReg;
            public byte bSectorNumberReg;
            public byte bCylLowReg;
            public byte bCylHighReg;
            public byte bDriveHeadReg;
            public byte bCommandReg;
            public byte bReserved;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 512)]
        internal struct IdSector
        {
            public ushort wGenConfig;
            public ushort wNumCyls;
            public ushort wReserved;
            public ushort wNumHeads;
            public ushort wBytesPerTrack;
            public ushort wBytesPerSector;
            public ushort wSectorsPerTrack;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public ushort[] wVendorUnique;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] sSerialNumber;
            public ushort wBufferType;
            public ushort wBufferSize;
            public ushort wECCSize;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] sFirmwareRev;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
            public byte[] sModelNumber;
            public ushort wMoreVendorUnique;
            public ushort wDoubleWordIO;
            public ushort wCapabilities;
            public ushort wReserved1;
            public ushort wPIOTiming;
            public ushort wDMATiming;
            public ushort wBS;
            public ushort wNumCurrentCyls;
            public ushort wNumCurrentHeads;
            public ushort wNumCurrentSectorsPerTrack;
            public uint ulCurrentSectorCapacity;
            public ushort wMultSectorStuff;
            public uint ulTotalAddressableSectors;
            public ushort wSingleWordDMA;
            public ushort wMultiWordDMA;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public byte[] bReserved;
        }

        #endregion
    }
}
