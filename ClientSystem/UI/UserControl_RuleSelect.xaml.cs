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

namespace ClientSystem.UI
{
    /// <summary>
    /// UserControl_RuleSelect.xaml 的交互逻辑
    /// </summary>
    public partial class UserControl_RuleSelect : UserControl
    {
        public UserControl_RuleSelect()
        {
            InitializeComponent();
        }

        public IEnumerable<IGrouping<string, DataSystem.DB.Rule>> RuleList
        {
            get
            {
                return DataSystem.Data.Current.Rules.GroupBy(p => p.Group);
            }
        }
        /// <summary>
        /// 选中班规
        /// </summary>
        public DataSystem.DB.Rule SelectRule { get; set; }
    }
}
