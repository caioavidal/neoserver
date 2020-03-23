﻿using NeoServer.Game.Contracts.Item;
using NeoServer.Game.Enums.Location.Structs;

namespace NeoServer.Networking.Packets.Messages
{
    public interface INetworkMessage: IReadOnlyNetworkMessage
    {
        void AddByte(byte b);
        void AddBytes(byte[] bytes);
        void AddPaddingBytes(int count);
        void AddPayloadLength();
        void AddPayloadLengthSpace();
        void AddString(string value);
        void AddUInt16(ushort value);
        void AddUInt32(uint value);
        void AddUInt8(sbyte value);
        byte[] AddHeader(bool addChecksum = true);
        void AddItem(IItem item);
        void AddLocation(Location location);
        void AddLength();
    }
}
