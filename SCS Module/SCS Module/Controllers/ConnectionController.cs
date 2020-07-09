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
        dragVertex
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
        public static Point pointToSurroundWire;
        public static bool isWireSelected = false;
        public static int SelectedWireIndex;
        public static int VertexNumber;
        static public void DOWN(object sender, MouseEventArgs e)
        {
            mousePosition = e.Location;
            isRoomSelected = false;
            if (e.Button == MouseButtons.Left)
            {
                switch (Mode)
                {
                    //case modeConnection.editWire:
                    //    if (pointNumber != -1)
                    //    {
                    //        Schemes_Editor.wires[wireIndex].veryfied[localSheet][pointNumber] = true;
                    //        Mode = modeConnection.dragVertex;
                    //    }

                    //break;
                    case modeConnection.buildConnection:
                        for (int i = Schemes_Editor.mainWorkList.Count - 1; i > -1; i--)
                        {
                            if (Schemes_Editor.mainWorkList[i].inside(e.Location, localSheet))
                            {
                                if (Schemes_Editor.mainWorkList[i] is inboxes)
                                {
                                    //свободные интерфейсы оборудования
                                    List<Equipment.Compatibility> list = new List<Equipment.Compatibility>();
                                    var buffList = Schemes_Editor.mainList.Find(x => x.id == Schemes_Editor.mainWorkList[i].globalId).compatibilities;
                                    foreach (var j in buffList) list.Add(new Equipment.Compatibility() { count = j.count, interfaceType = j.interfaceType, isMama = j.isMama });
                                    for (int j = 0, jj = 0; j < list.Count; j++, jj++)
                                        if (((inboxes)Schemes_Editor.mainWorkList[i]).seized[jj] == list[j].count)
                                        {
                                            list.RemoveAt(jj);
                                            jj--;
                                        }
                                    if (list.Count == 0)
                                    {
                                        MessageBox.Show("Отсутствует свободный интерфейс");
                                    }
                                    else
                                    {
                                        if (!targetWire.isFirstSeized)
                                        {
                                            //чек сразу
                                            var buff = list.Where(x => x.isMama != targetWire.MyOwnSecond.isMama && x.interfaceType.id == targetWire.MyOwnSecond.interfaceType.id).ToList();
                                            if (buff != null && buff.Count != 0)
                                            {
                                                var item = Schemes_Editor.mainList.Find(x => x.id == Schemes_Editor.mainWorkList[i].globalId).compatibilities.Find(x => x.interfaceType.id == buff[0].interfaceType.id);
                                                int biff = Schemes_Editor.mainList.Find(x => x.id == Schemes_Editor.mainWorkList[i].globalId).compatibilities.IndexOf(item);


                                                targetWire.isFirstSeized = true;
                                                targetWire.firstEquip = (inboxes)Schemes_Editor.mainWorkList[i];
                                                targetWire.OtherFirst = item;

                                                ((inboxes)Schemes_Editor.mainWorkList[i]).seized[biff]++;
                                                Mode = modeConnection.doNothing_NOSCALEMODE;
                                             //   targetWire.createPoints();
                                            }
                                            else
                                            {
                                                interfaceSelector sel = new interfaceSelector(list, targetWire.MyOwnFirst);
                                                if (sel.ShowDialog() == DialogResult.OK)
                                                {
                                                    var item = Schemes_Editor.mainList.Find(x => x.id == Schemes_Editor.mainWorkList[i].globalId).compatibilities.Find(x => x.interfaceType.id == sel.selectedId);
                                                    int biff = Schemes_Editor.mainList.Find(x => x.id == Schemes_Editor.mainWorkList[i].globalId).compatibilities.IndexOf(item);

                                                    targetWire.isFirstSeized = true;
                                                    targetWire.firstEquip = (inboxes)Schemes_Editor.mainWorkList[i];
                                                    targetWire.OtherFirst = item;


                                                    ((inboxes)Schemes_Editor.mainWorkList[i]).seized[biff]++;
                                                    //    MessageBox.Show("Выберите второе оборудование");
                                                }
                                            }
                                        }
                                        else if (!targetWire.isSecondSeized)
                                        {
                                            //чек сразу
                                            var buff = list.Where(x => x.isMama != targetWire.MyOwnSecond.isMama && x.interfaceType.id == targetWire.MyOwnSecond.interfaceType.id).ToList();
                                            if (buff != null && buff.Count != 0)
                                            {
                                                var item = Schemes_Editor.mainList.Find(x => x.id == Schemes_Editor.mainWorkList[i].globalId).compatibilities.Find(x => x.interfaceType.id == buff[0].interfaceType.id);
                                                int biff = Schemes_Editor.mainList.Find(x => x.id == Schemes_Editor.mainWorkList[i].globalId).compatibilities.IndexOf(item);


                                                targetWire.isSecondSeized = true;
                                                targetWire.secondEquip = (inboxes)Schemes_Editor.mainWorkList[i];
                                                targetWire.OtherSecond = item;

                                                ((inboxes)Schemes_Editor.mainWorkList[i]).seized[biff]++;
                                                Mode = modeConnection.doNothing_NOSCALEMODE;
                                                targetWire.createPoints();
                                            }
                                            else
                                            {
                                                interfaceSelector sel = new interfaceSelector(list, targetWire.MyOwnSecond);
                                                if (sel.ShowDialog() == DialogResult.OK)
                                                {
                                                    var item = Schemes_Editor.mainList.Find(x => x.id == Schemes_Editor.mainWorkList[i].globalId).compatibilities.Find(x => x.interfaceType.id == sel.selectedId);
                                                    int biff = Schemes_Editor.mainList.Find(x => x.id == Schemes_Editor.mainWorkList[i].globalId).compatibilities.IndexOf(item);


                                                    targetWire.isSecondSeized = true;
                                                    targetWire.secondEquip = (inboxes)Schemes_Editor.mainWorkList[i];
                                                    targetWire.OtherSecond = item;

                                                    ((inboxes)Schemes_Editor.mainWorkList[i]).seized[biff]++;
                                                    Mode = modeConnection.doNothing_NOSCALEMODE;
                                                    targetWire.createPoints();
                                                }
                                            }
                                        }
                                    }

                                }
                                else if (Schemes_Editor.mainWorkList[i] is free)
                                {
                                    //свободные интерфейсы оборудования
                                    List<Equipment.Compatibility> list = new List<Equipment.Compatibility>();
                                    var buffList = Schemes_Editor.mainList.Find(x => x.id == Schemes_Editor.mainWorkList[i].globalId).compatibilities;
                                    foreach (var j in buffList) list.Add(new Equipment.Compatibility() { count = j.count, interfaceType = j.interfaceType, isMama = j.isMama });
                                    for (int j = 0, jj = 0; j < list.Count; j++, jj++)
                                        if (((free)Schemes_Editor.mainWorkList[i]).seized[jj] == list[j].count)
                                        {
                                            list.RemoveAt(jj);
                                            jj--;
                                        }
                                    if (list.Count == 0)
                                    {
                                        MessageBox.Show("Отсутствует свободный интерфейс");
                                    }
                                    else
                                    {
                                        if (!targetWire.isFirstSeized)
                                        {
                                            //чек сразу
                                            var buff = list.Where(x => x.isMama != targetWire.MyOwnSecond.isMama && x.interfaceType.id == targetWire.MyOwnSecond.interfaceType.id).ToList();
                                            if (buff != null && buff.Count != 0)
                                            {
                                                var item = Schemes_Editor.mainList.Find(x => x.id == Schemes_Editor.mainWorkList[i].globalId).compatibilities.Find(x => x.interfaceType.id == buff[0].interfaceType.id);
                                                int biff = Schemes_Editor.mainList.Find(x => x.id == Schemes_Editor.mainWorkList[i].globalId).compatibilities.IndexOf(item);


                                                targetWire.isFirstSeized = true;
                                                targetWire.firstEquip = (free)Schemes_Editor.mainWorkList[i];
                                                targetWire.OtherFirst = item;

                                                ((free)Schemes_Editor.mainWorkList[i]).seized[biff]++;
                                                Mode = modeConnection.doNothing_NOSCALEMODE;
                                             //   targetWire.createPoints();
                                            }
                                            else
                                            {
                                                interfaceSelector sel = new interfaceSelector(list, targetWire.MyOwnFirst);
                                                if (sel.ShowDialog() == DialogResult.OK)
                                                {
                                                    var item = Schemes_Editor.mainList.Find(x => x.id == Schemes_Editor.mainWorkList[i].globalId).compatibilities.Find(x => x.interfaceType.id == sel.selectedId);
                                                    int biff = Schemes_Editor.mainList.Find(x => x.id == Schemes_Editor.mainWorkList[i].globalId).compatibilities.IndexOf(item);

                                                    targetWire.isFirstSeized = true;
                                                    targetWire.firstEquip = (free)Schemes_Editor.mainWorkList[i];
                                                    targetWire.OtherFirst = item;


                                                    ((free)Schemes_Editor.mainWorkList[i]).seized[biff]++;
                                                    //    MessageBox.Show("Выберите второе оборудование");
                                                }
                                            }
                                        }
                                        else if (!targetWire.isSecondSeized)
                                        {
                                            //чек сразу
                                            var buff = list.Where(x => x.isMama != targetWire.MyOwnSecond.isMama && x.interfaceType.id == targetWire.MyOwnSecond.interfaceType.id).ToList();
                                            if (buff != null && buff.Count != 0)
                                            {
                                                var item = Schemes_Editor.mainList.Find(x => x.id == Schemes_Editor.mainWorkList[i].globalId).compatibilities.Find(x => x.interfaceType.id == buff[0].interfaceType.id);
                                                int biff = Schemes_Editor.mainList.Find(x => x.id == Schemes_Editor.mainWorkList[i].globalId).compatibilities.IndexOf(item);


                                                targetWire.isSecondSeized = true;
                                                targetWire.secondEquip = (free)Schemes_Editor.mainWorkList[i];
                                                targetWire.OtherSecond = item;

                                                ((free)Schemes_Editor.mainWorkList[i]).seized[biff]++;
                                                Mode = modeConnection.doNothing_NOSCALEMODE;
                                                targetWire.createPoints();
                                            }
                                            else
                                            {
                                                //
                                                interfaceSelector sel = new interfaceSelector(list, targetWire.MyOwnSecond);
                                                if (sel.ShowDialog() == DialogResult.OK)
                                                {
                                                    var item = Schemes_Editor.mainList.Find(x => x.id == Schemes_Editor.mainWorkList[i].globalId).compatibilities.Find(x => x.interfaceType.id == sel.selectedId);
                                                    int biff = Schemes_Editor.mainList.Find(x => x.id == Schemes_Editor.mainWorkList[i].globalId).compatibilities.IndexOf(item);


                                                    targetWire.isSecondSeized = true;
                                                    targetWire.secondEquip = (free)Schemes_Editor.mainWorkList[i];
                                                    targetWire.OtherSecond = item;

                                                    ((free)Schemes_Editor.mainWorkList[i]).seized[biff]++;
                                                    Mode = modeConnection.doNothing_NOSCALEMODE;
                                                    targetWire.createPoints();
                                                }
                                            }
                                        }
                                    }
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

                        if (isWireSelected)
                        {
                            var t = Schemes_Editor.wires[SelectedWireIndex].inside(localSheet, e.Location);
                            if (t.isExists)
                            {
                                VertexNumber = t.ExistingIndex;
                                if (VertexNumber != -1)
                                {
                                    Mode = modeConnection.dragVertex;
                                    movable = Schemes_Editor.wires[SelectedWireIndex];
                                }
                            }
                            else //создать новую опорную точку
                            {
                                VertexNumber = Schemes_Editor.wires[SelectedWireIndex].insertPoint(t.vertex, localSheet);
                                if (VertexNumber != -1)
                                {
                                    Mode = modeConnection.dragVertex;
                                    movable = Schemes_Editor.wires[SelectedWireIndex];
                                }
                            }
                        }
                        if (movable != null) break;
                        for (int i = Schemes_Editor.mainWorkList.Count - 1; i > -1; i--)
                        {
                            if (Schemes_Editor.mainWorkList[i].inside(e.Location, localSheet))
                            {
                                if (Schemes_Editor.mainWorkList[i] is inboxes)
                                {
                                    Prev = new Point(e.Location.X - ((inboxes)Schemes_Editor.mainWorkList[i]).locations[localSheet].X, e.Location.Y - ((inboxes)Schemes_Editor.mainWorkList[i]).locations[localSheet].Y);
                                    Mode = modeConnection.dragShkafnoe;
                                    movable = Schemes_Editor.mainWorkList[i];

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

                        for (int i = 0; i < Schemes_Editor.rooms.Count; i++)
                        {
                            if (Schemes_Editor.rooms[i].inside(e.Location, localSheet))
                            {
                                isRoomSelected = true;
                                Prev = new Point(e.Location.X - Schemes_Editor.rooms[i].locations[localSheet].X, e.Location.Y - Schemes_Editor.rooms[i].locations[localSheet].Y);
                                movable = Schemes_Editor.rooms[i];
                                Mode = modeConnection.moveRoom;
                                break;
                            }
                        }
                        break;


                    case modeConnection.doNothing_SCALEMODE:
                        moveTargetIndex = -1;
                        for (int i = 0; i < Schemes_Editor.mainWorkList.Count; i++)
                        {
                            //if (Schemes_Editor.mainWorkList[i] is wire_s)
                            //    continue;

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
            else
            {
                ////////////////
                movable = null;
                isDrawSelected = false;
                if (isWireSelected)
                {
                    movable = Schemes_Editor.wires[SelectedWireIndex];
                    ContextMenu menushka = new ContextMenu(new MenuItem[] { new MenuItem("Добавить выноску", handler), new MenuItem("Копировать", handler), new MenuItem("Удалить", handler), new MenuItem("Удалить узел", handler), new MenuItem("Изменить название", handler) });
                    menushka.Show(father.pictureBox2, e.Location);
                    return;
                }
                for (int i = Schemes_Editor.mainWorkList.Count - 1; i > -1; i--)
                {
                    if (Schemes_Editor.mainWorkList[i].inside(e.Location, localSheet))
                    {
                        if (Schemes_Editor.mainWorkList[i] is inboxes)
                        {

                            movable = Schemes_Editor.mainWorkList[i];

                            isDrawSelected = true;
                            ContextMenu menushka = new ContextMenu(new MenuItem[] { new MenuItem("Добавить выноску", handler), new MenuItem("Копировать", handler), new MenuItem("Удалить", handler), new MenuItem("Удалить узел", handler), new MenuItem("Изменить название", handler) });
                            menushka.Show(father.pictureBox2, e.Location);
                            return;
                        }
                        if (Schemes_Editor.mainWorkList[i] is boxes)
                        {
                            movable = Schemes_Editor.mainWorkList[i];
                            isDrawSelected = true;
                            ContextMenu menushka = new ContextMenu(new MenuItem[] { new MenuItem("Добавить выноску", handler), new MenuItem("Копировать", handler), new MenuItem("Удалить", handler), new MenuItem("Удалить узел", handler), new MenuItem("Изменить название", handler) });
                            menushka.Show(father.pictureBox2, e.Location);
                            return;
                        }
                        if (Schemes_Editor.mainWorkList[i] is free)
                        {
                            movable = Schemes_Editor.mainWorkList[i];
                            isDrawSelected = true;
                            ContextMenu menushka = new ContextMenu(new MenuItem[] { new MenuItem("Добавить выноску", handler), new MenuItem("Копировать", handler), new MenuItem("Удалить", handler), new MenuItem("Удалить узел", handler), new MenuItem("Изменить название", handler) });
                            menushka.Show(father.pictureBox2, e.Location);
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
                        ContextMenu menushka = new ContextMenu(new MenuItem[] { new MenuItem("Добавить выноску", handler), new MenuItem("Копировать", handler), new MenuItem("Удалить", handler), new MenuItem("Удалить узел", handler), new MenuItem("Изменить название", handler) });
                        menushka.Show(father.pictureBox2, e.Location);
                        return;
                    }
                }

                ////////////////




            }
        }
        static bool isDrawSelected = false;
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
                            movable.labels[localSheet] = cr.roomName;
                        }
                        break;
                    case "Добавить выноску":

                        break;
                    case "Копировать":
                        int id = RevitProvider.copy(Schemes_Editor.mainList.Find(x => x.id == movable.globalId));
                        if(movable is inboxes)
                        {
                            Schemes_Editor.mainWorkList.Add(new inboxes()
                            {
                                numberOfUnits = Convert.ToInt32(movable.properties["Занимаемых юнитов (шт)"]),
                                localID = id,
                                globalId = movable.globalId,
                                locations = new Point[] { new Point(0, 0), new Point(0, 0), new Point(0, 0), new Point(0, 0) },
                                scales = new List<Point>(movable.scales).ToArray()
                            });
                        }
                        if(movable is free)
                        {
                            Schemes_Editor.mainWorkList.Add(new free()
                            {
                                localID = id,
                                globalId = movable.globalId,
                                locations = new Point[] { new Point(0, 0), new Point(0, 0), new Point(0, 0), new Point(0, 0) },
                                scales = new List<Point>(movable.scales).ToArray()
                            });

                        }
                        break;
                    case "Удалить":
                        foreach (Wire i in Schemes_Editor.wires)
                        {
                            if (i.firstEquip.localID == ((drawer)movable).localID)
                            {
                                if (i.firstEquip is inboxes)
                                {
                                    ((inboxes)i.firstEquip).seized[Schemes_Editor.mainList.Find(x => x.id == ((inboxes)i.firstEquip).globalId).compatibilities.FindIndex(x => x.interfaceType.id == i.OtherFirst.interfaceType.id)]--;
                                }
                                if (i.secondEquip is inboxes)
                                {
                                    ((inboxes)i.secondEquip).seized[Schemes_Editor.mainList.Find(x => x.id == ((inboxes)i.secondEquip).globalId).compatibilities.FindIndex(x => x.interfaceType.id ==i.OtherSecond.interfaceType.id)]--;
                                }

                                if (i.firstEquip is free)
                                {
                                    ((free)i.firstEquip).seized[Schemes_Editor.mainList.Find(x => x.id == ((free)i.firstEquip).globalId).compatibilities.FindIndex(x => x.interfaceType.id == i.OtherFirst.interfaceType.id)]--;
                                }
                                if (i.secondEquip is free)
                                {
                                    ((free)i.secondEquip).seized[Schemes_Editor.mainList.Find(x => x.id == ((free)i.secondEquip).globalId).compatibilities.FindIndex(x => x.interfaceType.id == i.OtherSecond.interfaceType.id)]--;
                                }
                                Schemes_Editor.wires.RemoveAll(x => x.localID == i.localID);
                            }
                            if (i.secondEquip.localID == ((drawer)movable).localID)
                            {
                                if (i.firstEquip is inboxes)
                                {
                                    ((inboxes)i.firstEquip).seized[Schemes_Editor.mainList.Find(x => x.id == ((inboxes)i.firstEquip).globalId).compatibilities.FindIndex(x => x.interfaceType.id == i.OtherFirst.interfaceType.id)]--;
                                }
                                if (i.secondEquip is inboxes)
                                {
                                    ((inboxes)i.secondEquip).seized[Schemes_Editor.mainList.Find(x => x.id == ((inboxes)i.secondEquip).globalId).compatibilities.FindIndex(x => x.interfaceType.id == i.OtherSecond.interfaceType.id)]--;
                                }


                                if (i.firstEquip is free)
                                {
                                    ((free)i.firstEquip).seized[Schemes_Editor.mainList.Find(x => x.id == ((free)i.firstEquip).globalId).compatibilities.FindIndex(x => x.interfaceType.id == i.OtherFirst.interfaceType.id)]--;
                                }
                                if (i.secondEquip is free)
                                {
                                    ((free)i.secondEquip).seized[Schemes_Editor.mainList.Find(x => x.id == ((free)i.secondEquip).globalId).compatibilities.FindIndex(x => x.interfaceType.id == i.OtherSecond.interfaceType.id)]--;
                                }
                                Schemes_Editor.wires.RemoveAll(x => x.localID == i.localID);
                            }
                            Schemes_Editor.mainWorkList.RemoveAll(x => x.localID == ((drawer)movable).localID);
                        }
                        break;

                }
            }
            if (isRoomSelected)
            {

            }
            if (isWireSelected)
            {
                switch (((MenuItem)sender).Text)
                {
                    case "Изменить название":
                        RoomCreator cr = new RoomCreator();
                        if (cr.ShowDialog() == DialogResult.OK)
                        {
                            movable.labels[localSheet] = cr.roomName;
                        }
                        break;
                    case "Добавить выноску":
                        Schemes_Editor.wires[SelectedWireIndex].createVinosku(Schemes_Editor.wires[SelectedWireIndex].inside(localSheet, mousePosition).vertex, localSheet);
                        movable = Schemes_Editor.wires[SelectedWireIndex];
                        Mode = modeConnection.moveVinosku;
                        break;
                    case "Копировать":
                        var result = Schemes_Editor.mainList.Find(x => x.id == Schemes_Editor.wires[SelectedWireIndex].globalId);
                        int current = -1;
                        foreach (var i in Schemes_Editor.wires)
                            if (i.localID > current)
                                current = i.localID;
                        current++;

                        if (result.compatibilities.Count == 1)
                            Schemes_Editor.wires.Add(new Wire() { localID = current, globalId = result.id, MyOwnFirst = result.compatibilities[0], MyOwnSecond = result.compatibilities[0] });
                        else
                            Schemes_Editor.wires.Add(new Wire() { localID = current, globalId = result.id, MyOwnFirst = result.compatibilities[0], MyOwnSecond = result.compatibilities[1] });
                        //      MessageBox.Show("выберите начальное оборудование");
                        targetWire = Schemes_Editor.wires[Schemes_Editor.wires.Count - 1];
                        Mode = modeConnection.buildConnection;

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


                        if (t.firstEquip is free)
                        {
                            ((free)t.firstEquip).seized[Schemes_Editor.mainList.Find(x => x.id == ((free)t.firstEquip).globalId).compatibilities.FindIndex(x => x.interfaceType.id == t.OtherFirst.interfaceType.id)]--;
                        }
                        if (t.secondEquip is free)
                        {
                            ((free)t.secondEquip).seized[Schemes_Editor.mainList.Find(x => x.id == ((free)t.secondEquip).globalId).compatibilities.FindIndex(x => x.interfaceType.id == t.OtherSecond.interfaceType.id)]--;
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
            try
            {
                mousePosition = e.Location;
                switch (Mode)
                {
                    case modeConnection.moveVinosku:
                        float distt = Math.Abs(movable.vinoska[localSheet].vertex1.X - movable.vinoska[localSheet].vertex2.X);
                        movable.vinoska[localSheet].vertex1 = new Point((int)(e.Location.X - distt / 2.0f), e.Location.Y);
                        movable.vinoska[localSheet].vertex2 = new Point((int)(e.Location.X + distt / 2.0f), e.Location.Y);

                        if (movable.vinoska[localSheet].vertex2.X < movable.vinoska[localSheet].startPoint.X)
                        {
                            Point t = movable.vinoska[localSheet].vertex1;
                            movable.vinoska[localSheet].vertex1 = movable.vinoska[localSheet].vertex2;
                            movable.vinoska[localSheet].vertex2 = t;
                        }
                        break;


                    case modeConnection.dragVertex:
                        Schemes_Editor.wires[SelectedWireIndex].points[localSheet][VertexNumber] = new Point(e.Location.X, e.Location.Y);
                        break;

                    case modeConnection.moveRoom:
                        movable.move(new Point(e.X - Prev.X, e.Y - Prev.Y), localSheet);
                        break;

                    case modeConnection.doNothing_NOSCALEMODE:
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
                            if (Schemes_Editor.mainWorkList[i].inside(e.Location, localSheet))
                            {
                                indexToSurround = i;
                                break;
                            }
                        }

                        if (indexToSurround != -1 || isWireSelected) break;

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


                    case modeConnection.doNothing_SCALEMODE:
                        scalePoint = new Point(-1, -1);
                        //finding nearest dots
                        for (int i = 0; i < Schemes_Editor.mainWorkList.Count; i++)
                        {
                            //if (Schemes_Editor.mainWorkList[i] is wire_s)
                            //    continue;
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
            catch (Exception ex)
            {
                Mode = modeConnection.doNothing_NOSCALEMODE;
            }
        }

        static public void UP(object sender, MouseEventArgs e)
        {
            mousePosition = e.Location;
            isRoomSelected = false;
            switch (Mode)
            {
                case modeConnection.moveVinosku:
                    Mode = modeConnection.doNothing_NOSCALEMODE;
                    break;
                case modeConnection.dragVertex:
                    Mode = modeConnection.doNothing_NOSCALEMODE;
                    break;
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
                    Mode = modeConnection.doNothing_NOSCALEMODE;
                    break;
            }
        }

        static Point mousePosition;
        static public void draw(bool isNeed = true)
        {
            try
            {
                if (isNeed)
                    Schemes_Editor.gr[localSheet].Clear(Color.LightGray);

                foreach (var i in Schemes_Editor.mainWorkList)
                    i.drawCon(Schemes_Editor.gr[localSheet]);

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
                    case modeConnection.buildConnection:
                        if (targetWire != null && targetWire.isFirstSeized)
                        {
                            Schemes_Editor.gr[localSheet].DrawLine(Pens.Red,
                                new Point(targetWire.firstEquip.locations[localSheet].X + targetWire.firstEquip.scales[localSheet].X / 2,
                                targetWire.firstEquip.locations[localSheet].Y + targetWire.firstEquip.scales[localSheet].Y / 2),

                                mousePosition);
                        }
                        break;
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


                //найти крайнюю правую точку
                double right = 0;
                foreach (var i in Schemes_Editor.mainWorkList)
                {
                    Equipment.VectorPic pic = Schemes_Editor.mainList.Find(x => x.id == i.globalId).inConnectionScheme;
                    if (i.locations[localSheet].X + i.scales[localSheet].X > right)
                        right = i.locations[localSheet].X + i.scales[localSheet].X;
                }
                right += 100;
                //рисуем УГОШКИ
                int iSimulator = 0;
                if (Schemes_Editor.mainWorkList.Exists(x => !(x is boxes)))
                {
                    foreach (var i in Schemes_Editor.mainList)
                    {
                        if (i.inConnectionScheme == null) continue;
                        var h = i.inConnectionScheme.copy();
                        Equipment.Point WidthHeight = h.GetProp();
                        float Xprop = WidthHeight.X / (50 * 1.0f),
                            Yprop = WidthHeight.Y / (50 * 1.0f);
                        h.divide(Xprop, Yprop);


                        foreach (var j in h.circles)
                            Schemes_Editor.gr[localSheet].DrawEllipse(Pens.Black, (int)(right + (float)j.center.X - (float)j.radiusX), (iSimulator * 60) + 20 + (float)j.center.Y - (float)j.radiusY, (float)j.radiusX * 2, (float)j.radiusY * 2);

                        foreach (var j in h.polyLines)
                        {
                            for (int k = 0; k < j.Count - 1; k++)
                            {
                                Schemes_Editor.gr[localSheet].DrawLine(Pens.Black, (int)(j[k].X + right), (iSimulator * 60) + j[k].Y + 20, (int)(j[k + 1].X + right), (iSimulator * 60) + j[k + 1].Y + 20);
                            }
                        }

                        StringFormat f = new StringFormat();
                        f.Alignment = StringAlignment.Center;
                        f.LineAlignment = StringAlignment.Center;
                        Schemes_Editor.gr[localSheet].DrawString(i.description, new Font("Arial", 7), Brushes.Black, new RectangleF((float)right + 60, (iSimulator * 60) + 20, 300, 50));
                        iSimulator++;
                    }
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


        static public void ExportUGO(ref string result)
        {
            double right = 0;
            foreach (var i in Schemes_Editor.mainWorkList)
            {
                Equipment.VectorPic pic = Schemes_Editor.mainList.Find(x => x.id == i.globalId).inConnectionScheme;
                if (i.locations[localSheet].X + i.scales[localSheet].X > right)
                    right = i.locations[localSheet].X + i.scales[localSheet].X;
            }
            right += 100;
            //рисуем УГОШКИ
            int iSimulator = 0;
            int offsetConnection = 3000;
            if (Schemes_Editor.mainWorkList.Exists(x => !(x is boxes)))
            {
                foreach (var i in Schemes_Editor.mainList)
                {
                    if (i.inConnectionScheme == null) continue;
                    var h = i.inConnectionScheme.copy();
                    Equipment.Point WidthHeight = h.GetProp();
                    float Xprop = WidthHeight.X / (50 * 1.0f),
                        Yprop = WidthHeight.Y / (50 * 1.0f);
                    h.divide(Xprop, Yprop);


                    //foreach (var j in h.circles)
                    //    Schemes_Editor.gr[localSheet].DrawEllipse(Pens.Black, (int)(right + (float)j.center.X - (float)j.radiusX), (iSimulator * 60) + 20 + (float)j.center.Y - (float)j.radiusY, (float)j.radiusX * 2, (float)j.radiusY * 2);

                    foreach (var j in h.polyLines)
                    {
                        for (int k = 0; k < j.Count - 1; k++)
                        {
                            result += AutocadExport.drawLine((int)(offsetConnection+j[k].X + right), (iSimulator * 60) + j[k].Y + 20, (int)(offsetConnection+j[k + 1].X + right), (iSimulator * 60) + j[k + 1].Y + 20);
                        //    Schemes_Editor.gr[localSheet].DrawLine(Pens.Black, (int)(j[k].X + right), (iSimulator * 60) + j[k].Y + 20, (int)(j[k + 1].X + right), (iSimulator * 60) + j[k + 1].Y + 20);
                        }
                    }

                    StringFormat f = new StringFormat();
                    f.Alignment = StringAlignment.Center;
                    f.LineAlignment = StringAlignment.Center;
                    result += AutocadExport.drawText(new RectangleF(offsetConnection+(float)right + 60, (iSimulator * 60) + 20, 300, 50), i.description);
                //    Schemes_Editor.gr[localSheet].DrawString(i.description, new Font("Arial", 7), Brushes.Black, new RectangleF((float)right + 60, (iSimulator * 60) + 20, 300, 50));
                    iSimulator++;
                }
            }
        }
    }
}
