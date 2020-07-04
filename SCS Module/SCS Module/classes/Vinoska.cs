using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCS_Module
{
    public class Vinoska
    {
        public string text;
        public System.Drawing.Point vertex1, vertex2, startPoint;
        public Vinoska(string a, System.Drawing.Point start, System.Drawing.Point b, System.Drawing.Point c)
        {
            text = a;
            vertex1 = b;
            vertex2 = c;
            startPoint = start;
        }
    }
}
