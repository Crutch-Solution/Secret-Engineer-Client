using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.ApplicationServices;
using System.Reflection;

namespace SCS_Module
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Class1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Box_creator a = new Box_creator();
            a.ShowDialog();
            //TaskDialog.Show("hui", "her");
            //RibbonPanel p = fff(application);
            //PushButton but = p.AddItem(new PushButtonData("hui", "name", Assembly.GetExecutingAssembly().Location, "hui.comm")) as PushButton;
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
