using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BaseUI.BaseEnum;
using static BaseUI.gEnum;
using static BaseUI.gProperty;
using static BaseUI.Function;
using System.Collections.ObjectModel;

namespace BaseUI
{
    public partial class DashboardViewModel : BaseViewModel<DashboardModel, ISharedModel, DashboardViewCommand>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <param name="sm"></param>
        /// <param name="nav"></param>
        public DashboardViewModel(DashboardModel m, ISharedModel sm, INavigationService nav)
            : base(m, sm)
        {
            Navigation = nav;
            Navigation.PropertyChanged += (s, e) =>
            {
                if(e.PropertyName == nameof(Navigation.CurrentViewModel))
                    OnPropertyChanged(nameof(CurrentViewModel));
            };
        }

        /// <summary>
        /// DashboardView 처음 생성 시 호출되는 초기화 함수
        /// </summary>
        protected override void Initialise()
        {
            
        }

        /// <summary>
        /// 버튼 입력 시 동작 처리 함수
        /// </summary>
        /// <param name="cmd">버튼별 커맨드 파리미터</param>
        protected override void Execute(DashboardViewCommand cmd)
        {
            SystemLogger.Write(cmd.ToString(), Logger.Case.Button);
            switch(cmd)
            {
                
            }
        }

        /// <summary>
        /// SharedModel 에 있는 공통 데이터값과 이 ViewModel에서 UI에 Binding되어있는 변수 맵핑
        /// key = SharedModel의 Property 이름, value(Action) = 할당하는 Action
        /// </summary>
        protected override void PropertiesMapping()
        {
            _sharedModelPropertyMap = new Dictionary<string, Action>
            {
                [nameof(Shared.UiState)] = () => SetStatus(Shared.UiState),
                [nameof(Shared.BuildProgressValue)] = () => BuildProgressBarValue = Shared.BuildProgressValue,
                [nameof(Shared.ExpectedTotalRunningTime)] = () =>
                {
                    TotalRunningTime = Shared.ExpectedTotalRunningTime.ToString(@"d\.hh\:mm\:ss");
                },

            };
            Shared.PropertyChanged += PropertiesChanged;
        }

        public void HandleBuildFileListButtonClicked(int index)
        {
            SelectedBuildFileIndex = index;
        }

        private void SetStatus(UiStatus status)
        {
            if(!IsDesignMode)
            {
                SystemLogger.Write(status.ToString(), Logger.Case.Info);
            }
            switch(status)
            {
                case UiStatus.IDLE:
                    TagStatus = GetResourceObject("State.Idle");
                    break;

                case UiStatus.BUSY:
                    TagStatus = GetResourceObject("State.Busy");
                    break;

                case UiStatus.READY:
                    TagStatus = GetResourceObject("State.Ready");
                    break;

                case UiStatus.BUILD:
                    TagStatus = GetResourceObject("State.Build");
                    break;

                case UiStatus.PAUSE:
                    TagStatus = GetResourceObject("State.Pause");
                    break;

                case UiStatus.DONE:
                    TagStatus = GetResourceObject("State.Done");
                    break;

                case UiStatus.STOP:
                    TagStatus = GetResourceObject("State.Stop");
                    break;

                case UiStatus.WARNING:
                    TagStatus = GetResourceObject("State.Warn");
                    break;

                case UiStatus.ERROR:
                    TagStatus = GetResourceObject("State.Error");
                    break;
            }
        }

        #region Variables
        [ObservableProperty] private Enum _CmdParBtnPreview = DashboardViewCommand.OpenPreview;
        [ObservableProperty] private Enum _CmdParBtnBuildStart = DashboardViewCommand.OpenBuildStart;
        [ObservableProperty] [NotifyPropertyChangedFor(nameof(BuildProgressBarValueText))]private double _BuildProgressBarValue = 0.0d;
        public string BuildProgressBarValueText => BuildProgressBarValue.ToString("0.0") + "%";
        [ObservableProperty] private string _TotalRunningTime = "00:00:00";
        [ObservableProperty] private object _TagStatus = GetResourceObject("State.Idle");
        [ObservableProperty] private string _ElapsedTime = string.Empty;
        [ObservableProperty] private string _RemainingTime = string.Empty;
        [ObservableProperty] private string _StartDate = string.Empty;
        [ObservableProperty] private string _EndDate = string.Empty;
        [ObservableProperty] private string _CurrentHeight = string.Empty;
        [ObservableProperty] private int _TotalLayers = 0;
        [ObservableProperty] private int _CurrentLayer = 0;
        [ObservableProperty] private string _Layers = string.Empty;
        [ObservableProperty] private string _CurrentBuildProcess = string.Empty;
        [ObservableProperty] private ObservableCollection<BuildFileListViewData> _BuildFileList = new ObservableCollection<BuildFileListViewData>();
        [ObservableProperty] private int _SelectedBuildFileIndex = -1;
        #endregion
    }
}
