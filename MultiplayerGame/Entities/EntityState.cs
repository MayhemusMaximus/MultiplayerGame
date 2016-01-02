using System;

using Microsoft.Xna.Framework;

namespace MultiplayerGame
{
    public class EntityState : ICloneable
    {
        #region Constants and Fields



        #endregion Constants and Fields

        #region Public Properties

        public Vector2 Position { get; set; }

        public Vector2 Velocity { get; set; }

        #endregion Public Properties

        #region Public Methods and Operators

        public object Clone()
        {
            return new EntityState { Position = this.Position, Velocity = this.Velocity };
        }

        #endregion Public Methods and Operators
    }
}
