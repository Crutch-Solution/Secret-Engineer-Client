using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SCS_Module
{
    public partial class Box_creator : Form
    {
        public Box_creator()
        {
            InitializeComponent();
            units = new bool[10].ToList();
            equips = new equip[] {
                new equip() { label = "Cisco 1234", unit_size = 1 },
                new equip() { label = "Какой-то блок питания", unit_size = 3 }
            }.ToList();
        }
        int units_number; string box_ser;
        public class equip
        {
            public string label, name, ser;
            public int id, unit_size;
        }
        List<bool> units = new List<bool>();
        List<equip> equips = new List<equip>();
        private void Box_creator_Resize(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new Equipment_selector().ShowDialog();
        }
    }
}
