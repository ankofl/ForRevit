
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCSG
{
    public enum CommandType { None, Execute }
    public class CommandSwitcher : IExternalEventHandler
    {
		public CommandSwitcher()
		{
			externalEvent = ExternalEvent.Create(this);
		}

		private ExternalEvent externalEvent = null;

		public void Start(CommandType command)
		{
			commandType = command;

			externalEvent.Raise();
		}

        private CommandType commandType = CommandType.None;
        public void Execute(UIApplication uiApp)
        {
			try
			{
				Document doc = uiApp.ActiveUIDocument.Document;

				Transaction ts = new Transaction(doc, updaterName);
				ts.Start();

				switch (commandType)
				{
					case CommandType.Execute:
					{
						BuiltInCategory shapeCategory = BuiltInCategory.OST_Parking;

						doc.Delete(new FilteredElementCollector(doc).OfCategoryId(new ElementId(shapeCategory)).ToElementIds());

						List<Element> selectedPlanarFaces = new List<Element>();
						
						PlanarFace planarOne = FaceSelector.GetPlanarFace(uiApp);
						PlanarFace planarTwo = FaceSelector.GetPlanarFace(uiApp);

						if(planarOne.OnPlane(planarTwo.FaceNormal, planarTwo.Origin))
						{
							foreach (BooleanOperationsType booleanType in Enum.GetValues(typeof(BooleanOperationsType)))
							{
								List<PlanarFace> booleanPlanarFaces = planarOne.ExecuteBooleanOperation(planarTwo, booleanType);

								foreach (PlanarFace planarFace in booleanPlanarFaces)
								{
									Solid interSolid = CSG.Extrude(planarFace, 0.001);

									DirectShape ds = interSolid.CreateDirectShape(doc, shapeCategory);

									string name = Enum.GetName(typeof(BooleanOperationsType), booleanType);
									ds.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).Set(name);
								}
							}
						}

						break;
					}
					case CommandType.None:
					{
						break;
					}
					default:
					{
						throw new Exception($"Необработанный {nameof(CommandType)}");
					}
				}

				ts.Commit();
			}
			catch(Exception e)
			{
				TaskDialog.Show("Exeption in Switch CommandType", e.Message);
			}
			finally
			{
				commandType = CommandType.None;
			}
        }

        private readonly string updaterName = "updaterName";

        public string GetName()
        {
            return updaterName;
        }
    }
}
