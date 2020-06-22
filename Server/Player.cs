using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Player
    {
        private string name;
        private Socket socket;
        private string roomID;

        public Player(string name, Socket socket)
        {
            this.name = name;
            this.socket = socket;
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public Socket Socket
        {
            get
            {
                return socket;
            }

            set
            {
                socket = value;
            }
        }

        public string RoomID
        {
            get
            {
                return roomID;
            }

            set
            {
                roomID = value;
            }
        }
    }
}
