using Lidgren.Network;
using Lidgren.Network.Xna;

using Microsoft.Xna.Framework;

namespace MultiplayerGame
{
    public class UpdatePlayerStateMessage : IGameMessage
    {
        #region Constructors and Destructors

        public UpdatePlayerStateMessage(NetIncomingMessage im)
        {
            this.Decode(im);
        }

        public UpdatePlayerStateMessage()
        {

        }

        public UpdatePlayerStateMessage(Player player)
        {
            this.Id = player.Id;
            this.Position = player.SimulationState.Position;
            this.Velocity = player.SimulationState.Velocity;
            this.MessageTime = NetTime.Now;
        }

        #endregion Constructors and Destructors

        #region Public Properties

        public long Id { get; set; }

        public double MessageTime { get; set; }

        public GameMessageTypes MessageType
        {
            get { return GameMessageTypes.UpdatePlayerState; }
        }

        public Vector2 Position { get; set; }

        public Vector2 Velocity { get; set; }

        #endregion Public Properties

        #region Public Methods and Operators

        public void Decode(NetIncomingMessage im)
        {
            this.Id = im.ReadInt64();
            this.MessageTime = im.ReadDouble();
            this.Position = im.ReadVector2();
            this.Velocity = im.ReadVector2();
        }

        public void Encode(NetOutgoingMessage om)
        {
            om.Write(this.Id);
            om.Write(this.MessageTime);
            om.Write(this.Position);
            om.Write(this.Velocity);
        }

        #endregion Public Methods and Operators
    }
}
