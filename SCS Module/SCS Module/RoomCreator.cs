﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Equipment_Engineer
{
    public partial class RoomCreator : Form
    {
        public string roomName;
        public RoomCreator()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                roomName = textBox1.Text;
                DialogResult = DialogResult.OK;
            }
        }
    }
}
