using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UserControl = System.Windows.Controls.UserControl;

namespace BaseUI
{
    /// <summary>
    /// PopupView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PopupView : UserControl
    {
        public PopupView()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty dpPopupType = DependencyProperty.Register(nameof(PopupType), typeof(Enum), typeof(PopupView), new PropertyMetadata(BaseEnum.PopupType.Info, OnPopupTypeChanged));
        //public static readonly DependencyProperty dpPopupTItle = DependencyProperty.Register(nameof(PopupTItle), typeof(string), typeof(PopupView), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty dpPopupContent = DependencyProperty.Register(nameof(PopupContent), typeof(string), typeof(PopupView), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty dpCommand = DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(PopupView), new PropertyMetadata(null));
        public static readonly DependencyProperty dpCmdParBtnLeft = DependencyProperty.Register(nameof(CmdParBtnLeft), typeof(Enum), typeof(PopupView));
        public static readonly DependencyProperty dpCmdParBtnRight = DependencyProperty.Register(nameof(CmdParBtnRight), typeof(Enum), typeof(PopupView));
        public Enum PopupType
        {
            get => (Enum)GetValue(dpPopupType);
            set => SetValue(dpPopupType, value);
        }
        //public string PopupTItle
        //{
        //    get => (string)GetValue(dpPopupTItle);
        //    set => SetValue(dpPopupTItle, value);
        //}
        public string PopupContent
        {
            get => (string)GetValue(dpPopupContent);
            set => SetValue(dpPopupContent, value);
        }
        public ICommand Command
        {
            get => (ICommand)GetValue(dpCommand);
            set => SetValue(dpCommand, value);
        }
        public Enum CmdParBtnLeft
        {
            get => (Enum)GetValue(dpCmdParBtnLeft);
            set => SetValue(dpCmdParBtnLeft, value);
        }
        public Enum CmdParBtnRight
        {
            get => (Enum)GetValue(dpCmdParBtnRight);
            set => SetValue(dpCmdParBtnRight, value);
        }

        private static void OnPopupTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is PopupView view)
            {
                view.UpdateVisuals();
            }
        }

        private void UpdateVisuals()
        {
            switch(PopupType)
            {
                case BaseEnum.PopupType.Info:
                    TitleMessage.Text = Function.GetResourceString("AlarmListPopupLabel_Info");
                    SetTwoButtons();
                    break;
                case BaseEnum.PopupType.Warning:
                    TitleMessage.Text = Function.GetResourceString("AlarmListPopupLabel_Warn");
                    SetTwoButtons();
                    break;
                case BaseEnum.PopupType.Error:
                    TitleMessage.Text = Function.GetResourceString("AlarmListPopupLabel_Error");
                    SetTwoButtons();
                    break;
                case BaseEnum.PopupType.InfoSingle:
                    TitleMessage.Text = Function.GetResourceString("AlarmListPopupLabel_Info");
                    SetSingleButton();
                    break;
                case BaseEnum.PopupType.WarningSingle:
                    TitleMessage.Text = Function.GetResourceString("AlarmListPopupLabel_Warn");
                    SetSingleButton();
                    break;
                case BaseEnum.PopupType.ErrorSingle:
                    TitleMessage.Text = Function.GetResourceString("AlarmListPopupLabel_Error");
                    SetSingleButton();
                    break;

            }
        }

        private void SetTwoButtons()
        {
            BtnLeft.Content = Function.GetResourceString("Popup_Yes");
            BtnRight.Content = Function.GetResourceString("Popup_No");
            BtnLeft.Visibility = Visibility.Visible;
            BtnRight.Visibility = Visibility.Visible;
            BtnSingle.Visibility = Visibility.Collapsed;
        }

        private void SetSingleButton()
        {
            BtnSingle.Content = Function.GetResourceString("Popup_OK");
            BtnLeft.Visibility = Visibility.Collapsed;
            BtnRight.Visibility = Visibility.Collapsed;
            BtnSingle.Visibility = Visibility.Visible;
        }
    }
}
