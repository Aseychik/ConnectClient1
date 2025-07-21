using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConnectClient1
{
    public partial class Form1 : Form
    {
        List<string> lastMesseges = new List<string>();
        int posnow = -1;
        string path = @"servers.txt";

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
                        mainTextBox.Text += Environment.NewLine + "Send message: " + inputTextBox.Text;
                        inputTextBox.Text = "";
                        // Здесь отправка сообщения
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

        private void connectButtonClick(object sender, EventArgs e)
        {
            try
            {
                if (!File.Exists(path) || !File.ReadAllText(path).Contains(serverIpPort.Text))
                    File.AppendAllText(path, "\n" + serverIpPort.Text);
            }
            catch (Exception ex) { mainTextBox.AppendText(Environment.NewLine + "Error: " + ex.Message); }
            mainTextBox.AppendText(Environment.NewLine + "Connecting... with " + serverIpPort.Text);
        }
    }
}
