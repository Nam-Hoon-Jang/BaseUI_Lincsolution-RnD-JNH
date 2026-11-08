using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace BaseUI
{
    /// <summary>
    /// ViewModel을 캐시 메모리에 캐싱하여, Load 시에 중복생성을 방지할 수 있는 Loader 클래스
    /// View <=> View 간 전환 시에 ViewModel 간 전환을 쉽게 만들고, 중복생성을 방지.
    /// </summary>
    public static class ViewModelLoader
    {
        /// <summary>
        /// 현재 UI 에 들어있는 모든 실사용 ViewModel 과 TESTMODE 용 ViewModel 을 엮은 Dictionary
        /// 여기 없으면 key 및 value 가 없으므로 TestViewModel Load 불가능.
        /// </summary>
        private static readonly Dictionary<Type, Type> _testTypeMap = new()
        {
            { typeof(MainViewModel),            typeof(TestMainViewModel) },
            { typeof(DashboardViewModel),       typeof(TestDashboardViewModel) },
            //{ typeof(JobViewModel),             typeof(TestJobViewModel) },
            { typeof(MonitoringViewModel),      typeof(TestMonitoringViewModel) },
            //{ typeof(ControlViewModel),         typeof(TestControlViewModel) },
            //{ typeof(LogListViewModel),         typeof(TestLogListViewModel) },
            //{ typeof(BuildSettingsViewModel),   typeof(TestBuildSettingsViewModel) },
            { typeof(PreferenceViewModel),      typeof(TestPreferenceViewModel) },
            //
            //{typeof(SubJobJobsViewModel) , typeof(TestSubJobJobsViewModel) },
            //{typeof(SubJobJobListViewModel), typeof(TestSubJobJobListViewModel) },
            //{typeof(SubJobCameraViewModel), typeof(TestSubJobCameraViewModel) },
            //
            //{ typeof(SubMonitoringMotionViewModel), typeof(TestSubMonitoringMotionViewModel) },
            //{ typeof(SubMonitoringOpticViewModel), typeof(TestSubMonitoringOpticViewModel) },
            //{ typeof(SubMonitoringVatViewModel), typeof(TestSubMonitoringVatViewModel) },
            //{ typeof(SubControlMotionViewModel), typeof(TestSubControlMotionViewModel) },
            //{ typeof(SubControlOpticViewModel),  typeof(TestSubControlOpticViewModel) },
            //{ typeof(SubControlVatViewModel),    typeof(TestSubControlVatViewModel) },
            //{ typeof(SubLogJobLogListViewModel),    typeof(TestSubLogJobLogListViewModel) },
            //{ typeof(SubLogSystemLogListViewModel),    typeof(TestSubLogSystemLogListViewModel) },
            //{ typeof(SubBuildSettingsBuildViewModel),       typeof(TestSubBuildSettingsBuildViewModel) },
            //{ typeof(SubBuildSettingsFunctionViewModel),    typeof(TestSubBuildSettingsFunctionViewModel) },
            //
            //{ typeof(SubPreferenceViewModel),   typeof(TestSubPreferenceViewModel) },
            //{ typeof(SubSettingsViewModel),     typeof(TestSubSettingsViewModel) },
        };
        /// <summary>
        /// 중복생성 방지용 캐시 메모리 
        /// </summary>
        private static readonly Dictionary<Type, object> _cache = new();
        /// <summary>
        /// 캐시 데이터에 캐싱하는 함수
        /// </summary>
        /// <param name="type">ViewModel type 입력 => typeof(ViewModel)</param>
        /// <param name="instance">캐싱할 ViewModel을 new로 초기화하여 생성하고 입력</param>
        public static void Cache(Type type, object instance)
        {
            _cache[type] = instance;
        }
        /// <summary>
        /// 캐시 메모리에서 ViewModel 삭제
        /// </summary>
        /// <typeparam name="T">캐시 메모리에서 지울 ViewModel</typeparam>
        public static void Remove<T>() where T : class => _cache.Remove(typeof(T));

        /// <summary>
        /// 캐시 메모리 초기화
        /// </summary>
        public static void ClearAll() => _cache.Clear();

        /// <summary>
        /// Load 를 사용하면, Cache 에 등록되어있는 ViewModel을 가져오며, 중복생성이 방지되어있음.
        /// </summary>
        /// <typeparam name="T">Load 하려는 ViewModel 클래스 입력</typeparam>
        /// <param name="provider">찾고자 하는 ServiceProvider => UI 전체에서는, App.xaml.cs 내부에 있는 Service 가 여기에 해당함.</param>
        /// <returns>T에 해당하는 ViewModel 반환.</returns>
        /// <exception cref="InvalidOperationException">Caching 되어 있지 않은 ViewModel 을 부를 때 발생.</exception>
        public static T Load<T>(IServiceProvider provider) where T : class
        {
            var key = typeof(T);
            if(_cache.TryGetValue(key, out var cached))
            {
                return (T)cached;
            }

#if TESTMODE
            /// DesignMode 인지 구분 => TESTMODE 는 런타임 테스트와 디자인뷰 테스트 둘 다 가능하게 구현되었음.
            /// 모든 TestViewModel은, DesignTime용 생성자와 런타임용 생성자가 별도로 있으며, IsInDesignMode()는 이를 확인하는 용도.
            if(IsInDesignMode())
            {
                var testTypeName = typeof(T).FullName?.Replace("ViewModel", "TestViewModel");
                var testType = Type.GetType(testTypeName ?? "");
                if(testType != null)
                {
                    var testInstance = Activator.CreateInstance(testType);
                    _cache[key] = testInstance!;
                    return (T)testInstance!;
                }
            }
            else
            {
                //테스트모드 런타임
                if(_testTypeMap.TryGetValue(key, out var testType))
                {
                    var testInstance = provider.GetRequiredService(testType);
                    _cache[key] = testInstance!;
                    return (T)(object)testInstance!;
                }
            }
#endif
            /// TESTMODE 가 아닐 때 진입하는 실사용 Load
            try
            {
                var instance = provider.GetRequiredService<T>();
                _cache[key] = instance!;
                return instance!;
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException($"❌ ViewModelLoader: {key.Name} 생성 실패 → 반드시 App.xaml.cs에서 Cache 필요함\n{ex.Message}", ex);
            }
        }
        /// <summary>
        /// 현재 디버그 상태가 디자인모드인지, 런타임모드인지 구분하는 함수.
        /// WPF 디자이너는 빌드를 통해 디자이너에서 일부 기능들이 실행될 수 있음
        /// </summary>
        /// <returns>디자인모드면 true, 아니면 false</returns>
        private static bool IsInDesignMode()
        {
            return DesignerProperties.GetIsInDesignMode(new DependencyObject());
        }
    }
}
