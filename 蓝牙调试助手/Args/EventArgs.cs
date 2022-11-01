using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 蓝牙调试助手.Args
{
    public class CharacteristicValueEventArgs : EventArgs
    {
        /// <summary>
        /// 改变的值
        /// </summary>
        public byte[] Value { get; set; }

        /// <summary>
        /// 操作结果
        /// </summary>
        public bool State { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 接收到数据大小
        /// </summary>
        public int ByteCount { get; set; }


        /// <summary>
        /// 是否为ASCII格式
        /// </summary>
        public bool IsASCII { get; set; }
    }

    /// <summary>
    /// 特征编码信息
    /// </summary>
    public class SubscriptionEncodingEventArgs : EventArgs
    {
        /// <summary>
        /// 是否为ASCII格式
        /// </summary>
        public bool IsASCII { get; set; }

       
    }
}
