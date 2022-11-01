using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 蓝牙调试助手.Models
{
    public class AppSetting
    {
        /// <summary>
        /// 是否为亮色
        /// </summary>
        public bool IsLigth { get; set; }

        /// <summary>
        /// 是否添加CRC
        /// </summary>
        public bool IsAddCRC { get; set; }

        /// <summary>
        /// 配置栏是否使用带结果写入
        /// </summary>
        public bool IsWriteWithResult { get; set; }

        /// <summary>
        /// 是否循环读取
        /// </summary>
        public bool IsLoopWrite { get; set; }

        /// <summary>
        /// 配置列表
        /// </summary>
        public ObservableCollection<Config> Configs;

        public AppSetting()
        {
            Configs = new ObservableCollection<Config>();
        }
    }
}
