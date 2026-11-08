using BaseUI;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Button = System.Windows.Controls.Button;
using Forms = System.Windows.Forms;

namespace BaseUI
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            this.Loaded += (_, _) =>
            {
                if(this.DataContext is IMainViewEvents vm) vm.OnLoaded();
                AlarmListView.ButtonClickedEvent += OnAlarmListViewButtonClick;
            };
        }

        private void WindowDrag(object sender, MouseButtonEventArgs e)
        {
            if(e.ClickCount >= 2)
            {
                WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
                return;
            }
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                if(WindowState == WindowState.Maximized)
                {
                    ResotreFromMaximized(e);
                }
                else
                {
                    DragMove();
                }
            }
        }

        private void ResotreFromMaximized(MouseButtonEventArgs e)
        {
            var mouseX = e.GetPosition(this).X;
            var widthRatio = mouseX / ActualWidth;
            WindowState = WindowState.Normal;

            Left = Forms.Cursor.Position.X - (RestoreBounds.Width * widthRatio);
            Top = 0;

            DragMove();
        }

        private void MinimizeWindow(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            WindowState = WindowState.Minimized;
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            Close();
        }

        private void AnimateMenuColumn(double from, double to, double seconds = 0.25)
        {
            var animation = new GridResizeAnimation
            {
                From = new GridLength(from, GridUnitType.Pixel),
                To = new GridLength(to, GridUnitType.Pixel),
                Duration = TimeSpan.FromSeconds(seconds),
                FillBehavior = System.Windows.Media.Animation.FillBehavior.HoldEnd
            };

            ResizeableMainColumn.BeginAnimation(ColumnDefinition.WidthProperty, animation);
        }

        private void AnimateViewWidth(double from, double to, double seconds = 0.25)
        {
            var animation = new DoubleAnimation
            {
                From = from,
                To = to,
                Duration = TimeSpan.FromSeconds(seconds),
                FillBehavior = System.Windows.Media.Animation.FillBehavior.HoldEnd
            };

            BtnDashboard.BeginAnimation(FrameworkElement.WidthProperty, animation);
            BtnBuild.BeginAnimation(FrameworkElement.WidthProperty, animation);
            BtnMonitoring.BeginAnimation(FrameworkElement.WidthProperty, animation);
            BtnControl.BeginAnimation(FrameworkElement.WidthProperty, animation);
            BtnLogList.BeginAnimation(FrameworkElement.WidthProperty, animation);
            BtnBuildParameter.BeginAnimation(FrameworkElement.WidthProperty, animation);
            BtnPreference.BeginAnimation(FrameworkElement.WidthProperty, animation);
        }

        private const double _menuCollapsedWidth = 88;
        private const double _menuExpandedWidth = 280;
        private bool _isMenuCollapsed = false;
        private void BtnExpand_Click(object sender, RoutedEventArgs e)
        {
            if(_isMenuCollapsed)
            {
                AnimateMenuColumn(_menuCollapsedWidth, _menuExpandedWidth);
                AnimateViewWidth(_menuCollapsedWidth, _menuExpandedWidth);
                Logo.Data = Function.GetResourceGeometry("GeometryLincsolution");
                _isMenuCollapsed = false;
            }
            else
            {
                AnimateMenuColumn(_menuExpandedWidth, _menuCollapsedWidth);
                AnimateViewWidth(_menuExpandedWidth, _menuCollapsedWidth);
                Logo.Data = Function.GetResourceGeometry("GeometryLogoOnly");
                _isMenuCollapsed = true;
            }
            if(this.DataContext is IMainViewEvents vm)
            {
                vm.HandleMenuExpanded(_isMenuCollapsed);
            }
        }

        private void OnAlarmListViewButtonClick(object sender, int index)
        {
            if(this.DataContext is IMainViewEvents vm)
            {
                vm.HandleAlarmListViewButtonClicked(index);
            }
        }
    }
}