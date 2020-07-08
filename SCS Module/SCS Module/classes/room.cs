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
        public System.Drawing.Rectangle[] locations = new System.Drawing.Rectangle[] { new System.Drawing.Rectangle(0, 0, 200, 200), new System.Drawing.Rectangle(0, 0, 200, 200), new System.Drawing.Rectangle(), new System.Drawing.Rectangle(0, 0, 200, 200) };
        public string roomName;
        public void move(Point a, int index)
        {
            locations[index] = new System.Drawing.Rectangle(a.X, a.Y, locations[index].Width, locations[index].Height);
        }
        public void draw(Graphics g, int index)
        {
            g.DrawRectangle(Pens.Red, locations[index]);
            StringFormat f = new StringFormat();
            f.Alignment = StringAlignment.Center;

            g.DrawString(roomName, new Font("Arial", 14), Brushes.DarkRed, new RectangleF(locations[index].X, locations[index].Y - 30, locations[index].Width, 30), f);
        }
        public bool inside(Point a, int index)
        {
            return locations[index].Contains(a);
        }
    }
}
