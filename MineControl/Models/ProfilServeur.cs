namespace MineControl.Models
{
    public class ProfilServeur
    {
        public string AdresseIp { get; set; } = "";
        public int Port { get; set; } = 22;
        public string NomUtilisateur { get; set; } = "";
        public string CheminCle { get; set; } = "";
        public string EmpreinteHoteSha256 { get; set; } = "";
    }
}
