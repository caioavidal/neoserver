﻿using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Contracts.Creatures;
using System.Collections.Generic;

namespace NeoServer.Game.Creatures.Npcs
{
    public sealed class NpcType : INpcType
    {
        public IDialog[] Dialogs { get; init; }

        public string Name { get; set; }

        public string Description { get; }

        public uint MaxHealth { get; set; }

        public ushort Speed { get; set; }

        public IDictionary<LookType, ushort> Look { get; set; }
        public string Script { get; set; }

        public bool IsLuaScript => Script?.EndsWith(".lua") ?? false;
        public IDictionary<string, dynamic> CustomAttributes { get; } = new Dictionary<string, dynamic>();
        public string[] Marketings { get; init; }
    }

    public sealed class Dialog: IDialog
    {
        public string[] OnWords { get ; init; }
        public string[] Answers { get ; init; }
        public string Action { get; init; }
        /// <summary>
        /// Indicated how many times to back in dialog
        /// </summary>
        public byte Back { get; init; }
        public string StoreAt { get; init; }

        public bool End { get; init; }
        public IDialog[] Then { get; init; }
    }
}
