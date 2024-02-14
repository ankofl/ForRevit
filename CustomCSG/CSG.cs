using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCSG
{
	public static class CSG
	{
		public static XYZ Project(this XYZ point, XYZ origin, XYZ normal)
		{
			XYZ a = point - origin;

			double dotA = a.DotProduct(normal);

			a -= normal * dotA;

			XYZ proj = a + origin;
			return proj;
		}
		public static List<PlanarFace> ExecuteBooleanOperation(this PlanarFace faceOne, PlanarFace faceTwo, BooleanOperationsType booleanType)
		{
			List<PlanarFace> faceArray = new List<PlanarFace>();

			if (faceOne.OnPlane(faceTwo.FaceNormal, faceTwo.Origin))
			{
				Solid solidOne = Extrude(faceOne, 0.01);
				Solid solidTwo = Extrude(faceTwo, 0.01);

				Solid solidInter = BooleanOperationsUtils.ExecuteBooleanOperation(solidOne, solidTwo, booleanType);

				foreach (Solid solidSplitted in SolidUtils.SplitVolumes(solidInter))
				{
					foreach (Face face in solidSplitted.Faces)
					{
						if (face is PlanarFace pFace)
						{
							if (faceOne.OnPlane(pFace.FaceNormal, pFace.Origin))
							{
								faceArray.Add(pFace);
							}
						}
					}
				}
			}
			
			return faceArray;
		}

		public static bool OnPlane(this PlanarFace pFace, XYZ normal, XYZ origin)
		{
			if (pFace.FaceNormal.IsAlmostEqualTo(normal))
			{
				XYZ projected = pFace.Origin.Project(origin, normal);

				double dist = Math.Abs(pFace.Origin.DistanceTo(projected));
				if (dist < 0.001)
				{
					return true;
				}
			}
			return false;
		}

		public static Solid Extrude(this PlanarFace face, double dist)
		{
			List<CurveLoop> loops = CurveLoopBuilder.GetCurveLoops(face.EdgeLoops);

			return GeometryCreationUtilities.CreateExtrusionGeometry(loops, face.FaceNormal.Negate(), dist);
		}

		public static DirectShape CreateDirectShape(this Solid solid, Document doc, BuiltInCategory category)
		{
			DirectShape ds = DirectShape.CreateElement(doc, new ElementId(category));

			ds.AppendShape(new List<GeometryObject> { solid });

			return ds;
		}
	}
}
