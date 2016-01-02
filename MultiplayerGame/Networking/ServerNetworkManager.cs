using System;
using Lidgren.Network;


namespace MultiplayerGame
{
    public class ServerNetworkManager : INetworkManager
    {
        #region Constants and Fields

        private bool isDisposed;

        private NetServer netServer;

        #endregion Constants and Fields

        #region Public Methods and Operators

        public void Connect()
        {
            var config = new NetPeerConfiguration("Asteroid")
            {
                Port = Convert.ToInt32("14242"),
                // SimulatedMinimumLatency = 0.2f,
                // SimulatedLoss = 0.1f
            };

            config.EnableMessageType(NetIncomingMessageType.WarningMessage);
            config.EnableMessageType(NetIncomingMessageType.VerboseDebugMessage);
            config.EnableMessageType(NetIncomingMessageType.ErrorMessage);
            config.EnableMessageType(NetIncomingMessageType.Error);
            config.EnableMessageType(NetIncomingMessageType.DebugMessage);
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);

            this.netServer = new NetServer(config);
            this.netServer.Start();
        }

        public NetOutgoingMessage CreateMessage()
        {
            return this.netServer.CreateMessage();
        }

        public void Disconnect()
        {
            this.netServer.Shutdown("Bye");
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        public NetIncomingMessage ReadMessage()
        {
            return this.netServer.ReadMessage();
        }

        public void Recycle(NetIncomingMessage im)
        {
            this.netServer.Recycle(im);
        }

        public void SendMessage(IGameMessage gameMessage)
        {
            NetOutgoingMessage om = this.netServer.CreateMessage();
            om.Write((byte)gameMessage.MessageType);
            gameMessage.Encode(om);

            this.netServer.SendToAll(om, NetDeliveryMethod.ReliableUnordered);
        }

        #endregion Public Methods and Operators

        #region Methods

        private void Dispose(bool disposing)
        {
            if(!this.isDisposed)
            {
                if(disposing)
                {
                    this.Disconnect();
                }

                this.isDisposed = true;
            }
        }

        #endregion Methods
    }
}
