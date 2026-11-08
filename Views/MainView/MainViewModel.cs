using CommunityToolkit.Mvvm.ComponentModel;
using static BaseUI.gEnum;
using static BaseUI.gProperty;
using static BaseUI.Function;
using static BaseUI.BaseEnum;
using System.Windows.Media;
using System.Windows;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace BaseUI
{
    /// <summary>
    /// MainView.xaml 윈도우 자체 이벤트 발생 시,
    /// 해당 이벤트에 ViewModel 함수를 구독시키고,
    /// ViewModel 에서 처리하기 위한 인터페이스.
    /// 인터페이스 형태로 사용해야 TESTMODE 랑 RUNTIME 에도 유연하게 작동할 수 있음.
    /// </summary>
    public interface IMainViewEvents
    {
        void OnLoaded();
        void HandleAlarmListViewButtonClicked(int index);
        void HandleMenuExpanded(bool isCollapsed);
    }

    public partial class MainViewModel : BaseViewModel<MainModel, ISharedModel, MainViewCommand>, IMainViewEvents
    {
        /// <summary>
        /// MainViewModel 생성자.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="sm"></param>
        public MainViewModel(MainModel m, ISharedModel sm, INavigationService nav)
            : base(m, sm)
        {
            Navigation = nav;
            Navigation.PropertyChanged += (s, e) =>
            {
                if(e.PropertyName == nameof(Navigation.CurrentViewModel))
                {
                    OnPropertyChanged(nameof(CurrentViewModel));
                }
            };
            CommandPopup = new RelayCommand<PopupCommand>(ExecutePopup);
#if !TESTMODE
            if(!IsDesignMode)
            {
                /// 디자인 모드가 아니라면,
                Navigation.NavigateTo(() => ViewModelLoader.Load<DashboardViewModel>(App.Services));
            }
            else
            {
                /// 디자인 모드라면, Test 뷰모델로 업로드해서 디자인 타임에 할당.
                DesignTimeViewModel = new TestDashboardViewModel();
            }
#endif

        }

        /// <summary>
        /// UI의 MainView.xaml 에서 Loaded 이벤트 발생 시 호출되는 함수.
        /// </summary>
        public void OnLoaded()
        {
            Model.GetUserData();
        }

        /// <summary>
        /// UI의 알람 리스트 뷰 버튼 핸들러 함수
        /// </summary>
        /// <param name="index">선택된 알람의 인덱스 번호</param>
        public void HandleAlarmListViewButtonClicked(int index)
        {
            if(index >= 0)
            {
                Debug.WriteLine($"Alarm List View Button Clicked, index: {index}");
                SystemLogger.Write($"Alarm List View Button Clicked, index: {index}", Logger.Case.Button);
                var alarm = AlarmList[index];
                var type = AlarmList[index].AlarmType;
                if(type.Equals(GetResourceObject("Alarm.Info")))
                {
                    OpenPopup(BaseEnum.PopupType.InfoSingle, alarm.Message, PopupCommand.Ok);
                }
                else if(type.Equals(GetResourceObject("Alarm.Warn")))
                {
                    OpenPopup(BaseEnum.PopupType.WarningSingle, alarm.Message, PopupCommand.Ok);
                }
                else if(type.Equals(GetResourceObject("Alarm.Error")))
                {
                    OpenPopup(BaseEnum.PopupType.ErrorSingle, alarm.Message, PopupCommand.Ok);
                }
            }
        }

        /// <summary>
        /// UI의 Menu 확장/축소 버튼 핸들러 함수
        /// </summary>
        /// <param name="isCollapsed">확장/축소 여부 파라미터</param>
        public void HandleMenuExpanded(bool isCollapsed) => Shared.IsMenuCollapsed = isCollapsed;

        /// <summary>
        /// MainView 처음 생성 시 호출되는 초기화 함수.
        /// </summary>
        protected override void Initialise()
        {
            Shared.AlarmList.CollectionChanged += OnAlarmCollectionChanged;
        }

        /// <summary>
        /// 버튼 입력 시 동작 처리 함수
        /// </summary>
        /// <param name="cmd">버튼별 커맨드 파리미터</param>
        protected override void Execute(MainViewCommand cmd)
        {
            SystemLogger.Write(cmd.ToString(), Logger.Case.Button);
            switch(cmd)
            {
                case MainViewCommand.ChangeThema:
                    Model.SetViewThema(IsCheckedBtnChangeThema);
                    break;

                case MainViewCommand.OpenAlarmList:
                    if(AlarmListPopupVisibility.Equals(Visibility.Collapsed))
                    {
                        AlarmListPopupVisibility = Visibility.Visible;
                    }
                    else
                    {
                        AlarmListPopupVisibility = Visibility.Collapsed;
                    }
                    break;

                case MainViewCommand.AllAlarmClear:
                    OpenPopup(BaseEnum.PopupType.Warning, GetResourceString("Alarm_Warn_ClearAllAlarms"), PopupCommand.ClearAllAlarm);
                    break;

                case MainViewCommand.EmergencyStop:
                    if(IsCheckedBtnEmergencyStop)
                    {
                        SetStatus(UiStatus.ERROR);
                    }
                    else
                    {
                        SetStatus(UiStatus.IDLE);
                    }
                    break;

                case MainViewCommand.OpenDashboard:
                    Navigation.NavigateTo(() => ViewModelLoader.Load<DashboardViewModel>(App.Services));
                    break;

                case MainViewCommand.OpenBuild:
                    
                    break;

                case MainViewCommand.OpenMonitoring:
                    Navigation.NavigateTo(() => ViewModelLoader.Load<MonitoringViewModel>(App.Services));
                    break;

                case MainViewCommand.OpenPreference:
                    Navigation.NavigateTo(() => ViewModelLoader.Load<PreferenceViewModel>(App.Services));
                    break;
            }
        }

        /// <summary>
        /// 팝업창에서 확인 버튼 또는 취소 버튼 입력 시 상호작용 처리 함수
        /// </summary>
        /// <param name="cmd">입력된 팝업창의 커맨드 Enum</param>
        protected void ExecutePopup(PopupCommand cmd)
        {
            SystemLogger.Write(cmd.ToString(), Logger.Case.Button);
            switch(cmd)
            {
                case PopupCommand.Ok:
                case PopupCommand.No:
                    
                    break;

                case PopupCommand.ClearAllAlarm:
                    Shared.AlarmList.Clear();
                    break;
            }

            PopupVisibility = Visibility.Collapsed;
            Shared.IsPopupOpen = false;
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
                [nameof(Shared.BuildProgressValue)] = () => ValueProgressBarTitle = Shared.BuildProgressValue,
            };
            Shared.PropertyChanged += PropertiesChanged;
        }

        protected void SetStatus(UiStatus status)
        {
            if(!IsDesignMode)
            {
                SystemLogger.Write(status.ToString(), Logger.Case.Info);
            }
            switch(status)
            {
                case UiStatus.IDLE:
                    SetUiControlTags("State.Idle");
                    break;

                case UiStatus.BUSY:
                    SetUiControlTags("State.Busy");
                    break;

                case UiStatus.READY:
                    SetUiControlTags("State.Ready");
                    break;

                case UiStatus.BUILD:
                    SetUiControlTags("State.Build");
                    break;

                case UiStatus.PAUSE:
                    SetUiControlTags("State.Pause");
                    break;

                case UiStatus.DONE:
                    SetUiControlTags("State.Done");
                    break;

                case UiStatus.STOP:
                    SetUiControlTags("State.Stop");
                    break;

                case UiStatus.WARNING:
                    SetUiControlTags("State.Warn");
                    break;

                case UiStatus.ERROR:
                    SetUiControlTags("State.Error");
                    break;
            }
        }
        protected void SetUiControlTags(string tag)
        {
            TagGridStatus = TagGridTitle = TagGridTitleLogo = TagProgressBarTitle = GetResourceObject(tag);
        }

        protected void OnAlarmCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            AlarmList = Shared.AlarmList;
        }

        protected void OpenPopup(PopupType type, string content, PopupCommand cmdL, PopupCommand cmdR = PopupCommand.No)
        {
            PopupType = type;
            CmdParBtnPopupLeft = cmdL;
            CmdParBtnPopupRight = cmdR;
            PopupContent = content;
            PopupVisibility = Visibility.Visible;
            Shared.IsPopupOpen = true;
        }
        #region UI Variables
        public ICommand CommandPopup { get; }

        [ObservableProperty] private Enum _CmdParBtnChangeThema = MainViewCommand.ChangeThema;
        [ObservableProperty] private Enum _CmdParBtnAlarm = MainViewCommand.OpenAlarmList;
        [ObservableProperty] private Enum _CmdParBtnEmergencyStop = MainViewCommand.EmergencyStop;
        [ObservableProperty] private Enum _CmdParBtnDashboard = MainViewCommand.OpenDashboard;
        [ObservableProperty] private Enum _CmdParBtnBuild = MainViewCommand.OpenBuild;
        [ObservableProperty] private Enum _CmdParBtnMonitoring = MainViewCommand.OpenMonitoring;
        [ObservableProperty] private Enum _CmdParBtnControl = MainViewCommand.OpenControl;
        [ObservableProperty] private Enum _CmdParBtnLogList = MainViewCommand.OpenLogList;
        [ObservableProperty] private Enum _CmdParBtnBuildParameter = MainViewCommand.OpenBuildParameter;
        [ObservableProperty] private Enum _CmdParBtnPreference = MainViewCommand.OpenPreference;
        [ObservableProperty] private Enum _CmdParBtnAlarmAllClear = MainViewCommand.AllAlarmClear;
        [ObservableProperty] private Enum _PopupType = BaseEnum.PopupType.Info;
        [ObservableProperty] private Enum _CmdParBtnPopupLeft = PopupCommand.Ok;
        [ObservableProperty] private Enum _CmdParBtnPopupRight = PopupCommand.No;

        [ObservableProperty] private object _TagGridTitleLogo = GetResourceObject("State.Idle");
        [ObservableProperty] private object _TagGridTitle = GetResourceObject("State.Idle");
        [ObservableProperty] private object _TagGridStatus = GetResourceObject("State.Idle");
        [ObservableProperty] private object _TagAlarmBtn = GetResourceObject("State.Off");
        [ObservableProperty] private object _TagProgressBarTitle = GetResourceObject("State.Idle");

        [ObservableProperty] private bool _IsCheckedBtnChangeThema;
        [ObservableProperty] private bool _IsCheckedBtnAlarm;
        [ObservableProperty] private bool _IsCheckedBtnEmergencyStop;

        [ObservableProperty] private string _PopupContent = string.Empty;

        [ObservableProperty] private double _ValueProgressBarTitle = 0;        
        
        [ObservableProperty] private Visibility _AlarmListPopupVisibility = Visibility.Collapsed;
        [ObservableProperty] private Visibility _PopupVisibility = Visibility.Collapsed;

        [ObservableProperty] private ObservableCollection<Alarm> _AlarmList = new ObservableCollection<Alarm>();
        #endregion
    }
}
