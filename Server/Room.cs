using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Room
    {
        private string id;
        private List<Player> listPlayer;
        private int turn;

        public Room(string id)
        {
            this.id = id;
            ListPlayer = new List<Player>();
            this.turn = 0;
        }

        public bool AddPlayer(Player player)
        {
            if (ListPlayer.Count > 2)
                return false;
            ListPlayer.Add(player);
            return true;
        }

        public string Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        public int Turn
        {
            get
            {
                return turn;
            }

            set
            {
                turn = value;
            }
        }

        internal List<Player> ListPlayer
        {
            get
            {
                return listPlayer;
            }

            set
            {
                listPlayer = value;
            }
        }
    }
}
