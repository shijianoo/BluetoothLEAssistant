using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 蓝牙调试助手.Models
{
    public class Config : ObservableObject
    {
        private string _value;
        /// <summary>
        /// 写入的特征值
        /// </summary>
        public string Value
        {
            get { return _value; }
            set { _value = value; RaisePropertyChanged("Value"); }
        }

        private Eoding _eoding;
        /// <summary>
        /// 编码格式
        /// </summary>
        public Eoding Eoding
        {
            get { return _eoding; }
            set { _eoding = value; RaisePropertyChanged("Eoding"); }
        }

        private string _note;
        /// <summary>
        /// 备注
        /// </summary>
        public string Note
        {
            get { return _note; }
            set { _note = value; RaisePropertyChanged("Note"); }
        }

    }
}
