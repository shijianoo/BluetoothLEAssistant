using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 蓝牙调试助手.Models
{
    public class WriteResult
    {
        /// <summary>
        /// 操作结果
        /// </summary>
        public bool State { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 写的内容
        /// </summary>
        public byte[] Content { get; set; }

        /// <summary>
        /// 写入返回的结果
        /// </summary>
        public byte[] ReturnsResult { get; set; }

        /// <summary>
        /// 写大小
        /// </summary>
        public int ByteCount { get; set; }

    }
}
