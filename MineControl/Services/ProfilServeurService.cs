using MineControl.Models;
using System.IO;
using System.Text.Json;

namespace MineControl.Services
{
    public class ProfilServeurService
    {
        #region [Champs]

        private readonly string _CheminFichier = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "MineControl",
            "ProfilServeur.Json");

        #endregion


        #region [Enregistrer]

        public async Task EnregistrerProfilAsync(ProfilServeur pProfil)
        {
            string? lDossier = Path.GetDirectoryName(_CheminFichier);

            if (string.IsNullOrWhiteSpace(lDossier) == false)
            {
                Directory.CreateDirectory(lDossier);
            }

            await using FileStream lFichier = File.Create(_CheminFichier);

            await JsonSerializer.SerializeAsync(lFichier, pProfil);
        }

        #endregion


        #region [Charger]

        public async Task<ProfilServeur?> ChargerProfilAsync()
        {
            if (File.Exists(_CheminFichier) == false)
            {
                return null;
            }

            await using FileStream lFichier = File.OpenRead(_CheminFichier);

            return await JsonSerializer.DeserializeAsync<ProfilServeur>(lFichier);
        }

        #endregion
    }
}
