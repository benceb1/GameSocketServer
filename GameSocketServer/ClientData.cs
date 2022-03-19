using CommonData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameSocketServer
{
    public class ClientData
    {
        public Socket clientSocket;
        public Thread clientThread;
        public string id;

        public ClientData()
        {
            id = Guid.NewGuid().ToString();
        }

        public ClientData(Socket clientSocket) : this()
        {
            this.clientSocket = clientSocket;
            clientThread = new Thread(Server.DataIn);
            clientThread.Start(this);
            SendRegistrationPacket();
        }

        public void SendRegistrationPacket()
        {
            Packet p = new Packet(PacketType.Registration, "server");
            p.Gdata.Add(id);
            GameState state = Server.GameState;
            Player player = new Player(id);
            state.players.Add(player);
            p.dataObject = state;
            clientSocket.Send(p.ToBytes());
        }
    }
}
