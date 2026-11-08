using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using static BaseUI.gProperty;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace BaseUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>중복 실행 방지용 Mutex</summary>
        private Mutex mutex;

        /// <summary>UI의 의존성 주입을 위한 서비스 제공자</summary>
        public static IServiceProvider Services
        {
            get; private set;
        }

        /// <summary>
        /// Application 실행 시, 가장 처음 실행되는 함수
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplicationStart(object sender, StartupEventArgs e)
        {
            try
            {
                mutex = new Mutex(false, "Lincsolution UI");
            }
            /// Exception 발생 시, 현재 App 강제종료
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Shutdown();
            }
            /// 중복 실행 확인 시, 현재 App 강제종료
            if(!mutex.WaitOne(0, false))
            {
                MessageBox.Show("Application Duplicated -- Shut Down");
                Application.Current.Shutdown();
            }

            /// UI 최초 부팅 시 실행할 메소드
            gFunc.RunAtStartUp();

            /// UI 강제종료 시 기록할 로그 연동
            this.DispatcherUnhandledException += (s, ex) =>
            {
                FatalLogger.Write($"DispatcherUnhandledException : {ex.Exception}", Logger.Case.Fatal);
            };
            AppDomain.CurrentDomain.UnhandledException += (s, ex) =>
            {
                FatalLogger.Write($"UnhandledException : {ex.ExceptionObject as Exception}", Logger.Case.Fatal);
            };
            TaskScheduler.UnobservedTaskException += (s, ex) =>
            {
                FatalLogger.Write($"UnobservedTaskException : {ex.Exception}");
                ex.SetObserved();
            };

            /// 현재까지 등록한 ServiceCollection을 ServiceProvider 로 생성하고, 맨 처음 만들었던 IServiceProvider 에 할당.
            Services = CreateServiceCollections().BuildServiceProvider();

            /// Services 에 등록한 UI 요소들 캐싱.
            CacheOnViewModelLoader();

            /// MainView는 무언가의 하위 View 가 아니므로, ViewModelLoader.Load 로 직접 ViewModel 로드
            var mainVm = ViewModelLoader.Load<MainViewModel>(Services);
            /// 로딩한 ViewModel을 이제부터 Show 할 MainView의 DataContext에 할당.
            var mainView = new MainView
            {
                DataContext = mainVm
            };
            SystemLogger.Write("UI Booting Completed.");
            /// MainView Show
            mainView.Show();
        }

        /// <summary>
        /// IServiceProvder Services 에 의존성 주입하는 함수.
        /// 모든 VIew는 VIewModel과 연결되고, 모든 Viewmodel은 Model과 연결.
        /// ViewModel은 View에서 Display에 필요한 모든 변수들(글자, 색상, 버튼상태, 요소의 Visibility, 이미지 등)의 값을 통제.
        /// ViewModel에서 관리하는 변수들은, View를 '표현'하는 데 사용되는 변수들만 직접 관리.
        /// View의 DataContext는 ViewModel과 바인딩되며, ViewModel의 원본값 변경 시, 즉시 View에 반영.
        /// Model은 ViewModel에서 View에 바인딩하는 변수들 중, 외부에서 가져오는 요소거나, 복잡한 계산식 등이 보여주기위해
        /// 선제조건이 필요한 값들을 관리하는 클래스. ex) 센서값, 3D 파일 모델링, 카메라 데이터 등.
        /// ViewModel 은 그저 View에 보여주는 것만 담당하며, 모든 원본 데이터들은 Model에서 제어.
        /// </summary>
        private ServiceCollection CreateServiceCollections()
        {
            var services = new ServiceCollection();

            /// SharedModel => 모든 ViewModel에서 공유하는 공유모델
            /// 이외 순서대로 MainView, JobView, MonitoringView, ControlView, LogListView, BuildSettingsView, PreferenceView 에서 사용하는 모델
            /// 하단 모델 전체를 서비스 컬렉션에 추가.
            /// SharedModel 은 모든 곳에서 공유되니, Singleton 으로 추가하고, 나머지는 Transient 로 추가

            services.AddSingleton<SharedModel>();
            services.AddSingleton<ISharedModel>(sp => sp.GetRequiredService<SharedModel>());
            services.AddTransient<MainModel>();

            services.AddTransient<DashboardModel>();
            services.AddTransient<MonitoringModel>();
            services.AddTransient<PreferenceModel>();

            /// 각 ViewModel 간 화면 전환을 위한 NavigationService 서비스 추가. 마찬가지로 공유하는 클래스이므로 Singleton으로 등록.
            services.AddSingleton<NavigationService>();
            /// NavigationService 서비스 클래스를 인터페이스인 INavigationService로 사용할 것이므로, Singleton으로 등록.
            services.AddSingleton<INavigationService, NavigationService>();


#if TESTMODE
            SystemLogger.Write("TESTMODE is ON");
            services.AddTransient<TestMainViewModel>();
            services.AddTransient<TestDashboardViewModel>();
            services.AddTransient<TestMonitoringViewModel>();            
            services.AddTransient<TestPreferenceViewModel>();
#else
            SystemLogger.Write("TESTMODE is OFF");
            services.AddTransient<MainViewModel>();
            services.AddTransient<DashboardViewModel>();
            services.AddTransient<MonitoringViewModel>();
            services.AddTransient<PreferenceViewModel>();
#endif
            return services;
        }

        /// <summary>
        /// Services에 등록한 Model 및 ViewModel 들을,
        /// ViewModelLoader 사용하여 캐싱. 중복 실행 방지 및 초기 화면 세팅.
        /// </summary>
        private void CacheOnViewModelLoader()
        {
            var mainNav = new NavigationService(Services);

            var dashboardNav  = new NavigationService(Services);
            var monitoringNav = new NavigationService(Services);
            var preferenceNav = new NavigationService(Services);

            ViewModelLoader.Load<DashboardViewModel>(Services);
            ViewModelLoader.Load<MonitoringViewModel>(Services);
            ViewModelLoader.Load<PreferenceViewModel>(Services);
        }
    }
}
