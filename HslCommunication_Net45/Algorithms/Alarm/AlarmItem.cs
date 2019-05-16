using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace HslCommunication.Algorithms.Alarm
{
    /// <summary>
    /// 单次报警的信息内容
    /// </summary>
    public class AlarmItem
    {
        #region Contructor

        /// <summary>
        /// 实例化一个默认的对象
        /// </summary>
        public AlarmItem( )
        {
            this.uniqueId = Interlocked.Increment( ref AlarmIdCurrent );
        }

        /// <summary>
        /// 使用默认的用户id和报警描述信息来初始化报警
        /// </summary>
        /// <param name="userId">用户的自身的id标识信息</param>
        /// <param name="alarmDescription">报警的描述信息</param>
        public AlarmItem( int userId, string alarmDescription )
        {
            this.uniqueId = Interlocked.Increment( ref AlarmIdCurrent );
            this.userId = userId;
            this.alarmDescription = alarmDescription;
        }

        /// <summary>
        /// 使用默认的用户id和报警描述信息来初始化报警
        /// </summary>
        /// <param name="alarmCode">报警的代号</param>
        /// <param name="userId">用户的自身的id标识信息</param>
        /// <param name="alarmDescription">报警的描述信息</param>
        public AlarmItem( int alarmCode, int userId, string alarmDescription )
        {
            this.uniqueId = Interlocked.Increment( ref AlarmIdCurrent );
            this.alarmCode = alarmCode;
            this.userId = userId;
            this.alarmDescription = alarmDescription;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// 本次系统运行的唯一报警信息，用来标识操作的信息的
        /// </summary>
        public long UniqueId => uniqueId;

        /// <summary>
        /// 报警的ID信息
        /// </summary>
        public int AlarmCode
        {
            get => alarmCode;
            set => alarmCode = value;
        }

        /// <summary>
        /// 用户自带的标记信息，可以用来区分不同的设备的情况
        /// </summary>
        public int UserId
        {
            get => userId;
            set => userId = value;
        }

        #endregion

        #region Private Member

        private long uniqueId = 0;                              // 报警的唯一标识
        private int alarmCode = 0;                              // 当按照错误码处理的情况的代号
        private int userId = 0;                                 // 自定义的标识信息，可以用来标记不同的设备信息
        private DateTime startTime = DateTime.Now;              // 报警开始的时间
        private DateTime finishTime = DateTime.Now;             // 报警的结束时间
        private string alarmDescription = string.Empty;         // 报警的基本描述
        private bool isChecked = false;                         // 是否被检查过
        private bool isViewed = false;                          // 是否被查看过
        private string checkName = string.Empty;                // 被检查人的账户
        private AlarmDegree alarmDegree = AlarmDegree.Hint;     // 报警的等级信息

        private static long AlarmIdCurrent = 0;

        #endregion

    }
}
