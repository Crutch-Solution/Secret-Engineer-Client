using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SCS_Module
{
    class AutocadExport
    {
        static public string drawLine(Point a, Point b)
        {
            string result = "";
            result = $"pline {a.X},{-a.Y} {b.X},{-b.Y} \r\n";
            return result;
        }
        static public string drawLine(Equipment.Point a, Equipment.Point b)
        {
            string result = "";
            result = $"pline {a.X},{-a.Y} {b.X},{-b.Y} \r\n";
            return result;
        }
        static public string drawLine(float x1, float y1, float x2, float y2)
        {
            string result = "";
            result = $"pline {x1},{-y1} {x2},{-y2} \r\n";
            return result;
        }
        static public string drawText(RectangleF rect, string text)
        {
            float Xoffset = 0, Yoffset = 0;
            Xoffset =(int)( (rect.Width - (text.Length*8)) / 2);
            Yoffset = (rect.Height - 10) / 2+10;
            return $"text {rect.X + Xoffset},{-rect.Y- Yoffset} 0 {text}\r\n";
        }
        static public string drawText(float a, float b, float c, float d, string text)
        {
            return drawText(new RectangleF(a, b, c, d), text);
        }
        static public string drawrect(Rectangle rect)
        {
            string result = "";
            result += drawLine(rect.X, rect.Y, rect.X + rect.Width, +rect.Y);
            result += drawLine(rect.X + rect.Width, rect.Y, rect.X + rect.Width, +rect.Y+rect.Height);
            result += drawLine(rect.X + rect.Width, rect.Y + rect.Height, rect.X, +rect.Y + rect.Height);
            result += drawLine(rect.X, rect.Y + rect.Height, rect.X, rect.Y);
            return result;
        }
    }
}
