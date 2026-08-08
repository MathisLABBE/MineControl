using MineControl.Helpers;
using MineControl.Models;
using MineControl.Services;
using MineControl.SSH;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace MineControl.ViewModel
{
    public class ConnexionServeurViewModel : INotifyPropertyChanged
    {
        #region [Champs]

        private readonly Moteur _Moteur = Moteur.Instance;

        private readonly ProfilServeurService _ProfilServeurService = new();

        #endregion


        #region [Propriétés]

        private string _AdresseIp = AppHelper.SshAdresseIp;
        public string AdresseIp
        {
            get => _AdresseIp;
            set => SetProperty(ref _AdresseIp, value);
        }


        private int _Port = AppHelper.SshPort;
        public int Port
        {
            get => _Port;
            set => SetProperty(ref _Port, value);
        }


        private string _NomUtilisateur = AppHelper.SshNomUtilisateur;
        public string NomUtilisateur
        {
            get => _NomUtilisateur;
            set => SetProperty(ref _NomUtilisateur, value);
        }


        private string _EmpreinteHoteSha256 = AppHelper.SshEmpreinteHote;
        public string EmpreinteHoteSha256
        {
            get => _EmpreinteHoteSha256;
            set => SetProperty(ref _EmpreinteHoteSha256, value);
        }

        private string _CheminCle = AppHelper.SshCheminCle;
        public string CheminCle
        {
            get => _CheminCle;
            set => SetProperty(ref _CheminCle, value);
        }


        private string _StatutConnexion = "Connexion non testée";
        public string StatutConnexion
        {
            get => _StatutConnexion;
            set => SetProperty(ref _StatutConnexion, value);
        }

        public bool EstConnexionEnCours { get; set; }

        private EtatServeurMinecraft _EtatServeur = EtatServeurMinecraft.Inconnu;
        public EtatServeurMinecraft EtatServeur
        {
            get => _EtatServeur;
            set => SetProperty(ref _EtatServeur, value);
        }

        private DateTime? _DerniereActualisation;
        public DateTime? DerniereActualisation
        {
            get => _DerniereActualisation;
            set => SetProperty(ref _DerniereActualisation, value);
        }

        private readonly DispatcherTimer _TimerActualisation = new();

        #endregion


        #region [Constructeur]

        public ConnexionServeurViewModel()
        {
            _TimerActualisation.Interval = TimeSpan.FromSeconds(10);
            _TimerActualisation.Tick += TimerActualisation_Tick;
        }

        #endregion


        #region [Connecter]

        public async Task ConnecterAsync(string pPassphrase)
        {
            if (EstConnexionEnCours)
            {
                return;
            }

            EstConnexionEnCours = true;
            StatutConnexion = "Connexion en cours...";

            SshConnexion? lNouvelleConnexion = null;

            try
            {
                if (_Moteur.ConnexionSsh?.EstConnecte == true)
                {
                    StatutConnexion = "Une connexion SSH est déjà établie.";

                    return;
                }

                lNouvelleConnexion = new()
                {
                    AdresseIp = AdresseIp,
                    Port = Port,
                    NomUtilisateur = NomUtilisateur,
                    EmpreinteHoteSha256 = EmpreinteHoteSha256,
                    CheminCle = CheminCle
                };

                await lNouvelleConnexion.ConnecterAsync(pPassphrase);

                Retour_SshCommande lRetour = await lNouvelleConnexion.ExecuterCommandeAsync("echo connexion-ok");

                if (lRetour.CodeRetour != 0)
                {
                    throw new InvalidOperationException(
                        $"Commande de test échouée : {lRetour.Erreur}");
                }

                _Moteur.ConnexionSsh = lNouvelleConnexion;

                _Moteur.ServiceMinecraft = new MinecraftService(lNouvelleConnexion);

                EtatServeurMinecraft lRetourEtat = await _Moteur.ServiceMinecraft.RecupererEtatAsync();
                EtatServeur = lRetourEtat;
                DerniereActualisation = DateTime.Now;
                Debug.WriteLine($"État Minecraft : {lRetourEtat}");

                _TimerActualisation.Start();

                StatutConnexion = "Connexion SSH réussie.";
            }
            catch (Exception lException)
            {
                Debug.WriteLine(lException);

                lNouvelleConnexion?.Deconnecter();

                StatutConnexion = SshErreurHelper.RecupererMessageUtilisateur(lException);
            }
            finally
            {
                EstConnexionEnCours = false;
            }
        }

        #endregion


        #region [État]

        public async Task ActualiserEtatServeurAsync()
        {
            if (_Moteur.ServiceMinecraft == null)
            {
                EtatServeur = EtatServeurMinecraft.Inaccessible;
                return;
            }

            try
            {
                EtatServeur = await _Moteur.ServiceMinecraft.RecupererEtatAsync();
            }
            catch
            {
                EtatServeur = EtatServeurMinecraft.Inaccessible;
            }
            finally
            {
                DerniereActualisation = DateTime.Now;
            }
        }

        #endregion


        #region [Timer]

        private async void TimerActualisation_Tick(object? sender, EventArgs e)
        {
            await ActualiserEtatServeurAsync();
        }

        #endregion


        #region [Profil]

        public async Task EnregistrerProfilAsync()
        {
            ProfilServeur lProfil = new()
            {
                AdresseIp = AdresseIp,
                Port = Port,
                NomUtilisateur = NomUtilisateur,
                CheminCle = CheminCle,
                EmpreinteHoteSha256 = EmpreinteHoteSha256
            };

            await _ProfilServeurService.EnregistrerProfilAsync(lProfil);
        }

        public async Task ChargerProfilAsync()
        {
            ProfilServeur? lProfil = await _ProfilServeurService.ChargerProfilAsync();

            if (lProfil == null)
            {
                return;
            }

            AdresseIp = lProfil.AdresseIp;
            Port = lProfil.Port;
            NomUtilisateur = lProfil.NomUtilisateur;
            CheminCle = lProfil.CheminCle;
            EmpreinteHoteSha256 = lProfil.EmpreinteHoteSha256;
        }

        #endregion


        #region [INotifyPropertyChanged]

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T pVariable, T pValeur, [CallerMemberName] string? pNomPropriete = null)
        {
            if (EqualityComparer<T>.Default.Equals(pVariable, pValeur))
            {
                return false;
            }

            pVariable = pValeur;
            OnPropertyChanged(pNomPropriete);

            return true;
        }

        #endregion
    }
}
