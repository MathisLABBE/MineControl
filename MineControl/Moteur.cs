namespace MineControl
{
    public sealed class Moteur
    {
        #region [Singleton]

        private static readonly Moteur instance = new();

        public static Moteur Instance
        {
            get { return instance; }
        }

        public static void Demarrer()
        {
            Console.WriteLine("Le moteur démarre.");
        }

        #endregion


        #region [Constructeur]

        private Moteur() { }

        #endregion
    }
}
