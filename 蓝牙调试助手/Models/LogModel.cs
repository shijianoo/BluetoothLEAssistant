using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace 蓝牙调试助手.Models
{
    public class LogModel
    {
        /// <summary>
        /// 消息
        /// </summary>
        public string MAC { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public byte[] Value { get; set; }

        /// <summary>
        /// 日志类型
        /// </summary>
        public LogType LogType { get; set; }

        /// <summary>
        /// 是否显示为ASCII格式
        /// </summary>
        public bool IsASCII { get; set; }
    }

    /// <summary>
    /// 日志类型
    /// </summary>
    public enum LogType : uint
    {
        /// <summary>
        /// 一般的消息
        /// </summary>
        General = 1,

        /// <summary>
        /// 写出去消息
        /// </summary>
        Write = 2,

        /// <summary>
        /// 读取类的信息
        /// </summary>
        Read = 4,

        /// <summary>
        /// 通知类的消息
        /// </summary>
        Notiyf = 8,

        /// <summary>
        /// 错误的消息
        /// </summary>
        Error = 16
    }
}
