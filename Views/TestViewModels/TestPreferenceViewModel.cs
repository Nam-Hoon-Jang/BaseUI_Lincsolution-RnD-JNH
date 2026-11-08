using static BaseUI.gEnum;
using static BaseUI.gProperty;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BaseUI
{
    public class TestPreferenceViewModel : PreferenceViewModel
    {
        /// <summary>
        /// WPF 디자인 뷰에서만 호출되는 생성자
        /// </summary>
        public TestPreferenceViewModel() 
            : base(new PreferenceModel(), new DummySharedModel(), new DummyNavigationService())
        {
            DesignTimeInitialise();
        }

        /// <summary>
        /// 디버거 기호 중 TESTMODE 활성 시 호출되는 생성자
        /// </summary>
        public TestPreferenceViewModel(PreferenceModel m, ISharedModel sm, INavigationService nav)
            : base(m, sm, nav)
        {
            if(DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
            {
                return;
            }
            Debug.WriteLine($"[TestPreferenceViewModel] 생성됨 : {this.GetHashCode()}");
        }

        /// <summary>
        /// 디자인 뷰에서만 호출되는 초기화 함수
        /// </summary>
        private void DesignTimeInitialise()
        {
            var assm = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion.ToString();
            SoftwareVersion = assm;
            LanguageListSource = new List<string>()
            {
                TEXT.LANG.English,
                TEXT.LANG.Korean
            };
            SelectedLanguage = TEXT.LANG_CODE.English;
            BuildFileDir = "C:\\Demo\\data";
            DesignTimeTask();
        }

        private async Task DesignTimeTask()
        {
            using(var tm = new TaskManager())
            {
                tm.Start(async token =>
                {
                    while(true)
                    {
                        SystemDateTime = DateTime.Now;
                        await Task.Delay(1000);
                    }
                }, 5000);
                await tm.RunTask;
            }
        }
    }
}
