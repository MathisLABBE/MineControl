namespace MineControl.SSH
{
    public class Retour_SshCommande
    {
        #region [Propriétés]

        public int? CodeRetour { get; set; } = 0;
        public string Sortie { get; set; } = "";
        public string Erreur { get; set; } = "";

        #endregion


        #region [Constructeur]

        public Retour_SshCommande() { }

        #endregion
    }
}
