using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
    public partial class Client : Form
    {
        IPEndPoint IP;
        TCPClient client;
        string name;
        string roomCode;
        bool clickRunable;
        int myTurn;

        public Client()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            //IP: Địa chỉ của server
            IP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9999);
            client = new TCPClient(IP);
        }
        /// <summary>
        /// Kết nối tới server
        /// </summary>

        /// <summary>
        /// Đóng kết nối khi đóng form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Client_FormClosed(object sender, FormClosedEventArgs e)
        {
            client.Close();
        }
        /// <summary>
        /// add message vào khung chat
        /// </summary>
        /// <param name="message"></param>
        void AddMessage(string message)
        {
            listMessage.Items.Add(new ListViewItem() { Text = message });
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            name = tbxName.Text;
            roomCode = tbxRoom.Text;
            client.Connect(name, roomCode);

            Thread th = new Thread(() =>
            {
                while (true)
                {
                    String buff = client.Receive();

                    String[] str = buff.Split('$');
                    foreach (var item in str)
                    {
                        Messag mes = new Messag(item);
                        ProcessData(client.Socket, mes);
                    }
                    
                }
            });
            th.IsBackground = true;
            th.Start();
        }
        void ProcessData(Socket client, Messag mes)
        {
            switch (mes.Opcode)
            {
                case 201:
                    AddMessage("Tham gia phòng thành công");
                    break;
                case 202:
                    AddMessage("Tham gia phòng thất bại");
                    client.Close();
                    break;
                case 203:
                    int i = Convert.ToInt32(mes.Payload);
                    switch (i)
                    {
                        case 1:
                            groupBox1.Text = mes.Sender;
                            if (name.Equals(mes.Sender))
                            {
                                groupBox1.Text += " (Me)";
                                myTurn = 1;
                            }
                                
                            break;
                        case 2:
                            groupBox2.Text = mes.Sender;
                            if (name.Equals(mes.Sender))
                            {
                                groupBox2.Text += " (Me)";
                                myTurn = 2;
                            }
                            break;
                        case 3:
                            groupBox3.Text = mes.Sender;
                            if (name.Equals(mes.Sender))
                            {
                                groupBox3.Text += " (Me)";
                                myTurn = 3;
                            }
                            break;
                    }
                    break;
                case 210:
                    AddMessage("Trò chơi bắt đầu");
                    break;
                case 211:
                    lbQuestion.Text = mes.Payload;
                    break;
                case 212:
                    LoadQuestion(mes.Payload);
                    LoadHint();
                    break;
                case 221:
                    AddMessage(mes.Sender + ": " + mes.Payload);
                    OpenKeyword(Convert.ToInt32(mes.Payload));
                    break;
                case 222:
                    AddMessage(mes.Sender + ": " + mes.Payload);
                    OpenHintword(Convert.ToInt32(mes.Payload));
                    break;
                case 223:
                    int turn = Convert.ToInt32(mes.Payload);
                    SetTurn(turn);
                    if (turn == myTurn)
                        clickRunable = true;
                    else
                        clickRunable = false;
                    break;

            }
        }

        /// <summary>
        /// Cài đặt màu theo lượt cho các player
        /// </summary>
        /// <param name="k"></param>
        void SetTurn(int k)
        {
            switch (k)
            {
                case 1:
                    groupBox1.BackColor = Color.DeepSkyBlue;
                    groupBox2.BackColor = SystemColors.Control;
                    groupBox3.BackColor = SystemColors.Control;
                    break;
                case 2:
                    groupBox1.BackColor = SystemColors.Control;
                    groupBox2.BackColor = Color.DeepSkyBlue;
                    groupBox3.BackColor = SystemColors.Control;
                    break;
                case 3:
                    groupBox1.BackColor = SystemColors.Control;
                    groupBox2.BackColor = SystemColors.Control;
                    groupBox3.BackColor = Color.DeepSkyBlue;
                    break;
            }
        }

        void OpenKeyword(int k)
        {
            pnlAnswer.Controls[k].Text = "T";
        }
        void OpenHintword(int k)
        {
            pnlHint.Controls[k].Enabled = false;
        }
        

        private void LoadQuestion(string answer)
        {
            for (int i = 0; i < answer.Length; i++)
            {
                Button btn = new Button()
                {
                    TabStop = false,
                    Size = new Size(40, 40)
                };

                if (answer[i].Equals(' '))
                    btn.Enabled = false;                        
                else if(!answer[i].Equals('*'))
                    btn.Text = answer[i].ToString();
                    

                btn.Click += Btn_Click1;
                this.BeginInvoke((Action)(() =>
                {
                    pnlAnswer.Controls.Add(btn);
                }));
            }
        }

        private void Btn_Click1(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn.Text != String.Empty)
                return;
            if (clickRunable)
            {
                int i = pnlAnswer.Controls.IndexOf(btn);
                Messag mes = new Messag(111, name, i.ToString());
                client.Send(mes.ToString());
                clickRunable = false;
            }
            else
            {
                MessageBox.Show("Xin lỗi chưa đến lượt của bạn.");
            }

        }

        private void LoadHint()
        {
            for (char c = 'A'; c <= 'Z'; c++)
            {
                Button btn = new Button()
                {
                    Size = new Size(40, 40),
                    TabStop = false,
                    Text = c.ToString()
                };
                btn.Click += Btn_Click;
                this.BeginInvoke((Action)(() =>
                {
                    pnlHint.Controls.Add(btn);
                }));
            }
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            if (clickRunable)
            {
                Button btn = sender as Button;
                int i = pnlHint.Controls.IndexOf(btn);
                Messag mes = new Messag(112, name, i.ToString());
                client.Send(mes.ToString());
                clickRunable = false;
            }
            else
            {
                MessageBox.Show("Xin lỗi chưa đến lượt của bạn.");
            }
        }
    }
}
