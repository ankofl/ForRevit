using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCSG
{
    [Transaction(TransactionMode.Manual)]
    public class CommandCaller : IExternalCommand
    {
        public static CommandSwitcher myEvent;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
			Result result;
			try
            {
				myEvent = new CommandSwitcher();

				myEvent.Start(CommandType.Execute);

                result = Result.Succeeded;
			}
            catch(Exception e)
            {
                TaskDialog.Show("CommandCaller exeption", e.Message);

                result = Result.Failed;
            }            

            return result;
        }
    }
}