using System.Windows;

namespace MineControl
{
    public partial class MainWindow : Window
    {
        #region [Constructeur]

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        #endregion
    }
}