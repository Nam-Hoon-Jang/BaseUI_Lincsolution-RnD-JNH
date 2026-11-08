using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using System.Windows.Input;
using DependencyObject = System.Windows.DependencyObject;


/////////////////////////////////////////////////////////////////////////////////////////////
///                                                                                       ///
///     MVVM Pattern 에 대한 간략한 설명                                                  /// 
///                                                                                       /// 
///     MVVM = Model - VIew - ViewModel 의 약어로,                                        /// 
///     Model 과 View의 상호작용 사이의 연결점으로 ViewModel 을 사용하는 패턴을 의미함.   /// 
///     ViewModel 을 연결점으로 사용하여, Model 과 View 사이의 의존성을 줄여,             /// 
///     이후의 View 재사용성과 안정성을 높이는 방법                                       /// 
///     Model 에서 에러가 발생해도, ViewModel 을 통해 간접적으로 연결되므로,              /// 
///     ViewModel 에서 에러가 발생하지 않도록 방지하는 것으로 안정성을 높이고,            /// 
///     Model 에서 사용되는 서비스 로직이 변경될 때,                                      /// 
///     Model 만을 재설계하는 것으로, View에 대한 직접적인 수정이 필요없어짐.             /// 
///     이를 통해 상호의존성을 줄여주어, 재사용이 좋아짐.                                 ///
///                                                                                       ///
///     일반적으로, View 에는 절대 변하지 않는 데이터만 직접 컨트롤에 Value를 할당하고,   ///
///     변수들은 ViewModel 로부터 Binding 받아서 연결하고,                                ///
///     ViewModel의 변수들은 값 변경시 자동으로 View에 반영될 수 있도록                   ///
///     set 시에 PropertyChanged 이벤트를 발생시킬 수 있어야 함.                          ///
///     그리고 Model은, View 에서 보여줄 변수 데이터를 직접 처리하고,                     ///
///     처리된 데이터를 ViewModel에 연동하고, 값 변경에 대한 이벤트를 발생시켜야 함.      ///
///                                                                                       ///
///     이를 통해, 복잡한 연산이 필요한 서비스들은 모두 Model 에서 실행되고,              ///
///     ViewModel은 View에서 보여주는 변수들과 View의 디자인만 관리하며,                  ///
///     View는 유저와의 상호작용만을 함.                                                  ///
///     이를 통해 역할 분리과 되어, 각 파트의 수정과 관리가 간편해지고,                   ///
///     복잡한 서비스 로직의 변경이 필요할 때, Model 만을 수정하거나,                     ///
///     해당 모델의 복사본을 두고 연동하면서 테스트하기가 용이함.                         ///
///                                                                                       /// 
/////////////////////////////////////////////////////////////////////////////////////////////

namespace BaseUI
{
    /// <summary>
    /// 공유 데이터 없이, Command 구별만 필요한 View 에서 사용할 ViewModel
    /// </summary>
    /// <typeparam name="TEnum">Command Parameter 구분용 Enum 변수</typeparam>
    public abstract class BaseViewModel<TEnum> : ObservableValidator, INavigationNotice where TEnum : Enum
    {
        /// <summary>UI에서 User 상호작용 시 실행되는 명령 인터페이스. BaseViewModel 생성 시에 RelayCommand로 자동 초기화. 대부분의 ViewModel에서 반드시 사용</summary>
        public ICommand Command { get; }

        /// <summary>View.xaml 파일의 Design View 에서 사용할 ViewModel. 실제 Runtime이 아닌, WPF Design View 에서만 반영되는 ViewModel</summary>
        protected virtual object? DesignTimeViewModel { get; set; } = null;

        /// <summary>ViewModel 간 화면 전환에 사용하는 인터페이스. => NavigationService 를 사용하며, 화면 전환이 필요한 ViewModel 에서만 사용.</summary>
        protected INavigationService? Navigation { get; set; }

        /// <summary>
        /// 실제 프로그램이 실행되는 Runtime 에서 사용되는 ViewModel. 외부에서 get 할 때, NavigationService 클래스 인스턴스에서 CurrentViewModel을 가져오고, 
        /// 값이 null 이라면, 자동으로 DesignTimeViewModel을 반환.
        /// </summary>
        public object? CurrentViewModel
        {
            get
            {
                var current = Navigation?.CurrentViewModel;
                return ReferenceEquals(current, this) ? null : current ?? DesignTimeViewModel;
            }
        }

        /// <summary>BaseViewModel 생성함수.</summary>
        /// <param name="messenger">ViewModel 들 간의 간략한 데이터 전달용으로 사용되는 메신저 인터페이스</param>
        protected BaseViewModel() 
        {
            /// RelayCommand 에 Execute 등록하면서 초기화
            Command = new RelayCommand<TEnum>(Execute);
            /// 초기화
            Initialise();
        }

        /// <summary>ViewModel 초기화 시 필요한 내용들 넣을 함수. => 여기서는 선언만 해두고, 수현은 상속 클래스에서.</summary>
        protected abstract void Initialise();

        /// <summary> View <-> ViewModel 간 상호작용 시 적용되는 RelayCommand. => 여기서는 선언만 해두고, 구현은 상속 클래스에서.</summary>
        /// <param name="param">Enum 타입으로 각 ViewModel 에서 사용할 명령 구분.</param>
        protected abstract void Execute(TEnum param);

        public virtual void OnNavigatedTo(object? parameter = null) { }

        public virtual void OnNavigatedFrom() { }
    }

    /// <summary>
    /// 공통적으로 사용되는 공유 데이터가 필요한 View 에서 사용할 ViewModel
    /// </summary>
    /// <typeparam name="TShared">공통 데이터 모델</typeparam>
    /// <typeparam name="TEnum">Command Parameter 구분용 Enum</typeparam>
    public abstract partial class BaseViewModel<TShared, TEnum> : ObservableValidator, INavigationNotice
    where TShared : class
    where TEnum : Enum
    {
        /// <summary>UI에서 User 상호작용 시 실행되는 명령 인터페이스. BaseViewModel 생성 시에 RelayCommand로 자동 초기화. 대부분의 ViewModel에서 반드시 사용</summary>
        public ICommand Command { get; }

        /// <summary>View.xaml 파일의 Design View 에서 사용할 ViewModel. 실제 Runtime이 아닌, WPF Design View 에서만 반영되는 ViewModel</summary>
        protected virtual object? DesignTimeViewModel { get; set; } = null;

        /// <summary>ViewModel 간 화면 전환에 사용하는 인터페이스. => NavigationService 를 사용하며, 화면 전환이 필요한 ViewModel 에서만 사용.</summary>
        protected INavigationService? Navigation { get; set; }

        /// <summary>
        /// 실제 프로그램이 실행되는 Runtime 에서 사용되는 ViewModel. 외부에서 get 할 때, NavigationService 클래스 인스턴스에서 CurrentViewModel을 가져오고, 
        /// 값이 null 이라면, 자동으로 DesignTimeViewModel을 반환.
        /// </summary>
        public object? CurrentViewModel
        {
            get
            {
                var current = Navigation?.CurrentViewModel;
                return ReferenceEquals(current, this) ? null : current ?? DesignTimeViewModel;
            }
        }

        /// <summary>ViewModel 들이 공통적으로 조회하는 데이터 처리를 위한 공유 모델</summary>
        protected TShared Shared;

        /// <summary>
        /// 위 Shared 의 데이터랑 연동해서 자동으로 갱신할 Property들 맵핑용 Dictionary
        /// 맵핑 데이터 구조는, MCPData.json 이나, PLCData.json 에 있는 파일의 Name 데이터를 key 값으로 두어야 함.
        /// 그리고, 호출 시에 자동으로 ViewModel 에 있는 UI 와 바인딩된 데이터를 갱신해주는 로직을 Action 으로 묶어서 적용.
        /// 한 개의 요소 구조는, [key, Action(데이터 갱신 로직)] 형태로 생성.
        /// </summary>
        protected Dictionary<string, Action> _sharedModelPropertyMap;

        /// <summary>현재 실행 중인 프로세스가 디자인타임뷰 인지 판단하는 변수</summary>
        protected static bool IsDesignMode => DesignerProperties.GetIsInDesignMode(new DependencyObject());

        /// <summary>
        /// BaseViewModel 생성함수.
        /// </summary>
        /// <param name="shared">ViewModel 에서 대채로 많이 사용되는 데이터를 위한 공유 모델</param>
        /// <param name="messenger">ViewModel 들 간의 간략한 데이터 전달용으로 사용되는 메신저 인터페이스</param>
        protected BaseViewModel(TShared shared)
        {
            /// RelayCommand 에 Execute 등록하면서 초기화
            Command = new RelayCommand<TEnum>(Execute);
            /// SharedModel 할당 => Dependecy Injection 형식으로 할당해서, View /  ViewModel 간 의존성 낮추기
            this.Shared = shared;
            /// 초기화
            Initialise();
            /// 디자인타임뷰 모드라면, 실제 TESTMODE 또는 RUNTIME 용의 PropertiesMapping 실행하지 않음.
            if(IsDesignMode)
            {
                DesignTimePropertiesMapping();
            }
            else
            {
                /// 실제 사용하는 내부 변수 데이터 맵핑
                PropertiesMapping();
            }
        }

        /// <summary>ViewModel 초기화 시 필요한 내용들 넣을 함수. => 여기서는 선언만 해두고, 수현은 상속 클래스에서.</summary>
        protected abstract void Initialise();

        /// <summary> View <-> ViewModel 간 상호작용 시 적용되는 RelayCommand. => 여기서는 선언만 해두고, 구현은 상속 클래스에서.</summary>
        /// <param name="param">Enum 타입으로 각 ViewModel 에서 사용할 명령 구분.</param>
        protected abstract void Execute(TEnum param);

        /// <summary>
        /// _sharedModelPropertyMap 맵핑해주기 위한 함수 => 여기서는 선언만 해두고, 구현은 상속 클래스에서.
        /// 이 함수에서 _sharedModelPropertyMap 초기화 하면서 데이터 맵핑하고, 
        /// Shared.PropertyChanged 에 하단에 있는 PropertiesChanged 추가하도록 설계해야 함.
        /// </summary>
        protected abstract void PropertiesMapping();

        /// <summary>
        /// Shared에 있는 Property 값 변경 이벤트 등록용. PropertiesMapping 에서 이 이벤트 발생할 때 변경할 값들 별도로 등록.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void PropertiesChanged(object? sender, PropertyChangedEventArgs e)
        {
            if(_sharedModelPropertyMap.TryGetValue(e.PropertyName, out Action? setter))
            {
                setter();
            }
        }
        /// <summary>WPF xaml Design View 에서 보여지는 디자인타임 때 사용할 PropertiesMapping</summary>
        protected virtual void DesignTimePropertiesMapping() { }

        public virtual void OnNavigatedTo(object? parameter = null) { }

        public virtual void OnNavigatedFrom() { }
    }

    /// <summary>
    /// 해당 View 에서만 특수하게 사용되는 기능이 있을 때 사용하는 ViewModel
    /// </summary>
    /// <typeparam name="TModel">특수기능 및 데이터 처리용 모델 클래스</typeparam>
    /// <typeparam name="TShared">공통 공유 데이터 처리용 모델</typeparam>
    /// <typeparam name="TEnum">Command Parameter 구분용 Enum</typeparam>
    public abstract class BaseViewModel<TModel, TShared, TEnum> : ObservableValidator, INavigationNotice
    where TModel : ObservableObject, IModelWithSharedModel<TShared>
    where TShared : class
    where TEnum : Enum
    {
        /// <summary>UI에서 User 상호작용 시 실행되는 명령 인터페이스. BaseViewModel 생성 시에 RelayCommand로 자동 초기화. 대부분의 ViewModel에서 반드시 사용</summary>
        public ICommand Command { get; }

        /// <summary>View.xaml 파일의 Design View 에서 사용할 ViewModel. 실제 Runtime이 아닌, WPF Design View 에서만 반영되는 ViewModel</summary>
        protected virtual object? DesignTimeViewModel { get; set; } = null;

        /// <summary>ViewModel 간 화면 전환에 사용하는 인터페이스. => NavigationService 를 사용하며, 화면 전환이 필요한 ViewModel 에서만 사용.</summary>
        protected INavigationService? Navigation { get; set; }

        /// <summary>
        /// 실제 프로그램이 실행되는 Runtime 에서 사용되는 ViewModel. 외부에서 get 할 때, NavigationService 클래스 인스턴스에서 CurrentViewModel을 가져오고, 
        /// 값이 null 이라면, 자동으로 DesignTimeViewModel을 반환.
        /// </summary>
        public object? CurrentViewModel
        {
            get
            {
                var current = Navigation?.CurrentViewModel;
                return ReferenceEquals(current, this) ? null : current ?? DesignTimeViewModel;
            }
        }

        /// <summary>ViewModel 들이 공통적으로 조회하는 데이터 처리를 위한 공유 모델</summary>
        protected TShared Shared { get; }

        /// <summary>
        /// 위 Shared 의 데이터랑 연동해서 자동으로 갱신할 Property들 맵핑용 Dictionary
        /// 맵핑 데이터 구조는, MCPData.json 이나, PLCData.json 에 있는 파일의 Name 데이터를 key 값으로 두어야 함.
        /// 그리고, 호출 시에 자동으로 ViewModel 에 있는 UI 와 바인딩된 데이터를 갱신해주는 로직을 Action 으로 묶어서 적용.
        /// 한 개의 요소 구조는, [key, Action(데이터 갱신 로직)] 형태로 생성.
        /// </summary>
        protected Dictionary<string, Action> _sharedModelPropertyMap;

        /// <summary>공통 공유 데이터가 아닌, 별개의 ViewModel 에서만 거의 사용되는 데이터를 처리하는 클래스</summary>
        protected TModel Model { get; }

        /// <summary>현재 실행 중인 프로세스가 디자인타임뷰 인지 판단하는 변수</summary>
        protected static bool IsDesignMode => DesignerProperties.GetIsInDesignMode(new DependencyObject());

        /// <summary>BaseViewModel 생성함수</summary>
        /// <param name="model">ViewModel 단독으로 데이터 처리하는 클래스</param>
        /// <param name="shared">ViewModel 에서 대채로 많이 사용되는 데이터를 위한 공유 모델</param>
        /// <param name="messenger">ViewModel 들 간의 간략한 데이터 전달용으로 사용되는 메신저 인터페이스</param>
        protected BaseViewModel(TModel model, TShared shared)
        {
            /// Model에 외부에서 Model 주입.
            Model = model;
            /// Model에 SharedModel 주입.
            Model.AttachShared(shared);
            /// RelayCommand 에 Execute 등록하면서 초기화
            Command = new RelayCommand<TEnum>(Execute);
            /// SharedModel 할당 => Dependecy Injection 형식으로 할당해서, View /  ViewModel 간 의존성 낮추기
            this.Shared = shared;
            
            /// 디자인타임뷰 모드라면, 실제 TESTMODE 또는 RUNTIME 용의 PropertiesMapping 실행하지 않음.
            if(IsDesignMode)
            {
                DesignTimePropertiesMapping();
            }
            else
            {
                /// 초기화
                Initialise();
                /// 실제 사용하는 내부 변수 데이터 맵핑
                PropertiesMapping();
            }
            /// Model의 PropertyChanged 이벤트 발생할 때 실행할 이벤트 핸들러 등록
            Model.PropertyChanged += OnModelPropertyChanged;
        }

        /// <summary>ViewModel 초기화 시 필요한 내용들 넣을 함수. => 여기서는 선언만 해두고, 수현은 상속 클래스에서.</summary>
        protected abstract void Initialise();

        /// <summary> View <-> ViewModel 간 상호작용 시 적용되는 RelayCommand. => 여기서는 선언만 해두고, 구현은 상속 클래스에서.</summary>
        /// <param name="param">Enum 타입으로 각 ViewModel 에서 사용할 명령 구분.</param>
        protected abstract void Execute(TEnum param);

        /// <summary>
        /// _sharedModelPropertyMap 맵핑해주기 위한 함수 => 여기서는 선언만 해두고, 구현은 상속 클래스에서.
        /// 이 함수에서 _sharedModelPropertyMap 초기화 하면서 데이터 맵핑하고, 
        /// Shared.PropertyChanged 에 하단에 있는 PropertiesChanged 추가하도록 설계해야 함.
        /// </summary>
        protected abstract void PropertiesMapping();

        /// <summary>
        /// Shared에 있는 Property 값 변경 이벤트 등록용. PropertiesMapping 에서 이 이벤트 발생할 때 변경할 값들 별도로 등록.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void PropertiesChanged(object? sender, PropertyChangedEventArgs e)
        {
            if(_sharedModelPropertyMap.TryGetValue(e.PropertyName, out Action? setter))
            {
                setter();
            }
        }

        protected virtual void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // override로 처리할 것
        }
        /// <summary>WPF xaml Design View 에서 보여지는 디자인타임 때 사용할 PropertiesMapping</summary>
        protected virtual void DesignTimePropertiesMapping() { }

        public virtual void OnNavigatedTo(object? parameter = null) { }

        public virtual void OnNavigatedFrom() { }
    }
}
