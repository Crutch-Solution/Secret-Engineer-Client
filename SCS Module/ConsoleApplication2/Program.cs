using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SCS_Module;
namespace ConsoleApplication2
{
    class Program
    {
        [STAThread]
      public  static void Main(string[] args)
        {
            Internet.Establish();
            (new SCS_Module.Schemes_Editor()).ShowDialog();
        }
    }
}
