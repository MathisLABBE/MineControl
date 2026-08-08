using MineControl.Models;
using MineControl.SSH;

namespace MineControl.Services
{
    public class MinecraftService
    {
        #region [Champs]

        private readonly ISshConnexion _ConnexionSsh;

        #endregion


        #region [Constructeur]

        public MinecraftService(ISshConnexion pConnexionSsh)
        {
            _ConnexionSsh = pConnexionSsh;
        }

        #endregion


        #region [État]

        public async Task<EtatServeurMinecraft> RecupererEtatAsync(CancellationToken pCancellationToken = default)
        {
            Retour_SshCommande lRetour = await _ConnexionSsh.ExecuterCommandeAsync(
                    "systemctl is-active minecraft.service", pCancellationToken);

            return ConvertirEtat(lRetour.Sortie);
        }

        private static EtatServeurMinecraft ConvertirEtat(string pEtat)
        {
            switch (pEtat.Trim().ToLowerInvariant())
            {
                case "active":
                    return EtatServeurMinecraft.Demarre;

                case "inactive":
                    return EtatServeurMinecraft.Arrete;

                case "activating":
                    return EtatServeurMinecraft.Demarrage;

                case "deactivating":
                    return EtatServeurMinecraft.Arret;

                case "failed":
                    return EtatServeurMinecraft.Erreur;

                default:
                    return EtatServeurMinecraft.Inconnu;
            }
        }

        #endregion
    }
}
