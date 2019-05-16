using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HslCommunication.Algorithms.Alarm
{
    /// <summary>
    /// 报警的等级，主要是区分提示性报警，错误性报警，致命性报警。
    /// </summary>
    public enum AlarmDegree
    {
        /// <summary>
        /// 提示性报警，通常是仅仅作为提示性的消息，会自动恢复的报警
        /// </summary>
        Hint = 1,
        /// <summary>
        /// 错误性报警，通常是发送质量问题的报警，需要管理人员手动确认的
        /// </summary>
        Error,
        /// <summary>
        /// 致命性的报警，出现了重大的设备问题，需要停机维修的情况
        /// </summary>
        Fatal,
    }
}
