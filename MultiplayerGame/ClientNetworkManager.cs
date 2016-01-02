using System;
using System.Net;

using Lidgren.Network;


namespace MultiplayerGame
{
    public class ClientNetworkManager : INetworkManager
    {
        #region Constants and Fields

        private bool isDisposed;

        private NetClient netClient;

        #endregion Constants and Fields

        #region Public Methods and Operators

        public void Connect()
        {
            var config = new NetPeerConfiguration("Asteroid")
            {
                SimulatedMinimumLatency = 0.2f,
                // SimulatedLoss = 0.1f
            };

            config.EnableMessageType(NetIncomingMessageType.WarningMessage);
            config.EnableMessageType(NetIncomingMessageType.VerboseDebugMessage);
            config.EnableMessageType(NetIncomingMessageType.ErrorMessage);
            config.EnableMessageType(NetIncomingMessageType.Error);
            config.EnableMessageType(NetIncomingMessageType.DebugMessage);
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);

            this.netClient = new NetClient(config);
            this.netClient.Start();

            this.netClient.Connect(new IPEndPoint(NetUtility.Resolve("127.0.0.1"), Convert.ToInt32("14242")));
        }

        public NetOutgoingMessage CreateMessage()
        {
            return this.netClient.CreateMessage();
        }

        public void Disconnect()
        {
            this.netClient.Disconnect("Bye");
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        public NetIncomingMessage ReadMessage()
        {
            return this.netClient.ReadMessage();
        }

        public void Recycle(NetIncomingMessage im)
        {
            this.netClient.Recycle(im);
        }

        public void SendMessage(IGameMessage gameMessage)
        {
            NetOutgoingMessage om = this.netClient.CreateMessage();
            om.Write((byte)gameMessage.MessageType);
            gameMessage.Encode(om);

            this.netClient.SendMessage(om, NetDeliveryMethod.ReliableOrdered);
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

        #endregion
    }
}
