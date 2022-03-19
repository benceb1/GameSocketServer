using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace CommonData
{
    [Serializable]
    public class Packet
    {
        public List<string> Gdata;
        
        public Object dataObject;

        public int packetInt;
        public bool backetBool;
        public string senderId;
        public PacketType packetType;

        public Packet(PacketType packetType, string senderId)
        {
            Gdata = new List<string>();
            this.senderId = senderId;
            this.packetType = packetType;
        }

        public byte[] ToBytes()
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, this);
                return ms.ToArray();
            }
        }

        public Packet(byte[] packetBytes)
        {
            BinaryFormatter bf = new BinaryFormatter();

            using (var ms = new MemoryStream(packetBytes))
            {
                Packet packet = (Packet)bf.Deserialize(ms);
                this.dataObject = packet.dataObject;
                this.Gdata = packet.Gdata;
                this.packetInt = packet.packetInt;
                this.backetBool = packet.backetBool;
                this.senderId = packet.senderId;
                this.packetType = packet.packetType;
            }
        }
    }

    public enum PacketType
    {
        Registration,
        Chat,
        PositionChanged,
        SetPlayer,
        GameStateUpdate
    }
}
