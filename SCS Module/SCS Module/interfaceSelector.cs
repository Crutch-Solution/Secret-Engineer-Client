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
    public partial class interfaceSelector : Form
    {
        public int selectedId = 0;
        public interfaceSelector(List<Equipment.Compatibility> list, Equipment.Compatibility target1, Equipment.Compatibility target2 = null)
        {

            InitializeComponent();
            if (target2 == null) {
                foreach (var i in list.Where(x => x.isMama != target1.isMama && x.interfaceType.id == target1.interfaceType.id))
                {
                    Button b = new Button();
                    b.Text = i.interfaceType.name;
                    b.Dock = DockStyle.Top;
                    b.Height = 35;
                    int buffId = i.interfaceType.id;
                    b.MouseClick += (c, d) =>
                    {
                        selectedId = buffId;
                        DialogResult = DialogResult.OK;
                        Close();
                    };

                    panel1.Controls.Add(b);
                }
            }
            else
            {
                foreach (var i in list.Where(x => x.isMama != target1.isMama && (x.interfaceType.id == target1.interfaceType.id || x.interfaceType.id == target2.interfaceType.id)))
                {
                    Button b = new Button();
                    b.Text = i.interfaceType.name;
                    b.Dock = DockStyle.Top;
                    b.Height = 35;
                    int buffId = i.interfaceType.id;
                    b.MouseClick += (c, d) =>
                    {
                        selectedId = buffId;
                        DialogResult = DialogResult.OK;
                        Close();
                    };

                    panel1.Controls.Add(b);
                }
            }
        }
    }
}
