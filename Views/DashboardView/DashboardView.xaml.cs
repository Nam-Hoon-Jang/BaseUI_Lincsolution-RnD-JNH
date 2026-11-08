using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using UserControl = System.Windows.Controls.UserControl;

namespace BaseUI
{
    /// <summary>
    /// DashboardView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
            BuildFileList.ButtonClickedEvent += OnBuildFileListButtonClick;
        }

        public ISharedModel? _shared;

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if(DesignerProperties.GetIsInDesignMode(this))
                return;
            _shared ??= App.Services.GetRequiredService<ISharedModel>();
            _shared.PropertyChanged -= SharedPropertyChanged;
            _shared.PropertyChanged += SharedPropertyChanged;
            ApplyExpandedState();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if(_shared is not null)
                _shared.PropertyChanged -= SharedPropertyChanged;
        }

        private void OnBuildFileListButtonClick(object sender, int index)
        {
            if(this.DataContext is DashboardViewModel vm)
            {
                vm.HandleBuildFileListButtonClicked(index);
            }
        }

        private const double _menuCollapsedWidth = 1640;
        private const double _menuExpandedWidth = 1832;

        private const double _CollapsedLeftWidth = 635;
        private const double _ExpandedLeftWidth = 960;

        private const double _CollapsedMiddleWidth = 589;
        private const double _ExpandedMiddleWidth = 824;

        private const double _CollapsedRightWidth = 352;
        private const double _ExpandedRightWidth = 0;

        private const double _CollapsedResizableColumnWidth = 16;
        private const double _ExpandedResizableColumnWidth = 0;

        private void SharedPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName != nameof(ISharedModel.IsMenuCollapsed))
                return;

            if(_shared?.IsMenuCollapsed == true)
            {
                AnimateViewWidth(_menuCollapsedWidth, _menuExpandedWidth);
                ColumnLeft.BeginAnimation(ColumnDefinition.WidthProperty, AnimateViewColumn(_CollapsedLeftWidth, _ExpandedLeftWidth));
                ColumnMiddle.BeginAnimation(ColumnDefinition.WidthProperty, AnimateViewColumn(_CollapsedMiddleWidth, _ExpandedMiddleWidth));
                ColumnRight.BeginAnimation(ColumnDefinition.WidthProperty, AnimateViewColumn(_CollapsedRightWidth, _ExpandedRightWidth));
                ResizableColumn.BeginAnimation(ColumnDefinition.WidthProperty, AnimateViewColumn(_CollapsedResizableColumnWidth, _ExpandedResizableColumnWidth));
            }
            else
            {
                AnimateViewWidth(_menuExpandedWidth, _menuCollapsedWidth);
                ColumnLeft.BeginAnimation(ColumnDefinition.WidthProperty, AnimateViewColumn(_ExpandedLeftWidth, _CollapsedLeftWidth));
                ColumnMiddle.BeginAnimation(ColumnDefinition.WidthProperty, AnimateViewColumn(_ExpandedMiddleWidth, _CollapsedMiddleWidth));
                ColumnRight.BeginAnimation(ColumnDefinition.WidthProperty, AnimateViewColumn(_ExpandedRightWidth, _CollapsedRightWidth));
                ResizableColumn.BeginAnimation(ColumnDefinition.WidthProperty, AnimateViewColumn(_ExpandedResizableColumnWidth, _CollapsedResizableColumnWidth));
            }
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

            DashboardViewWindow.BeginAnimation(FrameworkElement.WidthProperty, animation);
        }

        private GridResizeAnimation AnimateViewColumn(double from, double to, double seconds = 0.25)
        {
            return new GridResizeAnimation
            {
                From = new GridLength(from, GridUnitType.Pixel),
                To = new GridLength(to, GridUnitType.Pixel),
                Duration = TimeSpan.FromSeconds(seconds),
                FillBehavior = System.Windows.Media.Animation.FillBehavior.HoldEnd
            };
        }
        private void ApplyExpandedState()
        {
            if(_shared?.IsMenuCollapsed == true)
            {
                DashboardViewWindow.Width = _menuExpandedWidth;
                ColumnLeft.Width = new GridLength(_ExpandedLeftWidth);
                ColumnMiddle.Width = new GridLength(_ExpandedMiddleWidth);
                ColumnRight.Width = new GridLength(_ExpandedRightWidth);
                ResizableColumn.Width = new GridLength(_ExpandedResizableColumnWidth);
            }
            else if(_shared?.IsMenuCollapsed == false)
            {
                DashboardViewWindow.Width = _menuCollapsedWidth;
                ColumnLeft.Width = new GridLength(_CollapsedLeftWidth);
                ColumnMiddle.Width = new GridLength(_CollapsedMiddleWidth);
                ColumnRight.Width = new GridLength(_CollapsedRightWidth);
                ResizableColumn.Width = new GridLength(_CollapsedResizableColumnWidth);
            }
        }
    }
}
