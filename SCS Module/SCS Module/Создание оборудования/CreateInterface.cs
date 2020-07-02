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

namespace SCS_Module
{
    public partial class CreateInterface : Form
    {
        Creator father; bool isRepl = false;
        public CreateInterface(Creator a, Equipment.Compatibility com = null)
        {
            InitializeComponent();
            father = a;
            if (com != null)
            {
                isRepl = true;
                textBox3.Text = com.count.ToString();
                response = new ServerToExecutive() { interfaceTypes = new InterfaceType[] {com.interfaceType}.ToList(), success = true, responseType = RequestType.GetInterfaces };
                if (com.isMama) radioButton2.Checked = true;
                else radioButton1.Checked = true;
                rebuildSelected(0);
            }
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Equipment.Compatibility com = new Equipment.Compatibility();
            com.isMama = radioButton2.Checked;
            com.count = Convert.ToInt32(textBox3.Text);
            com.interfaceType = response.interfaceTypes[selected];
            if (isRepl) father.replacer(com);
            else father.add_interface(com);
            Close();
        }
        ServerToExecutive response;
        int selected;
        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //send to server
                Internet.send(new ExecutiveToServer() { requestType = RequestType.GetInterfaces, searchString = textBox1.Text });
                response = Internet.receive();
                if (response.success)
                {
                    panel1.Controls.Clear();
                    int cols = (panel1.Width - 1) / 101;
                    for (int i = 0; i < response.interfaceTypes.Count; i++)
                    {
                        PictureBox box = new PictureBox() { Width = 100, Height = 120 };
                        box.SizeMode = PictureBoxSizeMode.StretchImage;
                        Bitmap b = new Bitmap(100, 120);
                        Graphics g = Graphics.FromImage(b);
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        g.DrawImage(Image.FromStream(new MemoryStream(Convert.FromBase64String(response.interfaceTypes[i].image))), 0, 20, 100, 100);
                        g.DrawString(response.interfaceTypes[i].name, new Font("Arial", 8), Brushes.Black, 4, 4);
                        int i_buf = i;
                        box.MouseDown += (aa, bb) => rebuildSelected(i_buf);
                        panel1.Controls.Add(box);
                        box.Location = new Point(101 * (i % cols), 121 *( i / cols));
                        box.Image = b;
                        box.Refresh();
                    }
                    Refresh();
                }
            }
        }
        delegate void deg(object sender, MouseEventArgs e);
        public void rebuildSelected(int num)
        {
            selected = num;
            int cols = (panel1.Width - 1) / 101;
            panel1.Controls.Clear();
            for (int i = 0; i < response.interfaceTypes.Count; i++)
            {
                PictureBox box = new PictureBox() { Width = 100, Height = 120 };
                box.SizeMode = PictureBoxSizeMode.StretchImage;
                Bitmap b = new Bitmap(100, 120);
                Graphics g = Graphics.FromImage(b);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.DrawImage(Image.FromStream(new MemoryStream(Convert.FromBase64String(response.interfaceTypes[i].image))), 0, 20, 100, 100);
                g.DrawString(response.interfaceTypes[i].name, new Font("Arial", 8), Brushes.Black, 4, 4);
                if (i == num)
                    g.DrawLines(new Pen(Color.Green, 4), new PointF[] { new PointF() { X = 2, Y = 2 }, new PointF() { X = 98, Y = 2 }, new PointF() { X = 98, Y = 118 }, new PointF() { X = 2, Y = 118 }, new PointF() { X = 2, Y = 2 } });
                int i_buf = i;
                box.MouseDown += (aa, bb) => rebuildSelected(i_buf);
                panel1.Controls.Add(box);
                box.Location = new Point(101 * (i % cols), 121 * (i / cols));
                box.Image = b;
                box.Refresh();
            }
            Refresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.ShowDialog();
        }
    }
}
