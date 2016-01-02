using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lidgren.Network;

namespace MultiplayerGame
{
    public interface INetworkManager : IDisposable
    {
        void Connect();

        NetOutgoingMessage CreateMessage();

        void Disconnect();

        NetIncomingMessage ReadMessage();

        void Recycle(NetIncomingMessage im);

        void SendMessage(IGameMessage gameMessage);
    }
}
