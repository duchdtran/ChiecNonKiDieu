using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    class TCPClient
    {
        private IPEndPoint IP;
        private Socket socket;

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

        public TCPClient(IPEndPoint IP)
        {
            this.IP = IP;
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
        }

        public void Connect(string name, string roomCode)
        {
            try
            {
                Socket.Connect(IP);
                Messag mes = new Messag(101, name, roomCode);
                Send(mes.ToString());
            }
            catch
            {
                MessageBox.Show("Không thể kết nối tới server", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        /// <summary>
        /// Đóng kết nối
        /// </summary>
        public void Close()
        {
            Socket.Close();
        }
        /// <summary>
        /// Gửi dữ liệu
        /// </summary>
        public void Send(String buff)
        {
            if (buff != string.Empty)
            {
                Socket.Send(Serialize(buff));
            }
        }
        /// <summary>
        /// Nhận dữ liệu
        /// </summary>
        public String Receive()
        {
            try
            {
                byte[] data = new byte[1024 * 5000];
                Socket.Receive(data);

                string message = (string)Deserialize(data);
                return message;
            }
            catch (Exception)
            {
                Close();
            }
            return "";

        }

        

        /// <summary>
        /// Phân mảnh
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        byte[] Serialize(object obj)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();

            formatter.Serialize(stream, obj);
            return stream.ToArray();
        }

        /// <summary>
        /// Gom mảnh
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        object Deserialize(byte[] data)
        {
            MemoryStream stream = new MemoryStream(data);
            BinaryFormatter formatter = new BinaryFormatter();

            return formatter.Deserialize(stream);
        }

    }
}
