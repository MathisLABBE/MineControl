using MineControl.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace MineControl.Vues
{
    public partial class ConnexionServeurControl : UserControl
    {
        #region [Constructeur]

        public ConnexionServeurControl()
        {
            InitializeComponent();

            DataContext = new ConnexionServeurViewModel();
        }

        #endregion


        #region [Initialisation]

        private async void ConnexionServeurControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is ConnexionServeurViewModel lViewModel)
            {
                await lViewModel.ChargerProfilAsync();
            }
        }

        #endregion


        #region [Connecter]

        private async void BtnConnexion_Click(object sender, EventArgs e)
        {
            if (DataContext is not ConnexionServeurViewModel lViewModel)
            {
                return;
            }

            string lPassphrase = PbPassphrase.Password;
            PbPassphrase.Clear();

            try
            {
                await lViewModel.ConnecterAsync(lPassphrase);
            }
            finally
            {
                lPassphrase = "";
            }
        }

        #endregion


        #region [État]

        private async void BtnActualiserEtat_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is ConnexionServeurViewModel lViewModel)
            {
                await lViewModel.ActualiserEtatServeurAsync();
            }
        }

        #endregion


        #region [Profil]

        private async void BtnEnregistrerProfil_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ConnexionServeurViewModel lViewModel)
            {
                await lViewModel.EnregistrerProfilAsync();
            }
        }

        #endregion
    }
}
