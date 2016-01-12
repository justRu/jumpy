using System.Windows.Controls;

namespace Jumpy
{
    /// <summary>
    /// Interaction logic for JumpKeysControl.xaml
    /// </summary>
    public partial class JumpKeysControl : UserControl
    {
        public JumpKeysViewModel Model { get; set; }

        public JumpKeysControl(JumpKeysViewModel model)
        {
            InitializeComponent();
            Model = model;
            DataContext = model;
        }
    }
}
