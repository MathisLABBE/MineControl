using System.Configuration;

namespace MineControl.Helpers
{
    public static class AppHelper
    {
        #region [Propriétés]

        public static string SshAdresseIp => ConfigurationManager.AppSettings["SshAdresseIp"]
            ?? throw new ConfigurationErrorsException(
                "SshAdresseIp : Le paramètre est absent ou vide dans la configuration.");
        public static string SshCheminCle => ConfigurationManager.AppSettings["SshCheminCle"]
            ?? throw new ConfigurationErrorsException(
                "SshCheminCle : Le paramètre est absent ou vide dans la configuration.");
        public static string SshNomUtilisateur => ConfigurationManager.AppSettings["SshNomUtilisateur"]
            ?? throw new ConfigurationErrorsException(
                "SshNomUtilisateur : Le paramètre est absent ou vide dans la configuration.");
        public static string SshEmpreinteHote => ConfigurationManager.AppSettings["SshEmpreinteHote"]
            ?? throw new ConfigurationErrorsException(
                "SshEmpreinteHote : Le paramètre est absent ou vide dans la configuration.");
        public static int SshPort
        {
            get
            {
                string? lValeur = ConfigurationManager.AppSettings["SshPort"];

                if (int.TryParse(lValeur, out int lPort) == false
                    || lPort < 1
                    || lPort > 65535)
                {
                    throw new ConfigurationErrorsException(
                        "SshPort : le port doit être un nombre compris entre 1 et 65535.");
                }

                return lPort;
            }
        }

        #endregion
    }
}
