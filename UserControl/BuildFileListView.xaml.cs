using BaseUI;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Brush = System.Windows.Media.Brush;
using Button = System.Windows.Controls.Button;
using UserControl = System.Windows.Controls.UserControl;

namespace BaseUI
{
    /// <summary>
    /// ListViewPanel.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class BuildFileListView : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        /// <summary>버튼 클릭 시 발생시킬 이벤트. 외부에서 구독하는 용도</summary>
        public event EventHandler<int> ButtonClickedEvent;

        /// <summary>버튼 클릭 시 발생시킬 이벤트 처리기. 내부에서 이벤트 발생시키는 시점 지정용</summary>
        protected void RaiseButtonClickedEvent(int index)
        {
            ButtonClickedEvent?.Invoke(this, index);
        }

        public BuildFileListView()
        {
            InitializeComponent();
            FileList = new ObservableCollection<BuildFileListViewData>();
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        public ISharedModel? _shared;

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if(DesignerProperties.GetIsInDesignMode(this))
                return;
            _shared ??= App.Services.GetRequiredService<ISharedModel>();
            _shared.PropertyChanged -= SharedPropertyChanged;
            _shared.PropertyChanged += SharedPropertyChanged;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if(_shared is not null)
                _shared.PropertyChanged -= SharedPropertyChanged;
        }

        private const double _CollapsedWidth = 541;
        private const double _ExpandedWidth = 776;
        private const double _CollapsedColumnWidth = 0;
        private const double _ExpandedColumnWidth = 301;

        private void SharedPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName != nameof(ISharedModel.IsMenuCollapsed))
                return;

            if(_shared?.IsMenuCollapsed == true)
            {
                AnimateViewWidth(_CollapsedWidth, _ExpandedWidth);
                ResizeableColumnHeader.BeginAnimation(ColumnDefinition.WidthProperty, AnimateViewColumn(_CollapsedColumnWidth, _ExpandedColumnWidth));
                ExpandColumnAnimation();
            }
            else
            {
                AnimateViewWidth(_ExpandedWidth, _CollapsedWidth);
                ResizeableColumnHeader.BeginAnimation(ColumnDefinition.WidthProperty, AnimateViewColumn(_ExpandedColumnWidth, _CollapsedColumnWidth));
                CollapseColumnAnimation();
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

            BuildFileListViewWindow.BeginAnimation(FrameworkElement.WidthProperty, animation);
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

        private void CollapseColumnAnimation()
        {
            var animation = new DoubleAnimation
            {
                From = ResizeableColumnWidth,
                To = _CollapsedColumnWidth,
                Duration = TimeSpan.FromSeconds(0.25)
            };
            BeginAnimation(dpResizeableColumnWidth, animation);
        }

        private void ExpandColumnAnimation()
        {
            var animation = new DoubleAnimation
            {
                From = ResizeableColumnWidth,
                To = _ExpandedColumnWidth,
                Duration = TimeSpan.FromSeconds(0.25)
            };
            BeginAnimation(dpResizeableColumnWidth, animation);
        }

        public static readonly DependencyProperty dpResizeableColumnWidth = DependencyProperty.Register(nameof(ResizeableColumnWidth), typeof(double), typeof(BuildFileListView), new PropertyMetadata(_CollapsedColumnWidth));
        public static readonly DependencyProperty dpItemsSource  =  DependencyProperty.Register(nameof(ItemsSource),
            typeof(object), typeof(BuildFileListView), new PropertyMetadata(null, OnItemsSourceChanged));

        public static readonly DependencyProperty dpCommand = DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(BuildFileListView), new PropertyMetadata(null));
        public static readonly DependencyProperty dpCmdParBtnPreview = DependencyProperty.Register(nameof(CmdParBtnPreview), typeof(Enum), typeof(BuildFileListView));
        public static readonly DependencyProperty dpCmdParBtnBuildStart = DependencyProperty.Register(nameof(CmdParBtnBuildStart), typeof(Enum), typeof(BuildFileListView));

        public object ItemsSource
        {
            get => GetValue(dpItemsSource); set => SetValue(dpItemsSource, value);
        }
        public double ResizeableColumnWidth
        {
            get => (double)GetValue(dpResizeableColumnWidth); set => SetValue(dpResizeableColumnWidth, value);
        }
        public ICommand Command
        {
            get => (ICommand)GetValue(dpCommand); set => SetValue(dpCommand, value);
        }
        public Enum CmdParBtnPreview
        {
            get => (Enum)GetValue(dpCmdParBtnPreview); set => SetValue(dpCmdParBtnPreview, value);
        }
        public Enum CmdParBtnBuildStart
        {
            get => (Enum)GetValue(dpCmdParBtnBuildStart); set => SetValue(dpCmdParBtnBuildStart, value);
        }

        private INotifyCollectionChanged? SubscribedList;
        public ObservableCollection<BuildFileListViewData> FileList
        {
            get; private set;
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = (BuildFileListView)d;
            c.DetachCollectionHandlers(e.OldValue);
            c.AttachCollectionHandlers(e.NewValue);
            c.UpdateItems();
        }

        private void AttachCollectionHandlers(object? newValue)
        {
            if(newValue is INotifyCollectionChanged incc)
            {
                CollectionChangedEventManager.AddHandler(incc, OnCollectionChanged);
                SubscribedList = incc;
            }
        }

        private void DetachCollectionHandlers(object? oldValue)
        {
            if(SubscribedList is not null)
            {
                CollectionChangedEventManager.RemoveHandler(SubscribedList, OnCollectionChanged);
                SubscribedList = null;
            }
        }

        private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if(!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke((Action)(() => OnCollectionChanged(sender, e)));
                return;
            }
            UpdateItems();
        }

        private void UpdateItems()
        {
            FileList.Clear();
            if(ItemsSource is IEnumerable<BuildFileListViewData> source)
            {
                foreach(var item in source)
                {
                    FileList.Add(item);
                }
            }
        }

        private void AllCheck(object sender, RoutedEventArgs e)
        {
            if(ItemsSource is IEnumerable<BuildFileListViewData> source)
            {
                foreach(var item in source)
                {
                    item.IsChecked = true;
                }
                UpdateItems();
            }
        }

        private void UnCheckAll(object sender, RoutedEventArgs e)
        {
            if(ItemsSource is IEnumerable<BuildFileListViewData> source)
            {
                foreach(var item in source)
                {
                    item.IsChecked = false;
                }
                UpdateItems();
            }
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            if(sender is Button btn)
            {
                var item = (sender as FrameworkElement).DataContext;
                int index = ContentList.Items.IndexOf(item);
                RaiseButtonClickedEvent(index);
            }
        }
    }
}
