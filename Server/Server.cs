using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Server
{
    public partial class Form1 : Form
    {
        IPEndPoint IP;
        TCPServer server;

        Dictionary<string, Room> listRoom;
        Dictionary<string, Player> listPlayer;

        string question;
        string answer;

        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;

            listRoom = new Dictionary<string, Room>();
            listPlayer = new Dictionary<string, Player>();

            LoadQuestion();

            //IP: Địa chỉ của server
            IP = new IPEndPoint(IPAddress.Any, 9999);
            server = new TCPServer(IP);

            Thread listen = new Thread(() =>
            {
                while (true)
                {
                    Socket client = server.Listen();
                    if (client != null)
                    {
                        Thread receive = new Thread(() => {
                            while (true)
                            {
                                String buff = server.Receive(client);

                                String[] str = buff.Split('$');
                                foreach (var item in str)
                                {
                                    if (item != String.Empty)
                                    {
                                        Message mes = new Message(item);
                                        AddMessage(mes.Sender + ": " + mes.Opcode + " " + mes.Payload);
                                        ProcessData(client, mes);
                                    }
                                }
                               
                            }
                        });
                        receive.IsBackground = true;
                        receive.Start();
                    }
                }
            });

            listen.IsBackground = true;
            listen.Start();
        }

        private void LoadQuestion()
        {
            question = "Thủ đô của Việt Nam";
            answer = "I can't do it";
        }

        /// <summary>
        /// Xử lí dữ liệu
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        private void ProcessData(Socket client, Message message)
        {
            string roomID = "";
            Room room;
            Message mes;
            int sttPlayer;
            switch (message.Opcode)
            {
                
                case 101:
                    AddPlayer(client, message);
                    roomID = listPlayer[message.Sender].RoomID;
                    room = listRoom[roomID];
                    if(room.ListPlayer.Count == 3)
                    {
                        StartGame(room, client);
                    }
                    break;
                case 111:
                    roomID = listPlayer[message.Sender].RoomID;
                    room = listRoom[roomID];

                    sttPlayer = room.ListPlayer.IndexOf(listPlayer[message.Sender]) + 1;
                    if (room.Turn == sttPlayer)
                    {
                        mes = new Message(221, message.Sender, message.Payload);
                        server.SendAllRoom(room, mes.ToString());
                        room.Turn = (room.Turn + 1);
                        if (room.Turn > 3)
                            room.Turn = 1;
                        SendTurnPlay(room);
                    }
                    break;
                case 112:
                    roomID = listPlayer[message.Sender].RoomID;
                    room = listRoom[roomID];

                    sttPlayer = room.ListPlayer.IndexOf(listPlayer[message.Sender]) + 1;
                    if (room.Turn == sttPlayer)
                    {
                        mes = new Message(222, message.Sender, message.Payload);
                        server.SendAllRoom(room, mes.ToString());
                        room.Turn = (room.Turn + 1);
                        if (room.Turn > 3)
                            room.Turn = 1;
                        SendTurnPlay(room);
                    }
                    break;
                default:
                    //sai cú pháp
                    break;
            }
        }

        /// <summary>
        /// Bắt đầu trò chơi
        /// </summary>
        /// <param name="room"></param>
        /// <param name="client"></param>
        void StartGame(Room room, Socket client)
        {
            //Bắt đầu trò chơi
            Message mes1 = new Message(210, "Server", "");
            server.SendAllRoom(room, mes1.ToString());

            //Gửi câu hỏi
            Message mes2 = new Message(211, "Server", question);
            server.SendAllRoom(room, mes2.ToString());

            //Gửi đáp án
            string str = answer;
            for (int i = 0; i < answer.Length; i++)
            {
                if (Char.IsLetter(str[i]))
                {
                    str = str.Replace(str[i], '*');
                }
            }
            Message mes3 = new Message(212, "Server", str.ToString());
            server.SendAllRoom(room, mes3.ToString());

            //Gửi lượt chơi
            room.Turn = 1;
            SendTurnPlay(room);
        }

        /// <summary>
        /// Gửi lượt chơi cho các người chơi trong phòng
        /// </summary>
        /// <param name="room"></param>
        void SendTurnPlay(Room room)
        {
            Message mes4 = new Message(223, "Server", room.Turn.ToString());
            server.SendAllRoom(room, mes4.ToString());
        }

        /// <summary>
        /// Thêm người chơi vào phòng
        /// </summary>
        /// <param name="client"></param>
        /// <param name="mes"></param>
        void AddPlayer(Socket client, Message mes)
        {
            Player player = new Player(mes.Sender, client);
            listPlayer[player.Name] = player;
            if (listRoom.ContainsKey(mes.Payload))
            {
                //Phòng đã tồn tại
                if (!listRoom[mes.Payload].AddPlayer(player))
                {
                    //Phòng đầy người
                    Message send_buff = new Message(202, "Server", "");
                    server.Send(client, send_buff.ToString());
                    client.Close();
                }
                else
                {
                    //Tham gia phòng thành công
                    Message send_buff = new Message(201, "Server", "");
                    player.RoomID = mes.Payload;
                    server.Send(client, send_buff.ToString());
                    List<Player> lPlayer = listRoom[player.RoomID].ListPlayer;

                    for (int i = 0; i < lPlayer.Count; i++)
                    {
                        Message send_buff1 = new Message(203, lPlayer[i].Name, (i+1).ToString());
                        server.Send(client, send_buff1.ToString());
                    }

                    Message send_buff2 = new Message(203, player.Name, lPlayer.Count.ToString());
                    server.SendRoom(listRoom[mes.Payload], client, send_buff2.ToString());
                }
            }
            else
            {
                //Phòng chưa tồn tại
                Room room = new Room(mes.Payload);
                room.AddPlayer(player);
                listRoom.Add(room.Id, room);

                player.RoomID = mes.Payload;

                Message send_buff = new Message(201, player.Name, "");
                server.Send(client, send_buff.ToString());
                Message send_buff1 = new Message(203, player.Name, "1");
                server.Send(client, send_buff1.ToString());
            }
        }

        /// <summary>
        /// add message vào khung chat
        /// </summary>
        /// <param name="message"></param>
        void AddMessage(string message)
        {
            listMessage.Items.Add(new ListViewItem() { Text = message });
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            server.Close();
        }
    }
}
