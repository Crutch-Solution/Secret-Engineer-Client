using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.ApplicationServices;
using System.Reflection;
using System.Net.Sockets;
using System.Windows.Forms;
using Autodesk.Revit.UI.Selection;

namespace SCS_Module
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Class1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Internet.Establish();
            (new Schemes_Editor(commandData)).ShowDialog();
        /*    Document doc = commandData.Application.ActiveUIDocument.Document;
            OpenFileDialog file = new OpenFileDialog();
            Autodesk.Revit.DB.Family family = null;

            if (file.ShowDialog() == DialogResult.OK)
            {
                Transaction tr = new Transaction(doc);
                tr.Start("My Super trans");

                if (doc.LoadFamily(file.FileName, out family))
                {
                    String name = family.Name;
                    TaskDialog.Show("Revit", "Family file has been loaded. Its name is " + name);

                    FilteredElementCollector collector1 = new FilteredElementCollector(doc);


                    var fsym = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).Cast<FamilySymbol>().ToList();

                    foreach (FamilySymbol symbq in fsym)
                    {
                        if (symbq.Family.Name == name)
                        {
                            //// Get the handle of current document.
                            //UIDocument uidoc = commandData.Application.ActiveUIDocument;

                            //// Get the element selection of current document.
                            //Selection selection = uidoc.Selection;
                            //var selectedIds = uidoc.Selection.GetElementIds().ToList();


                            //IList<Reference> sideFaces = HostObjectUtils.GetSideFaces(doc.GetElement(selectedIds[0]), ShellLayerType.Interior);
                            //Reference interiorFaceRef = sideFaces[0];

                            //XYZ location = new XYZ(4, 2, 8);
                            //XYZ refDir = new XYZ(0, 0, 1);

                            //FamilyInstance instance = document.Create.NewFamilyInstance(interiorFaceRef, location, refDir, symbol);



                            if (!symbq.IsActive)
                                symbq.Activate();
                            var hui =   doc.Create.NewFamilyInstance(new XYZ(0, 0, 0), symbq, structuralType: Autodesk.Revit.DB.Structure.StructuralType.NonStructural );
                            TaskDialog.Show("gd", hui.Id.ToString());
                        }
                    }
                    tr.Commit();
                }
            }
            //TaskDialog.Show("hui", "her");
            //RibbonPanel p = fff(application);
            //PushButton but = p.AddItem(new PushButtonData("hui", "name", Assembly.GetExecutingAssembly().Location, "hui.comm")) as PushButton;
    */      
    return Result.Succeeded;
        }
        public Result OnStartup(UIControlledApplication application)
        {
          
            RibbonPanel p = fff(application);
            PushButton but = p.AddItem(new PushButtonData("hui", "name", Assembly.GetExecutingAssembly().Location, "hui.comm")) as PushButton;
            return Result.Succeeded;
        }

        public RibbonPanel fff(UIControlledApplication a)
        {
            a.CreateRibbonPanel("Пезда тоби ревит");
            foreach (var i in a.GetRibbonPanels())
                if (i.Name == "Пезда тоби ревит")
                    return i;
            return null;
        }
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}
