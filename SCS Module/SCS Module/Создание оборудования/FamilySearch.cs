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
    public partial class FamilySearch : Form
    {
        public FamilySearch()
        {
            InitializeComponent();
        }
        public Equipment equipment;
        ServerToExecutive response;
        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                Internet.send(new ExecutiveToServer() { requestType = RequestType.GetEquipmentWithFamilies, searchString = textBox1.Text });
                response = Internet.receive();
                if (response.success)
                {
                    for(int i = 0; i < response.equipments.Count; i++)
                    {
                        TableLayoutPanel panel = new TableLayoutPanel();
                        panel.RowCount = 2;
                        panel.ColumnCount = 3;
                        panel.RowStyles.Clear();
                        for (int j = 0; j < 2; j++)
                            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
                        panel.ColumnStyles.Clear();
                        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
                        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
                        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));

                        PictureBox box = new PictureBox() { Width = 150, Height = 150 };
                        box.Image = Image.FromStream(new MemoryStream(Convert.FromBase64String(response.equipments[i].preview)));
                        panel.Controls.Add(box, 0, 0);
                        panel.SetRowSpan(box, 2);

                        Label name = new Label() { Text = response.equipments[i].name, Dock = DockStyle.Fill };
                        panel.Controls.Add(name, 1, 0);

                        Label desc = new Label() { Text = response.equipments[i].description, Dock = DockStyle.Fill };
                        panel.Controls.Add(desc, 1, 1);

                        Button add = new Button() { Text = "Добавить" };
                        int buff = i;
                        add.MouseClick += (a, b) => 
                        {
                            equipment = response.equipments[i];
                            DialogResult = DialogResult.OK;
                            Close();
                        };
                        panel.Controls.Add(add, 2, 0);

                        panel1.Controls.Add(panel);
                        panel.Location = new Point(0, 151 * i);
                        Refresh();
                    }
                }
            }
        }
    }
}
