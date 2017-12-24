using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace ClientSystem.UI
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:ClientSystem.UI"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:ClientSystem.UI;assembly=ClientSystem.UI"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误: 
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:CustomControl_OpenWin/>
    ///
    /// </summary>
    [TemplatePart(Name ="Button_Close",Type =typeof(Button))]
    public class CustomControl_OpenWin : MaterialDesignThemes.Wpf.Card
    {
        static CustomControl_OpenWin()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomControl_OpenWin), new FrameworkPropertyMetadata(typeof(CustomControl_OpenWin)));
            
        }

        public event Action<CustomControl_OpenWin> Closed; 

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Button button_Close = (Button)Template.FindName("Button_Close", this);
            ////button_Close.Click += (s, e) =>
            ////{
            ////    Closed?.Invoke(this);
            ////};
            Unloaded += (s, e) =>
            {
                Closed?.Invoke(this);
            };
        }

        public string Tilte
        {
            get { return (string)GetValue(TilteProperty); }
            set { SetValue(TilteProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Tilte.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TilteProperty =
            DependencyProperty.Register("Tilte", typeof(string), typeof(CustomControl_OpenWin), new PropertyMetadata("标题"));

        /// <summary>
        /// 关闭,等同于点关闭按钮
        /// </summary>
        public void Close()
        {
            Button button_Close = (Button)Template.FindName("Button_Close", this);
            button_Close.Command.Execute(null);
        }

        public async void Close(TimeSpan Time)
        {
            await Task.Run(() =>
            {
                Thread.Sleep((int)Time.TotalMilliseconds);
            });
            Close();
        }

    }
}
