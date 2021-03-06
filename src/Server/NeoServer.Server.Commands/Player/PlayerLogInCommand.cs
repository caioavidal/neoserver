using NeoServer.Loaders.Guilds;
using NeoServer.Loaders.Interfaces;
using NeoServer.Server.Contracts;
using NeoServer.Server.Contracts.Commands;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players;
using System.Collections.Generic;
using System.Linq;

namespace NeoServer.Server.Commands
{
    public class PlayerLogInCommand: ICommand
    {
        private readonly IGameServer game;
        private readonly IEnumerable<IPlayerLoader> playerLoaders;
        private readonly GuildLoader guildLoader;
        public PlayerLogInCommand(IGameServer game, IEnumerable<IPlayerLoader> playerLoaders, GuildLoader guildLoader)
        {
            this.game = game;
            this.playerLoaders = playerLoaders;
            this.guildLoader = guildLoader;
        }

        public void Execute(PlayerModel playerRecord, IConnection connection)
        {
            if (playerRecord is null)
            {
                //todo validations here
                return;
            }

            if (!game.CreatureManager.TryGetLoggedPlayer((uint)playerRecord.PlayerId, out var player))
            {
                if (playerLoaders.FirstOrDefault(x => x.IsApplicable(playerRecord)) is not IPlayerLoader playerLoader) return;

                guildLoader.Load(playerRecord?.GuildMember?.Guild);
                player = playerLoader.Load(playerRecord);
            }

            game.CreatureManager.AddPlayer(player, connection);

            player.Login();
            player.LoadVipList(playerRecord.Account.VipList.Select(x => ((uint)x.PlayerId, x.Player?.Name)));
        }
    }
}