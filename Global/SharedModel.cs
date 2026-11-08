using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Threading;
using static BaseUI.BaseEnum;

namespace BaseUI
{
    public interface ISharedModel : IBaseSharedModel<Alarm>
    {
        /// <summary>UI에서 사용하는 유저가 변경 가능한 데이터들</summary>
        public UserData UserData { get; set; }
        /// <summary>MainView Menu 탭 접힘/펼침 공유용 변수</summary>
        public bool IsMenuCollapsed { get; set; }
        /// <summary>현재 빌드 중인 파일명</summary>
        //public string CurrentBuildFile { get; set; }
        /// <summary>현재 진행 중인 Build 진행도</summary>
        public double BuildProgressValue { get; set; }
        /// <summary>최초 예측된 총 빌드 시간. 빌드 시작 최초 1회만 갱신</summary>
        public TimeSpan ExpectedTotalRunningTime { get; set; }
        /// <summary>남은 빌드 시간 예측값(갱신)</summary>
        public TimeSpan EstimatedBuildTime{ get; set; }
        /// <summary>팝업창 열림 여부</summary>
        public bool IsPopupOpen { get; set; }
    }
    public partial class SharedModel : BaseSharedModel<Alarm>, ISharedModel
    {
        /// <summary>
        /// 공통 공유 데이터 생성자.
        /// </summary>
        /// <param name="timerInterval">기본 500ms</param>
        public SharedModel(int timerInterval = 500) : base(timerInterval)
        {
            
        }

        /// <summary>
        /// UserData 값 변경 시, 자동 업데이트
        /// </summary>
        /// <param name="userdata"></param>
        partial void OnUserDataChanged(UserData userdata)
        {
            try
            {
                File.WriteAllText(userdata.UserDataPath, JsonSerializer.Serialize<UserData>(userdata));
                gProperty.SystemLogger.Write("User Data Updated");
            }
            catch(Exception ex)
            {
                gProperty.SystemLogger.Write(ex.Message, Logger.Case.Exception);
            }
        }

        /// <summary>
        /// SharedModel 내부 리소스 Dispose 메소드
        /// </summary>
        public override void Dispose()
        {
            /// TODO : 여기에 SharedModel이 관리하는 리소스 해제 코드 작성


            /// BaseSharedModel 에 있는 Dispose 최종 실행.
            base.Dispose();
        }

        /// <summary>
        /// Dispose 메소드가 호출되지 않은 경우, SharedModel 객체가 가비지 컬렉션에 의해 수집될 때 최종적으로 호출되는 소멸자.
        /// </summary>
        ~SharedModel()
        {
            if(disposed) return;
            Dispose();
        }

        #region Variables
        [ObservableProperty] private UserData _UserData = new UserData();
        [ObservableProperty] private volatile bool _IsMenuCollapsed = false;
        [ObservableProperty] private double _BuildProgressValue = 0.0d;
        [ObservableProperty] private TimeSpan _ExpectedTotalRunningTime;
        [ObservableProperty] private TimeSpan _EstimatedBuildTime;
        [ObservableProperty] private bool _IsPopupOpen = false;
        #endregion
    }

    public sealed class DummySharedModel : BaseDummySharedModel<Alarm>, ISharedModel
    {
        public UserData UserData { get; set; }
        public bool IsMenuCollapsed { get; set; }
        public double BuildProgressValue { get; set; }
        public TimeSpan ExpectedTotalRunningTime { get; set; }
        public TimeSpan EstimatedBuildTime{ get; set; }
        public bool IsPopupOpen { get; set; }
    }
}
