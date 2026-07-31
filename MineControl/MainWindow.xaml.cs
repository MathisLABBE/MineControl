using MineControl.Helpers;
using MineControl.SSH;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;

namespace MineControl
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region [Propriétés]

        private readonly Moteur _Moteur = Moteur.Instance;

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


        #region [Connexion]
        private async void BtnTestConnexionSSH_Click(object sender, RoutedEventArgs e)
        {
            await ConnecterAsync();
        }

        private async Task ConnecterAsync()
        {
            BtnTestConnexionSSH.IsEnabled = false;
            TbTestConnexion = "Connexion en cours...";

            SshConnexion? lNouvelleConnexion = null;

            try
            {
                if (_Moteur.ConnexionSsh?.EstConnecte == true)
                {
                    TbTestConnexion = "Une connexion SSH est déjà établie.";
                    return;
                }

                lNouvelleConnexion = new()
                {
                    AdresseIp = AppHelper.SshAdresseIp,
                    Port = AppHelper.SshPort,
                    NomUtilisateur = AppHelper.SshNomUtilisateur,
                    EmpreinteHoteSha256 = AppHelper.SshEmpreinteHote
                };

                await LancerConnexionSshAsync(lNouvelleConnexion);

                Retour_SshCommande lRetour = await lNouvelleConnexion.ExecuterCommandeAsync("echo connexion-ok");

                if (lRetour.CodeRetour != 0)
                {
                    throw new InvalidOperationException(
                        $"Commande de test échouée : {lRetour.Erreur}");
                }

                _Moteur.ConnexionSsh = lNouvelleConnexion;

                TbTestConnexion = "Connexion SSH réussie.";
            }
            catch (Exception lEx)
            {
                Debug.WriteLine(lEx);

                lNouvelleConnexion?.Deconnecter();

                TbTestConnexion = SshErreurHelper.RecupererMessageUtilisateur(lEx);
            }
            finally
            {
                PbPassphrase.Clear();
                BtnTestConnexionSSH.IsEnabled = true;
            }
        }

        private Task LancerConnexionSshAsync(SshConnexion pConnexion)
        {
            string lPassphrase = PbPassphrase.Password;
            PbPassphrase.Clear();

            try
            {
                return pConnexion.ConnecterAsync(lPassphrase);
            }
            finally
            {
                lPassphrase = "";
            }
        }

        #endregion


        #region [MainWindow]

        protected override void OnClosed(EventArgs e)
        {
            _Moteur.DeconnecterSsh();

            base.OnClosed(e);
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
    }
}