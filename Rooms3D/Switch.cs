using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using CustomCSG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rooms3D
{
    [Transaction(TransactionMode.Manual)]
    public class Switch : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            Transaction ts = new Transaction(doc, "3D Spaces");
            ts.Start();

            BuiltInCategory bic = BuiltInCategory.OST_Signage;

            IList<Element> listElem = new FilteredElementCollector(doc).OfCategory(bic).WhereElementIsNotElementType().ToElements();

            bool clearModel = false;
            List<ElementId> ToDel = new List<ElementId>();
            foreach (Element  oldShape in listElem)
            {
                string comment = oldShape.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).AsString();
                if (comment != null && comment.Contains(" "))
                {
                    if (!clearModel)
                    {
                        clearModel = true;
                    }

                    ToDel.Add(oldShape.Id);
                }
            }
            doc.Delete(ToDel);

            if(!clearModel)
            {
                List<Element> roomsAndSpaces = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_MEPSpaces).ToList();
                roomsAndSpaces.AddRange(new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rooms));

                foreach (Element elem in roomsAndSpaces)
                {
                    try
                    {
                        Solid solid = (Solid)elem.get_Geometry(new Options()).ElementAt(0);

                        DirectShape ds1 = solid.CreateDirectShape(doc, BuiltInCategory.OST_Parking);
                        ds1.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).Set(elem.Name);
                    }
                    catch { }
                }
            }

            ts.Commit();

            return Result.Succeeded;
        }
    }
}
