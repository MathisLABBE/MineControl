using System.Windows;

namespace MineControl
{
    public partial class App : Application
    {
        #region [Fermer]

        protected override void OnExit(ExitEventArgs e)
        {
            Moteur.Instance.DeconnecterSsh();

            base.OnExit(e);
        }

        #endregion
    }
}