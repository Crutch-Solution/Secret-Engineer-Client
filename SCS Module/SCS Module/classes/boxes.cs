using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace SCS_Module
{
    public class boxes : drawer
    {
        public int units = -1;
        public float unitSize;
        public List<inboxes> equipInside = new List<inboxes>();
        public List<int> positions = new List<int>(); //from upper
        public List<int> unitsSeized = new List<int>();
        public override bool inside(Point a, int scheme)
        {
            bool result = false;
            if (a.X > locations[scheme].X && a.X < locations[scheme].X + scales[scheme].X)
                if (a.Y > locations[scheme].Y && a.Y < locations[scheme].Y + scales[scheme].Y)
                    result = true;
            return result;
        }
        public override void drawBox(Graphics g)
        {
            var target = Schemes_Editor.mainList.Find(x => x.id == globalId);
            if (units == -1)
                units = Convert.ToInt32(target.properties["Количество юнитов (шт)"]);
            int scheme = 2;
            g.DrawRectangle(Pens.Black, locations[scheme].X, locations[scheme].Y, scales[scheme].X, scales[scheme].Y);
            g.DrawRectangle(Pens.Black, locations[scheme].X + 20, locations[scheme].Y + 30, scales[scheme].X - 40, scales[scheme].Y - 60);
            unitSize = (scales[scheme].Y - 60) / (units * 1.0f);

            for (int j = 0; j < equipInside.Count; j++)
            {
                equipInside[j].locations[scheme] = new Point(locations[scheme].X + 20, (int)(locations[scheme].Y + 30 + positions[j] * unitSize));
                equipInside[j].scales[scheme] = new Point(scales[scheme].X - 40, (int)unitSize);
                equipInside[j].rebuildVinosku(scheme);
            }


            //по надписи
            StringFormat f = new StringFormat();
            f.Alignment = StringAlignment.Center;
            //найти название
            if (Schemes_Editor.mainList.Find(x => x.id == globalId) != null)
            {
                string roomName = Schemes_Editor.mainList.Find(x => x.id == globalId).name;
                g.DrawString(roomName, new Font("Arial", 10), Brushes.DarkRed, new RectangleF(locations[scheme].X, locations[scheme].Y - 30, scales[scheme].X, 30), f);

            }

            //по таблице
            //leftupper point
            int one = 30, two = 100, three = 150;
            int offs = 30;

            Point lu = new Point(locations[scheme].X + scales[scheme].X + offs, locations[scheme].Y - offs);
            //heaader
            g.DrawString("№ поз.", new Font("Arial", 8), Brushes.Black, new RectangleF(lu.X, lu.Y, one, 30), f);
            g.DrawString("Наименование", new Font("Arial", 8), Brushes.Black, new RectangleF(lu.X + one, lu.Y, two, 30), f);
            g.DrawString("Описание", new Font("Arial", 8), Brushes.Black, new RectangleF(lu.X + one + two, lu.Y, three, 30), f);
            //lines
            for (int i = 0; i < equipInside.Count + 2; i++)
                g.DrawLine(Pens.Black, lu.X, lu.Y + 30 * i, lu.X + one + two + three, lu.Y + 30 * i);

            g.DrawLine(Pens.Black, lu.X, lu.Y, lu.X, lu.Y + 30 * (equipInside.Count + 1));
            g.DrawLine(Pens.Black, lu.X + one, lu.Y, lu.X + one, lu.Y + 30 * (equipInside.Count + 1));
            g.DrawLine(Pens.Black, lu.X + one + two, lu.Y, lu.X + one + two, lu.Y + 30 * (equipInside.Count + 1));
            g.DrawLine(Pens.Black, lu.X + one + two + three, lu.Y, lu.X + one + two + three, lu.Y + 30 * (equipInside.Count + 1));

            f.LineAlignment = StringAlignment.Far;
            f.Alignment = StringAlignment.Center;
            for (int i = 1; i < equipInside.Count + 1; i++)
            {
                g.DrawString(i.ToString(), new Font("Arial", 7), Brushes.Black, new RectangleF(lu.X, lu.Y + 30 * i, one, 30), f);
                g.DrawString(equipInside[i - 1].labels[scheme], new Font("Arial", 7), Brushes.Black, new RectangleF(lu.X + one, lu.Y + 30 * i, two, 30), f);
                g.DrawString(Schemes_Editor.mainList.Find(x => x.id == equipInside[i - 1].globalId).description, new Font("Arial", 7), Brushes.Black, new RectangleF(lu.X + one + two, lu.Y + 30 * i, three, 30), f);
            }
            //мерная палка left doen start point
            lu = new Point(locations[scheme].X - 20,(int)( locations[scheme].Y + scales[scheme].Y - 30-unitSize));
            g.DrawLine(Pens.Black, locations[scheme].X - 20, locations[scheme].Y + scales[scheme].Y - 30, locations[scheme].X - 5, locations[scheme].Y + scales[scheme].Y - 30);
            for (int i = 0; i < units; i++)
            {
                g.DrawRectangle(Pens.Black, lu.X, lu.Y - unitSize * i, 3, unitSize);
                if ((i + 1) % 5 == 0)
                {
                   
                    g.DrawString((i + 1).ToString(), new Font("Arial", 7), Brushes.Black, lu.X + 4, lu.Y - unitSize * i);
                }
            }
            g.DrawLine(Pens.Black, locations[scheme].X - 20, locations[scheme].Y + scales[scheme].Y - 30-unitSize*units, locations[scheme].X - 5, locations[scheme].Y + scales[scheme].Y - 30 - unitSize * units);
        }

        public override void drawStr(Graphics g)
        {
            var target = Schemes_Editor.mainList.Find(x => x.id == globalId);
            if (units == -1)
                units = Convert.ToInt32(target.properties["Количество юнитов (шт)"]);
            int i = 3;
            g.DrawRectangle(Pens.Black, locations[i].X, locations[i].Y, scales[i].X, scales[i].Y);
            //    g.DrawRectangle(Pens.Black, locations[i].X + 20, locations[i].Y + 30, scales[i].X - 40, scales[i].Y - 60);
            unitSize = (scales[i].Y - 60) / (units * 1.0f);

            for (int j = 0; j < equipInside.Count; j++)
            {
                //equipInside[j].locations[i] = new Point(locations[i].X + 20, (int)(locations[i].Y + 30 + positions[j] * unitSize));
                //equipInside[j].scales[i] = new Point(scales[i].X - 40, (int)unitSize-30);////
                equipInside[j].locations[i] = new Point(locations[i].X + 20, (int)(locations[i].Y + 30 + j * unitSize));
                equipInside[j].scales[i] = new Point(scales[i].X - 40, (int)unitSize - 30);////

            }

            if (equipInside.Count > 1)
            {
                for (int j = 1; j < equipInside.Count; j++)
                {
                    g.DrawLine(new Pen(Color.Blue, 3), locations[i].X + scales[i].X / 2 - 13, locations[i].Y + 30 + unitSize * j - 28, locations[i].X + scales[i].X / 2 + 13, locations[i].Y + 30 + (unitSize * j - 2));
                    g.DrawLine(new Pen(Color.Blue, 3), locations[i].X + scales[i].X / 2 - 13, locations[i].Y + 30 + (unitSize) * j - 2, locations[i].X + scales[i].X / 2 + 13, locations[i].Y + 30 + (unitSize * j - 28));
                }
            }

            //по надписи
            StringFormat f = new StringFormat();
            f.Alignment = StringAlignment.Center;
            //найти название
            if (Schemes_Editor.mainList.Find(x => x.id == globalId) != null)
            {
                string roomName = Schemes_Editor.mainList.Find(x => x.id == globalId).name;
                g.DrawString(roomName, new Font("Arial", 10), Brushes.DarkRed, new RectangleF(locations[i].X, locations[i].Y - 30, scales[i].X, 30), f);

            }
        }

        public override void move(Point offset, int scheme)
        {
            locations[scheme] = offset;
        }

        public override void createVinosku(int index)
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

        public override void drawCon(Graphics g)
        {
            int sheetIndex = 1;
            g.DrawRectangle(Pens.Black, locations[sheetIndex].X, locations[sheetIndex].Y, scales[sheetIndex].X, scales[sheetIndex].Y);
            //по надписи
            StringFormat f = new StringFormat();
            f.Alignment = StringAlignment.Center;
            //найти название
            if (Schemes_Editor.mainList.Find(x => x.id == globalId) != null)
            {
                string roomName = Schemes_Editor.mainList.Find(x => x.id == globalId).name;
                g.DrawString(roomName, new Font("Arial", 10), Brushes.DarkRed, new RectangleF(locations[sheetIndex].X, locations[sheetIndex].Y - 30, scales[sheetIndex].X, 30), f);

            }
        }

        public override void offset(Point offset, int scheme)
        {

        }

        public override void drawPlaceExp(ref string result)
        {

        }

        public override void drawConExp(ref string result)
        {

        }

        public override void drawBoxExp(ref string result)
        {
            var target = Schemes_Editor.mainList.Find(x => x.id == globalId);
            if (units == -1)
                units = Convert.ToInt32(target.properties["Количество юнитов (шт)"]);

            int scheme = 2;



            result += AutocadExport.drawrect(new Rectangle(locations[scheme].X, locations[scheme].Y, scales[scheme].X, scales[scheme].Y));
            result += AutocadExport.drawrect(new Rectangle(locations[scheme].X + 20, locations[scheme].Y + 30, scales[scheme].X - 40, scales[scheme].Y - 60));

            unitSize = (scales[scheme].Y - 60) / (units * 1.0f);


            //найти название
            if (Schemes_Editor.mainList.Find(x => x.id == globalId) != null)
            {
                string boxName = Schemes_Editor.mainList.Find(x => x.id == globalId).name;

                result += AutocadExport.drawText(new Rectangle(locations[scheme].X, locations[scheme].Y - 30, scales[scheme].X, 30), boxName);

            }

            //по таблице
            //leftupper point
            int one = 30, two = 100, three = 150;
            int total = one + two + three;
            int heigth = 30;

           Point lu = new Point(locations[scheme].X+scales[scheme].X+30, locations[scheme].Y);
            //heaader
            result += AutocadExport.drawText(lu.X, lu.Y, one, heigth, "Поз");
            result += AutocadExport.drawText(lu.X+one, lu.Y, two, heigth, "Наименование");
            result += AutocadExport.drawText(lu.X+one+two, lu.Y, three, heigth, "Описание");

            for (int i = 0; i < equipInside.Count + 2; i++)
                result += AutocadExport.drawLine(lu.X, lu.Y + heigth * i, lu.X + total, lu.Y + heigth * i);

            result += AutocadExport.drawLine(lu.X, lu.Y, lu.X, lu.Y+heigth* (equipInside.Count + 1));
            result += AutocadExport.drawLine(lu.X+one, lu.Y, lu.X + one, lu.Y + heigth * (equipInside.Count + 1));
            result += AutocadExport.drawLine(lu.X + one+two, lu.Y, lu.X + one + two, lu.Y + heigth * (equipInside.Count + 1));
            result += AutocadExport.drawLine(lu.X + one + two+three, lu.Y, lu.X + one + two + three, lu.Y + heigth * (equipInside.Count + 1));

            for (int i = 1; i < equipInside.Count + 1; i++)
            {
                result += AutocadExport.drawText(lu.X, lu.Y + i*heigth, one, heigth, i.ToString());
                result += AutocadExport.drawText(lu.X + one, lu.Y + i * heigth, two, heigth, equipInside[i - 1].labels[scheme]);
                result += AutocadExport.drawText(lu.X + one + two, lu.Y + i * heigth, three, heigth, Schemes_Editor.mainList.Find(x => x.id == equipInside[i - 1].globalId).description);



                //    g.DrawString(j.ToString(), new Font("Arial", 7), Brushes.Black, new RectangleF(lu.X, lu.Y + 30 * i, one, 30), f);
                //    g.DrawString(equipInside[j - 1].boxLabel, new Font("Arial", 7), Brushes.Black, new RectangleF(lu.X + one, lu.Y + 30 * i, two, 30), f);
                //    g.DrawString(Schemes_Editor.mainList.Find(x => x.id == equipInside[j - 1].globalId).description, new Font("Arial", 7), Brushes.Black, new RectangleF(lu.X + one + two, lu.Y + 30 * i, three, 30), f);
            }
            //int num = 1;
            //foreach (var j in equipInside)
            //{

            //    if (j.vinoska != null)
            //    {
            //        result += $"pline {j.vinoska.startPoint.X},{-j.vinoska.startPoint.Y} {j.vinoska.vertex1.X},{-j.vinoska.vertex1.Y}  {j.vinoska.vertex2.X},{-j.vinoska.vertex2.Y} \r\n";
            //        result += $"text {j.vinoska.vertex2.X},{-j.vinoska.vertex2.Y} 0 {num}\r\n";
            //    }
            //    num++;
            //}
        }

        public override void drawStrExp(ref string result)
        {

        }

        public override void rebuildVinosku(int a)
        {
            throw new NotImplementedException();
        }
    }
}
