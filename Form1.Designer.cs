namespace ConnectClient1
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.mainTextBox = new System.Windows.Forms.TextBox();
            this.inputTextBox = new System.Windows.Forms.TextBox();
            this.sendButton = new System.Windows.Forms.Button();
            this.serverIpPort = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(194, 336);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(141, 46);
            this.button1.TabIndex = 0;
            this.button1.Text = "Connect";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.connectButtonClick);
            // 
            // mainTextBox
            // 
            this.mainTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainTextBox.BackColor = System.Drawing.SystemColors.ControlLight;
            this.mainTextBox.Font = new System.Drawing.Font("MS Reference Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.mainTextBox.ForeColor = System.Drawing.Color.Black;
            this.mainTextBox.Location = new System.Drawing.Point(341, 12);
            this.mainTextBox.Multiline = true;
            this.mainTextBox.Name = "mainTextBox";
            this.mainTextBox.ReadOnly = true;
            this.mainTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.mainTextBox.Size = new System.Drawing.Size(447, 370);
            this.mainTextBox.TabIndex = 1;
            this.mainTextBox.Text = "Initial Text";
            // 
            // inputTextBox
            // 
            this.inputTextBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.inputTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.inputTextBox.Location = new System.Drawing.Point(341, 388);
            this.inputTextBox.Name = "inputTextBox";
            this.inputTextBox.Size = new System.Drawing.Size(375, 27);
            this.inputTextBox.TabIndex = 2;
            this.inputTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.inputKeyDown);
            this.inputTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.inputKeyPress);
            // 
            // sendButton
            // 
            this.sendButton.Location = new System.Drawing.Point(725, 388);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(63, 27);
            this.sendButton.TabIndex = 3;
            this.sendButton.Text = "Send";
            this.sendButton.UseVisualStyleBackColor = true;
            // 
            // serverIpPort
            // 
            this.serverIpPort.FormattingEnabled = true;
            this.serverIpPort.Location = new System.Drawing.Point(12, 306);
            this.serverIpPort.Name = "serverIpPort";
            this.serverIpPort.Size = new System.Drawing.Size(323, 24);
            this.serverIpPort.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(12, 107);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 20);
            this.label1.TabIndex = 5;
            this.label1.Text = "Input line:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(16, 284);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(256, 20);
            this.label2.TabIndex = 6;
            this.label2.Text = "Server: server_ip:server_port";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.serverIpPort);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.inputTextBox);
            this.Controls.Add(this.mainTextBox);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox mainTextBox;
        private System.Windows.Forms.TextBox inputTextBox;
        private System.Windows.Forms.Button sendButton;
        private System.Windows.Forms.ComboBox serverIpPort;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}

