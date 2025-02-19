﻿using System;
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
            RevitProvider.commandData = commandData;
            (new Schemes_Editor()).ShowDialog();

            return Result.Succeeded;
        }
        public Result OnStartup(UIControlledApplication application)
        {
          
            RibbonPanel p = fff(application);
            PushButton but = p.AddItem(new PushButtonData("XXX", "name", Assembly.GetExecutingAssembly().Location, "X")) as PushButton;
            return Result.Succeeded;
        }

        public RibbonPanel fff(UIControlledApplication a)
        {
            a.CreateRibbonPanel("XXXX");
            foreach (var i in a.GetRibbonPanels())
                if (i.Name == "XXXXXXXXXXт")
                    return i;
            return null;
        }
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}
