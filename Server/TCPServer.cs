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

namespace Server
{
    class TCPServer
    {
        IPEndPoint IP;
        Socket server;

        public TCPServer(IPEndPoint IP)
        {
            this.IP = IP;
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            server.Bind(IP);
        }

        /// <summary>
        /// Kết nối tới server
        /// </summary>
        public Socket Listen()
        {
            try
            {
                while (true)
                {
                    server.Listen(100);
                    Socket client = server.Accept();
                    return client;
                }
            }
            catch
            {
                IP = new IPEndPoint(IPAddress.Any, 9999);
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            }
            return null;
        }

        /// <summary>
        /// Đóng kết nối
        /// </summary>
        public void Close()
        {
            server.Close();
        }
        /// <summary>
        /// Gửi dữ liệu
        /// </summary>
        public void Send(Socket client, String buff)
        {
            if (buff != string.Empty)
            {
                client.Send(Serialize(buff));
                Thread.Sleep(50);
            }
        }
        /// <summary>
        /// Gửi dữ liệu
        /// </summary>
        public void SendRoom(Room room, Socket client, String buff)
        {
            if (buff == string.Empty)
                return;
            foreach (Player item in room.ListPlayer)
            {
                if(item.Socket != client)
                {
                    item.Socket.Send(Serialize(buff));
                    Thread.Sleep(50);
                }
            }
            
        }
        /// <summary>
        /// Gửi dữ liệu
        /// </summary>
        public void SendAllRoom(Room room, String buff)
        {
            if (buff == string.Empty)
                return;
            foreach (Player item in room.ListPlayer)
            {
                item.Socket.Send(Serialize(buff));
                Thread.Sleep(50);
            }

        }
        /// <summary>
        /// Nhận dữ liệu
        /// </summary>
        public string Receive(Object obj)
        {
            Socket client = obj as Socket;
            try
            {
                byte[] data = new byte[1024 * 5000];
                client.Receive(data);

                string message = (string)Deserialize(data);
                return message;
            }
            catch (Exception)
            {
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
