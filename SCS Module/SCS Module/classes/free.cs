using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace SCS_Module
{
    public class free : drawer
    {
        public List<int> seized = new List<int>();
        public override void createVinosku(int index)
        {
            ////
        }

        public override void drawBox(Graphics g)
        {
            /////////////////////
        }

        public override void drawBoxExp(ref string result)
        {

        }

        public override void drawCon(Graphics g)
        {
            if (seized.Count == 0)
                foreach (var i in Schemes_Editor.mainList.Find(x => x.id == globalId).compatibilities)
                    seized.Add(0);


            int localSheetIndex = 1;
            List<Equipment.VectorPic> list = new List<Equipment.VectorPic>();
            if (Schemes_Editor.mainList.Find(x => x.id == globalId).inPlacementScheme != null)
                list.Add(Schemes_Editor.mainList.Find(x => x.id == globalId).inPlacementScheme.copy());
            else
                list.Add(null);

            if (Schemes_Editor.mainList.Find(x => x.id == globalId).inConnectionScheme != null)
                list.Add(Schemes_Editor.mainList.Find(x => x.id == globalId).inConnectionScheme.copy());
            else
                list.Add(null);

            if (Schemes_Editor.mainList.Find(x => x.id == globalId).inBox != null)
                list.Add(Schemes_Editor.mainList.Find(x => x.id == globalId).inBox.copy());
            else
                list.Add(null);

            if (Schemes_Editor.mainList.Find(x => x.id == globalId).inStructural != null)
                list.Add(Schemes_Editor.mainList.Find(x => x.id == globalId).inStructural.copy());
            else
                list.Add(null);

            if (list[localSheetIndex] != null)
            {
                Equipment.Point WidthHeight = list[localSheetIndex].GetProp();
                float Xprop = WidthHeight.X / (scales[localSheetIndex].X * 1.0f),
                    Yprop = WidthHeight.Y / (scales[localSheetIndex].Y * 1.0f);
                list[localSheetIndex].divide(Xprop, Yprop);

                foreach (var j in list[localSheetIndex].circles)
                    g.DrawEllipse(Pens.Black, locations[localSheetIndex].X + (float)j.center.X - (float)j.radiusX, locations[localSheetIndex].Y + (float)j.center.Y - (float)j.radiusY, (float)j.radiusX * 2, (float)j.radiusY * 2);

                foreach (var j in list[localSheetIndex].polyLines)
                {
                    for (int k = 0; k < j.Count - 1; k++)
                    {
                        g.DrawLine(Pens.Black, j[k].X + locations[localSheetIndex].X, j[k].Y + locations[localSheetIndex].Y, j[k + 1].X + locations[localSheetIndex].X, j[k + 1].Y + locations[localSheetIndex].Y);
                    }
                }
            }
            //по надписи
            StringFormat f = new StringFormat();
            f.Alignment = StringAlignment.Center;
            //найти название
            if (Schemes_Editor.mainList.Find(x => x.id == globalId) != null)
            {
                string roomName = Schemes_Editor.mainList.Find(x => x.id == globalId).name;
                g.DrawString(roomName, new Font("Arial", 10), Brushes.DarkRed, new RectangleF(locations[localSheetIndex].X, locations[localSheetIndex].Y - 30, scales[localSheetIndex].X, 30), f);

            }
        }

        public override void drawConExp(ref string result)
        {

        }

        public override void drawPlace(Graphics g)
        {
            int localSheetIndex = 0;
            List<Equipment.VectorPic> list = new List<Equipment.VectorPic>();
            if (Schemes_Editor.mainList.Find(x => x.id == globalId).inPlacementScheme != null)
                list.Add(Schemes_Editor.mainList.Find(x => x.id == globalId).inPlacementScheme.copy());
            else
                list.Add(null);

            if (Schemes_Editor.mainList.Find(x => x.id == globalId).inConnectionScheme != null)
                list.Add(Schemes_Editor.mainList.Find(x => x.id == globalId).inConnectionScheme.copy());
            else
                list.Add(null);

            if (Schemes_Editor.mainList.Find(x => x.id == globalId).inBox != null)
                list.Add(Schemes_Editor.mainList.Find(x => x.id == globalId).inBox.copy());
            else
                list.Add(null);

            if (Schemes_Editor.mainList.Find(x => x.id == globalId).inStructural != null)
                list.Add(Schemes_Editor.mainList.Find(x => x.id == globalId).inStructural.copy());
            else
                list.Add(null);

            if (list[localSheetIndex] != null)
            {
                Equipment.Point WidthHeight = list[localSheetIndex].GetProp();
                float Xprop = WidthHeight.X / (scales[localSheetIndex].X * 1.0f),
                    Yprop = WidthHeight.Y / (scales[localSheetIndex].Y * 1.0f);
                list[localSheetIndex].divide(Xprop, Yprop);

                foreach (var j in list[localSheetIndex].circles)
                    g.DrawEllipse(Pens.Black, locations[localSheetIndex].X + (float)j.center.X - (float)j.radiusX, locations[localSheetIndex].Y + (float)j.center.Y - (float)j.radiusY, (float)j.radiusX * 2, (float)j.radiusY * 2);

                foreach (var j in list[localSheetIndex].polyLines)
                {
                    for (int k = 0; k < j.Count - 1; k++)
                    {
                        g.DrawLine(Pens.Black, j[k].X + locations[localSheetIndex].X, j[k].Y + locations[localSheetIndex].Y, j[k + 1].X + locations[localSheetIndex].X, j[k + 1].Y + locations[localSheetIndex].Y);
                    }
                }
            }
            //по надписи
            StringFormat f = new StringFormat();
            f.Alignment = StringAlignment.Center;
            //найти название
            if (Schemes_Editor.mainList.Find(x => x.id == globalId) != null)
            {
                string roomName = Schemes_Editor.mainList.Find(x => x.id == globalId).name;
                g.DrawString(roomName, new Font("Arial", 10), Brushes.DarkRed, new RectangleF(locations[localSheetIndex].X, locations[localSheetIndex].Y - 30, scales[localSheetIndex].X, 30), f);

            }
        }

        public override void drawPlaceExp(ref string result)
        {

        }

        public override void drawStr(Graphics g)
        {
            int localSheetIndex = 3;
            List<Equipment.VectorPic> list = new List<Equipment.VectorPic>();
            if (Schemes_Editor.mainList.Find(x => x.id == globalId).inPlacementScheme != null)
                list.Add(Schemes_Editor.mainList.Find(x => x.id == globalId).inPlacementScheme.copy());
            else
                list.Add(null);

            if (Schemes_Editor.mainList.Find(x => x.id == globalId).inConnectionScheme != null)
                list.Add(Schemes_Editor.mainList.Find(x => x.id == globalId).inConnectionScheme.copy());
            else
                list.Add(null);

            if (Schemes_Editor.mainList.Find(x => x.id == globalId).inBox != null)
                list.Add(Schemes_Editor.mainList.Find(x => x.id == globalId).inBox.copy());
            else
                list.Add(null);

            if (Schemes_Editor.mainList.Find(x => x.id == globalId).inStructural != null)
                list.Add(Schemes_Editor.mainList.Find(x => x.id == globalId).inStructural.copy());
            else
                list.Add(null);

            if (list[localSheetIndex] != null)
            {
                Equipment.Point WidthHeight = list[localSheetIndex].GetProp();
                float Xprop = WidthHeight.X / (scales[localSheetIndex].X * 1.0f),
                    Yprop = WidthHeight.Y / (scales[localSheetIndex].Y * 1.0f);
                list[localSheetIndex].divide(Xprop, Yprop);

                foreach (var j in list[localSheetIndex].circles)
                    g.DrawEllipse(Pens.Black, locations[localSheetIndex].X + (float)j.center.X - (float)j.radiusX, locations[localSheetIndex].Y + (float)j.center.Y - (float)j.radiusY, (float)j.radiusX * 2, (float)j.radiusY * 2);

                foreach (var j in list[localSheetIndex].polyLines)
                {
                    for (int k = 0; k < j.Count - 1; k++)
                    {
                        g.DrawLine(Pens.Black, j[k].X + locations[localSheetIndex].X, j[k].Y + locations[localSheetIndex].Y, j[k + 1].X + locations[localSheetIndex].X, j[k + 1].Y + locations[localSheetIndex].Y);
                    }
                }
            }
            //по надписи
            StringFormat f = new StringFormat();
            f.Alignment = StringAlignment.Center;
            //найти название
            if (Schemes_Editor.mainList.Find(x => x.id == globalId) != null)
            {
                string roomName = Schemes_Editor.mainList.Find(x => x.id == globalId).name;
                g.DrawString(roomName, new Font("Arial", 10), Brushes.DarkRed, new RectangleF(locations[localSheetIndex].X, locations[localSheetIndex].Y - 30, scales[localSheetIndex].X, 30), f);

            }
        }

        public override void drawStrExp(ref string result)
        {

        }

        public override bool inside(Point a, int scheme)
        {
            bool result = false;
            if (a.X > locations[scheme].X && a.X < locations[scheme].X + scales[scheme].X)
                if (a.Y > locations[scheme].Y && a.Y < locations[scheme].Y + scales[scheme].Y)
                    result = true;
            return result;
        }

        public override void move(Point offset, int scheme)
        {
            locations[scheme] = offset;
        }

        public override void offset(Point offset, int scheme)
        {

        }

        public override void rebuildVinosku(int a)
        {
            throw new NotImplementedException();
        }
    }
}
