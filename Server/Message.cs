using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    class Message
    {
        private int opcode;
        private string sender;
        private int length;
        private string payload;

        public Message(string mes)
        {
            string[] str = mes.Split('|');
            if (str.Length != 4)
                return;

            try
            {
                this.Opcode = Convert.ToInt32(str[0]);
                this.Sender = str[1];
                this.Length = Convert.ToInt32(str[2]);
                this.Payload = str[3];
            }
            catch
            {
                MessageBox.Show("Sai định dạng gói tin");
            }
        }

        public Message(int opcode, string sender, string payload)
        {
            this.Opcode = opcode;
            this.Sender = sender;
            this.Length = payload.Length;
            this.Payload = payload;
        }

        public override string ToString()
        {
            return opcode + "|" + sender + "|" + length + "|" + payload + "$";
        }

        public int Opcode
        {
            get
            {
                return opcode;
            }

            set
            {
                opcode = value;
            }
        }

        public string Sender
        {
            get
            {
                return sender;
            }

            set
            {
                sender = value;
            }
        }

        public int Length
        {
            get
            {
                return length;
            }

            set
            {
                length = value;
            }
        }

        public string Payload
        {
            get
            {
                return payload;
            }

            set
            {
                payload = value;
            }
        }
        
    }
}
