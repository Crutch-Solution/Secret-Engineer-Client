using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SCS_Module
{

    public partial class Schemes_Editor : Form
    {
        public Schemes_Editor()
        {
            InitializeComponent();
            pictureBox3.MouseDown += new System.Windows.Forms.MouseEventHandler(PlacementController.DOWN);
            pictureBox3.MouseMove += new System.Windows.Forms.MouseEventHandler(PlacementController.MOVE);
            pictureBox3.MouseUp += new System.Windows.Forms.MouseEventHandler(PlacementController.UP);

            pictureBox2.MouseDown += new System.Windows.Forms.MouseEventHandler(ConnectionController.DOWN);
            pictureBox2.MouseMove += new System.Windows.Forms.MouseEventHandler(ConnectionController.MOVE);
            pictureBox2.MouseUp += new System.Windows.Forms.MouseEventHandler(ConnectionController.UP);

            pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(BoxController.DOWN);
            pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(BoxController.MOVE);
            pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(BoxController.UP);

            strct.MouseDown += new System.Windows.Forms.MouseEventHandler(StructuralController.DOWN);
            strct.MouseMove += new System.Windows.Forms.MouseEventHandler(StructuralController.MOVE);
            strct.MouseUp += new System.Windows.Forms.MouseEventHandler(StructuralController.UP);





            BoxController.father = this;

            bitmaps[0] = new Bitmap(pictureBox3.Width, pictureBox3.Height);
            bitmaps[1] = new Bitmap(pictureBox2.Width, pictureBox2.Height);
            bitmaps[2] = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            bitmaps[3] = new Bitmap(strct.Width, strct.Height);
            sheets = new PictureBox[] { pictureBox3, pictureBox2, pictureBox1, strct };
            for (int i = 0; i < 4; i++)
            {
                gr[i] = Graphics.FromImage(bitmaps[i]);
                gr[i].SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            }
        }
        static public List<Equipment> mainList = new List<Equipment>();
        static public List<drawer> mainWorkList = new List<drawer>();
        static public List<Room> rooms = new List<Room>();
        static public List<Wire> wires = new List<Wire>();


        static public Rectangle[] recs = new Rectangle[4];
        static public Bitmap[] bitmaps = new Bitmap[4];
        static public Graphics[] gr = new Graphics[4];
        static public PictureBox[] sheets;


        private void добавитьСхемуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Searcher searcher = new Searcher();
            if (searcher.ShowDialog() == DialogResult.OK)
            {
                if (searcher.result.isBox)
                {
                    ////
                    if (searcher.result.inConnectionScheme != null) foreach (var i in searcher.result.inConnectionScheme.circles)
                        {
                            i.radiusX = i.radius; i.radiusY = i.radius;
                        }
                    if (searcher.result.inConnectionScheme != null) foreach (var i in searcher.result.inConnectionScheme.arcs)
                        {
                            i.radiusX = i.radius; i.radiusY = i.radius;
                        }
                    if (searcher.result.inStructural != null) foreach (var i in searcher.result.inStructural.circles)
                        {
                            i.radiusX = i.radius; i.radiusY = i.radius;
                        }
                    if (searcher.result.inStructural != null) foreach (var i in searcher.result.inStructural.arcs)
                        {
                            i.radiusX = i.radius; i.radiusY = i.radius;
                        }
                    if (searcher.result.inBox != null) foreach (var i in searcher.result.inBox.circles)
                        {
                            i.radiusX = i.radius; i.radiusY = i.radius;
                        }
                    if (searcher.result.inBox != null) foreach (var i in searcher.result.inBox.arcs)
                        {
                            i.radiusX = i.radius; i.radiusY = i.radius;
                        }
                    if (searcher.result.inPlacementScheme != null) foreach (var i in searcher.result.inPlacementScheme.circles)
                        {
                            i.radiusX = i.radius; i.radiusY = i.radius;
                        }
                    if (searcher.result.inPlacementScheme != null) foreach (var i in searcher.result.inPlacementScheme.arcs)
                        {
                            i.radiusX = i.radius; i.radiusY = i.radius;
                        }
                    ////
                    if (!mainList.Exists(x => x.id == searcher.result.id))
                        mainList.Add(searcher.result);
                    int current = -1;
                    foreach (var i in mainWorkList)
                        if (i.localID > current)
                            current = i.localID;
                    current++;
                    //calculate proportions for 10 000
                    List<Equipment.Point> fake = new List<Equipment.Point>();
                    if (searcher.result.inPlacementScheme != null) fake.Add(searcher.result.inPlacementScheme.GetProp());
                    else fake.Add(null);
                    if (searcher.result.inConnectionScheme != null) fake.Add(searcher.result.inConnectionScheme.GetProp());
                    else fake.Add(null);
                    if (searcher.result.inBox != null) fake.Add(searcher.result.inBox.GetProp());
                    else fake.Add(null);
                    if (searcher.result.inStructural != null) fake.Add(searcher.result.inStructural.GetProp());
                    else fake.Add(null);

                    List<Point> real = new List<Point>();
                    for (int i = 0; i < 4; i++)
                    {
                        if (fake[i] == null) { real.Add(new Point(100, 100)); continue; }
                        float x = (float)Math.Sqrt(10000 / (fake[i].X * fake[i].Y));
                        real.Add(new Point() { X = (int)(fake[i].X * x), Y = (int)(fake[i].Y * x) });
                    }

                    mainWorkList.Add(new boxes() { localID = current, globalId = searcher.result.id, locations = new Point[] { new Point(0, 0), new Point(0, 0), new Point(0, 0), new Point(0, 0) }, scales = real.ToArray() });
                    List<drawer> sortedList = new List<drawer>();
                    sortedList.AddRange(mainWorkList.FindAll(x => x is boxes));
                    sortedList.AddRange(mainWorkList.FindAll(x => !(x is boxes)));
                    mainWorkList = sortedList;
                }
                else if (searcher.result.isInBox)
                {
                    ////

                    if (searcher.result.inConnectionScheme != null) foreach (var i in searcher.result.inConnectionScheme.circles)
                        {
                            i.radiusX = i.radius; i.radiusY = i.radius;
                        }
                    if (searcher.result.inConnectionScheme != null) foreach (var i in searcher.result.inConnectionScheme.arcs)
                        {
                            i.radiusX = i.radius; i.radiusY = i.radius;
                        }
                    if (searcher.result.inStructural != null) foreach (var i in searcher.result.inStructural.circles)
                        {
                            i.radiusX = i.radius; i.radiusY = i.radius;
                        }
                    if (searcher.result.inStructural != null) foreach (var i in searcher.result.inStructural.arcs)
                        {
                            i.radiusX = i.radius; i.radiusY = i.radius;
                        }
                    if (searcher.result.inBox != null) foreach (var i in searcher.result.inBox.circles)
                        {
                            i.radiusX = i.radius; i.radiusY = i.radius;
                        }
                    if (searcher.result.inBox != null) foreach (var i in searcher.result.inBox.arcs)
                        {
                            i.radiusX = i.radius; i.radiusY = i.radius;
                        }
                    if (searcher.result.inPlacementScheme != null) foreach (var i in searcher.result.inPlacementScheme.circles)
                        {
                            i.radiusX = i.radius; i.radiusY = i.radius;
                        }
                    if (searcher.result.inPlacementScheme != null) foreach (var i in searcher.result.inPlacementScheme.arcs)
                        {
                            i.radiusX = i.radius; i.radiusY = i.radius;
                        }
                    ////
                    for (int i = 0; i < searcher.result.compatibilities.Count; i++)
                        searcher.result.compatibilities[i].isMama = true;
                    if (!mainList.Exists(x => x.id == searcher.result.id))
                        mainList.Add(searcher.result);
                    int current = -1;
                    foreach (var i in mainWorkList)
                        if (i.localID > current)
                            current = i.localID;
                    current++;
                    //calculate proportions for 10 000
                    List<Equipment.Point> fake = new List<Equipment.Point>();
                    if (searcher.result.inPlacementScheme != null) fake.Add(searcher.result.inPlacementScheme.GetProp());
                    else fake.Add(null);
                    if (searcher.result.inConnectionScheme != null) fake.Add(searcher.result.inConnectionScheme.GetProp());
                    else fake.Add(null);
                    if (searcher.result.inBox != null) fake.Add(searcher.result.inBox.GetProp());
                    else fake.Add(null);
                    if (searcher.result.inStructural != null) fake.Add(searcher.result.inStructural.GetProp());
                    else fake.Add(null);

                    List<Point> real = new List<Point>();
                    for (int i = 0; i < 4; i++)
                    {
                        if (fake[i] == null) { real.Add(new Point(100, 100)); continue; }
                        float x = (float)Math.Sqrt(10000 / (fake[i].X * fake[i].Y));
                        real.Add(new Point() { X = (int)(fake[i].X * x), Y = (int)(fake[i].Y * x) });
                    }

                    mainWorkList.Add(new inboxes()
                    {
                        numberOfUnits = Convert.ToInt32(searcher.result.properties["Занимаемых юнитов (шт)"]),
                        localID = current,
                        globalId = searcher.result.id,
                        locations = new Point[] { new Point(0, 0), new Point(0, 0), new Point(0, 0), new Point(0, 0) },
                        scales = real.ToArray()
                    });

                }
                else if (searcher.result.isWire)
                {
                    int current = -1;
                    foreach (var i in mainWorkList)
                        if (i.localID > current)
                            current = i.localID;
                    current++;
                    if (!mainList.Exists(x => x.id == searcher.result.id))
                        mainList.Add(searcher.result);
                    if (searcher.result.compatibilities.Count == 1)
                        wires.Add(new Wire() { MyOwnFirst = searcher.result.compatibilities[0], MyOwnSecond = searcher.result.compatibilities[0] });
                    else wires.Add(new Wire() { MyOwnFirst = searcher.result.compatibilities[0], MyOwnSecond = searcher.result.compatibilities[1] });
                    tabControl1.SelectedIndex = 1;
                    MessageBox.Show("выберите начальное оборудование");
                    ConnectionController.targetWire = wires[wires.Count - 1];
                    ConnectionController.Mode = modeConnection.buildConnection;
                }
                else
                {
                    if (searcher.result.inConnectionScheme != null) foreach (var i in searcher.result.inConnectionScheme.circles)
                        {
                            i.radiusX = i.radius; i.radiusY = i.radius;
                        }
                    if (searcher.result.inConnectionScheme != null) foreach (var i in searcher.result.inConnectionScheme.arcs)
                        {
                            i.radiusX = i.radius; i.radiusY = i.radius;
                        }
                    if (searcher.result.inStructural != null) foreach (var i in searcher.result.inStructural.circles)
                        {
                            i.radiusX = i.radius; i.radiusY = i.radius;
                        }
                    if (searcher.result.inStructural != null) foreach (var i in searcher.result.inStructural.arcs)
                        {
                            i.radiusX = i.radius; i.radiusY = i.radius;
                        }
                    if (searcher.result.inBox != null) foreach (var i in searcher.result.inBox.circles)
                        {
                            i.radiusX = i.radius; i.radiusY = i.radius;
                        }
                    if (searcher.result.inBox != null) foreach (var i in searcher.result.inBox.arcs)
                        {
                            i.radiusX = i.radius; i.radiusY = i.radius;
                        }
                    if (searcher.result.inPlacementScheme != null) foreach (var i in searcher.result.inPlacementScheme.circles)
                        {
                            i.radiusX = i.radius; i.radiusY = i.radius;
                        }
                    if (searcher.result.inPlacementScheme != null) foreach (var i in searcher.result.inPlacementScheme.arcs)
                        {
                            i.radiusX = i.radius; i.radiusY = i.radius;
                        }
                    ////
                    if (!mainList.Exists(x => x.id == searcher.result.id))
                        mainList.Add(searcher.result);
                    int current = -1;
                    foreach (var i in mainWorkList)
                        if (i.localID > current)
                            current = i.localID;
                    current++;
                    //calculate proportions for 10 000
                    List<Equipment.Point> fake = new List<Equipment.Point>();
                    if (searcher.result.inPlacementScheme != null) fake.Add(searcher.result.inPlacementScheme.GetProp());
                    else fake.Add(null);
                    if (searcher.result.inConnectionScheme != null) fake.Add(searcher.result.inConnectionScheme.GetProp());
                    else fake.Add(null);
                    if (searcher.result.inBox != null) fake.Add(searcher.result.inBox.GetProp());
                    else fake.Add(null);
                    if (searcher.result.inStructural != null) fake.Add(searcher.result.inStructural.GetProp());
                    else fake.Add(null);

                    List<Point> real = new List<Point>();
                    for (int i = 0; i < 4; i++)
                    {
                        if (fake[i] == null) { real.Add(new Point(100, 100)); continue; }
                        float x = (float)Math.Sqrt(10000 / (fake[i].X * fake[i].Y));
                        real.Add(new Point() { X = (int)(fake[i].X * x), Y = (int)(fake[i].Y * x) });
                    }

                    mainWorkList.Add(new free()
                    {
                        localID = current,
                        globalId = searcher.result.id,
                        locations = new Point[] { new Point(0, 0), new Point(0, 0), new Point(0, 0), new Point(0, 0) },
                        scales = real.ToArray()
                    });
                }
                foreach (var i in mainWorkList)
                    i.drawBox(gr[sheetIndex]);
                pictureBox3.Image = bitmaps[0];
                pictureBox2.Image = bitmaps[1];
                pictureBox1.Image = bitmaps[2];
                strct.Image = bitmaps[3];
                Refresh();
            }
        }





        private void Schemes_Editor_Resize(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < 4; i++)
                {
                    bitmaps[i] = new Bitmap(sheets[sheetIndex].Width, sheets[sheetIndex].Height);
                }
                for (int i = 0; i < 4; i++)
                {
                    gr[i] = Graphics.FromImage(bitmaps[i]);
                    gr[i].SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                }
                BoxController.draw();
            }
            catch (Exception ex) { }
        }


        private void radioButton1_MouseClick(object sender, MouseEventArgs e)
        {
            // radioButton1.Checked = !radioButton1.Checked;
        }
        bool scale_mode = false;
        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "Режим масштабирования\nвыключен")
            {
                button1.Text = "Режим масштабирования\nвключен";
                button1.BackColor = Color.LightGreen;
                BoxController.Mode = modeShkaf.doNothing_SCALEMODE;
                StructuralController.Mode = modeStruct.doNothing_SCALEMODE;
                PlacementController.Mode = modePlacement.doNothing_SCALEMODE;
                ConnectionController.Mode = modeConnection.doNothing_SCALEMODE;

                button2.Text = "Режим редактирования кабеля выключен";
                button2.BackColor = Color.Pink;
            }
            else if (BoxController.Mode == modeShkaf.doNothing_SCALEMODE)
            {
                button1.Text = "Режим масштабирования\nвыключен";
                button1.BackColor = Color.Pink;
                BoxController.Mode = modeShkaf.doNothing_NOSCALEMODE;
                StructuralController.Mode = modeStruct.doNothing_NOSCALEMODE;
                PlacementController.Mode = modePlacement.doNothing_NOSCALEMODE;
                ConnectionController.Mode = modeConnection.doNothing_NOSCALEMODE;

                button2.Text = "Режим редактирования кабеля выключен";
                button2.BackColor = Color.Pink;
            };
            BoxController.draw();
            StructuralController.draw();
            PlacementController.draw();
            ConnectionController.draw();
        }
        static public double distance(Equipment.Point a, Equipment.Point b)
        {
            return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }
        static public double distance(Point a, Point b)
        {
            return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }
        public static int sheetIndex;




        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            sheetIndex = ((TabControl)sender).SelectedIndex;
            PlacementController.draw();
            ConnectionController.draw();
            StructuralController.draw();
            BoxController.draw();
        }

        private void добавитьКомнатуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RoomCreator cr = new RoomCreator();
            if (cr.ShowDialog() == DialogResult.OK)
            {
                rooms.Add(new Room() { roomName = cr.roomName });
                PlacementController.draw();
                ConnectionController.draw();
                StructuralController.draw();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

            if (button2.Text == "Режим редактирования кабеля включен")
            {
                button1.Text = "Режим масштабирования\nвыключен";
                button1.BackColor = Color.Pink;
                BoxController.Mode = modeShkaf.doNothing_NOSCALEMODE;
                StructuralController.Mode = modeStruct.doNothing_NOSCALEMODE;
                PlacementController.Mode = modePlacement.doNothing_NOSCALEMODE;
                ConnectionController.Mode = modeConnection.doNothing_NOSCALEMODE;

                button2.Text = "Режим редактирования кабеля выключен";
                button2.BackColor = Color.Pink;
            }
            else
            {
                button1.Text = "Режим масштабирования\nвыключен";
                button1.BackColor = Color.Pink;
                BoxController.Mode = modeShkaf.editWire;
                StructuralController.Mode = modeStruct.editWire;
                PlacementController.Mode = modePlacement.editWire;
                ConnectionController.Mode = modeConnection.editWire;

                button2.Text = "Режим редактирования кабеля включен";
                button2.BackColor = Color.LightGreen;
            }
            BoxController.draw();
            StructuralController.draw();
            PlacementController.draw();
            ConnectionController.draw();
        }
    }
    public class Wire
    {
        public List<List<Point>> points = new List<List<Point>>();
        public Equipment.Compatibility MyOwnFirst, MyOwnSecond;
        public bool first = true, second = true;
        public dynamic firstEquip = null, secondEquip = null;
        public void draw(Graphics g, int i)
        {

            if (points.Count == 0) return;


            var t = (drawer)firstEquip;
            var pa = new Point(t.locations[i].X + t.scales[i].X / 2, t.locations[i].Y + t.scales[i].Y / 2);
            var q = (drawer)secondEquip;
            Point dva = new Point(q.locations[i].X + q.scales[i].X / 2, q.locations[i].Y + q.scales[i].Y / 2);

            points[i][0] = pa;
            points[i][points[i].Count-1] = dva;

            for (int j = 0; j < points[i].Count + 1; j++)
            {
                g.DrawLine(Pens.Black, points[i][0], points[i][points[i].Count - 1]);
            }

            //var t = (drawer)firstEquip;
            //var pa = new Point(t.locations[i].X + t.scales[i].X / 2, t.locations[i].Y + t.scales[i].Y / 2);
            //var q = (drawer)secondEquip;
            //Point dva = new Point(q.locations[i].X + q.scales[i].X / 2, q.locations[i].Y + q.scales[i].Y / 2);
            //g.DrawLine(Pens.Black, pa,dva);

        }
        public void createPoints()
        {
            points = new List<List<Point>>();
            points.Add(new List<Point>());
            var t = (drawer)firstEquip;
            for (int i = 0; i < 4; i++)
            {
                points.Add(new List<Point>());
                points[i].Add(new Point(t.locations[i].X + t.scales[i].X / 2, t.locations[i].Y + t.scales[i].Y / 2));
            }
            for(int i = 0; i < 4; i++)
            {
                var q = (drawer)secondEquip;
                Point dva = new Point(q.locations[i].X + q.scales[i].X / 2, q.locations[i].Y + q.scales[i].Y / 2);
                double dist = Schemes_Editor.distance(dva, points[i][0]);
                double shag = dist / 10.0;
                Point prev = points[i][0];
                double angle = Math.Atan((dva.X - (points[i][0].X * 1.0)) / (dva.Y - (points[i][0].Y * 1.0)));
                for (int j = 0; j < 10; j++)
                {
                    points[i].Add(new Point((int)(prev.X+ shag * Math.Sin(angle)), (int)(prev.Y + shag * Math.Cos(angle))));
                    prev = new Point((int)(prev.X + shag * Math.Sin(angle)), (int)(prev.Y + shag * Math.Cos(angle)));
                }
                points[i].Add(dva);
            }

        }
    }
    public class Room
    {
        public Rectangle[] locations = new Rectangle[] { new Rectangle(0, 0, 200, 200), new Rectangle(0, 0, 200, 200), new Rectangle(), new Rectangle(0, 0, 200, 200) };
        public string roomName;
        public void move(Point a, int index)
        {
            locations[index] = new Rectangle(a.X, a.Y, locations[index].Width, locations[index].Height);
        }
        public void draw(Graphics g, int index )
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

    public class Vinoska
    {
        public string text;
        public Point vertex1, vertex2, startPoint;
        public Vinoska(string a, Point start, Point b, Point c)
        {
            text = a;
            vertex1 = b;
            vertex2 = c;
            startPoint = start;
        }
    }
    public abstract class drawer
    {
        public Vinoska vinoska;
        public Point[] locations; //position on list
        public Point[] scales;
        public int globalId, localID;
        public abstract void createVinosku();
        public abstract void drawPlace(Graphics g);
        public abstract void drawCon(Graphics g);
        public abstract void drawBox(Graphics g);
        public abstract void drawStr(Graphics g);
        public abstract bool inside(Point a, int scheme);
        public abstract void move(Point offset, int scheme);
        public abstract void offset(Point offset, int scheme);
    }
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
            int i = 2;
            g.DrawRectangle(Pens.Black, locations[i].X, locations[i].Y, scales[i].X, scales[i].Y);
            g.DrawRectangle(Pens.Black, locations[i].X + 20, locations[i].Y + 30, scales[i].X - 40, scales[i].Y - 60);
            unitSize = (scales[i].Y - 60) / (units * 1.0f);

            for (int j = 0; j < equipInside.Count; j++)
            {
                equipInside[j].locations[i] = new Point(locations[i].X + 20, (int)(locations[i].Y + 30 + positions[j] * unitSize));
                equipInside[j].scales[i] = new Point(scales[i].X - 40, (int)unitSize);
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
                equipInside[j].locations[i] = new Point(locations[i].X + 20, (int)(locations[i].Y + 30 + positions[j] * unitSize));
                equipInside[j].scales[i] = new Point(scales[i].X - 40, (int)unitSize);
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

        public override void createVinosku()
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
    }
    public class inboxes : drawer
    {
        public List<int> seized = new List<int>();
        public bool isShowingOnConnectionScheme = true;
        public List<UsedInterface> listINterfaces = new List<UsedInterface>();
        public bool inbox = false;
        public int numberOfUnits = -1;
        public override void move(Point offset, int scheme)
        {
            if (vinoska != null)
            {
                vinoska.startPoint = new Point(locations[scheme].X + scales[scheme].X / 2, locations[scheme].Y + scales[scheme].Y / 2);

                vinoska.vertex1 = new Point(offset.X + locations[scheme].X - vinoska.vertex1.X, offset.Y + locations[scheme].Y - vinoska.vertex1.Y);
                vinoska.vertex2 = new Point(offset.X + locations[scheme].X - vinoska.vertex2.X, offset.Y + locations[scheme].Y - vinoska.vertex2.Y);
            }
            locations[scheme] = offset;

        }
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
            int localSheetIndex = 2;
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
            if (vinoska != null)
            {
                g.DrawLines(Pens.Blue, new Point[] { vinoska.startPoint, vinoska.vertex1, vinoska.vertex2 });
                g.DrawString(vinoska.text, new Font("Arial", 14), Brushes.DarkGreen, vinoska.vertex1.X, vinoska.vertex1.Y - 30);
            }
        }

        public override void createVinosku()
        {
            if (inbox)
            {
                //поиск целевого шкафа
                int index = ((boxes)Schemes_Editor.mainWorkList.Find(x => x is boxes && ((boxes)x).equipInside.Exists(y => y.localID == localID))).equipInside.IndexOf(this);
                vinoska = new Vinoska(index.ToString(), new Point(locations[2].X + scales[2].X / 2, locations[2].Y + scales[2].Y / 2), locations[Schemes_Editor.sheetIndex], new Point(locations[Schemes_Editor.sheetIndex].X + 30, locations[Schemes_Editor.sheetIndex].Y));
            }
        }

        public override void drawPlace(Graphics g)
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

                StringFormat f = new StringFormat();
                f.Alignment = StringAlignment.Center;
                //найти название
                if (Schemes_Editor.mainList.Find(x => x.id == globalId) != null)
                {
                    string roomName = Schemes_Editor.mainList.Find(x => x.id == globalId).name;
                    g.DrawString(roomName, new Font("Arial", 10), Brushes.DarkRed, new RectangleF(locations[localSheetIndex].X, locations[localSheetIndex].Y - 30, scales[localSheetIndex].X, 30), f);

                }

            }
        }

        public override void drawStr(Graphics g)
        {
            int localSheetIndex = 3;

            g.DrawLines(new Pen(Brushes.Black, 3), new Point[] { locations[localSheetIndex], new Point(locations[localSheetIndex].X + scales[localSheetIndex].X, locations[localSheetIndex].Y), new Point(locations[localSheetIndex].X + scales[localSheetIndex].X, locations[localSheetIndex].Y + scales[localSheetIndex].Y), new Point(locations[localSheetIndex].X, locations[localSheetIndex].Y + scales[localSheetIndex].Y), locations[localSheetIndex] });

            //if (vinoska != null)
            //{
            //    g.DrawLines(Pens.Blue, new Point[] { vinoska.startPoint, vinoska.vertex1, vinoska.vertex2 });
            //    g.DrawString(vinoska.text, new Font("Arial", 14), Brushes.DarkGreen, vinoska.vertex1.X, vinoska.vertex1.Y - 30);
            //}
        }

        public override void offset(Point offset, int scheme)
        {
         
        }

       
    }

    public class free : drawer
    {
        public List<int> seized = new List<int>();
        public override void createVinosku()
        {
        ////
        }

        public override void drawBox(Graphics g)
        {
         /////////////////////
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
    }


    public class UsedInterface
    {
        public UsedInterface connected;
        public int id; //для сопоставления
        public bool isMama;
        public int fathersLocalID;
    }

}
