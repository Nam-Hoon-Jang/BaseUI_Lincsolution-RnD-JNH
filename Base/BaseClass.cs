using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using static BaseUI.BaseEnum;

namespace BaseUI
{
    /// <summary>
    /// BaseSharedModel의 인터페이스의 Base
    /// </summary>
    public interface IBaseSharedModel<TAlarm> : INotifyPropertyChanged where TAlarm : BaseAlarm
    {
        /// <summary>UI 상태 공유용 Enum</summary>
        public UiStatus UiState { get; set; }

        /// <summary>UI에서 갖고 있는 알람 리스트</summary>
        public ObservableCollection<TAlarm> AlarmList { get; set; }

        /// <summary>UI 시스템 시간 표시 및 공유용</summary>
        public DateTime SystemTime { get; }
    }

    /// <summary>
    /// 전체 View에서 공유하는 데이터 처리 클래스의 Base
    /// </summary>
    /// <typeparam name="TAlarm">알람 클래스</typeparam>
    public partial class BaseSharedModel<TAlarm> : ObservableObject, IDisposable, IBaseSharedModel<TAlarm>
        where TAlarm : BaseAlarm
    {
        [ObservableProperty] private UiStatus _UiState;
        [ObservableProperty] private ObservableCollection<TAlarm> _AlarmList = new ObservableCollection<TAlarm>();

        public DateTime SystemTime => DateTime.Now;

        /// <summary>Dispose 상태 확인용 내부 변수</summary>
        protected bool disposed = false;
        /// <summary>UI 시간 갱신용 스탑워치</summary>
        protected readonly DispatcherTimer sysTimer;

        /// <summary>
        /// 생성과 동시에 시스템 시간 확인용 타이머 시작
        /// </summary>
        /// <param name="timerInterval">시스템 시간 확인 타이머 호출 간격(ms)</param>
        public BaseSharedModel(int timerInterval = 500)
        {
            sysTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(timerInterval)
            };
            sysTimer.Tick += (_, _) =>
            {
                OnPropertyChanged(nameof(SystemTime));
            };
            sysTimer.Start();
        }

        /// <summary>
        /// SharedModel 내부 리소스 Dispose 메소드
        /// </summary>
        public virtual void Dispose()
        {
            sysTimer.Stop();
            sysTimer.Dispatcher.DisableProcessing();

            disposed = true;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose 메소드가 호출되지 않은 경우, SharedModel 객체가 가비지 컬렉션에 의해 수집될 때 최종적으로 호출되는 소멸자.
        /// </summary>
        ~BaseSharedModel()
        {
            if(disposed) return;
            Dispose();
        }
    }

    /// <summary>
    /// Design Time View 에서 보여줄 때 임의로 사용하기 위한 더미 SharedModel의 Base
    /// </summary>
    /// <typeparam name="TAlarm"></typeparam>
    public partial class BaseDummySharedModel<TAlarm> : ObservableObject, IBaseSharedModel<TAlarm> where TAlarm : BaseAlarm
    {
        public UiStatus UiState { get; set; }
        public ObservableCollection<TAlarm> AlarmList { get; set; }
        public DateTime SystemTime { get; set; }
    }

    /// <summary>
    /// 알람 클래스의 Base
    /// </summary>
    public partial class BaseAlarm : ObservableObject
    {
        /// <summary>생성된 알람의 ID -> 고유값으로 쓰기 위해 Guid로 생성</summary>
        public Guid Id { get; } = Guid.NewGuid();

        /// <summary>알람 생성일. yyyy-MM-dd HH:mm:ss 로 생성</summary>
        public string CreatedAt{ get; }

        /// <summary>알람 메세지.</summary>
        public string Message { get; }

        /// <summary>알람 해제 여부 확인용 변수</summary>
        [ObservableProperty] private bool _IsResolved = false;

        /// <summary>
        /// 알람 생성자
        /// </summary>
        /// <param name="ex">예외</param>
        public BaseAlarm(Exception ex)
        {
            Message = ex.Message;
            CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }

    /// <summary>
    /// 유저 데이터의 Base
    /// </summary>
    public partial class BaseUserData : ObservableObject
    {
        [ObservableProperty] private string _UserDataPath = string.Empty;
        [ObservableProperty] private string _BuildFileDir = string.Empty;
        [ObservableProperty] private string _UserLanguage = string.Empty;
        [ObservableProperty] private int _BuildLogStoragePeriod = 30;
        [ObservableProperty] private int _SystemLogStoragePeriod = 30;

        /// <summary>
        /// BaseUserData 내용 복사하기 편하도록 만든 복사용 함수
        /// 이 클래스를 상속받는 클래스에서 Clone() 실행할 때, 실행히 편함.
        /// </summary>
        /// <param name="target"></param>
        protected void CopyBaseTo(BaseUserData target)
        {
            target.UserDataPath = UserDataPath;
            target.BuildFileDir = BuildFileDir;
            target.UserLanguage = UserLanguage;
            target.BuildLogStoragePeriod = BuildLogStoragePeriod;
            target.SystemLogStoragePeriod = SystemLogStoragePeriod;
        }
    }

    /// <summary>
    /// 빌드 데이터의 Base
    /// </summary>
    public partial class BaseBuildData : ObservableObject
    {
        [ObservableProperty] private string _BuildDataPath = string.Empty;
        [ObservableProperty] private string _BuildFilePath = string.Empty;
        [ObservableProperty] private string _BuildFileName = string.Empty;
        [ObservableProperty] private string _BuildLogFilePath = string.Empty;
        
        [ObservableProperty] private Enum _BuildStatus = BaseEnum.BuildStatus.IDLE;

        [ObservableProperty] private DateTime _BuildFileUploadDate;
        [ObservableProperty] private DateTime _BuildStartDate;
        [ObservableProperty] private DateTime _BuildEndDate;
        [ObservableProperty] private DateTime _EstimatedBuildEndDate;
        [ObservableProperty] private TimeSpan _BuildTotalRunningTimeExpected;
        [ObservableProperty] private TimeSpan _BuildElapsedTime;
    }
}
