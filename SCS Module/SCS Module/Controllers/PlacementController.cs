using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SCS_Module
{
    public enum modePlacement
    {
        doNothing_SCALEMODE,
        doNothing_NOSCALEMODE,
        dragShkaf,
        scaleSomething,
        moveVinosku,
        constantMove,
        moveRoom,
        dragFree,
        dragVertex
    }
    public static class PlacementController
    {
        static public Schemes_Editor father;
        static public int pointNum;
        static public dynamic movable;
        static public Point Prev;
        static public modePlacement Mode = modePlacement.doNothing_NOSCALEMODE;
        //Для перетаскивания шкафного
        static public bool hasRect = false;
        static public Rectangle rect;
        static public boxes boxForInser;
        static public int indexToSurround = -1;
        static public int indexToInsertInBox;
        static public Point scalePoint = new Point(-1, -1);
        static public bool isRoomSelected = false, isDrawSelected = false;
        //
        //Для перетаскивания
        static public int moveTargetIndex = -1;
        //
        static int localSheet = 0;
        static Point mousePosition;
        static public void DOWN(object sender, MouseEventArgs e)
        {
            mousePosition = e.Location;
            if (e.Button == MouseButtons.Right)
            {
                movable = null;
                isDrawSelected = false;
                for (int i = Schemes_Editor.mainWorkList.Count - 1; i > -1; i--)
                {
                    if (Schemes_Editor.mainWorkList[i].inside(e.Location, localSheet))
                    {
                        if (Schemes_Editor.mainWorkList[i] is boxes)
                        {
                            movable = Schemes_Editor.mainWorkList[i];
                            isDrawSelected = true;
                            ContextMenu menushka = new ContextMenu(new MenuItem[] { new MenuItem("Добавить выноску", handler), new MenuItem("Копировать", handler), new MenuItem("Удалить", handler), new MenuItem("Изменить название", handler) });
                            menushka.Show(father.pictureBox3, e.Location);
                            return;
                        }
                        if (Schemes_Editor.mainWorkList[i] is free)
                        {
                            movable = Schemes_Editor.mainWorkList[i];
                            isDrawSelected = true;
                            ContextMenu menushka = new ContextMenu(new MenuItem[] { new MenuItem("Добавить выноску", handler), new MenuItem("Копировать", handler), new MenuItem("Удалить", handler), new MenuItem("Изменить название", handler) });
                            menushka.Show(father.pictureBox3, e.Location);
                            return;
                        }
                        break;
                    }
                }
                for (int i = 0; i < Schemes_Editor.rooms.Count; i++)
                {
                    if (Schemes_Editor.rooms[i].inside(e.Location, localSheet))
                    {
                        isRoomSelected = true;
                        movable = Schemes_Editor.rooms[i];
                        ContextMenu menushka = new ContextMenu(new MenuItem[] { new MenuItem("Удалить", handler), new MenuItem("Изменить название", handler) });
                        menushka.Show(father.pictureBox3, e.Location);
                        return;
                    }
                }
                if (isWireSelected)
                {
                    movable = Schemes_Editor.wires[SelectedWireIndex];
                    ContextMenu menushka = new ContextMenu(new MenuItem[] { new MenuItem("Добавить выноску", handler), new MenuItem("Удалить", handler), new MenuItem("Удалить узел", handler), new MenuItem("Изменить название", handler) });
                    menushka.Show(father.pictureBox3, e.Location);
                    return;
                }
            }
            else
            {
                switch (Mode)
                {
                    case modePlacement.doNothing_NOSCALEMODE:
                        movable = null;
                        isRoomSelected = false;
                        if (isWireSelected)
                        {
                            var t = Schemes_Editor.wires[SelectedWireIndex].inside(localSheet, e.Location);
                            if (t.isExists)
                            {
                                VertexNumber = t.ExistingIndex;
                                if (VertexNumber != -1)
                                {
                                    Mode = modePlacement.dragVertex;
                                    movable = 1;
                                }
                            }
                            else //создать новую опорную точку
                            {
                                VertexNumber = Schemes_Editor.wires[SelectedWireIndex].insertPoint(t.vertex, localSheet);
                                if (VertexNumber != -1)
                                {
                                    Mode = modePlacement.dragVertex;
                                    movable = 1;
                                }
                            }
                        }
                        if (movable != null) break;

                        for (int i = Schemes_Editor.mainWorkList.Count - 1; i > -1; i--)
                        {
                            if (Schemes_Editor.mainWorkList[i].inside(e.Location, localSheet))
                            {
                                if (Schemes_Editor.mainWorkList[i] is boxes)
                                {
                                    Prev = new Point(e.Location.X - ((boxes)Schemes_Editor.mainWorkList[i]).locations[localSheet].X, e.Location.Y - ((boxes)Schemes_Editor.mainWorkList[i]).locations[localSheet].Y);
                                    Mode = modePlacement.dragShkaf;
                                    movable = Schemes_Editor.mainWorkList[i];
                                    break;
                                }
                                if (Schemes_Editor.mainWorkList[i] is free)
                                {
                                    Prev = new Point(e.Location.X - ((free)Schemes_Editor.mainWorkList[i]).locations[localSheet].X, e.Location.Y - ((free)Schemes_Editor.mainWorkList[i]).locations[localSheet].Y);
                                    Mode = modePlacement.dragShkaf;
                                    movable = Schemes_Editor.mainWorkList[i];
                                    break;
                                }
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
                                    Mode = modePlacement.moveRoom;
                                }
                            }
                        }
                        break;


                    case modePlacement.doNothing_SCALEMODE:
                        moveTargetIndex = -1;
                        isRoomSelected = false;
                        for (int i = 0; i < Schemes_Editor.mainWorkList.Count; i++)
                        {
                            //if (Schemes_Editor.mainWorkList[i] is wire_s)
                            //    continue;
                            if (Schemes_Editor.mainWorkList[i] is inboxes)
                                continue;

                            int a = Schemes_Editor.mainWorkList[i].locations[localSheet].X, b = Schemes_Editor.mainWorkList[i].locations[localSheet].Y, c = Schemes_Editor.mainWorkList[i].locations[localSheet].X + Schemes_Editor.mainWorkList[i].scales[localSheet].X, d = Schemes_Editor.mainWorkList[i].locations[localSheet].Y + Schemes_Editor.mainWorkList[i].scales[localSheet].Y;
                            if (Schemes_Editor.distance(new Point(a, b), e.Location) < 20)
                            {
                                scalePoint = new Point(a, b); moveTargetIndex = i; pointNum = 0;
                                Mode = modePlacement.scaleSomething;
                                break;
                            }
                            if (Schemes_Editor.distance(new Point(a, d), e.Location) < 20)
                            {
                                scalePoint = new Point(a, d); moveTargetIndex = i; pointNum = 3;
                                Mode = modePlacement.scaleSomething;
                                break;
                            }
                            if (Schemes_Editor.distance(new Point(c, b), e.Location) < 20)
                            {
                                scalePoint = new Point(c, b); moveTargetIndex = i; pointNum = 1;
                                Mode = modePlacement.scaleSomething;
                                break;
                            }
                            if (Schemes_Editor.distance(new Point(c, d), e.Location) < 20)
                            {
                                scalePoint = new Point(c, d); moveTargetIndex = i; pointNum = 2;
                                Mode = modePlacement.scaleSomething;
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
                                    Mode = modePlacement.scaleSomething;
                                    break;
                                }
                                if (Schemes_Editor.distance(new Point(a, d), e.Location) < 20)
                                {
                                    isRoomSelected = true;
                                    scalePoint = new Point(a, d); moveTargetIndex = i; pointNum = 3;
                                    Mode = modePlacement.scaleSomething;
                                    break;
                                }
                                if (Schemes_Editor.distance(new Point(c, b), e.Location) < 20)
                                {
                                    isRoomSelected = true;
                                    scalePoint = new Point(c, b); moveTargetIndex = i; pointNum = 1;
                                    Mode = modePlacement.scaleSomething;
                                    break;
                                }
                                if (Schemes_Editor.distance(new Point(c, d), e.Location) < 20)
                                {
                                    isRoomSelected = true;
                                    scalePoint = new Point(c, d); moveTargetIndex = i; pointNum = 2;
                                    Mode = modePlacement.scaleSomething;
                                    break;
                                }
                            }
                        }
                        break;
                }
            }
        }
        static dynamic element;
        private static void handler(object sender, EventArgs e)
        {
            if (isDrawSelected)
            {
                switch (((MenuItem)sender).Text)
                {
                    case "Изменить название":
                        RoomCreator cr = new RoomCreator();
                        if (cr.ShowDialog() == DialogResult.OK)
                        {
                            element.labels[localSheet] = cr.roomName;
                        }
                        break;
                    case "Добавить выноску":

                        break;
                    case "Копировать":

                        break;
                    case "Удалить":
                        foreach (var i in Schemes_Editor.wires)
                        {
                            if (i.firstEquip.localID == ((drawer)movable).localID)
                            {
                                var t = ((Wire)movable);
                                if (t.firstEquip is inboxes)
                                {
                                    ((inboxes)t.firstEquip).seized[Schemes_Editor.mainList.Find(x => x.id == ((inboxes)t.firstEquip).globalId).compatibilities.FindIndex(x => x.interfaceType.id == t.OtherFirst.interfaceType.id)]--;
                                }
                                if (t.secondEquip is inboxes)
                                {
                                    ((inboxes)t.secondEquip).seized[Schemes_Editor.mainList.Find(x => x.id == ((inboxes)t.secondEquip).globalId).compatibilities.FindIndex(x => x.interfaceType.id == t.OtherSecond.interfaceType.id)]--;
                                }
                                Schemes_Editor.wires.RemoveAll(x => x.localID == t.localID);
                            }
                            if (i.secondEquip.localID == ((drawer)movable).localID)
                            {
                                var t = ((Wire)movable);
                                if (t.firstEquip is inboxes)
                                {
                                    ((inboxes)t.firstEquip).seized[Schemes_Editor.mainList.Find(x => x.id == ((inboxes)t.firstEquip).globalId).compatibilities.FindIndex(x => x.interfaceType.id == t.OtherFirst.interfaceType.id)]--;
                                }
                                if (t.secondEquip is inboxes)
                                {
                                    ((inboxes)t.secondEquip).seized[Schemes_Editor.mainList.Find(x => x.id == ((inboxes)t.secondEquip).globalId).compatibilities.FindIndex(x => x.interfaceType.id == t.OtherSecond.interfaceType.id)]--;
                                }
                                Schemes_Editor.wires.RemoveAll(x => x.localID == t.localID);
                            }
                            Schemes_Editor.mainWorkList.RemoveAll(x => x.localID == ((drawer)movable).localID);
                        }
                        break;

                }
            }
            if (isRoomSelected)
            {
                switch (((MenuItem)sender).Text)
                {
                    case "Изменить название":
                        RoomCreator cr = new RoomCreator();
                        if (cr.ShowDialog() == DialogResult.OK)
                        {
                            element.labels[localSheet] = cr.roomName;
                        }
                        break;
                    case "Удалить":
                        var t = ((Wire)movable);
                        if (t.firstEquip is inboxes)
                        {
                            ((inboxes)t.firstEquip).seized[Schemes_Editor.mainList.Find(x => x.id == ((inboxes)t.firstEquip).globalId).compatibilities.FindIndex(x => x.interfaceType.id == t.OtherFirst.interfaceType.id)]--;
                        }
                        if (t.secondEquip is inboxes)
                        {
                            ((inboxes)t.secondEquip).seized[Schemes_Editor.mainList.Find(x => x.id == ((inboxes)t.secondEquip).globalId).compatibilities.FindIndex(x => x.interfaceType.id == t.OtherSecond.interfaceType.id)]--;
                        }
                        Schemes_Editor.wires.RemoveAll(x => x.localID == t.localID);
                        break;
                }
            }
            if (isWireSelected)
            {
                switch (((MenuItem)sender).Text)
                {
                    case "Изменить название":
                        RoomCreator cr = new RoomCreator();
                        if (cr.ShowDialog() == DialogResult.OK)
                        {
                            element.labels[localSheet] = cr.roomName;
                        }
                        break;
                    case "Добавить выноску":
                        Schemes_Editor.wires[SelectedWireIndex].createVinosku(Schemes_Editor.wires[SelectedWireIndex].inside(localSheet, mousePosition).vertex, localSheet);
                        element = Schemes_Editor.wires[SelectedWireIndex];
                        Mode = modePlacement.moveVinosku;
                        break;
                    case "Удалить":
                        var t = ((Wire)movable);
                        if (t.firstEquip is inboxes)
                        {
                            ((inboxes)t.firstEquip).seized[Schemes_Editor.mainList.Find(x => x.id == ((inboxes)t.firstEquip).globalId).compatibilities.FindIndex(x => x.interfaceType.id == t.OtherFirst.interfaceType.id)]--;
                        }
                        if (t.secondEquip is inboxes)
                        {
                            ((inboxes)t.secondEquip).seized[Schemes_Editor.mainList.Find(x => x.id == ((inboxes)t.secondEquip).globalId).compatibilities.FindIndex(x => x.interfaceType.id == t.OtherSecond.interfaceType.id)]--;
                        }
                        Schemes_Editor.wires.RemoveAll(x => x.localID == t.localID);
                        break;
                    case "Удалить узел":
                        var tt = ((Wire)movable).inside(localSheet, mousePosition);
                        if (tt.vertex.X != -1 && tt.isExists)
                        {
                            ((Wire)movable).points[localSheet].RemoveAll(x => x.X == tt.vertex.X && x.Y == tt.vertex.Y);
                        }
                        break;
                }
            }
        }

        static public void MOVE(object sender, MouseEventArgs e)
        {
            mousePosition = e.Location;
            switch (Mode)
            {
                case modePlacement.dragVertex:
                    Schemes_Editor.wires[SelectedWireIndex].points[localSheet][VertexNumber] = new Point(e.Location.X, e.Location.Y);
                    break;
                case modePlacement.moveRoom:
                    movable.move(new Point(e.X-Prev.X, e.Y-Prev.Y), localSheet);
                    break;
                case modePlacement.doNothing_NOSCALEMODE:
                    indexToSurround = -1;
                    isRoomSelected = false;
                    isWireSelected = false;

                    for (int i = 0; i < Schemes_Editor.wires.Count; i++)
                    {
                        selectedVertex ver = Schemes_Editor.wires[i].inside(localSheet, e.Location);
                        if (ver.vertex.X != -1)
                        {
                            isWireSelected = true;
                            SelectedWireIndex = i;
                            break;
                        }
                    }
                    if (isWireSelected) break;
                    for (int i = Schemes_Editor.mainWorkList.Count - 1; i > -1; i--)
                    {
                        if (Schemes_Editor.mainWorkList[i].inside(e.Location, localSheet) && !(Schemes_Editor.mainWorkList[i] is inboxes))
                        {
                            indexToSurround = i;
                            break;
                        }
                    }
                    if (indexToSurround != -1) break;
                    for (int i = 0; i < Schemes_Editor.rooms.Count; i++)
                    {
                        if (Schemes_Editor.rooms[i].inside(e.Location, localSheet))
                        {
                            isRoomSelected = true;
                            indexToSurround = i;
                            break;
                        }
                    }
                    break;


                case modePlacement.doNothing_SCALEMODE:
                    scalePoint = new Point(-1, -1);
                    //finding nearest dots
                    for (int i = 0; i < Schemes_Editor.mainWorkList.Count; i++)
                    {
                        if (Schemes_Editor.mainWorkList[i] is inboxes)
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

                case modePlacement.dragShkaf:
                    movable.move(new Point(e.Location.X - Prev.X, e.Location.Y - Prev.Y), localSheet);
                    break;

          

                case modePlacement.scaleSomething:
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
            mousePosition = e.Location;
            isRoomSelected = false;
            switch (Mode)
            {
                case modePlacement.dragVertex:
                    Mode = modePlacement.doNothing_NOSCALEMODE;
                    break;
                case modePlacement.moveRoom:
                    Mode = modePlacement.doNothing_NOSCALEMODE;
                    break;
                case modePlacement.dragShkaf:
                    Mode = modePlacement.doNothing_NOSCALEMODE;
                    break;
                case modePlacement.scaleSomething:
                    Mode = modePlacement.doNothing_SCALEMODE;
                    break;
            }
        }
        public static bool isWireSelected = false;
        public static int SelectedWireIndex;
        public static int VertexNumber;
        static public void draw(bool isNeed = true)
        {
            try
            {
                if (isNeed)
                    Schemes_Editor.gr[localSheet].Clear(Color.LightGray);

                foreach (var i in Schemes_Editor.mainWorkList)
                    i.drawPlace(Schemes_Editor.gr[localSheet]);

                foreach (var i in Schemes_Editor.rooms)
                    i.draw(Schemes_Editor.gr[localSheet], localSheet);

                if (isWireSelected)
                    for (int i = 0; i < Schemes_Editor.wires.Count; i++)
                    {
                        if (SelectedWireIndex == i)
                            Schemes_Editor.wires[i].draw(Schemes_Editor.gr[localSheet], localSheet, true);
                        else
                            Schemes_Editor.wires[i].draw(Schemes_Editor.gr[localSheet], localSheet);
                    }
                else
                    foreach (var i in Schemes_Editor.wires)
                        i.draw(Schemes_Editor.gr[localSheet], localSheet);

                switch (Mode)
                {
                    case modePlacement.doNothing_NOSCALEMODE:
                        if (indexToSurround != -1)
                        {
                            if (isRoomSelected)
                                Schemes_Editor.gr[localSheet].DrawRectangle(Pens.Blue, Schemes_Editor.rooms[indexToSurround].locations[localSheet]);
                            else
                                Schemes_Editor.gr[Schemes_Editor.sheetIndex].DrawRectangle(Pens.Blue, Schemes_Editor.mainWorkList[indexToSurround].locations[Schemes_Editor.sheetIndex].X - 1, Schemes_Editor.mainWorkList[indexToSurround].locations[Schemes_Editor.sheetIndex].Y - 1, Schemes_Editor.mainWorkList[indexToSurround].scales[Schemes_Editor.sheetIndex].X + 2, Schemes_Editor.mainWorkList[indexToSurround].scales[Schemes_Editor.sheetIndex].Y + 2);
                        }
                        break;




                    case modePlacement.doNothing_SCALEMODE:
                        if (scalePoint.X != -1)
                        {
                            Schemes_Editor.gr[Schemes_Editor.sheetIndex].DrawEllipse(new Pen(Color.Red, 2), scalePoint.X - 5, scalePoint.Y - 5, 10, 10);
                        }
                        foreach (var i in Schemes_Editor.mainWorkList)
                        {
                            if (i is inboxes) continue;
                            Schemes_Editor.gr[Schemes_Editor.sheetIndex].DrawRectangle(Pens.Blue, i.locations[Schemes_Editor.sheetIndex].X - 1, i.locations[Schemes_Editor.sheetIndex].Y - 1, i.scales[Schemes_Editor.sheetIndex].X + 2, i.scales[Schemes_Editor.sheetIndex].Y + 2);
                        }
                        break;



                    case modePlacement.dragShkaf:
                        Schemes_Editor.gr[Schemes_Editor.sheetIndex].DrawRectangle(Pens.Blue, Schemes_Editor.mainWorkList[indexToSurround].locations[Schemes_Editor.sheetIndex].X - 1, Schemes_Editor.mainWorkList[indexToSurround].locations[Schemes_Editor.sheetIndex].Y - 1, Schemes_Editor.mainWorkList[indexToSurround].scales[Schemes_Editor.sheetIndex].X + 2, Schemes_Editor.mainWorkList[indexToSurround].scales[Schemes_Editor.sheetIndex].Y + 2);
                        break;

                    case modePlacement.scaleSomething:
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
            catch (Exception exp) { }
        }
    }
}
