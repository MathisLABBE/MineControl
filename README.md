# MineControl – superviser un serveur Minecraft à distance

Application de bureau qui se connecte en **SSH** à la machine Linux qui héberge un serveur Minecraft et affiche l'**état du service** en temps réel (démarré, arrêté, en cours de démarrage, en erreur…), sans avoir à ouvrir un terminal.

Projet personnel, réalisé seul, en cours.

## Fonctionnalités

- Connexion SSH par **clé privée** (avec ou sans passphrase).
- **Vérification de l'identité du serveur** : la connexion est refusée si l'empreinte SHA-256 de la clé d'hôte ne correspond pas à celle attendue (protection contre l'usurpation de serveur).
- Lecture de l'état du service `minecraft.service` (via `systemctl is-active`), **actualisée automatiquement toutes les 10 secondes**.
- Messages d'erreur compréhensibles (clé introuvable, passphrase incorrecte, clé refusée par le serveur, serveur injoignable, délai dépassé…).
- Enregistrement du **profil de connexion** en local (`%LOCALAPPDATA%\MineControl\ProfilServeur.Json`), rechargé au démarrage.

## Technologies

- C# / .NET 10, WPF (MVVM)
- [SSH.NET](https://github.com/sshnet/SSH.NET) pour la connexion SSH
- xUnit v3 et Moq pour les tests unitaires

## Organisation du code

| Dossier | Rôle |
|---|---|
| `SSH/` | `ISshConnexion` et son implémentation : connexion, vérification de l'empreinte, exécution de commandes |
| `Services/` | `MinecraftService` (interprétation de l'état du service) et `ProfilServeurService` (sauvegarde du profil) |
| `ViewModel/` | Logique de l'écran de connexion, actualisation périodique |
| `Vues/` | Interface WPF |
| `MineControl.Tests/` | Tests unitaires |

`MinecraftService` ne dépend que de l'interface `ISshConnexion` : dans les tests, la connexion SSH est **simulée avec Moq**, ce qui permet de vérifier l'interprétation de l'état du serveur sans serveur réel.

## Lancer le projet

Prérequis : Windows et le [SDK .NET 10](https://dotnet.microsoft.com/download).

```powershell
git clone https://github.com/MathisLABBE/MineControl.git
cd MineControl
dotnet run --project MineControl
```

Au premier lancement, renseignez dans l'application l'adresse du serveur, le port, l'utilisateur, le chemin de votre clé privée et l'empreinte SHA-256 du serveur, puis enregistrez le profil. Les valeurs par défaut peuvent aussi être placées dans `MineControl/App.config`.

Pour obtenir l'empreinte du serveur, sur le serveur lui-même :

```bash
ssh-keygen -lf /etc/ssh/ssh_host_ed25519_key.pub
```

Le serveur Minecraft doit tourner comme un service systemd nommé `minecraft.service`.

## Lancer les tests

```powershell
dotnet test MineControl.slnx
```

## Auteur

Mathis LABBE – étudiant en BUT Informatique en alternance · [GitHub](https://github.com/MathisLABBE) · [LinkedIn](https://www.linkedin.com/in/mathis-labbe-92518131b/)
