using MineControl.Models;
using MineControl.Services;
using MineControl.SSH;
using Moq;

namespace MineControl.Tests
{
    public class SshConnexionTests
    {
        [Fact]
        public void CreerConnexionSshSimulee()
        {
            Mock<ISshConnexion> lConnexionSsh = new();

            Assert.NotNull(lConnexionSsh);
        }

        [Fact]
        public async Task RecupererEtatAsync_Active_RetourneDemarre()
        {
            Mock<ISshConnexion> lConnexionSsh = new();

            lConnexionSsh
                .Setup(x => x.ExecuterCommandeAsync(
                    "systemctl is-active minecraft.service",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Retour_SshCommande
                {
                    Sortie = "active"
                });

            MinecraftService lService = new(lConnexionSsh.Object);

            EtatServeurMinecraft lEtat = await lService.RecupererEtatAsync(TestContext.Current.CancellationToken);

            Assert.Equal(EtatServeurMinecraft.Demarre, lEtat);
        }

        [Fact]
        public async Task RecupererEtatAsync_Inactive_RetourneArrete()
        {
            Mock<ISshConnexion> lConnexionSsh = new();

            lConnexionSsh
                .Setup(x => x.ExecuterCommandeAsync(
                    "systemctl is-active minecraft.service",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Retour_SshCommande
                {
                    Sortie = "inactive"
                });

            MinecraftService lService = new(lConnexionSsh.Object);

            EtatServeurMinecraft lEtat = await lService.RecupererEtatAsync(TestContext.Current.CancellationToken);

            Assert.Equal(EtatServeurMinecraft.Arrete, lEtat);
        }
    }
}
