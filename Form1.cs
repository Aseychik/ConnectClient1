using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConnectClient1
{
    public partial class Form1 : Form
    {
        List<string> lastMesseges = new List<string>();
        int posnow = -1;
        string path = @"servers.txt";
        Thread receiveThread;

        string nmessage = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (File.Exists(path))
            {
                try { serverIpPort.Items.AddRange(File.ReadAllText(path).Split('\n')); }
                catch (Exception ex) { mainTextBox.Text += Environment.NewLine + "Error: " + ex.Message; }
            }
            else {
                File.WriteAllText(path, "0:8068");
                serverIpPort.Items.Add("0:8068");
            }
        }

        private void inputKeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void inputKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    if (posnow != -1)
                    {
                        lastMesseges[lastMesseges.Count - 1] = inputTextBox.Text;
                        inputTextBox.ForeColor = Color.Black;
                        break;
                    }
                    if (inputTextBox.Text != "")
                    {
                        lastMesseges.Add(inputTextBox.Text);
                        SendMessage(inputTextBox.Text);
                        inputTextBox.Text = "";
                    }
                    break;
                case Keys.Up:
                    if (posnow == -1)
                    {
                        lastMesseges.Add(inputTextBox.Text);
                        inputTextBox.ForeColor = Color.DarkGray;
                    }
                    posnow = (posnow + 1) % lastMesseges.Count;
                    inputTextBox.Text = lastMesseges[lastMesseges.Count - posnow - 1];
                    break;
                case Keys.Down:
                    if (posnow == -1)
                    {
                        lastMesseges.Add(inputTextBox.Text);
                        inputTextBox.ForeColor = Color.DarkGray;
                    }
                    posnow = posnow == -1 ? lastMesseges.Count - 1 : posnow - 1;
                    break;
                default:
                    if (posnow != -1)
                    {
                        lastMesseges[lastMesseges.Count - 1] = lastMesseges[lastMesseges.Count - posnow - 1];
                        inputTextBox.ForeColor = Color.Black;
                        posnow = -1;
                    }
                    break;
            }
        }

        private void SendMessage(string message)
        {
            if (ClientWF.isConnected) {
                ClientWF.SendMessages(ClientWF.sendmessage, message, ref mainTextBox);
                mainTextBox.AppendText(Environment.NewLine + "Message send: " + inputTextBox.Text);
            }
        }

        public void ConnectButtonClick(object sender, EventArgs e)
        {
            try
            {
                if (!File.Exists(path) || !File.ReadAllText(path).Contains(serverIpPort.Text))
                    File.AppendAllText(path, "\n" + serverIpPort.Text);
            }
            catch (Exception ex) { mainTextBox.AppendText(Environment.NewLine + "Error: " + ex.Message); }

            if (ClientWF.isConnected) {
                mainTextBox.AppendText(Environment.NewLine + "Reconnecting... with " + serverIpPort.Text);
                BreakConnection();

                receiveThread?.Abort();
            }
            else
                mainTextBox.AppendText(Environment.NewLine + "Connecting... with " + serverIpPort.Text);


            ClientWF.ClientMain(outputText: ref mainTextBox, serverIpPort.Text, clientNameTextBox.Text);
            if (ClientWF.isConnected)
            {
                receiveThread = new System.Threading.Thread(() => ReceiveMessages(ClientWF.sendmessage));
                receiveThread.Start();
            }
        }

        public void PrintText(string text)
        {
            nmessage = text;
        }

        private void ReceiveMessages(NetworkStream stream)
        {
            byte[] buffer = new byte[1024];

            while (true)
            {
                try
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    
                    mainTextBox.AppendText(Environment.NewLine + message);
                }
                catch (IOException ex)
                {
                    PrintText(Environment.NewLine + $"Receive error: {ex.Message}");
                    break;
                }
                catch (Exception ex)
                {
                    PrintText(Environment.NewLine + $"Message receive error: {ex.Message}");
                    break;
                }
            }
        }

        private void BreakConnection(object sender, EventArgs e)
        {
            ClientWF.client?.Close();
            ClientWF.isConnected = false;
        }
        private void BreakConnection()
        {
            ClientWF.client?.Close();
            ClientWF.isConnected = false;
        }

        private void SendMessageButtonClick(object sender, EventArgs e)
        {
            if (inputTextBox.Text != "")
                SendMessage(inputTextBox.Text);
        }

        private void UpdateResieveMessage(object sender, EventArgs e)
        {
            mainTextBox.AppendText(nmessage);
            nmessage = "";
        }
    }
}
