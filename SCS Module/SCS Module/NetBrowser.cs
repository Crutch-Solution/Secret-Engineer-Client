using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.IO;
namespace SCS_Module
{
    public partial class NetBrowser : Form
    {
        public NetBrowser()
        {
            InitializeComponent();
        }
        TcpClient client;
        Internet inter;
        private void button1_Click(object sender, EventArgs e)
        {
            client = new TcpClient("192.168.0.104", 9097);
            inter = new Internet(client);
            inter.send(new Request() { name = textBox1.Text, pass = textBox2.Text });
            inter.send(new EngineerInputMessage() { requestType = RequestType.GetCategories });
            List<Category> res = inter.receive<List<Category>>();
            client.Close();
        }
    }
}
