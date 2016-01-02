using System;


namespace MultiplayerGame
{
    public class PlayerStateChangedArgs : EventArgs
    {
        #region Constructors and Destructors

        public PlayerStateChangedArgs(Player player)
        {
            this.Player = player;
        }

        #endregion Constructors and Destructors

        #region Public Properties

        public Player Player { get; private set; }

        #endregion Public Properties
    }
}
