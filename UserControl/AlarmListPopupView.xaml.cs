using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UserControl = System.Windows.Controls.UserControl;
using Button = System.Windows.Controls.Button;

namespace BaseUI
{
    /// <summary>
    /// AlarmListPopup.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AlarmListPopupView : UserControl
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

        public AlarmListPopupView()
        {
            InitializeComponent();
            AlarmList = new ObservableCollection<Alarm>();
        }

        public static readonly DependencyProperty dpItemsSource  =  DependencyProperty.Register(nameof(ItemsSource),
            typeof(object), typeof(AlarmListPopupView), new PropertyMetadata(null, OnItemsSourceChanged));

        public static readonly DependencyProperty dpCommand = DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(AlarmListPopupView), new PropertyMetadata(null));
        public static readonly DependencyProperty dpCmdParAlarmAllClear = DependencyProperty.Register(nameof(CmdParAlarmAllClear), typeof(Enum), typeof(AlarmListPopupView));
        private INotifyCollectionChanged? SubscribedList;
        public ObservableCollection<Alarm> AlarmList
        {
            get; private set;
        }
        public object ItemsSource
        {
            get => GetValue(dpItemsSource); set => SetValue(dpItemsSource, value);
        }
        public ICommand Command
        {
            get => (ICommand)GetValue(dpCommand); set => SetValue(dpCommand, value);
        }
        public Enum CmdParAlarmAllClear
        {
            get => (Enum)GetValue(dpCmdParAlarmAllClear); set => SetValue(dpCmdParAlarmAllClear, value);
        }
        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = (AlarmListPopupView)d;
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
            AlarmList.Clear();
            if(ItemsSource is IEnumerable<Alarm> source)
            {
                foreach(var item in source)
                {
                    AlarmList.Add(item);
                }
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
