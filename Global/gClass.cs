using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BaseUI.BaseEnum;

namespace BaseUI
{
    public partial class Alarm : BaseAlarm
    {
        public object AlarmType { get; }
        public Alarm(Exception ex, object alarmType) : base(ex)
        {
            AlarmType = alarmType;
        }
    }
    
    public partial class UserData : BaseUserData
    {

        public UserData Clone()
        {
            var copy = new UserData();
            CopyBaseTo(copy);

            /// TODO : BaseUserData 에서 상속받는게 아닌 추가 Property 생기면, 이 아래에
            /// copy.NewProperty = NewProperty; 
            /// 이런식으로 추가해줄것.

            return copy;
        }
    }

    public partial class BuildData : BaseBuildData
    {
        
    }

    public partial class BuildFileListViewData : INotifyPropertyChanged
    {
        private bool _isChecked;
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if(_isChecked != value)
                {
                    _isChecked = value;
                    OnPropertyChanged(nameof(IsChecked));
                }
            }
        }

        private string _FileName = string.Empty,
                       _UploadDate = string.Empty;

        private BuildStatus _Status;
        private ComponentStatus _BtnPreviewStatus, _BtnBuildStartStatus;
        public string FileName
        {
            get => _FileName;
            set
            {
                if(_FileName != value)
                {
                    _FileName = value;
                    OnPropertyChanged(nameof(FileName));
                }
            }
        }
        public string UploadDate
        {
            get => _UploadDate;
            set
            {
                if(_UploadDate != value)
                {
                    _UploadDate = value;
                    OnPropertyChanged(nameof(UploadDate));
                }
            }
        }

        public BuildStatus Status
        {
            get => _Status;
            set
            {
                if(_Status != value)
                {
                    _Status = value;
                    OnPropertyChanged(nameof(Status));
                }
            }
        }
        public ComponentStatus BtnPreviewStatus
        {
            get => _BtnPreviewStatus;
            set
            {
                if(_BtnPreviewStatus != value)
                {
                    _BtnPreviewStatus = value;
                    OnPropertyChanged(nameof(BtnPreviewStatus));
                }
            }
        }
        public ComponentStatus BtnBuildStartStatus
        {
            get => _BtnBuildStartStatus;
            set
            {
                if(_BtnBuildStartStatus != value)
                {
                    _BtnBuildStartStatus = value;
                    OnPropertyChanged(nameof(BtnBuildStartStatus));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
