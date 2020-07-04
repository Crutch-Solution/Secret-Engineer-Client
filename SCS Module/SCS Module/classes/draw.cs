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
        public System.Drawing.Point[] locations; //position on list
        public System.Drawing.Point[] scales;
        public int globalId, localID;
        public abstract void drawPlaceExp(ref string result);
        public abstract void drawConExp(ref string result);
        public abstract void drawBoxExp(ref string result);
        public abstract void drawStrExp(ref string result);



        public abstract void createVinosku();
        public abstract void drawPlace(Graphics g);
        public abstract void drawCon(Graphics g);
        public abstract void drawBox(Graphics g);
        public abstract void drawStr(Graphics g);
        public abstract bool inside(System.Drawing.Point a, int scheme);
        public abstract void move(System.Drawing.Point offset, int scheme);
        public abstract void offset(System.Drawing.Point offset, int scheme);
    }
}
