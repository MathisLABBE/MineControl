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
        public static int SshPort => int.Parse(
            ConfigurationManager.AppSettings["SshPort"]
                ?? throw new ConfigurationErrorsException(
                    "SshPort : Le paramètre est absent ou vide dans la configuration."));

        #endregion
    }
}
