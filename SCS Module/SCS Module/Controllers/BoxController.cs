using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SCS_Module
{
    public enum modeShkaf
    {
        doNothing_SCALEMODE,
        doNothing_NOSCALEMODE,
        dragShkafnoe,
        dragShkaf,
        scaleSomething,
        moveVinosku,
        constantMove,
        editWire
    }
    static public class BoxController
    {
        static public Schemes_Editor father;
    
        static public int pointNum;
        static public dynamic movable;
        static public Point Prev;
        static public modeShkaf Mode = modeShkaf.doNothing_NOSCALEMODE;
        //Для перетаскивания шкафного
        static public bool hasRect = false;
        static public Rectangle rect;
        static public boxes boxForInser;
        static public int indexToSurround = -1;
        static public int indexToInsertInBox;
        static public Point scalePoint = new Point(-1, -1);
        //
        //Для перетаскивания
        static public int moveTargetIndex = -1;
        //
       static int localSheet = 2;
        static public void draw(bool isNeed = true)
        {
            try
            {
                if (isNeed)
                    Schemes_Editor.gr[localSheet].Clear(Color.LightGray);

                foreach (var i in Schemes_Editor.mainWorkList)
                    i.drawBox(Schemes_Editor.gr[localSheet]);



                switch (Mode)
                {
                    case modeShkaf.doNothing_NOSCALEMODE:
                        if (indexToSurround != -1)
                        {
                            //if (Schemes_Editor.mainWorkList[indexToSurround] is wire_s)
                            //{

                            //}
                            //else
                            //{
                            //    Schemes_Editor.gr[Schemes_Editor.sheetIndex].DrawRectangle(Pens.Blue, Schemes_Editor.mainWorkList[indexToSurround].locations[Schemes_Editor.sheetIndex].X - 1, Schemes_Editor.mainWorkList[indexToSurround].locations[Schemes_Editor.sheetIndex].Y - 1, Schemes_Editor.mainWorkList[indexToSurround].scales[Schemes_Editor.sheetIndex].X + 2, Schemes_Editor.mainWorkList[indexToSurround].scales[Schemes_Editor.sheetIndex].Y + 2);
                            //}
                        }
                        break;




                    case modeShkaf.doNothing_SCALEMODE:
                        if (scalePoint.X != -1)
                        {
                            Schemes_Editor.gr[Schemes_Editor.sheetIndex].DrawEllipse(new Pen(Color.Red, 2), scalePoint.X - 5, scalePoint.Y - 5, 10, 10);
                        }
                        foreach (var i in Schemes_Editor.mainWorkList)
                        {
                            if (i is inboxes && ((inboxes)i).inbox) continue;
                            Schemes_Editor.gr[Schemes_Editor.sheetIndex].DrawRectangle(Pens.Blue, i.locations[Schemes_Editor.sheetIndex].X - 1, i.locations[Schemes_Editor.sheetIndex].Y - 1, i.scales[Schemes_Editor.sheetIndex].X + 2, i.scales[Schemes_Editor.sheetIndex].Y + 2);
                        }
                        break;


                    case modeShkaf.dragShkafnoe:
                        Schemes_Editor.gr[Schemes_Editor.sheetIndex].DrawRectangle(Pens.Blue, Schemes_Editor.mainWorkList[indexToSurround].locations[Schemes_Editor.sheetIndex].X - 1, Schemes_Editor.mainWorkList[indexToSurround].locations[Schemes_Editor.sheetIndex].Y - 1, Schemes_Editor.mainWorkList[indexToSurround].scales[Schemes_Editor.sheetIndex].X + 2, Schemes_Editor.mainWorkList[indexToSurround].scales[Schemes_Editor.sheetIndex].Y + 2);
                        if (hasRect)
                            Schemes_Editor.gr[Schemes_Editor.sheetIndex].DrawRectangle(Pens.Red, rect);
                        break;

                    case modeShkaf.dragShkaf:
                        Schemes_Editor.gr[Schemes_Editor.sheetIndex].DrawRectangle(Pens.Blue, Schemes_Editor.mainWorkList[indexToSurround].locations[Schemes_Editor.sheetIndex].X - 1, Schemes_Editor.mainWorkList[indexToSurround].locations[Schemes_Editor.sheetIndex].Y - 1, Schemes_Editor.mainWorkList[indexToSurround].scales[Schemes_Editor.sheetIndex].X + 2, Schemes_Editor.mainWorkList[indexToSurround].scales[Schemes_Editor.sheetIndex].Y + 2);
                        break;

                    case modeShkaf.scaleSomething:
                        Schemes_Editor.gr[Schemes_Editor.sheetIndex].DrawRectangle(Pens.Blue, Schemes_Editor.mainWorkList[moveTargetIndex].locations[Schemes_Editor.sheetIndex].X - 1, Schemes_Editor.mainWorkList[moveTargetIndex].locations[Schemes_Editor.sheetIndex].Y - 1, Schemes_Editor.mainWorkList[moveTargetIndex].scales[Schemes_Editor.sheetIndex].X + 2, Schemes_Editor.mainWorkList[moveTargetIndex].scales[Schemes_Editor.sheetIndex].Y + 2);
                        Schemes_Editor.gr[Schemes_Editor.sheetIndex].DrawEllipse(new Pen(Color.Red, 2), scalePoint.X - 5, scalePoint.Y - 5, 10, 10);
                        break;

                }
                //surrounding_no_selected
                for (int i = 0; i < 4; i++)
                {
                    Schemes_Editor.sheets[i].Image = Schemes_Editor.bitmaps[i];
                    Schemes_Editor.sheets[i].Refresh();
                }
            }
            catch (Exception exp) { }
        }



















        private static void handler(object sender, EventArgs e)
        {
            switch (((MenuItem)sender).Text)
            {
                case "Добавить выноску":
                    element.createVinosku();
                    if (element.vinoska == null)
                        return;
                        Mode = modeShkaf.moveVinosku;
                    break;
                case "Копировать":

                    father.copy(element.globalId, element);
                    break;
                case "Удалить":
                    break;
            }
        }



        //для контектного меню
        static drawer element;
        //
        public static void DOWN(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Right:
                    switch (Mode)
                    {
                        case modeShkaf.doNothing_NOSCALEMODE:
                            for (int i = Schemes_Editor.mainWorkList.Count - 1; i > -1; i--)
                            {
                                if (Schemes_Editor.mainWorkList[i].inside(e.Location, 2))
                                {
                                    if (Schemes_Editor.mainWorkList[i] is inboxes)
                                    {

                                    }
                                    if (Schemes_Editor.mainWorkList[i] is boxes)
                                    {

                                    }
                                    element = Schemes_Editor.mainWorkList[i];
                                    break;
                                }
                            }



                            ContextMenu menushka = new ContextMenu(new MenuItem[] { new MenuItem("Добавить выноску", handler), new MenuItem("Копировать", handler), new MenuItem("Удалить", handler) });
                            menushka.Show(father.pictureBox1, e.Location);
                            break;



                    }
                    break;
                   






                case MouseButtons.Left:
                    switch (Mode)
                    {
                        case modeShkaf.moveVinosku:
                            Mode = modeShkaf.doNothing_NOSCALEMODE;
                            break;
                        case modeShkaf.doNothing_NOSCALEMODE: //
                            movable = null;
                            for (int i = Schemes_Editor.mainWorkList.Count - 1; i > -1; i--)
                            {
                                if (Schemes_Editor.mainWorkList[i].inside(e.Location, 2))
                                {
                                    if (Schemes_Editor.mainWorkList[i] is inboxes)
                                    {
                                        Prev = new Point(e.Location.X - ((inboxes)Schemes_Editor.mainWorkList[i]).locations[2].X, e.Location.Y - ((inboxes)Schemes_Editor.mainWorkList[i]).locations[2].Y);
                                        Mode = modeShkaf.dragShkafnoe;
                                        hasRect = false;
                                        movable = Schemes_Editor.mainWorkList[i];
                                        foreach (boxes j in Schemes_Editor.mainWorkList.FindAll(x => x is boxes))
                                        {
                                            int pos = j.equipInside.IndexOf(j.equipInside.Find(x => x.localID == movable.localID));
                                            if (pos != -1)
                                            {
                                                j.equipInside.RemoveAt(pos);
                                                j.positions.RemoveAt(pos);
                                                j.unitsSeized.RemoveAt(pos);
                                            }
                                        }
                                      ((inboxes)Schemes_Editor.mainWorkList[i]).inbox = false;
                                    }
                                    if (Schemes_Editor.mainWorkList[i] is boxes)
                                    {
                                        movable = Schemes_Editor.mainWorkList[i];
                                        Prev = new Point(e.Location.X - ((boxes)Schemes_Editor.mainWorkList[i]).locations[2].X, e.Location.Y - ((boxes)Schemes_Editor.mainWorkList[i]).locations[2].Y);
                                        Mode = modeShkaf.dragShkaf;
                                    }
              
                                    break;
                                }
                            }
                            break;
                        case modeShkaf.doNothing_SCALEMODE:
                            moveTargetIndex = -1;
                            for (int i = 0; i < Schemes_Editor.mainWorkList.Count; i++)
                            {
                                //if (Schemes_Editor.mainWorkList[i] is wire_s)
                                //    continue;
                                if (Schemes_Editor.mainWorkList[i] is inboxes && ((inboxes)Schemes_Editor.mainWorkList[i]).inbox)
                                    continue;

                                int a = Schemes_Editor.mainWorkList[i].locations[2].X, b = Schemes_Editor.mainWorkList[i].locations[2].Y, c = Schemes_Editor.mainWorkList[i].locations[2].X + Schemes_Editor.mainWorkList[i].scales[2].X, d = Schemes_Editor.mainWorkList[i].locations[2].Y + Schemes_Editor.mainWorkList[i].scales[2].Y;
                                if (Schemes_Editor.distance(new Point(a, b), e.Location) < 20)
                                {
                                    scalePoint = new Point(a, b); moveTargetIndex = i; pointNum = 0;
                                    Mode = modeShkaf.scaleSomething;
                                    break;
                                }
                                if (Schemes_Editor.distance(new Point(a, d), e.Location) < 20)
                                {
                                    scalePoint = new Point(a, d); moveTargetIndex = i; pointNum = 3;
                                    Mode = modeShkaf.scaleSomething;
                                    break;
                                }
                                if (Schemes_Editor.distance(new Point(c, b), e.Location) < 20)
                                {
                                    scalePoint = new Point(c, b); moveTargetIndex = i; pointNum = 1;
                                    Mode = modeShkaf.scaleSomething;
                                    break;
                                }
                                if (Schemes_Editor.distance(new Point(c, d), e.Location) < 20)
                                {
                                    scalePoint = new Point(c, d); moveTargetIndex = i; pointNum = 2;
                                    Mode = modeShkaf.scaleSomething;
                                    break;
                                }
                            }
                            break;
                    }
                    break;
            }
      
        }



        public static void MOVE(object sender, MouseEventArgs e)
        {
            switch (Mode)
            {
                case modeShkaf.moveVinosku:
                    float distt = Math.Abs(element.vinoska.vertex1.X - element.vinoska.vertex2.X);
                    element.vinoska.vertex1 = new Point((int)(e.Location.X - distt / 2.0f), e.Location.Y);
                    element.vinoska.vertex2 = new Point((int)(e.Location.X + distt / 2.0f), e.Location.Y);

                    if(element.vinoska.vertex2.X< element.vinoska.startPoint.X)
                    {
                        Point t = element.vinoska.vertex1;
                        element.vinoska.vertex1 = element.vinoska.vertex2;
                        element.vinoska.vertex2 = t;
                    }
                    break;
                case modeShkaf.dragShkaf:
                    movable.move(new Point(e.Location.X - Prev.X, e.Location.Y - Prev.Y), 2);
                    break;
                case modeShkaf.dragShkafnoe:
                    hasRect = false;
                    inboxes pointer = (inboxes)movable;
                    pointer.inbox = false;
                    foreach (boxes j in Schemes_Editor.mainWorkList.FindAll(x => x is boxes))
                    {
                        if (!j.inside(e.Location, 2)) continue;
                        //если внутри необходимых свободных ячеек, то выделить их
                        List<int> acceptable = new List<int>();
                        for (int k = 0; k < j.units; k++)
                        {
                            acceptable.Add(k);
                        }
                        for (int k = 0; k < j.equipInside.Count; k++)
                            //метка всех занятых
                            for (int d = j.positions[k]; d < j.positions[k] + j.unitsSeized[k]; d++)
                                acceptable[d] = -1;
                        acceptable.RemoveAll(x => x == -1);
                        for (int k = 0; k < acceptable.Count - 1; k++)
                        {
                            int current = acceptable[k];
                            bool good = true;
                            for (int d = 1; d < pointer.numberOfUnits; d++)
                            {
                                if (k + d == acceptable.Count)
                                {
                                    good = false;
                                    break;
                                }
                                if (acceptable[k + d] != current + 1)
                                {
                                    good = false;
                                    break;
                                }
                                current++;
                            }
                            if (!good)
                            {
                                acceptable.RemoveAt(k);
                                k--;
                            }
                        }
                        //поиск тех, внутри которых находимся
                        foreach (var k in acceptable)
                        {
                            if (
                                e.X > j.locations[2].X + 20 &&
                                e.X < j.locations[2].X + j.scales[2].X - 20 &&
                                e.Y > j.locations[2].Y + 30 + k * (j.unitSize + 1) &&
                                e.Y < j.locations[2].Y + 30 + (k + 1) * (j.unitSize + 1))
                            {
                                hasRect = true;
                                rect = new Rectangle(j.locations[2].X + 20, (int)(j.locations[2].Y + 30 + k * (j.unitSize)), j.scales[2].X - 40, (int)j.unitSize);
                                boxForInser = j;
                                indexToInsertInBox = k;
                                movable.move(new Point(e.Location.X - Prev.X, e.Location.Y - Prev.Y), 2);
                                draw();
                                return;
                            }
                        }
                    }
                    movable.move(new Point(e.Location.X - Prev.X, e.Location.Y - Prev.Y), 2);
                    break;
                case modeShkaf.doNothing_NOSCALEMODE:
                    indexToSurround = -1;
                    for (int i = Schemes_Editor.mainWorkList.Count - 1; i > -1; i--)
                    {
                        if (Schemes_Editor.mainWorkList[i].inside(e.Location, 2))
                        {
                            indexToSurround = i;
                            break;
                        }
                    }
                    break;

                case modeShkaf.doNothing_SCALEMODE:
                    scalePoint = new Point(-1, -1);
                    //finding nearest dots
                    for (int i = 0; i < Schemes_Editor.mainWorkList.Count; i++)
                    {
                        //if (Schemes_Editor.mainWorkList[i] is wire_s)
                        //    continue;
                        if (Schemes_Editor.mainWorkList[i] is inboxes && ((inboxes)Schemes_Editor.mainWorkList[i]).inbox)
                            continue;
                        else
                        {
                            int a = Schemes_Editor.mainWorkList[i].locations[2].X, b = Schemes_Editor.mainWorkList[i].locations[2].Y, c = Schemes_Editor.mainWorkList[i].locations[2].X + Schemes_Editor.mainWorkList[i].scales[2].X, d = Schemes_Editor.mainWorkList[i].locations[2].Y + Schemes_Editor.mainWorkList[i].scales[2].Y;
                            if (Schemes_Editor.distance(new Point(a, b), e.Location) < 20)
                            {
                                scalePoint = new Point(a, b);
                                break;
                            }
                            if (Schemes_Editor.distance(new Point(a, d), e.Location) < 20)
                            {
                                scalePoint = new Point(a, d);
                                break;
                            }
                            if (Schemes_Editor.distance(new Point(c, b), e.Location) < 20)
                            {
                                scalePoint = new Point(c, b);
                                break;
                            }
                            if (Schemes_Editor.distance(new Point(c, d), e.Location) < 20)
                            {
                                scalePoint = new Point(c, d);
                                break;
                            }
                        }
                    }
                    break;

                case modeShkaf.scaleSomething:
                    var buff = Schemes_Editor.mainWorkList[moveTargetIndex];
                    if (pointNum == 0)
                    {
                        //num 3
                        Point p = new Point(buff.locations[2].X + buff.scales[2].X, buff.locations[2].Y + buff.scales[2].Y);
                        buff.scales[2] = new Point(p.X - e.Location.X, p.Y - e.Location.Y);
                        buff.locations[2] = e.Location;
                        scalePoint = e.Location;
                    }
                    if (pointNum == 1)
                    {
                        //num 3
                        Point p = new Point(buff.locations[2].X, buff.locations[2].Y + buff.scales[2].Y);
                        buff.scales[2] = new Point(e.Location.X - p.X, p.Y - e.Location.Y);
                        buff.locations[2] = new Point(e.Location.X - buff.scales[2].X, e.Location.Y);
                        scalePoint = e.Location;
                    }
                    if (pointNum == 2)
                    {
                        //num 3
                        buff.scales[2] = new Point(e.Location.X - buff.locations[2].X, e.Location.Y - buff.locations[2].Y);
                        // buff.locations[2] = e.Location;
                        scalePoint = e.Location;
                    }
                    if (pointNum == 3)
                    {
                        //num 3
                        Point p = new Point(buff.locations[2].X + buff.scales[2].X, buff.locations[2].Y);
                        buff.scales[2] = new Point(p.X - e.Location.X, e.Location.Y - p.Y);
                        buff.locations[2] = new Point(e.Location.X, p.Y);
                        scalePoint = e.Location;
                    }
                    break;

            }
            draw();
        }





        static public void UP(object sender, MouseEventArgs e)
        {
            switch (Mode)
            {
                case modeShkaf.dragShkaf:
                    Mode = modeShkaf.doNothing_NOSCALEMODE;
                    break;
                case modeShkaf.dragShkafnoe:
                    if (hasRect)
                    {
                        boxForInser.equipInside.Add(movable);
                        boxForInser.positions.Add(indexToInsertInBox);
                        boxForInser.unitsSeized.Add(movable.numberOfUnits);
                        ((inboxes)movable).inbox = true;
                    }
                    else ((inboxes)movable).inbox = false;
                    hasRect = false;
                    Mode = modeShkaf.doNothing_NOSCALEMODE;
                    break;
                case modeShkaf.scaleSomething:
                    Mode = modeShkaf.doNothing_SCALEMODE;
                    break;
            }

        }






    }
}
