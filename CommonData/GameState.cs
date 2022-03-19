using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonData
{
    [Serializable]
    public class GameState
    {
        public char[,] mapArray { get; set; } 
        public List<Player> players { get; set; }

        public GameState()
        {
            players = new List<Player>();
        }

        public void UpdatePayerPosition(Player player)
        {
            Player playerToChange = players.Where(p => p.Id == player.Id).FirstOrDefault();
            playerToChange.X = player.X;
            playerToChange.Y = player.Y;
        }
    }

    [Serializable]
    public class Player
    {
        static Random r = new Random();
        public string Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public ConsoleColor Color { get; set; }
        public Player(string Id)
        {
            this.Id = Id;
            X = 0;
            Y = 0;
            Color = (ConsoleColor)r.Next(0, 16);
        }
    }
}
