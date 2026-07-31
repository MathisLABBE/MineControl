using Renci.SshNet.Common;
using System.Configuration;
using System.IO;
using System.Net.Sockets;

namespace MineControl.SSH
{
    public static class SshErreurHelper
    {
        public static string RecupererMessageUtilisateur(Exception pEx)
        {
            return pEx switch
            {
                FileNotFoundException => "Clé privée : le fichier de clé SSH est introuvable.",

                UnauthorizedAccessException => "Clé privée : l'accès au fichier de clé SSH est refusé.",

                SshPassPhraseNullOrEmptyException => "Passphrase SSH : cette clé est protégée. Renseigne sa passphrase.",

                Exception lException
                    when EstErreurPassphrase(lException) => "Passphrase SSH : la passphrase saisie est incorrecte.",

                SshAuthenticationException => "Authentification SSH : le serveur a refusé la clé. "
                                            + "Vérifie l'utilisateur SSH et la clé publique installée sur le serveur.",

                SshOperationTimeoutException => "Connexion SSH : le délai d'attente a été dépassé.",

                SocketException
                {
                    SocketErrorCode: SocketError.HostNotFound
                                     or SocketError.NoData
                } => "Serveur SSH : l'adresse du serveur est introuvable.",

                SocketException
                {
                    SocketErrorCode: SocketError.ConnectionRefused
                } => "Serveur SSH : la connexion a été refusée. "
                   + "Vérifie que SSH est démarré et que le port est correct.",

                SocketException
                {
                    SocketErrorCode: SocketError.NetworkUnreachable
                                     or SocketError.HostUnreachable
                } => "Serveur SSH : le serveur est inaccessible. "
                   + "Vérifie que le serveur et Tailscale sont démarrés.",

                SocketException
                {
                    SocketErrorCode: SocketError.TimedOut
                } => "Serveur SSH : le serveur ne répond pas.",

                SocketException => "Serveur SSH : une erreur réseau empêche la connexion.",

                SshConnectionException => "Connexion SSH : la session SSH n'a pas pu être établie.",

                OperationCanceledException => "Connexion SSH : l'opération a été annulée.",

                ConfigurationErrorsException => "Configuration : un paramètre SSH est absent ou incorrect.",

                ArgumentOutOfRangeException => "Configuration SSH : le port doit être compris entre 1 et 65535.",

                ArgumentException lException => lException.Message,

                SshException => "Connexion SSH : une erreur SSH inattendue s'est produite.",

                IOException => "Clé privée : le fichier ne peut pas être lu ou son format est invalide.",

                _ => "Une erreur inattendue s'est produite pendant la connexion SSH."
            };
        }

        private static bool EstErreurPassphrase(Exception pException)
        {
            return pException.Message.Contains(
                "random check bytes",
                StringComparison.OrdinalIgnoreCase);
        }
    }
}