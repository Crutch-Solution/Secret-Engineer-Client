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
    class RevitProvider
    {

        public static int copy(Equipment target)
        {
            int id = -1;
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
                tr.Commit();
            }
            catch (Exception ex)
            {
                id = -1;
                try
                {
                    tr.Commit();
                }
                catch (Exception eeee)
                {
                }
            }
            return id;

        }
        public static void synchronizer()
        {
            return;
            string a = "";
            Transaction tr = null;
            try
            {
                Document doc = commandData.Application.ActiveUIDocument.Document;
                tr = new Transaction(doc);
                tr.Start("My Super trans");
                for (int i = 0; i < Schemes_Editor.mainWorkList.Count; i++)
                {


                    Element el = doc.GetElement(new ElementId(Schemes_Editor.mainWorkList[i].localID));
                    if (el == null)
                    {
                        foreach (var j in Schemes_Editor.mainWorkList)
                            if (j is boxes)
                            {
                                ((boxes)j).equipInside.RemoveAll(x => x.localID == Schemes_Editor.mainWorkList[i].localID);
                            }
                        Schemes_Editor.wires.RemoveAll(x => (x.firstEquip != null && x.firstEquip.localID == Schemes_Editor.mainWorkList[i].localID)
                        || (x.secondEquip != null && x.secondEquip.localID == Schemes_Editor.mainWorkList[i].localID));

                        a += $"Было уничтожено оборудование {Schemes_Editor.mainList.Find(x => x.id == Schemes_Editor.mainWorkList[i].globalId).name}\n";
                        Schemes_Editor.mainWorkList.RemoveAt(i);
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
        }
        public static ExternalCommandData commandData;
        static int shit = 0;
        public static int createInstance(byte[] bytes, string familyname)
        {
            shit++;
            return shit;
            Transaction tr = null;
            int id = -1;
            try
            {
                File.Create(familyname + ".rfa").Close();
                File.WriteAllBytes(familyname + ".rfa", bytes);
                Document doc = commandData.Application.ActiveUIDocument.Document;
                Autodesk.Revit.DB.Family family = null;

                tr = new Transaction(doc);
                tr.Start("My Super trans");
                //попытка найти существующее семейство
                var search = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).Cast<FamilySymbol>().ToList();
                bool good = false;
                foreach (FamilySymbol symbq in search)
                    if (symbq.Family.Name == familyname)
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
                    doc.LoadFamily(familyname + ".rfa", out family);

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
                id = -1;
                try
                {
                    tr.Commit();
                }
                catch (Exception eeee)
                {
                }
            }
            return id;
        }
    }
}
