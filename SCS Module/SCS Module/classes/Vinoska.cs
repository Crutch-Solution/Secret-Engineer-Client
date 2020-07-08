using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SCS_Module
{
    public class Vinoska
    {
        public string text;
        public Point vertex1, vertex2, startPoint;
        public Vinoska(string a, Point start, Point b, Point c)
        {
            text = a;
            vertex1 = b;
            vertex2 = c;
            startPoint = start;
        }
    }
}
