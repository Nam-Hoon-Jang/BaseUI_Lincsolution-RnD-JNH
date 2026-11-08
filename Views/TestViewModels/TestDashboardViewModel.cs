using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BaseUI.Function;

namespace BaseUI
{
    public class TestDashboardViewModel : DashboardViewModel
    {
        /// <summary>
        /// WPF 디자인 뷰에서만 호출되는 생성자
        /// </summary>
        public TestDashboardViewModel()
            : base(new DashboardModel(), new DummySharedModel(), new DummyNavigationService())
        {
            DesignTimeInitialise();
        }

        /// <summary>
        /// 디버거 기호 중 TESTMODE 활성 시 호출되는 생성자
        /// </summary>
        public TestDashboardViewModel(DashboardModel m, ISharedModel sm, INavigationService nav) 
            : base(m, sm, nav)
        {
            if(DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
            {
                return;
            }
            Debug.WriteLine($"[TestDashboardViewModel] 생성됨 : {this.GetHashCode()}");
        }

        protected override void Initialise()
        {
            TestModeTimeTask();
        }

        /// <summary>
        /// 디자인 뷰에서만 호출되는 초기화 함수
        /// </summary>
        private void DesignTimeInitialise()
        {
            TimeSpan t1 = new TimeSpan();
            DateTime nowDate = DateTime.Now;
            StartDate = nowDate.ToString("yyyy-MM-dd HH:mm:ss");
            TimeSpan bte = new TimeSpan(1, 0, 0);
            EndDate = nowDate.Add(bte).ToString("yyyy-MM-dd HH:mm:ss");
            decimal currentHeight = 0.0m;
            TotalLayers = 30;
            CurrentLayer = 0;
            Layers = $"{CurrentLayer} / {TotalLayers}";
            Task.Run(async () =>
            {
                while(true)
                {
                    t1 = t1.Add(TimeSpan.FromSeconds(1));
                    ElapsedTime = TotalRunningTime = t1.ToString(@"hh\:mm\:ss");
                    RemainingTime = bte.Subtract(t1).ToString(@"hh\:mm\:ss");
                    currentHeight += 0.1m;
                    CurrentHeight = currentHeight.ToString();
                    CurrentLayer++;
                    Layers = $"{CurrentLayer} / {TotalLayers}";
                    if(CurrentLayer >= TotalLayers)
                    {
                        CurrentLayer = 0;
                        currentHeight = 0.0m;
                    }
                    BuildProgressBarValue = (double)CurrentLayer / TotalLayers * 100;
                    await Task.Delay(1000);
                }
            });

            BuildFileList.Clear();
            for(int i = 0; i < 32; i++)
            {
                BuildFileList.Add(new BuildFileListViewData
                {
                    IsChecked = false,
                    FileName = $"Item{i}",
                    UploadDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    BtnPreviewStatus = (i % 2 == 0) ? BaseEnum.ComponentStatus.Enable : BaseEnum.ComponentStatus.Disable,
                    BtnBuildStartStatus = (i % 2 == 1) ? BaseEnum.ComponentStatus.Enable : BaseEnum.ComponentStatus.Disable
                });
            }
        }

        private async Task TestModeTimeTask()
        {
            BuildFileList.Clear();
            for(int i = 0; i < 32; i++)
            {
                BuildFileList.Add(new BuildFileListViewData
                {
                    IsChecked = false,
                    FileName = $"Item{i}",
                    UploadDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    BtnPreviewStatus = (i % 2 == 0) ? BaseEnum.ComponentStatus.Enable : BaseEnum.ComponentStatus.Disable,
                    BtnBuildStartStatus = (i % 2 == 1) ? BaseEnum.ComponentStatus.Enable : BaseEnum.ComponentStatus.Disable
                });
            }
            using(var tm = new TaskManager())
            {
                tm.Start(async token =>
                {
                    while(true)
                    {
                        Shared.ExpectedTotalRunningTime = Shared.ExpectedTotalRunningTime.Add(TimeSpan.FromSeconds(1));
                        if(Shared.BuildProgressValue >= 100)
                        {
                            Shared.BuildProgressValue = 0;
                        }
                        else
                        {
                            Shared.BuildProgressValue += 10;
                        }
                        await Task.Delay(1000);
                    }
                }, 5000);
                await tm.RunTask;
            }
        }
    }
}
