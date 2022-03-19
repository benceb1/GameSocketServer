using CommonData;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Client
{
    public class Client
    {
        public static Socket master;
        public static string id;
        public static GameState LocalGameState { get; set; } 

        static void Main(string[] args)
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

            master = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                master.Connect(remoteEP);

            }
            catch (Exception ex)
            {
                Console.WriteLine("cannot connect");
            }

            Console.WriteLine("client connected");

            Thread listenerThread = new Thread(DataIn);
            listenerThread.Start();

            Thread.Sleep(1000);

            demoGameStart();

            Console.ReadKey();
        }

        public static void demoGameStart()
        {
            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey();
                Player player = LocalGameState.players.Where(p => p.Id == id).FirstOrDefault();
                switch (keyInfo.Key)
                {
                    case ConsoleKey.W:
                        if (player.X > 0)
                        {
                            player.X--;
                            UpdatePlayerPosition(player);
                        }
                        break;

                    case ConsoleKey.A:
                        if (player.Y > 0)
                        {
                            player.Y--;
                            UpdatePlayerPosition(player);

                        }
                        break;
                    case ConsoleKey.S:
                        if (player.X < LocalGameState.mapArray.GetLength(0) - 1)
                        {
                            player.X++;
                            UpdatePlayerPosition(player);

                        }
                        break;
                    case ConsoleKey.D:
                        if (player.Y < LocalGameState.mapArray.GetLength(1) - 1)
                        {
                            player.Y++;
                            UpdatePlayerPosition(player);

                        }
                        break;
                    default:
                        break;
                }
            }
        }

        public static void UpdatePlayerPosition(Player player)
        {
            Packet p = new Packet(PacketType.PositionChanged, id);
            p.dataObject = player;
            master.Send(p.ToBytes());
        }

        public static void Render()
        {
            char[,] mapArray = LocalGameState.mapArray;
            Console.Clear();
            for (int i = 0; i < mapArray.GetLength(0); i++)
            {
                for (int j = 0; j < mapArray.GetLength(1); j++)
                {
                    Player player = LocalGameState.players.Where(p => p.X == i && p.Y == j).FirstOrDefault();

                    if (player == null)
                        Console.Write(mapArray[i, j]);

                    else
                    {
                        ConsoleColor c = Console.ForegroundColor;
                        Console.ForegroundColor = player.Color;
                        Console.Write("X");
                        Console.ForegroundColor = c;
                    }

                }
                Console.WriteLine();
            }
        }

        public static void DataIn()
        {
            byte[] Buffer;
            int readBytes;

            for (; ; )
            {
                try
                {
                    Buffer = new byte[master.SendBufferSize];
                    readBytes = master.Receive(Buffer);

                    if (readBytes > 0)
                    {
                        DataManager(new Packet(Buffer));
                    }
                }
                catch (SocketException e)
                {
                    Console.WriteLine("The server has disconnected!");
                    Console.ReadLine();
                    Environment.Exit(0);
                }
            }
        }

        public static void DataManager(Packet packet)
        {
            switch (packet.packetType)
            {
                case PacketType.Registration:
                    id = packet.Gdata[0];
                    Console.WriteLine("Recieved a packet for registration! responding: " + id);
                    // SET LOCAL GAMESTATE
                    LocalGameState = (GameState)packet.dataObject;
                    Render();
                    break;

                case PacketType.GameStateUpdate:

                    LocalGameState.UpdatePayerPosition((Player)packet.dataObject);
                    Render();
                    break;

                default:
                    break;
            }
        }
    }
}
