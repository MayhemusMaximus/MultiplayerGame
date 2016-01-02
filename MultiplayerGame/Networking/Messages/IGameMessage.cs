using Lidgren.Network;

namespace MultiplayerGame
{
    public interface IGameMessage
    {
        #region Public Properties

        GameMessageTypes MessageType { get; }

        #endregion Public Properties

        #region Public Methods and Operators

        void Decode(NetIncomingMessage im);

        void Encode(NetOutgoingMessage om);

        #endregion Public Methods and Operators
    }
}
