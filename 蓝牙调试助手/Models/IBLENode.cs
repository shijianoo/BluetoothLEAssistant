using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 蓝牙调试助手.Models
{
    /// <summary>
    /// 蓝牙信息节点
    /// </summary>
    interface IBLENode
    {
        string Name { get; set; }

        Guid UUid { get; set; }
    }
}
