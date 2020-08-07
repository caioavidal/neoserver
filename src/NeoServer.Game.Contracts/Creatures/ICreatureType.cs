﻿using NeoServer.Game.Enums.Creatures;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Contracts.Creatures
{
    public interface ICreatureType
    {
        string Name { get;  }
        string Description { get; }
        uint MaxHealth { get; }
        ushort Speed { get; }
        IDictionary<LookType, ushort> Look { get; }

    }
}
