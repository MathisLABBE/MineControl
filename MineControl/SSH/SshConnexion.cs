using MineControl.Helpers;
using Renci.SshNet;
using Renci.SshNet.Common;

namespace MineControl.SSH
{
    public class SshConnexion
    {
        #region [Propriétés]

        public string AdresseIp { get; set; } = "";
        public string NomUtilisateur { get; set; } = "";
        public int Port { get; set; } = 22;

        private SshClient? Client = null;

        #endregion


        #region [Constructeur]

        public SshConnexion() { }

        #endregion


        #region [Connexion]

        public Retour_SshCommande Connecter(string pPassPhrase)
        {
            PrivateKeyFile lClePrivee = new(AppHelper.SshCheminCle, pPassPhrase);

            PrivateKeyAuthenticationMethod lMethodeAuthentification = new(NomUtilisateur, lClePrivee);

            ConnectionInfo lInformationsConnexion = new(AdresseIp, Port, NomUtilisateur, lMethodeAuthentification)
            {
                Timeout = TimeSpan.FromSeconds(10)
            };

            SshClient lClient = new(lInformationsConnexion);

            lClient.Connect();

            if (lClient.IsConnected == false)
            {
                throw new SshConnectionException(
                    "Client : La connexion n'a pas pu être établie.");
            }

            Client = lClient;

            return ExecuterCommande("echo connexion-ok");
        }

        #endregion


        #region [Commande]

        public Retour_SshCommande ExecuterCommande(string pCommande)
        {
            if (Client == null)
            {
                throw new SshConnectionException(
                    "Client : Le client n'est pas connecté");
            }

            SshCommand lCommande = Client.CreateCommand(pCommande);
            lCommande.CommandTimeout = TimeSpan.FromSeconds(10);

            // Execute la commande
            string lSortie = lCommande.Execute();

            Retour_SshCommande lRetour = new()
            {
                Sortie = lSortie,
                CodeRetour = lCommande.ExitStatus,
                Erreur = lCommande.Error ?? ""
            };

            return lRetour;
        }

        #endregion
    }
}
