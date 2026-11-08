using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseUI
{
    public class TestMonitoringViewModel : MonitoringViewModel
    {
        /// <summary>
        /// WPF 디자인 뷰에서만 호출되는 생성자
        /// </summary>
        public TestMonitoringViewModel()
            : base(new MonitoringModel(), new DummySharedModel(), new DummyNavigationService())
        {
            DesignTimeInitialise();
        }

        public TestMonitoringViewModel(MonitoringModel m, ISharedModel sm, INavigationService nav)
            : base(m, sm, nav)
        {
            if(DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
            {
                return;
            }
            Debug.WriteLine($"[TestMonitoringViewModel] 생성됨 : {this.GetHashCode()}");
        }

        /// <summary>
        /// 디자인 뷰에서만 호출되는 초기화 함수
        /// </summary>
        private void DesignTimeInitialise()
        {
            
        }
    }
}
