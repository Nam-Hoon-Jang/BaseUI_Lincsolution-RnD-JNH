using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using static BaseUI.BaseEnum;
using static BaseUI.Function;
using static BaseUI.gEnum;
using static BaseUI.gProperty;

namespace BaseUI
{
    public partial class PreferenceViewModel : BaseViewModel<PreferenceModel, ISharedModel, PreferenceViewCommand>
    {
        /// <summary>
        /// PreferenceViewModel 생성자.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="sm"></param>
        public PreferenceViewModel(PreferenceModel m, ISharedModel sm, INavigationService nav)
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
        /// PreferenceView 처음 생성 시 호출되는 초기화 함수.
        /// </summary>
        protected override void Initialise()
        {
            LanguageListSource = new List<string>()
            {
                TEXT.LANG.English,
                TEXT.LANG.Korean
            };
        }

        /// <summary>
        /// 버튼 입력 시 동작 처리 함수
        /// </summary>
        /// <param name="cmd">버튼별 커맨드 파리미터</param>
        protected override void Execute(PreferenceViewCommand cmd)
        {
            SystemLogger.Write(cmd.ToString(), Logger.Case.Button);
            switch(cmd)
            {
                case PreferenceViewCommand.LanguageSet:
                    Model.SetLanguage(SelectedLanguage);
                    break;

                case PreferenceViewCommand.BuildFileDirectorySet:
                    Model.SetBuildFileDirectory();
                    break;

                case PreferenceViewCommand.BuildLogStoragePeriodSet:
                    Model.SetBuildLogStoragePeriod(BuildLogStoragePeriod);
                    break;

                case PreferenceViewCommand.SystemLogStoragePeriodSet:
                    Model.SetSystemLogStoragePeriod(SystemLogStoragePeriod);
                    break;
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
                [nameof(Shared.SystemTime)] = () => SystemDateTime = Shared.SystemTime,
                [nameof(Shared.UserData)] = () => 
                {
                    if(Shared.UserData.UserLanguage.Equals(gProperty.TEXT.LANG_CODE.English))
                    {
                        SelectedLanguage = gProperty.TEXT.LANG.English;
                    }
                    else if(Shared.UserData.UserLanguage.Equals(gProperty.TEXT.LANG_CODE.Korean))
                    {
                        SelectedLanguage = gProperty.TEXT.LANG.Korean;
                    }
                    BuildFileDir = Shared.UserData.BuildFileDir;
                },
            };
            Shared.PropertyChanged += PropertiesChanged;
        }

        #region Variables
        [ObservableProperty] private GridLength _ViewWidth = new GridLength(1640);
        [ObservableProperty] private Enum _CmdParBtnLangSet = PreferenceViewCommand.LanguageSet;
        [ObservableProperty] private Enum _CmdParBtnBuildFileDirSet = PreferenceViewCommand.BuildFileDirectorySet;
        [ObservableProperty] private Enum _CmdParBtnBuildLogPeriodSet = PreferenceViewCommand.BuildLogStoragePeriodSet;
        [ObservableProperty] private Enum _CmdParBtnSystemLogPeriodSet = PreferenceViewCommand.SystemLogStoragePeriodSet;
        [ObservableProperty] private DateTime _SystemDateTime = DateTime.Now;
        [ObservableProperty] private string _SoftwareVersion = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion.ToString();
        [ObservableProperty] private List<string> _LanguageListSource;
        [ObservableProperty] private string _SelectedLanguage = string.Empty;
        [ObservableProperty] private string _BuildFileDir = string.Empty;
        [ObservableProperty] private string _BuildLogStoragePeriod = string.Empty;
        [ObservableProperty] private string _SystemLogStoragePeriod = string.Empty;
        #endregion
    }
}
