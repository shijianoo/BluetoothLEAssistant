using DrawerDialogService;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using 蓝牙调试助手.Models;

namespace 蓝牙调试助手.ViewModel
{
    public class PairingPanelViewModel : ViewModelBase, IDrawerDialogResult<PairingOperatingResults>
    {
        /// <summary>
        /// Dialog返回值
        /// </summary>
        public PairingOperatingResults Result { get; set; }


        /// <summary>
        /// Dialog关闭方法
        /// </summary>
        public Action CloseAction { get; set; }


        private string _pin;
        /// <summary>
        /// PIN码
        /// </summary>
        public string Pin
        {
            get { return _pin; }
            set { _pin = value; RaisePropertyChanged("Pin"); }
        }


        private string _confirmBtnContent;
        /// <summary>
        /// 确认按钮内容
        /// </summary>
        public string ConfirmBtnContent
        {
            get { return _confirmBtnContent; }
            set { _confirmBtnContent = value; RaisePropertyChanged("ConfirmBtnContent"); }
        }

        private bool _confirmBtnIsVisible;
        /// <summary>
        /// 确认按钮是否可见
        /// </summary>
        public bool ConfirmBtnIsVisible
        {
            get { return _confirmBtnIsVisible; }
            set { _confirmBtnIsVisible = value; RaisePropertyChanged("ConfirmBtnIsVisible"); }
        }


        private string _cancelBtnContent;
        /// <summary>
        /// 取消按钮内容
        /// </summary>
        public string CancelBtnContent
        {
            get { return _cancelBtnContent; }
            set { _cancelBtnContent = value; RaisePropertyChanged("CancelBtnContent"); }
        }

        private bool _cancelBtnIsVisible;
        /// <summary>
        /// 取消按钮是否可见
        /// </summary>
        public bool CancelBtnIsVisible
        {
            get { return _cancelBtnIsVisible; }
            set { _cancelBtnIsVisible = value; RaisePropertyChanged("CancelBtnIsVisible"); }
        }

        /// <summary>
        /// 取消
        /// </summary>
        public RelayCommand ColseCommand { get; set; }

        /// <summary>
        ///确认命令
        /// </summary>
        public RelayCommand ConfirmCommand { get; set; }

        /// <summary>
        /// 取消命令
        /// </summary>
        public RelayCommand CancelCommand { get; set; }

        public PairingPanelViewModel()
        {
            ColseCommand = new RelayCommand(() =>
            {
                if (CloseAction != null)
                {
                    Result = new PairingOperatingResults
                    {
                        ConfirmPinMatchResult = false
                    };

                    CloseAction.Invoke();
                }

            });

            ConfirmCommand = new RelayCommand(Confirm_Command);
            CancelCommand = new RelayCommand(Cancel_Command);
        }

        

        /// <summary>
        /// 确认pin码
        /// </summary>
        /// <param name="pin"></param>
        public void ConfirmPinMatchAction(string pin)
        {
            ConfirmBtnContent = "是";
            CancelBtnContent = "否";
            ConfirmBtnIsVisible = true;
            CancelBtnIsVisible = true;
            Pin = pin;
        }

        /// <summary>
        /// 取消
        /// </summary>
        private void Cancel_Command()
        {
            Result = new PairingOperatingResults
            {
                ConfirmPinMatchResult = false
            };



            if (CloseAction != null)
            {
                CloseAction.Invoke();
            }
        }

        /// <summary>
        /// 确认
        /// </summary>
        private void Confirm_Command()
        {
            Result = new PairingOperatingResults
            {
                ConfirmPinMatchResult = true
            };



            if (CloseAction != null)
            {
                CloseAction.Invoke();
            }
        }


        public void SetPairingMode(DevicePairingKinds PairingKind)
        {
            switch (PairingKind)
            {
                case DevicePairingKinds.None:
                    break;
                case DevicePairingKinds.ConfirmOnly:
                    break;
                case DevicePairingKinds.DisplayPin:
                    break;
                case DevicePairingKinds.ProvidePin:
                    break;
                case DevicePairingKinds.ConfirmPinMatch:
                    break;
                case DevicePairingKinds.ProvidePasswordCredential:
                    break;
                default:
                    break;
            }
        }
    }
}
