namespace MineControl.SSH
{
    public interface ISshConnexion
    {
        Task<Retour_SshCommande> ExecuterCommandeAsync(string pCommande, CancellationToken pCancellationToken = default);
    }
}
