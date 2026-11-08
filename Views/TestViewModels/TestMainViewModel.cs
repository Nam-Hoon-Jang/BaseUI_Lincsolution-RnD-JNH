using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using static BaseUI.BaseEnum;
using static BaseUI.Function;
using static BaseUI.gEnum;
using static BaseUI.gProperty;

namespace BaseUI
{
    public partial class TestMainViewModel : MainViewModel
    {
        /// <summary>
        /// WPF 디자인 뷰에서만 호출되는 생성자
        /// </summary>
        public TestMainViewModel()
            : base(new MainModel(), new DummySharedModel(), new DummyNavigationService())
        {
            DesignTimeInitialise();
        }

        /// <summary>
        /// TESTMODE 켰을 때의 런타임에서 호출되는 생성자.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="sm"></param>
        public TestMainViewModel(MainModel m, ISharedModel sm, INavigationService nav)
            : base(m, sm, nav)
        {
            if(DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
            {
                return;
            }
            Debug.WriteLine($"[TestMainViewModel] 생성됨 : {this.GetHashCode()}");
            Initialise();
        }

        /// <summary>
        /// TESTMODE 에서 MainView는 App.xaml.cs 에서 로드 진행하고, 이 때 JobViewModel 불러와야 정상 부팅.
        /// TestMainViewModel 에서의 Intialise는 오버라이드 처리하지 않고, 기존 MainViewModel 의 Intialise 함수를 덮어썼음.
        /// 실행되는 시점을 TestMainViewModel의 생성자 내부에서 처리될 수 있도록 하기 위함.
        /// override 처리하면, App.xaml.cs 쪽에서 ViewModel 들 AddTransient 하는 과정에서 생성이 되버리는데, 
        /// 이 시점에는 아직 JobViewModel이 AddTransient로 생성되지 않은 시점이므로,
        /// Navigation 으로 불러올 경우 null 이라 터져버림.
        /// </summary>
        private void Initialise()
        {
            Navigation.NavigateTo(() => ViewModelLoader.Load<DashboardViewModel>(App.Services));
            SetStatus(UiStatus.IDLE);
            for(int i = 0; i < 12; i++)
            {
                if(i % 3 == 0)
                {
                    Shared.AlarmList.Add(new Alarm(new Exception("Test"), GetResourceObject("Alarm.Info")));
                }
                else if(i % 3 == 1)
                {
                    Shared.AlarmList.Add(new Alarm(new Exception("Test"), GetResourceObject("Alarm.Warn")));
                }
                else
                {
                    Shared.AlarmList.Add(new Alarm(new Exception("Test"), GetResourceObject("Alarm.Error")));
                }
            }
        }

        /// <summary>
        /// 디자인 뷰에서만 호출되는 초기화 함수
        /// </summary>
        private void DesignTimeInitialise()
        {
            DesignTimeViewModel = new TestDashboardViewModel();
            //DesignTimeViewModel = new TestPreferenceViewModel();
            IsCheckedBtnChangeThema = true;
            Model.SetViewThema(IsCheckedBtnChangeThema);
            TagAlarmBtn = GetResourceObject("State.On");
            ValueProgressBarTitle = 50;
            SetStatus(UiStatus.IDLE);
            AlarmListPopupVisibility = Visibility.Visible;

            for(int i = 0; i < 12; i++)
            {
                if(i % 3 == 0)
                {
                    AlarmList.Add(new Alarm(new Exception("Test"), GetResourceObject("Alarm.Info")));
                }
                else if(i % 3 == 1)
                {
                    AlarmList.Add(new Alarm(new Exception("Test"), GetResourceObject("Alarm.Warn")));
                }
                else
                {
                    AlarmList.Add(new Alarm(new Exception("Test"), GetResourceObject("Alarm.Error")));
                }
            }

            PopupType = BaseEnum.PopupType.Info;
            PopupContent = "Test Content";
            PopupVisibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 테스트 모드용.
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
                    if(IsCheckedBtnAlarm)
                    {
                        TagAlarmBtn = GetResourceObject("State.On");
                    }
                    else
                    {
                        TagAlarmBtn = GetResourceObject("State.Off");
                    }
                    //TestUiStatusChange();
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

        private void TestUiStatusChange()
        {
            if(TagGridTitle.Equals(GetResourceObject("State.Idle")))
            {
                Shared.UiState =(UiStatus.BUSY);
            }
            else if(TagGridTitle.Equals(GetResourceObject("State.Busy")))
            {
                Shared.UiState =(UiStatus.READY);
            }
            else if(TagGridTitle.Equals(GetResourceObject("State.Ready")))
            {
                Shared.UiState =(UiStatus.BUILD);
            }
            else if(TagGridTitle.Equals(GetResourceObject("State.Build")))
            {
                Shared.UiState =(UiStatus.PAUSE);
            }
            else if(TagGridTitle.Equals(GetResourceObject("State.Pause")))
            {
                Shared.UiState =(UiStatus.DONE);
            }
            else if(TagGridTitle.Equals(GetResourceObject("State.Done")))
            {
                Shared.UiState =(UiStatus.STOP);
            }
            else if(TagGridTitle.Equals(GetResourceObject("State.Stop")))
            {
                Shared.UiState =(UiStatus.WARNING);
            }
            else if(TagGridTitle.Equals(GetResourceObject("State.Warn")))
            {
                Shared.UiState =(UiStatus.ERROR);
            }
            else if(TagGridTitle.Equals(GetResourceObject("State.Error")))
            {
                Shared.UiState =(UiStatus.IDLE);
            }
        }
    }
}
