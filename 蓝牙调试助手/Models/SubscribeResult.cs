using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 蓝牙调试助手.Models
{
    /// <summary>
    /// 订阅操作结果
    /// </summary>
    public class SubscribeResult
    {
        /// <summary>
        /// 操作结果
        /// </summary>
        public bool State { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }
    }
}
