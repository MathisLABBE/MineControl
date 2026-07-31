using MineControl.Helpers;
using MineControl.SSH;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace MineControl
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region [Propriétés]

        private string _tbTestConnexion = "Connexion non testée";
        public string TbTestConnexion
        {
            get => _tbTestConnexion;
            set
            {
                if (_tbTestConnexion == value)
                {
                    return;
                }

                _tbTestConnexion = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region [Constructeur]

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        #endregion


        #region [INotifyPropertyChanged]

        public event PropertyChangedEventHandler?
            PropertyChanged;

        private void OnPropertyChanged(
            [CallerMemberName]
            string? propertyName = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private void BtnTestConnexionSSH_Click(object sender, RoutedEventArgs e)
        {
            SshConnexion lConnexion = new()
            {
                AdresseIp = AppHelper.SshAdresseIp,
                Port = AppHelper.SshPort,
                NomUtilisateur = AppHelper.SshNomUtilisateur
            };

            TbTestConnexion = lConnexion.Connecter(PbPassphrase.Password).Sortie;
        }
    }
}