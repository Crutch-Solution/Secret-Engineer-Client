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
    public partial class Searcher : Form
    {
        public Searcher()
        {
            InitializeComponent();
        }
        public Equipment result;
        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void Searcher_Resize(object sender, EventArgs e)
        {
            panel2.Controls.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                TableLayoutPanel panel = new TableLayoutPanel();
                panel.Width = panel2.Width;
                panel.Height = 120;
                panel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
                panel.RowCount = 2;
                panel.ColumnCount = 3;
                panel.RowStyles.Clear();
                for (int j = 0; j < 2; j++)
                    panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
                panel.ColumnStyles.Clear();
                panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
                panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
                panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));

                PictureBox box = new PictureBox() { Width = 120, Height = 120 };
                box.Margin = new Padding(0);
                box.SizeMode = PictureBoxSizeMode.StretchImage;
                try
                {
                    box.Image = Image.FromStream(new MemoryStream(Convert.FromBase64String(list[i].preview)));
                }
                catch (Exception ex) { }
                panel.Controls.Add(box, 0, 0);
                panel.SetRowSpan(box, 2);


                Label name = new Label() { Text = list[i].name, Dock = DockStyle.Fill, TextAlign = ContentAlignment.TopCenter };
                panel.Controls.Add(name, 1, 0);

                Label desc = new Label() { Text = list[i].description, Dock = DockStyle.Fill, TextAlign = ContentAlignment.TopCenter };
                panel.Controls.Add(desc, 1, 1);

                Button add = new Button() { Text = "Добавить", Width = 130, Anchor = AnchorStyles.None };
                int buff = i;
                add.MouseClick += (a, b) => { listAddClick(buff); };
                panel.Controls.Add(add, 2, 0);

                Button adv = new Button() { Text = "Подробности", Width = 130, Anchor = AnchorStyles.None };
                adv.MouseClick += (a, b) => { listAdvClick(buff); };
                panel.Controls.Add(adv, 2, 1);

                panel2.Controls.Add(panel);
                panel.Location = new Point(0, 121*i);
                panel.Refresh();
            }
        }

        public void listAddClick(int index)
        {
            Internet.send(new ExecutiveToServer() { requestType = RequestType.GetThird, searchString = textBox2.Text, equipmentId = list[index].id });
            ServerToExecutive response = Internet.receive();
            result = response.equipments[0];
            
            DialogResult = DialogResult.OK;
            Close();
        }
        public void listAdvClick(int index)
        {

        }

        private void добавитьНовоеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Creator creator = new Creator();
            if (creator.ShowDialog() == DialogResult.OK)
            {
         //       Internet.send(new ExecutiveToServer() { requestType = RequestType.GetThird, searchString = textBox2.Text});
            }
        }
        public List<Equipment> list = new List<Equipment>();
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                Internet.send(new ExecutiveToServer() { requestType = RequestType.GetFirst, searchString = textBox2.Text });
                ServerToExecutive response = Internet.receive();
                if (response.success)
                {
                    list = response.equipments;
                    Searcher_Resize(null, null);
                }
            }catch(Exception ex)
            {

            }
        }
    }
}
