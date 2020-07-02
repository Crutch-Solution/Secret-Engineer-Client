using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;

namespace SCS_Module
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string image = "";
        private void button1_Click(object sender, EventArgs e)
        {
            Internet.send(new ExecutiveToServer() { requestType = RequestType.AddInterface, interfaceType = new InterfaceType() { name = textBox1.Text, image = image } });
            if (Internet.receive().success)
            {
                MessageBox.Show("Получилось) теперь пробуйте найти свой порт");
                Close();
            }
            else
            {
                MessageBox.Show("Не получилось");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            if (file.ShowDialog() == DialogResult.OK)
            {
                image = Convert.ToBase64String(File.ReadAllBytes(file.FileName));
                label3.Text = $"Выбрано {file.FileName}";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsImage())
            {
                MemoryStream stream = new MemoryStream();
                Image img = Clipboard.GetImage();
                img.Save(stream,  ImageFormat.Png);
                image = Convert.ToBase64String(stream.ToArray());
                if (Clipboard.ContainsText()) label3.Text = $"Выбрано {Clipboard.GetText()}";
                else label3.Text = $"Выбран какой-то файл из буфера";
            }
            else MessageBox.Show("В буфере пусто");
        }
    }
}
