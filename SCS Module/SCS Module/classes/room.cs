using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCS_Module
{
    public class Room
    {
        int offsetConnection = 3000;
        public Rectangle[] locations = new Rectangle[] { new Rectangle(0, 0, 200, 200), new Rectangle(0, 0, 200, 200), new Rectangle(), new System.Drawing.Rectangle(0, 0, 200, 200) };
        public string[] labels = null;
        public void move(Point a, int index)
        {
            locations[index] = new System.Drawing.Rectangle(a.X, a.Y, locations[index].Width, locations[index].Height);
        }
        public void drawExp(ref string a, int index)
        {
            if (index == 2) return;
            a += AutocadExport.drawrect(new Rectangle(locations[index].X+ offsetConnection*index, locations[index].Y, locations[index].Width, locations[index].Height));
            a += AutocadExport.drawText(new RectangleF(locations[index].X + offsetConnection * index, locations[index].Y - 30, locations[index].Width, 30), labels[index]);
        }
        public void draw(Graphics g, int index)
        {
            g.DrawRectangle(Pens.Black, locations[index]);
            StringFormat f = new StringFormat();
            f.Alignment = StringAlignment.Center;

            g.DrawString(labels[index], new Font("Arial", 14), Brushes.DarkRed, new RectangleF(locations[index].X, locations[index].Y - 30, locations[index].Width, 30), f);
        }
        public bool inside(Point a, int index)
        {
            return locations[index].Contains(a);
        }
    }
}
