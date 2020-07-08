
using System;
using System.Collections.Generic;

using System.Drawing;
using System.IO;

using System.Text;

using System.Windows.Forms;

using Newtonsoft.Json;
namespace SCS_Module
{

    public partial class Schemes_Editor : System.Windows.Forms.Form
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
            ConnectionController.father = this;
            PlacementController.father = this;
            StructuralController.father = this;

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

            RevitProvider.synchronizer();
        }

        static public List<Equipment> mainList = new List<Equipment>();
        static public List<drawer> mainWorkList = new List<drawer>();
        static public List<Room> rooms = new List<Room>();
        static public List<Wire> wires = new List<Wire>();


        static public System.Drawing.Rectangle[] recs = new System.Drawing.Rectangle[4];
        static public Bitmap[] bitmaps = new Bitmap[4];
        static public Graphics[] gr = new Graphics[4];
        static public PictureBox[] sheets;

        public void copy(int globalID, drawer elem = null)
        {
            int id = -1;
            if (elem != null)
            {
                var target = mainList.Find(x => x.id == elem.globalId);

               id =  RevitProvider.copy(target);


                if (target.isBox)
                {
                    mainWorkList.Add(new boxes() { localID = id, globalId = elem.globalId, locations = new Point[] { new Point(0, 0), new Point(0, 0), new Point(0, 0), new Point(0, 0) }, scales = new List<Point>(elem.scales).ToArray() });
                }
                else if (target.isInBox)
                {

                    mainWorkList.Add(new inboxes()
                    {
                        numberOfUnits = Convert.ToInt32(target.properties["Занимаемых юнитов (шт)"]),
                        localID = id,
                        globalId = elem.globalId,
                        locations = new Point[] { new Point(0, 0), new Point(0, 0), new Point(0, 0), new Point(0, 0) },
                        scales = new List<Point>(elem.scales).ToArray()
                    });
                }
                else
                {
                    mainWorkList.Add(new free()
                    {
                        localID = id,
                        globalId = elem.globalId,
                        locations = new Point[] { new Point(0, 0), new Point(0, 0), new Point(0, 0), new Point(0, 0) },
                        scales = new List<Point>(elem.scales).ToArray()
                    });
                }
            }
            else
            {
                var target = mainList.Find(x => x.id == globalID);

                int current = -1;
                foreach (var i in mainWorkList)
                    if (i.localID > current)
                        current = i.localID;
                current++;


                if (target.compatibilities.Count == 1)
                    wires.Add(new Wire() { MyOwnFirst = target.compatibilities[0], MyOwnSecond = target.compatibilities[0] });
                else wires.Add(new Wire() { MyOwnFirst = target.compatibilities[0], MyOwnSecond = target.compatibilities[1] });
                tabControl1.SelectedIndex = 1;
                //      MessageBox.Show("выберите начальное оборудование");
                ConnectionController.targetWire = wires[wires.Count - 1];
                ConnectionController.Mode = modeConnection.buildConnection;
            }
        }



        private void добавитьСхемуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Searcher searcher = new Searcher();
            if (searcher.ShowDialog() == DialogResult.OK)
            {
                Equipment result = searcher.result;
                List<Equipment.Point> fake = new List<Equipment.Point>();
                radiusesFixer(ref result, ref fake);
                if (result.isBox)
                {

                    mainList.RemoveAll(x => x.id == result.id);
                    mainList.Add(result);



                    List<Point> real = new List<Point>();
                    for (int i = 0; i < 4; i++)
                    {
                        if (fake[i] == null) { real.Add(new Point(100, 100)); continue; }
                        float x = (float)Math.Sqrt(10000 / (fake[i].X * fake[i].Y));
                        real.Add(new Point() { X = (int)(fake[i].X * x), Y = (int)(fake[i].Y * x) });
                    }


                    int id = RevitProvider.createInstance(result.bytedFile, result.name);
                    bool hasFamily = id != -1;
                    Random r = new Random();
                    if (!hasFamily)
                    {
                        id = -1 * r.Next(0, int.MaxValue);
                        while (mainWorkList.Exists(x => x.localID == id))
                            id = -1 * r.Next(0, int.MaxValue);
                    }
                    mainWorkList.Add(new boxes() { hasFamily = hasFamily, localID = id, globalId = result.id, locations = new Point[] { new Point(0, 0), new Point(0, 0), new Point(0, 0), new Point(0, 0) }, scales = real.ToArray() });
                    List<drawer> sortedList = new List<drawer>();
                    sortedList.AddRange(mainWorkList.FindAll(x => x is boxes));
                    sortedList.AddRange(mainWorkList.FindAll(x => !(x is boxes)));
                    mainWorkList = sortedList;


                }
                else if (result.isInBox)
                {

                    mainList.RemoveAll(x => x.id == result.id);
                    mainList.Add(result);


                    //calculate proportions for 10 000


                    List<Point> real = new List<Point>();
                    for (int i = 0; i < 4; i++)
                    {
                        if (fake[i] == null)
                        {
                            real.Add(new Point(100, 100));
                            continue;
                        }
                        float x = (float)Math.Sqrt(10000 / (fake[i].X * fake[i].Y));
                        real.Add(new Point() { X = (int)(fake[i].X * x), Y = (int)(fake[i].Y * x) });
                    }


                    int id = RevitProvider.createInstance(result.bytedFile, result.name);
                    bool hasFamily = id != -1;
                    Random r = new Random();
                    if (!hasFamily)
                    {
                        id = -1 * r.Next(0, int.MaxValue);
                        while (mainWorkList.Exists(x => x.localID == id))
                            id = -1 * r.Next(0, int.MaxValue);
                    }

                    mainWorkList.Add(new inboxes()
                    {
                        hasFamily = hasFamily,
                        numberOfUnits = Convert.ToInt32(result.properties["Занимаемых юнитов (шт)"]),
                        localID = id,
                        globalId = result.id,
                        locations = new Point[] { new Point(0, 0), new Point(0, 0), new Point(0, 0), new Point(0, 0) },
                        scales = real.ToArray()
                    });


                }
                else if (result.isWire)
                {
                    int current = -1;
                    foreach (var i in wires)
                        if (i.localID > current)
                            current = i.localID;
                    current++;
                    if (!mainList.Exists(x => x.id == result.id))
                        mainList.Add(result);


                    if (result.compatibilities.Count == 1)
                        wires.Add(new Wire() { localID = current, globalId = result.id, MyOwnFirst = result.compatibilities[0], MyOwnSecond = result.compatibilities[0] });
                    else
                        wires.Add(new Wire() { localID= current, globalId = result.id, MyOwnFirst = result.compatibilities[0], MyOwnSecond = result.compatibilities[1] });
                    tabControl1.SelectedIndex = 1;
                    //      MessageBox.Show("выберите начальное оборудование");
                    ConnectionController.targetWire = wires[wires.Count - 1];
                    ConnectionController.Mode = modeConnection.buildConnection;
                }
                else
                {
                    mainList.RemoveAll(x => x.id == result.id);
                    mainList.Add(result);
                    //calculate proportions for 10 000

                    List<Point> real = new List<Point>();
                    for (int i = 0; i < 4; i++)
                    {
                        if (fake[i] == null) { real.Add(new Point(100, 100)); continue; }
                        float x = (float)Math.Sqrt(10000 / (fake[i].X * fake[i].Y));
                        real.Add(new Point() { X = (int)(fake[i].X * x), Y = (int)(fake[i].Y * x) });
                    }

                    int id = RevitProvider.createInstance(result.bytedFile, result.name);
                    bool hasFamily = id != -1;
                    Random r = new Random();
                    if (!hasFamily)
                    {
                        id = -1 * r.Next(0, int.MaxValue);
                        while (mainWorkList.Exists(x => x.localID == id))
                            id = -1 * r.Next(0, int.MaxValue);
                    }

                    mainWorkList.Add(new free()
                    {
                        hasFamily = hasFamily,
                        localID = id,
                        globalId = result.id,
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
            if (button1.Text == "Режим масштабирования выключен")
            {
                button1.Text = "Режим масштабирования включен";
                button1.BackColor = System.Drawing.Color.LightGreen;
                BoxController.Mode = modeShkaf.doNothing_SCALEMODE;
                StructuralController.Mode = modeStruct.doNothing_SCALEMODE;
                PlacementController.Mode = modePlacement.doNothing_SCALEMODE;
                ConnectionController.Mode = modeConnection.doNothing_SCALEMODE;

                button2.Text = "Режим редактирования кабеля выключен";
                button2.BackColor = System.Drawing.Color.Pink;
            }
            else if (BoxController.Mode == modeShkaf.doNothing_SCALEMODE)
            {
                button1.Text = "Режим масштабирования выключен";
                button1.BackColor = System.Drawing.Color.Pink;
                BoxController.Mode = modeShkaf.doNothing_NOSCALEMODE;
                StructuralController.Mode = modeStruct.doNothing_NOSCALEMODE;
                PlacementController.Mode = modePlacement.doNothing_NOSCALEMODE;
                ConnectionController.Mode = modeConnection.doNothing_NOSCALEMODE;

                button2.Text = "Режим редактирования кабеля выключен";
                button2.BackColor = System.Drawing.Color.Pink;
            };
            if(sheetIndex==0) PlacementController.draw();
            if (sheetIndex == 2) BoxController.draw();
            if (sheetIndex == 3) StructuralController.draw();

            if (sheetIndex == 1) ConnectionController.draw();
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


        void radiusesFixer(ref Equipment result, ref List<Equipment.Point> fake)
        {
            if (result.inConnectionScheme != null)
                foreach (var i in result.inConnectionScheme.circles)
                {
                    i.radiusX = i.radius;
                    i.radiusY = i.radius;
                }
            if (result.inStructural != null)
                foreach (var i in result.inStructural.circles)
                {
                    i.radiusX = i.radius;
                    i.radiusY = i.radius;
                }
            if (result.inBox != null)
                foreach (var i in result.inBox.circles)
                {
                    i.radiusX = i.radius;
                    i.radiusY = i.radius;
                }

            if (result.inPlacementScheme != null)
                foreach (var i in result.inPlacementScheme.circles)
                {
                    i.radiusX = i.radius;
                    i.radiusY = i.radius;
                }

            fake = new List<Equipment.Point>();
            if (result.inPlacementScheme != null) fake.Add(result.inPlacementScheme.GetProp());
            else fake.Add(null);
            if (result.inConnectionScheme != null) fake.Add(result.inConnectionScheme.GetProp());
            else fake.Add(null);
            if (result.inBox != null) fake.Add(result.inBox.GetProp());
            else fake.Add(null);
            if (result.inStructural != null) fake.Add(result.inStructural.GetProp());
            else fake.Add(null);
        }

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
                rooms.Add(new Room() { labels = new string[] {cr.roomName, cr.roomName , cr.roomName , cr.roomName } });
                PlacementController.draw();
                ConnectionController.draw();
                StructuralController.draw();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

         /*   if (button2.Text == "Режим редактирования кабеля включен")
            {
                button1.Text = "Режим масштабирования\nвыключен";
                button1.BackColor = System.Drawing.Color.Pink;
                BoxController.Mode = modeShkaf.doNothing_NOSCALEMODE;
                StructuralController.Mode = modeStruct.doNothing_NOSCALEMODE;
                PlacementController.Mode = modePlacement.doNothing_NOSCALEMODE;
                ConnectionController.Mode = modeConnection.doNothing_NOSCALEMODE;

                button2.Text = "Режим редактирования кабеля выключен";
                button2.BackColor = System.Drawing.Color.Pink;
            }
            else
            {
                button1.Text = "Режим масштабирования\nвыключен";
                button1.BackColor = System.Drawing.Color.Pink;
                BoxController.Mode = modeShkaf.editWire;
                StructuralController.Mode = modeStruct.editWire;
                PlacementController.Mode = modePlacement.editWire;
                ConnectionController.Mode = modeConnection.editWire;

                button2.Text = "Режим редактирования кабеля включен";
                button2.BackColor = System.Drawing.Color.LightGreen;
            }
            BoxController.draw();
            StructuralController.draw();
            PlacementController.draw();
            ConnectionController.draw();*/
        }

        private void экспортСхемToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //самый главный элемент
            string result = "";
            foreach(var i in mainWorkList)
            {
                i.drawPlaceExp(ref result);
                i.drawConExp(ref result);
                i.drawBoxExp(ref result);
                i.drawStrExp(ref result);
            }
            SaveFileDialog file = new SaveFileDialog();
            file.Filter = "Script files (*.scr)|*.scr";
            if(file.ShowDialog() == DialogResult.OK)
            {
                File.Create(file.FileName).Close();
                File.WriteAllText(file.FileName, result, Encoding.GetEncoding(1251));
            }
        }

        private void сохранитьСостояниеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog file = new SaveFileDialog();
            if(file.ShowDialog() == DialogResult.OK)
            {
                File.Create(file.FileName).Close();
                saver s = new saver();
                s.mainList = mainList;
                s.mainWorkList = mainWorkList;
                s.rooms = rooms;
                s.wires = wires;
                File.WriteAllText(file.FileName, JsonConvert.SerializeObject(s));
            }
        }
        class saver
        { 
            public List<Equipment> mainList = new List<Equipment>();
            public List<drawer> mainWorkList = new List<drawer>();
            public List<Room> rooms = new List<Room>();
            public List<Wire> wires = new List<Wire>();
        }

        private void загрузитьСостояниеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            if(file.ShowDialog() == DialogResult.OK)
            {
                saver s = JsonConvert.DeserializeObject<saver>(File.ReadAllText(file.FileName));
                mainList = s.mainList;
                mainWorkList = s.mainWorkList;
                rooms = s.rooms;
                wires = s.wires;
            }
        }
    }











    public class UsedInterface
    {
        public int id; //для сопоставления
        public bool isMama;
        public int fathersLocalID;
    }

}
