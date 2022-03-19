using CommonData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace GameSocketServer
{
    internal class Server
    {
        static Socket listenerSocket;
        static List<ClientData> _clients;
        public static GameState GameState;

        static void Main(string[] args)
        {
            GameState = new GameState()
            { 
                mapArray = new char[5, 5] { { '.', '.', '.', '.', '.' }, { '.', '.', '.', '.', '.' }, { '.', '.', '.', '.', '.' }, { '.', '.', '.', '.', '.' }, { '.', '.', '.', '.', '.' } }
            };

            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);
            Console.WriteLine("Starting server on " + ipAddress.ToString());
            listenerSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _clients = new List<ClientData>();
            listenerSocket.Bind(localEndPoint);

            Thread listenThread = new Thread(ListenThread);
            listenThread.Start();
        }

        public static void ListenThread()
        {
            for (; ; )
            {
                Console.WriteLine("in listener");
                listenerSocket.Listen(0);
                _clients.Add(new ClientData(listenerSocket.Accept()));
                Console.WriteLine("connected");
            }
        }

        public static void DataIn(object cSocket)
        {
            ClientData client = (ClientData)cSocket;

            byte[] Buffer;
            int readBytes;

            for (; ; )
            {
                try
                {
                    Buffer = new byte[client.clientSocket.SendBufferSize];
                    readBytes = client.clientSocket.Receive(Buffer);
                    if (readBytes > 0)
                    {
                        // handle data
                        Packet packet = new Packet(Buffer);
                        DataManager(packet);
                    }
                }
                catch (SocketException e)
                {
                    Console.WriteLine("A client disconnected! " + client.id);
                    break;
                }
            }
        }

        public static void DataManager(Packet packet)
        {
            switch (packet.packetType)
            {
                case PacketType.PositionChanged:
                    Player changedPlayer = (Player)packet.dataObject;
                    Player p_ToChange = GameState.players.Where(p => p.Id == changedPlayer.Id).FirstOrDefault();
                    p_ToChange.X = changedPlayer.X;
                    p_ToChange.Y = changedPlayer.Y;

                    Packet responsePacket1 = new Packet(PacketType.GameStateUpdate, "");
                    responsePacket1.dataObject = p_ToChange;
                    byte[] bytes = responsePacket1.ToBytes();
                    foreach (ClientData clientData in _clients)
                    {
                        clientData.clientSocket.Send(bytes);
                    }
                    break;
            }
        }
    }
}

