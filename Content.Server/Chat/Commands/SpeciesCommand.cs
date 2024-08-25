using Content.Server.Chat.Systems;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.Enums;

namespace Content.Server.Chat.Commands
{
    [AnyCommand]
    internal sealed class SpeciesCommand : IConsoleCommand
    {
        [Dependency] private readonly IEntityManager _e = default!;

        public string Command => "species";
        public string Description => "Send chat messages in the species specific chat channel.";
        public string Help => "species <text>";

        public void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            if (shell.Player is not { } player)
            {
                shell.WriteError("This command cannot be run from the server.");
                return;
            }

            if (player.AttachedEntity is not { Valid: true } entity)
                return;

            if (player.Status != SessionStatus.InGame)
                return;

            if (args.Length < 1)
                return;

            var message = string.Join(" ", args).Trim();
            if (string.IsNullOrEmpty(message))
                return;

            _e.System<ChatSystem>().TrySendInGameICMessage(entity, message, InGameICChatType.Species, ChatTransmitRange.Normal, false, shell, player);
        }
    }
}
