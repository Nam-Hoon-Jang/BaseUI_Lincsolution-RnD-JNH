using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using UserControl = System.Windows.Controls.UserControl;

namespace BaseUI
{
    /// <summary>
    /// PreferenceView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PreferenceView : UserControl
    {
        public PreferenceView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        public ISharedModel? _shared;

        private static readonly Regex _regexNumberOnly = new Regex("[^0-9]");
        private void TextBoxValidationNumOnly(object sender, TextCompositionEventArgs e)
        {
            e.Handled = _regexNumberOnly.IsMatch(e.Text);
        }

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

        private const double _menuCollapsedWidth = 1640;
        private const double _menuExpandedWidth = 1832;

        private void SharedPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName != nameof(ISharedModel.IsMenuCollapsed))
                return;

            if(_shared?.IsMenuCollapsed == true)
            {
                AnimateViewWidth(_menuCollapsedWidth, _menuExpandedWidth);
            }
            else
            {
                AnimateViewWidth(_menuExpandedWidth, _menuCollapsedWidth);
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

            PreferenceViewWindow.BeginAnimation(FrameworkElement.WidthProperty, animation);
        }

        private void ApplyExpandedState()
        {
            if(_shared?.IsMenuCollapsed == true)
            {
                PreferenceViewWindow.Width = _menuExpandedWidth;
            }
            else if(_shared?.IsMenuCollapsed == false)
            {
                PreferenceViewWindow.Width = _menuCollapsedWidth;
            }
        }
    }
}
