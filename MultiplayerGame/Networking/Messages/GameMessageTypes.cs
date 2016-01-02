using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiplayerGame
{
    public enum GameMessageTypes
    {
        UpdateAsteroidState,
        UpdatePlayerState,
        ShotFired,
        EnemySpawned
    }
}
