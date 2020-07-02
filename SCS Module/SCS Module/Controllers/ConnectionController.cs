using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace SCS_Module
{
    public enum modeConnection
    {
        doNothing_SCALEMODE,
        doNothing_NOSCALEMODE,
        dragShkaf,
        scaleSomething,
        moveVinosku,
        constantMove,
        dragShkafnoe,
        moveRoom, 
        buildConnection,
        editWire
    }
    public static class ConnectionController
    {


        static public Schemes_Editor father;
        static public int pointNum;
        static public dynamic movable;
        static public Point Prev;
        static public modeConnection Mode = modeConnection.doNothing_NOSCALEMODE;
        //Для перетаскивания шкафного
        static public bool hasRect = false;
        static public Rectangle rect;
        static public boxes boxForInser;
        static public int indexToSurround = -1;
        static public int indexToInsertInBox;
        static public Point scalePoint = new Point(-1, -1);
        static public bool isRoomSelected = false;
        //
        //Для перетаскивания
        static public int moveTargetIndex = -1;
        //
        static int localSheet = 1;
       public static Wire targetWire;
        static public void DOWN(object sender, MouseEventArgs e)
        {
            isRoomSelected = false;
            switch (Mode)
            {
                case modeConnection.buildConnection:
                    for (int i = Schemes_Editor.mainWorkList.Count - 1; i > -1; i--)
                    {
                        if(Schemes_Editor.mainWorkList[i].inside(e.Location, localSheet))
                        {
                            if (Schemes_Editor.mainWorkList[i] is inboxes)
                            {
                                //свободные интерфейсы оборудования
                                List<Equipment.Compatibility> list = Schemes_Editor.mainList.Find(x => x.id == Schemes_Editor.mainWorkList[i].globalId).compatibilities;

                                for(int j=0, jj=0; j<list.Count;j++, jj++)
                                    if(((inboxes)Schemes_Editor.mainWorkList[i]).seized[jj]== list[j].count)
                                    {
                                        list.RemoveAt(jj);
                                        jj--;
                                    }
                                if(list.Count == 0)
                                {
                                    MessageBox.Show("Отсутствует свободный интерфейс");
                                }
                                else
                                {
                                    if (targetWire.first && targetWire.second)
                                    {
                                        interfaceSelector sel = new interfaceSelector(list, targetWire.MyOwnFirst, targetWire.MyOwnSecond);
                                        if (sel.ShowDialog() == DialogResult.OK)
                                        {
                                            var item = Schemes_Editor.mainList.Find(x => x.id == Schemes_Editor.mainWorkList[i].globalId).compatibilities.Find(x => x.interfaceType.id == sel.selectedId);
                                            int biff = Schemes_Editor.mainList.Find(x => x.id == Schemes_Editor.mainWorkList[i].globalId).compatibilities.IndexOf(item);
                                            if (targetWire.MyOwnFirst.interfaceType.id == sel.selectedId)
                                            {
                                                targetWire.first = false;
                                                targetWire.firstEquip = (inboxes)Schemes_Editor.mainWorkList[i];
                                            }
                                            else
                                            {
                                                targetWire.second = false;
                                                targetWire.secondEquip = (inboxes)Schemes_Editor.mainWorkList[i];
                                            }
                                            ((inboxes)Schemes_Editor.mainWorkList[i]).seized[biff]++;
                                            MessageBox.Show("Выберите второе оборудование");
                                        }

                                    }
                                  else  if (targetWire.first)
                                    {
                                        interfaceSelector sel = new interfaceSelector(list, targetWire.MyOwnFirst);
                                        if (sel.ShowDialog() == DialogResult.OK)
                                        {
                                            var item = Schemes_Editor.mainList.Find(x => x.id == Schemes_Editor.mainWorkList[i].globalId).compatibilities.Find(x => x.interfaceType.id == sel.selectedId);
                                            int biff =   Schemes_Editor.mainList.Find(x => x.id == Schemes_Editor.mainWorkList[i].globalId).compatibilities.IndexOf(item);
                                            ((inboxes)Schemes_Editor.mainWorkList[i]).seized[biff]++;
                                            targetWire.first = false;
                                            targetWire.firstEquip = (inboxes)Schemes_Editor.mainWorkList[i];
                                            Mode = modeConnection.doNothing_NOSCALEMODE;

                                        }
                                    }
                  
                                 else  if (targetWire.second)
                                    {
                                        interfaceSelector sel = new interfaceSelector(list, targetWire.MyOwnSecond);
                                        if (sel.ShowDialog() == DialogResult.OK)
                                        {
                                            var item = Schemes_Editor.mainList.Find(x => x.id == Schemes_Editor.mainWorkList[i].globalId).compatibilities.Find(x => x.interfaceType.id == sel.selectedId);
                                            int biff = Schemes_Editor.mainList.Find(x => x.id == Schemes_Editor.mainWorkList[i].globalId).compatibilities.IndexOf(item);
                                            ((inboxes)Schemes_Editor.mainWorkList[i]).seized[biff]++;
                                            targetWire.second = false;
                                            targetWire.secondEquip = (inboxes)Schemes_Editor.mainWorkList[i];
                                            Mode = modeConnection.doNothing_NOSCALEMODE;
                                            targetWire.createPoints();
                                        }
                                    }
                                }
               
                            }
                            else if(Schemes_Editor.mainWorkList[i] is free)
                            {

                            }
                            else
                            {
                                MessageBox.Show("Ничего не выбрано");
                            }
                        }
                    }
                        break;
                case modeConnection.doNothing_NOSCALEMODE:
                    movable = null;
                    for (int i = Schemes_Editor.mainWorkList.Count - 1; i > -1; i--)
                    {
                        if (Schemes_Editor.mainWorkList[i].inside(e.Location, localSheet))
                        {
                            if (Schemes_Editor.mainWorkList[i] is inboxes)
                            {
                                Prev = new Point(e.Location.X - ((inboxes)Schemes_Editor.mainWorkList[i]).locations[localSheet].X, e.Location.Y - ((inboxes)Schemes_Editor.mainWorkList[i]).locations[localSheet].Y);
                                Mode = modeConnection.dragShkafnoe;
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
                                Prev = new Point(e.Location.X - ((boxes)Schemes_Editor.mainWorkList[i]).locations[localSheet].X, e.Location.Y - ((boxes)Schemes_Editor.mainWorkList[i]).locations[localSheet].Y);
                                Mode = modeConnection.dragShkaf;
                            }
                            if (Schemes_Editor.mainWorkList[i] is free)
                            {
                                movable = Schemes_Editor.mainWorkList[i];
                                Prev = new Point(e.Location.X - ((free)Schemes_Editor.mainWorkList[i]).locations[localSheet].X, e.Location.Y - ((free)Schemes_Editor.mainWorkList[i]).locations[localSheet].Y);
                                Mode = modeConnection.dragShkaf;
                            }
                            break;
                        }
                    }
                    if (movable != null) break;
                    else
                    {
                        for (int i = 0; i < Schemes_Editor.rooms.Count; i++)
                        {
                            if (Schemes_Editor.rooms[i].inside(e.Location, localSheet))
                            {
                                isRoomSelected = true;
                                Prev = new Point(e.Location.X - Schemes_Editor.rooms[i].locations[localSheet].X, e.Location.Y - Schemes_Editor.rooms[i].locations[localSheet].Y);
                                movable = Schemes_Editor.rooms[i];
                                Mode = modeConnection.moveRoom;
                            }
                        }
                    }
                    break;


                case modeConnection.doNothing_SCALEMODE:
                    moveTargetIndex = -1;
                    for (int i = 0; i < Schemes_Editor.mainWorkList.Count; i++)
                    {
                        //if (Schemes_Editor.mainWorkList[i] is wire_s)
                        //    continue;
                        if (Schemes_Editor.mainWorkList[i] is inboxes && ((inboxes)Schemes_Editor.mainWorkList[i]).inbox)
                            continue;

                        int a = Schemes_Editor.mainWorkList[i].locations[localSheet].X, b = Schemes_Editor.mainWorkList[i].locations[localSheet].Y, c = Schemes_Editor.mainWorkList[i].locations[localSheet].X + Schemes_Editor.mainWorkList[i].scales[localSheet].X, d = Schemes_Editor.mainWorkList[i].locations[localSheet].Y + Schemes_Editor.mainWorkList[i].scales[localSheet].Y;
                        if (Schemes_Editor.distance(new Point(a, b), e.Location) < 20)
                        {
                            scalePoint = new Point(a, b); moveTargetIndex = i; pointNum = 0;
                            Mode = modeConnection.scaleSomething;
                            break;
                        }
                        if (Schemes_Editor.distance(new Point(a, d), e.Location) < 20)
                        {
                            scalePoint = new Point(a, d); moveTargetIndex = i; pointNum = 3;
                            Mode = modeConnection.scaleSomething;
                            break;
                        }
                        if (Schemes_Editor.distance(new Point(c, b), e.Location) < 20)
                        {
                            scalePoint = new Point(c, b); moveTargetIndex = i; pointNum = 1;
                            Mode = modeConnection.scaleSomething;
                            break;
                        }
                        if (Schemes_Editor.distance(new Point(c, d), e.Location) < 20)
                        {
                            scalePoint = new Point(c, d); moveTargetIndex = i; pointNum = 2;
                            Mode = modeConnection.scaleSomething;
                            break;
                        }
                    }
                    if (moveTargetIndex != -1) break;
                    else
                    {
                        for (int i = 0; i < Schemes_Editor.rooms.Count; i++)
                        {
                            int a = Schemes_Editor.rooms[i].locations[localSheet].X, b = Schemes_Editor.rooms[i].locations[localSheet].Y, c = Schemes_Editor.rooms[i].locations[localSheet].X + Schemes_Editor.rooms[i].locations[localSheet].Width, d = Schemes_Editor.rooms[i].locations[localSheet].Y + Schemes_Editor.rooms[i].locations[localSheet].Height;
                            if (Schemes_Editor.distance(new Point(a, b), e.Location) < 20)
                            {
                                isRoomSelected = true;
                                scalePoint = new Point(a, b); moveTargetIndex = i; pointNum = 0;
                                Mode = modeConnection.scaleSomething;
                                break;
                            }
                            if (Schemes_Editor.distance(new Point(a, d), e.Location) < 20)
                            {
                                isRoomSelected = true;
                                scalePoint = new Point(a, d); moveTargetIndex = i; pointNum = 3;
                                Mode = modeConnection.scaleSomething;
                                break;
                            }
                            if (Schemes_Editor.distance(new Point(c, b), e.Location) < 20)
                            {
                                isRoomSelected = true;
                                scalePoint = new Point(c, b); moveTargetIndex = i; pointNum = 1;
                                Mode = modeConnection.scaleSomething;
                                break;
                            }
                            if (Schemes_Editor.distance(new Point(c, d), e.Location) < 20)
                            {
                                isRoomSelected = true;
                                scalePoint = new Point(c, d); moveTargetIndex = i; pointNum = 2;
                                Mode = modeConnection.scaleSomething;
                                break;
                            }
                        }
                    }
                    break;
            }
        }
        static public void MOVE(object sender, MouseEventArgs e)
        {
            switch (Mode)
            {
                case modeConnection.moveRoom:
                    movable.move(new Point(e.X - Prev.X, e.Y - Prev.Y), localSheet);
                    break;

                case modeConnection.doNothing_NOSCALEMODE:
                    indexToSurround = -1;
                    for (int i = Schemes_Editor.mainWorkList.Count - 1; i > -1; i--)
                    {
                        if (Schemes_Editor.mainWorkList[i].inside(e.Location, localSheet))
                        {
                            indexToSurround = i;
                            break;
                        }
                    }
                    if (indexToSurround != -1) break;
                    else
                    {
                        for (int i = 0; i < Schemes_Editor.rooms.Count; i++)
                        {
                            if (Schemes_Editor.rooms[i].inside(e.Location, localSheet))
                            {
                                isRoomSelected = true;
                                indexToSurround = i;
                                break;
                            }
                        }
                    }
                    break;


                case modeConnection.doNothing_SCALEMODE:
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
                            int a = Schemes_Editor.mainWorkList[i].locations[localSheet].X, b = Schemes_Editor.mainWorkList[i].locations[localSheet].Y, c = Schemes_Editor.mainWorkList[i].locations[localSheet].X + Schemes_Editor.mainWorkList[i].scales[localSheet].X, d = Schemes_Editor.mainWorkList[i].locations[localSheet].Y + Schemes_Editor.mainWorkList[i].scales[localSheet].Y;
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
                    if (scalePoint.X != -1) break;
                    else
                    {
                        for (int i = 0; i < Schemes_Editor.rooms.Count; i++)
                        {
                            int a = Schemes_Editor.rooms[i].locations[localSheet].X, b = Schemes_Editor.rooms[i].locations[localSheet].Y, c = Schemes_Editor.rooms[i].locations[localSheet].X + Schemes_Editor.rooms[i].locations[localSheet].Width, d = Schemes_Editor.rooms[i].locations[localSheet].Y + Schemes_Editor.rooms[i].locations[localSheet].Height;
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

                case modeConnection.dragShkaf:
                    movable.move(new Point(e.Location.X - Prev.X, e.Location.Y - Prev.Y), localSheet);
                    break;

                case modeConnection.dragShkafnoe:
                    hasRect = false;
                    inboxes pointer = (inboxes)movable;
                    pointer.inbox = false;
                    foreach (boxes j in Schemes_Editor.mainWorkList.FindAll(x => x is boxes))
                    {
                        if (!j.inside(e.Location, localSheet)) continue;
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
                                e.X > j.locations[localSheet].X + 20 &&
                                e.X < j.locations[localSheet].X + j.scales[localSheet].X - 20 &&
                                e.Y > j.locations[localSheet].Y + 30 + k * (j.unitSize + 1) &&
                                e.Y < j.locations[localSheet].Y + 30 + (k + 1) * (j.unitSize + 1))
                            {
                                hasRect = true;
                                rect = new Rectangle(j.locations[localSheet].X + 20, (int)(j.locations[2].Y + 30 + k * (j.unitSize)), j.scales[localSheet].X - 40, (int)j.unitSize);
                                boxForInser = j;
                                indexToInsertInBox = k;
                                movable.move(new Point(e.Location.X - Prev.X, e.Location.Y - Prev.Y), localSheet);
                                draw();
                                return;
                            }
                        }
                    }
                    movable.move(new Point(e.Location.X - Prev.X, e.Location.Y - Prev.Y), localSheet);
                    break;

                case modeConnection.scaleSomething:
                    if (isRoomSelected)
                    {
                        var buff = Schemes_Editor.rooms[moveTargetIndex];
                        if (pointNum == 0)
                        {
                            //num 3
                            Point p = new Point(buff.locations[localSheet].X + buff.locations[localSheet].Width, buff.locations[localSheet].Y + buff.locations[localSheet].Height);

                            buff.locations[localSheet] = new Rectangle(e.Location.X, e.Location.Y, p.X - e.Location.X, p.Y - e.Location.Y);

                            scalePoint = e.Location;
                        }
                        if (pointNum == 1)
                        {
                            //num 3
                            Point p = new Point(buff.locations[localSheet].X, buff.locations[localSheet].Y + buff.locations[localSheet].Height);

                            buff.locations[localSheet] = new Rectangle(buff.locations[localSheet].X, e.Location.Y, e.Location.X - p.X, p.Y - e.Location.Y);


                            scalePoint = e.Location;
                        }
                        if (pointNum == 2)
                        {
                            //num 3
                            buff.locations[localSheet] = new Rectangle(buff.locations[localSheet].X, buff.locations[localSheet].Y, e.Location.X - buff.locations[localSheet].X, e.Location.Y - buff.locations[localSheet].Y);
                            // buff.locations[2] = e.Location;
                            scalePoint = e.Location;
                        }
                        if (pointNum == 3)
                        {
                            //num 3
                            Point p = new Point(buff.locations[localSheet].X + buff.locations[localSheet].Width, buff.locations[localSheet].Y);

                            buff.locations[localSheet] = new Rectangle(e.Location.X, p.Y, p.X - e.Location.X, e.Location.Y - p.Y);

                            scalePoint = e.Location;
                        }
                    }
                    else
                    {
                        var buff = Schemes_Editor.mainWorkList[moveTargetIndex];
                        if (pointNum == 0)
                        {
                            //num 3
                            Point p = new Point(buff.locations[localSheet].X + buff.scales[localSheet].X, buff.locations[localSheet].Y + buff.scales[localSheet].Y);
                            buff.scales[localSheet] = new Point(p.X - e.Location.X, p.Y - e.Location.Y);
                            buff.locations[localSheet] = e.Location;
                            scalePoint = e.Location;
                        }
                        if (pointNum == 1)
                        {
                            //num 3
                            Point p = new Point(buff.locations[localSheet].X, buff.locations[localSheet].Y + buff.scales[localSheet].Y);
                            buff.scales[localSheet] = new Point(e.Location.X - p.X, p.Y - e.Location.Y);
                            buff.locations[localSheet] = new Point(e.Location.X - buff.scales[localSheet].X, e.Location.Y);
                            scalePoint = e.Location;
                        }
                        if (pointNum == 2)
                        {
                            //num 3
                            buff.scales[localSheet] = new Point(e.Location.X - buff.locations[localSheet].X, e.Location.Y - buff.locations[localSheet].Y);
                            // buff.locations[2] = e.Location;
                            scalePoint = e.Location;
                        }
                        if (pointNum == 3)
                        {
                            //num 3
                            Point p = new Point(buff.locations[localSheet].X + buff.scales[localSheet].X, buff.locations[localSheet].Y);
                            buff.scales[localSheet] = new Point(p.X - e.Location.X, e.Location.Y - p.Y);
                            buff.locations[localSheet] = new Point(e.Location.X, p.Y);
                            scalePoint = e.Location;
                        }
                    }
                    break;

            }
            draw();
        }

        static public void UP(object sender, MouseEventArgs e)
        {
            isRoomSelected = false;
            switch (Mode)
            {
                case modeConnection.moveRoom:
                    Mode = modeConnection.doNothing_NOSCALEMODE;
                    break;
                case modeConnection.dragShkaf:
                    Mode = modeConnection.doNothing_NOSCALEMODE;
                    break;
                case modeConnection.scaleSomething:
                    Mode = modeConnection.doNothing_SCALEMODE;
                    break;
                case modeConnection.dragShkafnoe:
                    if (hasRect)
                    {
                        boxForInser.equipInside.Add(movable);
                        boxForInser.positions.Add(indexToInsertInBox);
                        boxForInser.unitsSeized.Add(movable.numberOfUnits);
                        ((inboxes)movable).inbox = true;
                    }
                    else ((inboxes)movable).inbox = false;
                    hasRect = false;
                    Mode = modeConnection.doNothing_NOSCALEMODE;
                    break;
            }
        }


        static public void draw(bool isNeed = true)
        {
            if (isNeed)
                Schemes_Editor.gr[localSheet].Clear(Color.LightGray);

            foreach (var i in Schemes_Editor.mainWorkList)
                i.drawCon(Schemes_Editor.gr[localSheet]);

            foreach (var i in Schemes_Editor.rooms)
                i.draw(Schemes_Editor.gr[localSheet], localSheet);

            foreach (var i in Schemes_Editor.wires)
                i.draw(Schemes_Editor.gr[localSheet], localSheet);

            switch (Mode)
            {
                case modeConnection.doNothing_NOSCALEMODE:
                    if (indexToSurround != -1)
                    {
                        if (isRoomSelected)
                            Schemes_Editor.gr[localSheet].DrawRectangle(Pens.Blue, Schemes_Editor.rooms[indexToSurround].locations[localSheet]);
                        else
                            Schemes_Editor.gr[Schemes_Editor.sheetIndex].DrawRectangle(Pens.Blue, Schemes_Editor.mainWorkList[indexToSurround].locations[Schemes_Editor.sheetIndex].X - 1, Schemes_Editor.mainWorkList[indexToSurround].locations[Schemes_Editor.sheetIndex].Y - 1, Schemes_Editor.mainWorkList[indexToSurround].scales[Schemes_Editor.sheetIndex].X + 2, Schemes_Editor.mainWorkList[indexToSurround].scales[Schemes_Editor.sheetIndex].Y + 2);
                    }
                    break;




                case modeConnection.doNothing_SCALEMODE:
                    if (scalePoint.X != -1)
                    {
                        Schemes_Editor.gr[Schemes_Editor.sheetIndex].DrawEllipse(new Pen(Color.Red, 2), scalePoint.X - 5, scalePoint.Y - 5, 10, 10);
                    }
                    foreach (var i in Schemes_Editor.mainWorkList)
                    {
                        if (i is inboxes && ((inboxes)i).inbox)
                            continue;
                        Schemes_Editor.gr[Schemes_Editor.sheetIndex].DrawRectangle(Pens.Blue, i.locations[Schemes_Editor.sheetIndex].X - 1, i.locations[Schemes_Editor.sheetIndex].Y - 1, i.scales[Schemes_Editor.sheetIndex].X + 2, i.scales[Schemes_Editor.sheetIndex].Y + 2);
                    }
                    break;



                case modeConnection.dragShkaf:
                    Schemes_Editor.gr[Schemes_Editor.sheetIndex].DrawRectangle(Pens.Blue, Schemes_Editor.mainWorkList[indexToSurround].locations[Schemes_Editor.sheetIndex].X - 1, Schemes_Editor.mainWorkList[indexToSurround].locations[Schemes_Editor.sheetIndex].Y - 1, Schemes_Editor.mainWorkList[indexToSurround].scales[Schemes_Editor.sheetIndex].X + 2, Schemes_Editor.mainWorkList[indexToSurround].scales[Schemes_Editor.sheetIndex].Y + 2);
                    break;

                case modeConnection.scaleSomething:
                    if (isRoomSelected)
                        Schemes_Editor.gr[localSheet].DrawRectangle(Pens.Blue, Schemes_Editor.rooms[moveTargetIndex].locations[localSheet].X - 1, Schemes_Editor.rooms[moveTargetIndex].locations[localSheet].Y - 1, Schemes_Editor.rooms[moveTargetIndex].locations[Schemes_Editor.sheetIndex].Width + 2, Schemes_Editor.rooms[moveTargetIndex].locations[Schemes_Editor.sheetIndex].Height + 2);
                    else
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
    }
}
