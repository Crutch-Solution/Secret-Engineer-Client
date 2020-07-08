using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SCS_Module
{
    public abstract class drawer
    {
        public bool hasFamily = true;
        public Vinoska vinoska;
        public Point[] locations; //position on list
        public Point[] scales;
        public int globalId, localID;
        public abstract void drawPlaceExp(ref string result);
        public abstract void drawConExp(ref string result);
        public abstract void drawBoxExp(ref string result);
        public abstract void drawStrExp(ref string result);

        public abstract void rebuildVinosku();

        public abstract void createVinosku();
        public abstract void drawPlace(Graphics g);
        public abstract void drawCon(Graphics g);
        public abstract void drawBox(Graphics g);
        public abstract void drawStr(Graphics g);
        public abstract bool inside(Point a, int scheme);
        public abstract void move(Point offset, int scheme);
        public abstract void offset(Point offset, int scheme);
    }
}
