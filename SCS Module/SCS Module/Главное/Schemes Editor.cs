using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SCS_Module
{

    public partial class Schemes_Editor : System.Windows.Forms.Form
    {
        ExternalCommandData commandData;
        public Schemes_Editor(ExternalCommandData commandData)
        {
            InitializeComponent();
            string a = "";
            Transaction tr = null;
            try
            {
                Document doc = commandData.Application.ActiveUIDocument.Document;
                tr = new Transaction(doc);
                tr.Start("My Super trans");
                for (int i = 0; i < mainWorkList.Count; i++)
                {


                    Element el = doc.GetElement(new ElementId(mainWorkList[i].localID));
                    if (el == null)
                    {
                        foreach (var j in mainWorkList)
                            if (j is boxes)
                            {
                                ((boxes)j).equipInside.RemoveAll(x => x.localID == mainWorkList[i].localID);
                            }
                        wires.RemoveAll(x => (x.firstEquip != null && x.firstEquip.localID == mainWorkList[i].localID)
                        || (x.secondEquip != null && x.secondEquip.localID == mainWorkList[i].localID));

                        a += $"Было уничтожено оборудование {mainList.Find(x => x.id == mainWorkList[i].globalId).name}\n";
                        mainWorkList.RemoveAt(i);
                        i--;
                    }
                }
                tr.Commit();
            }
            catch (Exception ex)
            {
                try
                {
                    tr.Commit();
                }
                catch (Exception exxx)
                {

                }
            }

            if (a != "") MessageBox.Show(a);


            this.commandData = commandData;

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
                Transaction tr = null;
                try
                {
                    File.Create(target.name + ".rfa").Close();
                    File.WriteAllBytes(target.name + ".rfa", target.bytedFile);

                    Document doc = commandData.Application.ActiveUIDocument.Document;
                    Autodesk.Revit.DB.Family family = null;

                    tr = new Transaction(doc);
                    tr.Start("My Super trans");
                    //попытка найти существующее семейство
                    var search = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).Cast<FamilySymbol>().ToList();
                    bool good = false;
                    foreach (FamilySymbol symbq in search)
                        if (symbq.Family.Name == target.name)
                        {
                            if (!symbq.IsActive)
                                symbq.Activate();
                            var buffInstance = doc.Create.NewFamilyInstance(new XYZ(0, 0, 0), symbq, structuralType: Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                            buffInstance.Id.ToString();
                            id = Convert.ToInt32(buffInstance.Id.ToString());
                            good = true;
                            break;

                        }
                    if (!good)
                    {
                        doc.LoadFamily(target.name + ".rfa", out family);

                        var fsym = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).Cast<FamilySymbol>().ToList();

                        String name = family.Name;
                        foreach (FamilySymbol symbq in fsym)
                        {
                            if (symbq.Family.Name == name)
                            {
                                if (!symbq.IsActive)
                                    symbq.Activate();
                                var buffInstance = doc.Create.NewFamilyInstance(new XYZ(0, 0, 0), symbq, structuralType: Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                                buffInstance.Id.ToString();
                                id = Convert.ToInt32(buffInstance.Id.ToString());
                                break;
                            }
                        }
                    }
                    tr.Commit();
                }
                catch (Exception ex)
                {
                    try
                    {
                        tr.Commit();
                    }
                    catch (Exception exxx)
                    {

                    }
                }



                if (target.isBox)
                {
                    mainWorkList.Add(new boxes() { localID = id, globalId = elem.globalId, locations = new System.Drawing.Point[] { new System.Drawing.Point(0, 0), new System.Drawing.Point(0, 0), new System.Drawing.Point(0, 0), new System.Drawing.Point(0, 0) }, scales = new List<System.Drawing.Point>(elem.scales).ToArray() });
                }
                else if (target.isInBox)
                {

                    mainWorkList.Add(new inboxes()
                    {
                        numberOfUnits = Convert.ToInt32(target.properties["Занимаемых юнитов (шт)"]),
                        localID = id,
                        globalId = elem.globalId,
                        locations = new System.Drawing.Point[] { new System.Drawing.Point(0, 0), new System.Drawing.Point(0, 0), new System.Drawing.Point(0, 0), new System.Drawing.Point(0, 0) },
                        scales = new List<System.Drawing.Point>(elem.scales).ToArray()
                    });
                }
                else
                {
                    mainWorkList.Add(new free()
                    {
                        localID = id,
                        globalId = elem.globalId,
                        locations = new System.Drawing.Point[] { new System.Drawing.Point(0, 0), new System.Drawing.Point(0, 0), new System.Drawing.Point(0, 0), new System.Drawing.Point(0, 0) },
                        scales = new List<System.Drawing.Point>(elem.scales).ToArray()
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
                    Transaction tr = null;
                    int id = -1;
                    try
                    {
                        File.Create(searcher.result.name + ".rfa").Close();
                        File.WriteAllBytes(searcher.result.name + ".rfa", searcher.result.bytedFile);
                        Document doc = commandData.Application.ActiveUIDocument.Document;
                        Autodesk.Revit.DB.Family family = null;

                        tr = new Transaction(doc);
                        tr.Start("My Super trans");
                        //попытка найти существующее семейство
                        var search = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).Cast<FamilySymbol>().ToList();
                        bool good = false;
                        foreach (FamilySymbol symbq in search)
                            if (symbq.Family.Name == searcher.result.name)
                            {
                                if (!symbq.IsActive)
                                    symbq.Activate();
                                var buffInstance = doc.Create.NewFamilyInstance(new XYZ(0, 0, 0), symbq, structuralType: Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                                buffInstance.Id.ToString();
                                id = Convert.ToInt32(buffInstance.Id.ToString());
                                good = true;
                                break;
        
                            }
                        if (!good)
                        {
                            doc.LoadFamily(searcher.result.name + ".rfa", out family);

                            var fsym = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).Cast<FamilySymbol>().ToList();

                            String name = family.Name;
                            foreach (FamilySymbol symbq in fsym)
                            {
                                if (symbq.Family.Name == name)
                                {
                                    if (!symbq.IsActive)
                                        symbq.Activate();
                                    var buffInstance = doc.Create.NewFamilyInstance(new XYZ(0, 0, 0), symbq, structuralType: Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                                    buffInstance.Id.ToString();
                                    id = Convert.ToInt32(buffInstance.Id.ToString());
                                    break;
                                }
                            }
                        }
                            
                     
                        tr.Commit();
                        mainList.RemoveAll(x => x.id == searcher.result.id);
                            mainList.Add(searcher.result);

                        List<Equipment.Point> fake = new List<Equipment.Point>();
                        if (searcher.result.inPlacementScheme != null) fake.Add(searcher.result.inPlacementScheme.GetProp());
                        else fake.Add(null);
                        if (searcher.result.inConnectionScheme != null) fake.Add(searcher.result.inConnectionScheme.GetProp());
                        else fake.Add(null);
                        if (searcher.result.inBox != null) fake.Add(searcher.result.inBox.GetProp());
                        else fake.Add(null);
                        if (searcher.result.inStructural != null) fake.Add(searcher.result.inStructural.GetProp());
                        else fake.Add(null);

                        List<System.Drawing.Point> real = new List<System.Drawing.Point>();
                        for (int i = 0; i < 4; i++)
                        {
                            if (fake[i] == null) { real.Add(new System.Drawing.Point(100, 100)); continue; }
                            float x = (float)Math.Sqrt(10000 / (fake[i].X * fake[i].Y));
                            real.Add(new System.Drawing.Point() { X = (int)(fake[i].X * x), Y = (int)(fake[i].Y * x) });
                        }



                        mainWorkList.Add(new boxes() { localID = id, globalId = searcher.result.id, locations = new System.Drawing.Point[] { new System.Drawing.Point(0, 0), new System.Drawing.Point(0, 0), new System.Drawing.Point(0, 0), new System.Drawing.Point(0, 0) }, scales = real.ToArray() });
                        List<drawer> sortedList = new List<drawer>();
                        sortedList.AddRange(mainWorkList.FindAll(x => x is boxes));
                        sortedList.AddRange(mainWorkList.FindAll(x => !(x is boxes)));
                        mainWorkList = sortedList;



                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            tr.Commit();
                        }
                        catch(Exception eeee)
                        {

                        }
                    }
                    //int current = -1;
                    //foreach (var i in mainWorkList)
                    //    if (i.localID > current)
                    //        current = i.localID;
                    //current++;
                    //calculate proportions for 10 000
                   
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

                    ////////////////
                    int id = -1;
                    Transaction tr = null;
                    try
                    {
                        File.Create(searcher.result.name + ".rfa").Close();
                        File.WriteAllBytes(searcher.result.name + ".rfa", searcher.result.bytedFile);
                        Document doc = commandData.Application.ActiveUIDocument.Document;
                        Autodesk.Revit.DB.Family family = null;

                        tr = new Transaction(doc);
                        tr.Start("My Super trans");
                        var search = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).Cast<FamilySymbol>().ToList();
                        bool good = false;
                        foreach (FamilySymbol symbq in search)
                            if (symbq.Family.Name == searcher.result.name)
                            {
                                if (!symbq.IsActive)
                                    symbq.Activate();
                                var buffInstance = doc.Create.NewFamilyInstance(new XYZ(0, 0, 0), symbq, structuralType: Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                                buffInstance.Id.ToString();
                                id = Convert.ToInt32(buffInstance.Id.ToString());
                                good = true;
                                break;

                            }
                        if (!good)
                        {
                            doc.LoadFamily(searcher.result.name + ".rfa", out family);

                            var fsym = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).Cast<FamilySymbol>().ToList();

                            String name = family.Name;
                            foreach (FamilySymbol symbq in fsym)
                            {
                                if (symbq.Family.Name == name)
                                {
                                    if (!symbq.IsActive)
                                        symbq.Activate();
                                    var buffInstance = doc.Create.NewFamilyInstance(new XYZ(0, 0, 0), symbq, structuralType: Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                                    buffInstance.Id.ToString();
                                    id = Convert.ToInt32(buffInstance.Id.ToString());
                                    break;
                                }
                            }
                        }

                        tr.Commit();

                        mainList.RemoveAll(x => x.id == searcher.result.id);
                            mainList.Add(searcher.result);


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

                        List<System.Drawing.Point> real = new List<System.Drawing.Point>();
                        for (int i = 0; i < 4; i++)
                        {
                            if (fake[i] == null) { real.Add(new System.Drawing.Point(100, 100)); continue; }
                            float x = (float)Math.Sqrt(10000 / (fake[i].X * fake[i].Y));
                            real.Add(new System.Drawing.Point() { X = (int)(fake[i].X * x), Y = (int)(fake[i].Y * x) });
                        }

                        mainWorkList.Add(new inboxes()
                        {
                            numberOfUnits = Convert.ToInt32(searcher.result.properties["Занимаемых юнитов (шт)"]),
                            localID = id,
                            globalId = searcher.result.id,
                            locations = new System.Drawing.Point[] { new System.Drawing.Point(0, 0), new System.Drawing.Point(0, 0), new System.Drawing.Point(0, 0), new System.Drawing.Point(0, 0) },
                            scales = real.ToArray()
                        });




                    }
                    catch(Exception ex)
                    {
                        try
                        {
                            tr.Commit();
                        }
                        catch (Exception eee) { }
                    }



                    //////////////////
                   

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
              //      MessageBox.Show("выберите начальное оборудование");
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
                    int id = -1;
                    Transaction tr = null;
                    try
                    {
                        File.Create(searcher.result.name + ".rfa").Close();
                        File.WriteAllBytes(searcher.result.name + ".rfa", searcher.result.bytedFile);
                        Document doc = commandData.Application.ActiveUIDocument.Document;
                        Autodesk.Revit.DB.Family family = null;

                        tr = new Transaction(doc);
                        tr.Start("My Super trans");
                        var search = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).Cast<FamilySymbol>().ToList();
                        bool good = false;
                        foreach (FamilySymbol symbq in search)
                            if (symbq.Family.Name == searcher.result.name)
                            {
                                if (!symbq.IsActive)
                                    symbq.Activate();
                                var buffInstance = doc.Create.NewFamilyInstance(new XYZ(0, 0, 0), symbq, structuralType: Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                                buffInstance.Id.ToString();
                                id = Convert.ToInt32(buffInstance.Id.ToString());
                                good = true;
                                break;

                            }
                        if (!good)
                        {
                            doc.LoadFamily(searcher.result.name + ".rfa", out family);

                            var fsym = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).Cast<FamilySymbol>().ToList();

                            String name = family.Name;
                            foreach (FamilySymbol symbq in fsym)
                            {
                                if (symbq.Family.Name == name)
                                {
                                    if (!symbq.IsActive)
                                        symbq.Activate();
                                    var buffInstance = doc.Create.NewFamilyInstance(new XYZ(0, 0, 0), symbq, structuralType: Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                                    buffInstance.Id.ToString();
                                    id = Convert.ToInt32(buffInstance.Id.ToString());
                                    break;
                                }
                            }
                        }

                        tr.Commit();



                        mainList.RemoveAll(x => x.id == searcher.result.id);
                        mainList.Add(searcher.result);
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

                        List<System.Drawing.Point> real = new List<System.Drawing.Point>();
                        for (int i = 0; i < 4; i++)
                        {
                            if (fake[i] == null) { real.Add(new System.Drawing.Point(100, 100)); continue; }
                            float x = (float)Math.Sqrt(10000 / (fake[i].X * fake[i].Y));
                            real.Add(new System.Drawing.Point() { X = (int)(fake[i].X * x), Y = (int)(fake[i].Y * x) });
                        }

                        mainWorkList.Add(new free()
                        {
                            localID = id,
                            globalId = searcher.result.id,
                            locations = new System.Drawing.Point[] { new System.Drawing.Point(0, 0), new System.Drawing.Point(0, 0), new System.Drawing.Point(0, 0), new System.Drawing.Point(0, 0) },
                            scales = real.ToArray()
                        });
                    }catch(Exception ex)
                    {
                        try
                        {
                            tr.Commit();
                        }
                        catch(Exception eeee)
                        {

                        }
                    }
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
                button1.Text = "Режим масштабирования\nвыключен";
                button1.BackColor = System.Drawing.Color.Pink;
                BoxController.Mode = modeShkaf.doNothing_NOSCALEMODE;
                StructuralController.Mode = modeStruct.doNothing_NOSCALEMODE;
                PlacementController.Mode = modePlacement.doNothing_NOSCALEMODE;
                ConnectionController.Mode = modeConnection.doNothing_NOSCALEMODE;

                button2.Text = "Режим редактирования кабеля выключен";
                button2.BackColor = System.Drawing.Color.Pink;
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
        static public double distance(System.Drawing.Point a, System.Drawing.Point b)
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
            ConnectionController.draw();
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
    }




    

   




    public class UsedInterface
    {
        public int id; //для сопоставления
        public bool isMama;
        public int fathersLocalID;
    }

}
