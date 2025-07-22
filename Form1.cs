using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ConnectClient1
{
    public partial class Form1 : Form
    {
        List<string> lastMesseges = new List<string>();
        int posnow = -1;
        string path = @"servers.txt";
        Thread receiveThread;
        Dictionary<string, string> helpDict = new Dictionary<string, string>();
        bool sendfilepassword = false;

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
            FillHelp();
        }

        void FillHelp()
        {
            this.helpTextBox.Location = new System.Drawing.Point(256, 126);
            this.helpTextBox.Size = new System.Drawing.Size(310, 180);

            this.helpListBox.Location = new System.Drawing.Point(124, 126);
            this.helpListBox.Size = new System.Drawing.Size(131, 170);

            this.Controls.Add(this.helpTextBox);
            this.Controls.Add(this.helpListBox);
            helpListBox.BringToFront();
            helpTextBox.BringToFront();

            helpDict.Clear();
            helpDict.Add("+connect", "+connect Uid\r\nУстанавливает прямую передачу сообщений к клиенту с заданным Uid\r\nВсе сообщения будут передаваться только данному клиенту\r\nДля отключения напишите +connectEnd");
            helpDict.Add("+connectEnd", "+connectEnd\r\nОтключает прямую передачу сообщений, включаемую +connect");
            helpDict.Add("+sendto", "+sendto Uid message_text\r\nОтправляет сообщение message_text клиенту с заданым Uid");
            helpDict.Add("+sendfile", "+sendfile (выбор файла)\r\nОтправляет выбранный файл пользователю, к которому стоит прямая передача сообщений\r\nПрямое подключение включается через +connect");
            helpDict.Add("+sendfiletouid", "+sendfiletouid Uid_пользователя (выбор файла)\r\nОтправляет выбранный файл пользователю с заданным Uid");
            helpListBox.Items.Clear();
            helpListBox.Items.AddRange(helpDict.Keys.ToArray());
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
                        if (sendfilepassword)
                        {
                            sendfilepassword = false;
                            
                        }
                        lastMesseges.Add(inputTextBox.Text);
                        if (inputTextBox.Text.StartsWith("+"))
                        {
                            switch (inputTextBox.Text.Split()[0])
                            {
                                case "+sendfiletoserver":
                                    if (!ClientWF.isConnected) break;
                                    OpenFileDialog openFileDialog = new OpenFileDialog();
                                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                                    {
                                        mainTextBox.AppendText($"\r\nFile send: {openFileDialog.FileName}");
                                    }
                                    else
                                        break;

                                    SendMessage($"+sendfiletoserver {inputTextBox.Text} {new FileInfo(openFileDialog.FileName).Length}");
                                    mainTextBox.AppendText($"\r\nSent {new FileInfo(openFileDialog.FileName).Length} bytes");
                                    try
                                    {
                                        byte[] file = File.ReadAllBytes(openFileDialog.FileName);
                                        mainTextBox.AppendText($"\r\nfile have {file.Length} bytes");
                                        ClientWF.SendBytes(ClientWF.sendmessage, file, ref mainTextBox);
                                        mainTextBox.AppendText("\r\nsent");
                                    }
                                    catch (Exception ex)
                                    {
                                        mainTextBox.AppendText($"\r\nSend file error: {ex.Message}");
                                    }
                                    sendfilepassword = true;
                                    break;
                                case "+sendfile":
                                    try
                                    {
                                        OpenFileDialog fileDialog = new OpenFileDialog();
                                        if (fileDialog.ShowDialog() == DialogResult.OK)
                                        {
                                            mainTextBox.AppendText($"\r\nSelectedFile: {fileDialog.FileName}");
                                            SendMessage($"+sendfile {new FileInfo(fileDialog.FileName).Length}");
                                            try
                                            {
                                                byte[] file = File.ReadAllBytes(fileDialog.FileName);
                                                ClientWF.SendBytes(ClientWF.sendmessage, file, ref mainTextBox);
                                                break;
                                            }
                                            catch (Exception ex)
                                            {
                                                mainTextBox.AppendText($"\r\nSend file error: {ex.Message}");
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        mainTextBox.AppendText($"\r\nOpen file error: {ex.Message}");
                                    }
                                    break;
                                case "+sendfiletouid":
                                    try
                                    {
                                        OpenFileDialog fileDialog = new OpenFileDialog();
                                        if (fileDialog.ShowDialog() == DialogResult.OK)
                                        {
                                            mainTextBox.AppendText($"\r\nSelectedFile: {fileDialog.FileName}");
                                            SendMessage($"+sendfiletouid {inputTextBox.Text.Split(' ')[1]} {new FileInfo(fileDialog.FileName).Length}");
                                            try
                                            {
                                                byte[] file = File.ReadAllBytes(fileDialog.FileName);
                                                ClientWF.SendBytes(ClientWF.sendmessage, file, ref mainTextBox);
                                                break;
                                            }
                                            catch (Exception ex)
                                            {
                                                mainTextBox.AppendText($"\r\nSend file error: {ex.Message}");
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        mainTextBox.AppendText($"\r\nOpen file error: {ex.Message}");
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            SendMessage(inputTextBox.Text);
                        }
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
                mainTextBox.AppendText(Environment.NewLine + "Message send: " + message);
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
                ConnectInterfSwitch(false);
                receiveThread = new System.Threading.Thread(() => ReceiveMessages(ClientWF.sendmessage));
                receiveThread.Start();
            }
        }

        public void ConnectInterfSwitch(bool t)
        {
            label2.Visible = t;
            label2.Enabled = t;
            label3.Visible = t;
            label3.Enabled = t;
            connectionButton.Visible = t;
            connectionButton.Enabled = t;
            serverIpPort.Visible = t;
            serverIpPort.Enabled = t;
            clientNameTextBox.Visible = t;
            clientNameTextBox.Enabled = t;
            updateMessages.Enabled = t;
            updateMessages.Start();
        }

        public void PrintText(string text)
        {
            nmessage = text;
        }

        private void ReceiveByteMessage(NetworkStream stream, long len)
        {
            byte[] buffer = new byte[len];

            while (true)
            {
                try
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;
                    SaveFileDialog saveDialog = new SaveFileDialog();
                    saveDialog.ShowDialog();
                    File.WriteAllBytes(saveDialog.FileName, buffer);
                    PrintText($"File saved at: {saveDialog.FileName}");
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
                    if (message.StartsWith("+"))
                    {
                        switch (message.Split()[0])
                        {
                            case "+sendfile":
                                try
                                {
                                    byte[] nbuffer = new byte[long.Parse(message.Split()[1])];
                                    PrintText($"\r\nAccepted {nbuffer.Length} bytes");
                                    int nbytesRead = stream.Read(nbuffer, 0, nbuffer.Length);
                                    if (nbytesRead == 0) break;

                                    SaveFileDialog saveFile = new SaveFileDialog();
                                    if (saveFile.ShowDialog() == DialogResult.OK)
                                    {
                                        File.WriteAllBytes(saveFile.FileName, nbuffer);
                                        PrintText($"\r\nFile was saved at {saveFile.FileName}");
                                    }
                                    else PrintText("\r\nSave file error");
                                }
                                catch (Exception ex)
                                {
                                    PrintText($"\r\nSendfile error: {ex.Message}");
                                }
                                break;
                        }
                    }
                    PrintText(Environment.NewLine + message);
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
            BreakConnection();
        }

        private void BreakConnection()
        {
            ClientWF.client?.Close();
            ClientWF.isConnected = false;
            mainTextBox.AppendText(Environment.NewLine + "Disconnected");
            ConnectInterfSwitch(true);
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

        private void OpenHelpMenu(object sender, EventArgs e)
        {
            bool isOpen = !helpListBox.Enabled;

            helpListBox.Enabled = isOpen;
            helpListBox.Visible = isOpen;
            helpTextBox.Visible = isOpen;
            helpTextBox.Enabled = isOpen;
        }

        private void HelpSelValChanged(object sender, EventArgs e)
        {
            try
            {
                helpTextBox.Text = helpDict[helpListBox.Text];
            }
            catch { }
        }

        private void helpListBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void SelectFileButtonClick(object sender, EventArgs e)
        {
            try
            {
                openFileDialog1.AddExtension = true;
                DialogResult dialogResult = openFileDialog1.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    mainTextBox.AppendText("\r\nFile path: " + openFileDialog1.FileName);
                    long len = new FileInfo(openFileDialog1.FileName).Length;
                    //SendMessage("+sendfiletoserver Aboba4352 " + len);
                }
                else
                {
                    mainTextBox.AppendText("\r\nCouldn't select a file");
                }
            }
            catch (Exception ex)
            {
                mainTextBox.AppendText($"\r\nOpen file error: {ex.Message}");
            }
        }
    }
}
