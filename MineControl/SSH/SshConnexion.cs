using MineControl.Helpers;
using Renci.SshNet;
using Renci.SshNet.Common;
using System.IO;

namespace MineControl.SSH
{
    public class SshConnexion
    {
        #region [Propriétés]

        public string AdresseIp { get; set; } = "";
        public string NomUtilisateur { get; set; } = "";
        public int Port { get; set; } = 22;
        public string EmpreinteHoteSha256 { get; set; } = "";

        private SshClient? Client = null;
        public bool EstConnecte => Client?.IsConnected == true;

        #endregion


        #region [Constructeur]

        public SshConnexion() { }

        #endregion


        #region [Connexion]

        public Task ConnecterAsync(string pPassphrase, CancellationToken pCancellationToken = default)
        {
            try
            {
                ValiderConfiguration();

                if (Client != null && Client.IsConnected)
                {
                    return Task.CompletedTask;
                }

                Client?.Dispose();
                Client = null;

                SshClient lNouveauClient = CreerClientSsh(pPassphrase);

                Client = lNouveauClient;

                return ConnecterClientAsync(lNouveauClient, pCancellationToken);
            }
            finally
            {
                pPassphrase = "";
            }
        }

        private SshClient CreerClientSsh(string pPassphrase)
        {
            PrivateKeyFile lClePrivee;

            if (string.IsNullOrWhiteSpace(pPassphrase))
            {
                lClePrivee = new(AppHelper.SshCheminCle);
            }
            else
            {
                lClePrivee = new(AppHelper.SshCheminCle, pPassphrase);
            }

            PrivateKeyAuthenticationMethod lAuthentification = new(NomUtilisateur, lClePrivee);

            ConnectionInfo lInfoConnexion = new(AdresseIp, Port, NomUtilisateur, lAuthentification)
            {
                Timeout = TimeSpan.FromSeconds(10)
            };

            string lEmpreinteAttendue = NormaliserEmpreinteSha256(EmpreinteHoteSha256);

            SshClient lClient = new(lInfoConnexion)
            {
                KeepAliveInterval = TimeSpan.FromSeconds(30)
            };

            lClient.HostKeyReceived += (_, e) =>
            {
                string lEmpreinteRecue = NormaliserEmpreinteSha256(EmpreinteHoteSha256);
                e.CanTrust = string.Equals(lEmpreinteRecue, e.FingerPrintSHA256, StringComparison.OrdinalIgnoreCase);
            };

            return lClient;
        }

        private async Task ConnecterClientAsync(SshClient pClient, CancellationToken pCancellationToken)
        {
            try
            {
                await pClient.ConnectAsync(pCancellationToken);
            }
            catch
            {
                pClient.Dispose();

                if (ReferenceEquals(Client, pClient))
                {
                    Client = null;
                }

                throw;
            }
        }

        private static string NormaliserEmpreinteSha256(string pEmpreinte)
        {
            string lEmpreinte = pEmpreinte.Trim();

            const string lPrefixe = "SHA256:";

            if (lEmpreinte.StartsWith(lPrefixe, StringComparison.OrdinalIgnoreCase))
            {
                // Retire le préfixe de l'empreinte
                lEmpreinte = lEmpreinte[lPrefixe.Length..];
            }

            return lEmpreinte.TrimEnd('=');
        }

        private void ValiderConfiguration()
        {
            if (string.IsNullOrWhiteSpace(AdresseIp))
            {
                throw new ArgumentException(
                    "Adresse SSH : l'adresse du serveur ne peut pas être vide.");
            }

            if (string.IsNullOrWhiteSpace(NomUtilisateur))
            {
                throw new ArgumentException(
                    "Utilisateur SSH : le nom d'utilisateur ne peut pas être vide.");
            }

            if (Port is < 1 or > 65535)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(Port),
                    "Port SSH : le port doit être compris entre 1 et 65535.");
            }

            if (string.IsNullOrWhiteSpace(AppHelper.SshCheminCle))
            {
                throw new ArgumentException(
                    "Clé privée : le chemin de la clé ne peut pas être vide.");
            }

            if (!File.Exists(AppHelper.SshCheminCle))
            {
                throw new FileNotFoundException(
                    "Clé privée : le fichier de clé SSH est introuvable.",
                    AppHelper.SshCheminCle);
            }

            if (string.IsNullOrWhiteSpace(EmpreinteHoteSha256))
            {
                throw new ArgumentException(
                    "Serveur SSH : l'empreinte SHA-256 de la clé d'hôte est obligatoire.",
                    nameof(EmpreinteHoteSha256));
            }

            ValiderEmpreinteSha256(EmpreinteHoteSha256);
        }

        private static void ValiderEmpreinteSha256(string pEmpreinte)
        {
            string lEmpreinte = NormaliserEmpreinteSha256(pEmpreinte);

            try
            {
                int lLongueurAvecPadding =
                    ((lEmpreinte.Length + 3) / 4) * 4;

                string lValeurBase64 =
                    lEmpreinte.PadRight(lLongueurAvecPadding, '=');

                byte[] lHash = Convert.FromBase64String(lValeurBase64);

                if (lHash.Length != 32)
                {
                    throw new ArgumentException(
                        "Serveur SSH : l'empreinte doit être une empreinte SHA-256.");
                }
            }
            catch (FormatException lException)
            {
                throw new ArgumentException(
                    "Serveur SSH : le format de l'empreinte SHA-256 est invalide.",
                    nameof(pEmpreinte),
                    lException);
            }
        }

        #endregion


        #region [Deconnexion]

        public void Deconnecter()
        {
            if (Client == null)
            {
                return;
            }
            if (Client.IsConnected)
            {
                Client.Disconnect();
            }

            Client.Dispose();
            Client = null;
        }

        #endregion


        #region [Commande]

        public async Task<Retour_SshCommande> ExecuterCommandeAsync(string pCommande, CancellationToken pCancellationToken = default)
        {
            SshClient? lClient = Client;

            if (lClient == null || lClient.IsConnected == false)
            {
                throw new SshConnectionException(
                    "Client : le client SSH n'est pas connecté.");
            }
            if (string.IsNullOrWhiteSpace(pCommande))
            {
                throw new ArgumentException(
                    "Commande : la commande SSH ne peut pas être vide.",
                    nameof(pCommande));
            }

            SshCommand? lCommande = null;

            try
            {
                lCommande = lClient.CreateCommand(pCommande);
                lCommande.CommandTimeout = TimeSpan.FromSeconds(10);

                await lCommande.ExecuteAsync(pCancellationToken);

                Retour_SshCommande lRetour = new()
                {
                    CodeRetour = lCommande.ExitStatus,
                    Sortie = lCommande.Result,
                    Erreur = lCommande.Error ?? ""
                };

                return lRetour;
            }
            finally
            {
                lCommande?.Dispose();
            }
        }

        #endregion
    }
}
