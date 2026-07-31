using MineControl.SSH;

namespace MineControl
{
    public sealed class Moteur
    {
        #region [Singleton]

        public static Moteur Instance { get; } = new();

        #endregion


        #region [Propriétés]

        public SshConnexion? ConnexionSsh { get; set; }

        #endregion


        #region [Constructeur]

        private Moteur() { }

        #endregion


        #region [Deconnecter]

        public void DeconnecterSsh()
        {
            ConnexionSsh?.Deconnecter();
            ConnexionSsh = null;
        }

        #endregion
    }
}
