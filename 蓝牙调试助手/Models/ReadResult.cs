using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 蓝牙调试助手.Models
{
    public class ReadResult
    {   /// <summary>
         /// 操作结果
         /// </summary>
        public bool State { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 读取字节数据
        /// </summary>
        public byte[] Content { get; set; }

        /// <summary>
        /// 读取大小
        /// </summary>
        public int ByteCount { get; set; }

        /// <summary>
        /// 获取十进制
        /// </summary>
        /// <returns></returns>
        public uint ToUint()
        {
            try
            {
                Array.Reverse(Content);
                return BitConverter.ToUInt32(Content, 0);
            }
            catch
            {
                return 0;
            }
            
        }
    }
}
