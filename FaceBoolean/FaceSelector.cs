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
	public static class FaceSelector
	{
		public static PlanarFace GetPlanarFace(UIApplication uiApp)
		{
			while (true)
			{
				Reference r = uiApp.ActiveUIDocument.Selection.PickObject(ObjectType.Face);
				if (r != null)
				{
					Element elem = uiApp.ActiveUIDocument.Document.GetElement(r.ElementId);

					if (null != elem)
					{
						if (elem.GetGeometryObjectFromReference(r) is PlanarFace plFace)
						{
							return plFace;
						}
					}
				}
			}
		}
	}
}
