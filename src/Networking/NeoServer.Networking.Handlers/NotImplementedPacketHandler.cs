﻿using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Contracts.Network.Enums;
using Serilog.Core;
using System;

namespace NeoServer.Server.Handlers.Authentication
{
    public class NotImplementedPacketHandler : PacketHandler
    {
        private readonly GameIncomingPacketType packet;
        private readonly Logger logger;
        public NotImplementedPacketHandler(GameIncomingPacketType packet, Logger logger)
        {
            this.packet = packet;
            this.logger = logger;
        }

        public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
        {
            var enumText = Enum.GetName(typeof(GameIncomingPacketType), packet);

            enumText = string.IsNullOrWhiteSpace(enumText) ? packet.ToString("x") : enumText;
            logger.Error("Incoming Packet not handled: {packet}", enumText);
        }
    }
}
