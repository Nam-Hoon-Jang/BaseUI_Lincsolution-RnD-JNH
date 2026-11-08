using BaseUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BaseUI.gEnum;

namespace BaseUI
{
    public partial class MonitoringViewModel : BaseViewModel<MonitoringModel, ISharedModel, MonitoringViewCommand>
    {
        public MonitoringViewModel(MonitoringModel m, ISharedModel sm, INavigationService nav)
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
        /// MonitoringView 처음 생성 시 호출되는 초기화 함수.
        /// </summary>
        protected override void Initialise()
        {
            
        }

        /// <summary>
        /// 버튼 입력 시 동작 처리 함수
        /// </summary>
        /// <param name="cmd">버튼별 커맨드 파리미터</param>
        protected override void Execute(MonitoringViewCommand param)
        {
            
        }

        /// <summary>
        /// SharedModel 에 있는 공통 데이터값과 이 ViewModel에서 UI에 Binding되어있는 변수 맵핑
        /// key = SharedModel의 Property 이름, value(Action) = 할당하는 Action
        /// </summary>
        protected override void PropertiesMapping()
        {
            
        }
    }
}
