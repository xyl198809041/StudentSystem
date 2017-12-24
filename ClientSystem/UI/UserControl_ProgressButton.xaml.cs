using MaterialDesignThemes.Wpf;
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
using System.Windows.Threading;

namespace ClientSystem.UI
{
    /// <summary>
    /// UserControl_ProgressButton.xaml 的交互逻辑
    /// </summary>
    public partial class UserControl_ProgressButton : UserControl
    {
        public UserControl_ProgressButton()
        {
            InitializeComponent();
        }

        public enum ProgressType
        {
            /// <summary>
            /// 开始
            /// </summary>
            Start,
            /// <summary>
            /// 完成
            /// </summary>
            Done,
            /// <summary>
            /// 初始化
            /// </summary>
            Ini
        }

        public ProgressType ProgressValue
        {
            get { return (ProgressType)GetValue(ProgressValueProperty); }
            set { SetValue(ProgressValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ProgressValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProgressValueProperty =
            DependencyProperty.Register("ProgressValue", typeof(ProgressType), typeof(UserControl_ProgressButton), new PropertyMetadata(ProgressType.Ini,OnProgressValueChange));




        public PackIconKind Kind
        {
            get { return (PackIconKind)GetValue(KindProperty); }
            set { SetValue(KindProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Kind.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KindProperty =
            DependencyProperty.Register("Kind", typeof(PackIconKind), typeof(UserControl_ProgressButton), new PropertyMetadata(PackIconKind.AccessPoint));



        private static void OnProgressValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UserControl_ProgressButton button = d as UserControl_ProgressButton;
            ProgressType progressType = (ProgressType)e.NewValue;
            switch (progressType)
            {
                case ProgressType.Ini:
                    ButtonProgressAssist.SetValue(button.but, 0);
                    button.packIcon.Kind = button.Kind;
                    ButtonProgressAssist.SetIsIndicatorVisible(button.but, false);
                    break;
                case ProgressType.Start:
                    ButtonProgressAssist.SetValue(button.but, 0);
                    ButtonProgressAssist.SetIsIndicatorVisible(button.but, true);
                    button.packIcon.Kind = PackIconKind.Sync;
                    new DispatcherTimer(TimeSpan.FromMilliseconds(10), DispatcherPriority.Normal, (s, ee) =>
                    {
                        if (ButtonProgressAssist.GetValue(button.but) <= 50) ButtonProgressAssist.SetValue(button.but, ButtonProgressAssist.GetValue(button.but) + 1);
                        else
                        {
                            ((DispatcherTimer)s).Stop();
                        }
                    }, Dispatcher.CurrentDispatcher);
                    break;
                case ProgressType.Done:
                    new DispatcherTimer(TimeSpan.FromMilliseconds(10), DispatcherPriority.Normal, (s, ee) =>
                    {
                        if (ButtonProgressAssist.GetValue(button.but) <= 100) ButtonProgressAssist.SetValue(button.but, ButtonProgressAssist.GetValue(button.but) + 1);
                        else
                        {
                            ButtonProgressAssist.SetIsIndicatorVisible(button.but, false);
                            button.packIcon.Kind = PackIconKind.Check;
                            ((DispatcherTimer)s).Stop();
                        }
                    }, Dispatcher.CurrentDispatcher);
                    break;
            }


        }

        public event RoutedEventHandler Click;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            but.Click += (s, e) => { Click?.Invoke(s, e); };
        }
    }
}
