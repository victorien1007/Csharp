using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace Chat_Server
{
    partial class MainForm : Form
    {
        /// Designer variable used to keep track of non-visual components.  
        private IContainer components = null;
        private Label labelIP;
        private TextBox txtIP;
        private Label labelPort;
        private TextBox txtPort;
        private Button btnConnect;
        private Label labelMember;
        private ListBox listMember;
        private Label labelChatroom;
        private TextBox txtMsg;
        private Label labelMessage;
        private TextBox txtMsgSend;
        private Label labelTopic;
        private TextBox txtTpcSend;
        private Button btnSendMsg;
        private Button btnSendToAll;

        /// <summary>  
        /// Disposes resources used by the form.  
        /// </summary>  
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>  
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>  
        /// This method is required for Windows Forms designer support.  
        /// Do not change the method contents inside the source code editor. The Forms designer might  
        /// not be able to load this method if it was changed manually.  
        /// </summary>  
        private void InitializeComponent()
        {
            this.labelIP = new Label();
            this.txtIP = new TextBox();
            this.labelPort = new Label();
            this.txtPort = new TextBox();
            this.btnConnect = new Button();
            this.labelMember = new Label();
            this.listMember = new ListBox();
            this.labelChatroom = new Label();
            this.txtMsg = new TextBox();
            this.labelMessage = new Label();
            this.txtMsgSend = new TextBox();
            this.labelTopic = new Label();
            this.txtTpcSend = new TextBox();
            this.btnSendMsg = new Button();
            this.btnSendToAll = new Button();
            this.SuspendLayout();

            //   
            // labelIP  
            //   
            this.labelIP.AutoSize = true;
            this.labelIP.Location = new Point(270, 10);
            this.labelIP.Name = "labelIP";
            this.labelIP.Size = new Size(20, 10);
            this.labelIP.TabIndex = 4;
            this.labelIP.Text = "IP:";

            //   
            // txtIP  
            //   
            this.txtIP.Location = new Point(295, 5);
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new Size(75, 20);
            this.txtIP.TabIndex = 5;
            this.txtIP.Text = "127.0.0.1";

            //   
            // labelPort  
            //   
            this.labelPort.AutoSize = true;
            this.labelPort.Location = new Point(380, 10);
            this.labelPort.Name = "labelPort";
            this.labelPort.Size = new Size(20, 10);
            this.labelPort.TabIndex = 6;
            this.labelPort.Text = "Port:";

            //   
            // txtPort  
            //   
            this.txtPort.Location = new Point(415, 5);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new Size(75, 20);
            this.txtPort.TabIndex = 7;
            this.txtPort.Text = "4351";

            //   
            // btnConnect  
            //   
            this.btnConnect.Location = new Point(500, 5);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new Size(75, 25);
            this.btnConnect.TabIndex = 8;
            this.btnConnect.Text = "Build server";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.BtnBeginListenClick);

            //   
            // labelMember  
            //
            this.labelMember.AutoSize = true;
            this.labelMember.Location = new Point(5, 40);
            this.labelMember.Name = "labelMember";
            this.labelMember.Size = new Size(20, 10);
            this.labelMember.TabIndex = 9;
            this.labelMember.Text = "Members:";

            //   
            // listMember  
            //
            this.listMember.Location = new Point(5, 60);
            this.listMember.Name = "listTopic";
            this.listMember.Size = new Size(100, 375);
            this.listMember.TabIndex = 10;

            //   
            // labelChatroom  
            //
            this.labelChatroom.AutoSize = true;
            this.labelChatroom.Location = new Point(125, 40);
            this.labelChatroom.Name = "labelChatroom ";
            this.labelChatroom.Size = new Size(20, 10);
            this.labelChatroom.TabIndex = 11;
            this.labelChatroom.Text = "Chat room:";

            //   
            // txtMsg  
            //   
            this.txtMsg.Location = new Point(125, 60);
            this.txtMsg.Multiline = true;
            this.txtMsg.Name = "txtMsg";
            this.txtMsg.ScrollBars = ScrollBars.Vertical;
            this.txtMsg.Size = new Size(470, 250);
            this.txtMsg.TabIndex = 12;

            //   
            // labelMessage  
            //
            this.labelMessage.AutoSize = true;
            this.labelMessage.Location = new Point(125, 320);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new Size(20, 10);
            this.labelMessage.TabIndex = 13;
            this.labelMessage.Text = "Message:";

            //   
            // txtMsgSend  
            //   
            this.txtMsgSend.Location = new Point(125, 340);
            this.txtMsgSend.Name = "txtMsgSend";
            this.txtMsgSend.ScrollBars = ScrollBars.Vertical;
            this.txtMsgSend.Size = new Size(475, 20);
            this.txtMsgSend.TabIndex = 14;

            //   
            // labelTopic 
            //
            this.labelTopic.AutoSize = true;
            this.labelTopic.Location = new Point(125, 380);
            this.labelTopic.Name = "labelTopic";
            this.labelTopic.Size = new Size(20, 10);
            this.labelTopic.TabIndex = 15;
            this.labelTopic.Text = "Topic:";

            //   
            // txtTpcSend  
            //   
            this.txtTpcSend.Location = new Point(125, 400);
            this.txtTpcSend.Name = "txtTpcSend";
            this.txtTpcSend.ScrollBars = ScrollBars.Vertical;
            this.txtTpcSend.Size = new Size(475, 20);
            this.txtTpcSend.TabIndex = 16;

            //   
            // btnSendMsg  
            //   
            this.btnSendMsg.Location = new Point(440, 440);
            this.btnSendMsg.Name = "btnSendMsg";
            this.btnSendMsg.Size = new Size(75, 25);
            this.btnSendMsg.TabIndex = 17;
            this.btnSendMsg.Text = "Send";
            this.btnSendMsg.UseVisualStyleBackColor = true;
            this.btnSendMsg.Click += new System.EventHandler(this.BtnSendClick);


            //   
            // btnSendToAll  
            //   

            this.btnSendToAll.Location = new Point(530, 440);
            this.btnSendToAll.Name = "btnSendToAll";
            this.btnSendToAll.Size = new Size(75, 23);
            this.btnSendToAll.TabIndex = 5;
            this.btnSendToAll.Text = "SendToAll";
            this.btnSendToAll.UseVisualStyleBackColor = true;
            this.btnSendToAll.Click += new System.EventHandler(this.BtnSendToAllClick);

            //   
            // MainForm  
            //   
            this.AutoScaleDimensions = new SizeF(6F, 12F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(650, 500);
            this.Controls.Add(this.labelIP);
            this.Controls.Add(this.txtIP);
            this.Controls.Add(this.labelPort);
            this.Controls.Add(this.txtPort);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.labelMember);
            this.Controls.Add(this.listMember);
            this.Controls.Add(this.labelChatroom);
            this.Controls.Add(this.txtMsg);
            this.Controls.Add(this.labelMessage);
            this.Controls.Add(this.txtMsgSend);
            this.Controls.Add(this.labelTopic);
            this.Controls.Add(this.txtTpcSend);
            this.Controls.Add(this.btnSendMsg);
            this.Controls.Add(this.btnSendToAll);
            this.Name = "MainForm";
            this.Text = "Chat room server component";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
