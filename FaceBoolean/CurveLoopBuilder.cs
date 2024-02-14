using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCSG
{
	public static class CurveLoopBuilder
	{
		public static List<Line> GetLines(EdgeArray edgeArray)
		{
			List<Line> lines = new List<Line>();
			foreach (Edge edge in edgeArray)
			{
				lines.Add((Line)edge.AsCurve());
			}

			return lines;
		}

		public static List<CurveLoop> GetCurveLoops(EdgeArrayArray edgeArrayArray)
		{
			List<CurveLoop> loops = new List<CurveLoop>();

			foreach (EdgeArray edgeArray in edgeArrayArray)
			{
				loops.Add(CounterToLoop(GetLines(edgeArray)));
			}
			return loops;
		}

		public static CurveLoop CounterToLoop(List<Line> edgeArray)
		{
			CurveLoop curveLoop = new CurveLoop();
			foreach (Line line in CreateDirectLines(edgeArray))
			{
				curveLoop.Append(line);
			}
			return curveLoop;
		}

		public static List<Line> CreateDirectLines(List<Line> edgeArray)
		{
			if (edgeArray.Count < 3)
			{
				return edgeArray;
			}

			List<Line> orderedLines = new List<Line>();
			Line currentLine = edgeArray[0];
			orderedLines.Add(currentLine);
			edgeArray.RemoveAt(0);

			int d = 0;
			while (edgeArray.Count > 0 && d < 10001)
			{
				d++;
				bool foundNextLine = false;

				for (int i = 0; i < edgeArray.Count; i++)
				{
					if (currentLine.GetEndPoint(1).IsAlmostEqualTo(edgeArray[i].GetEndPoint(0)))
					{
						orderedLines.Add(edgeArray[i]);
						currentLine = edgeArray[i];
						edgeArray.RemoveAt(i);
						foundNextLine = true;
						break;
					}
					else if (currentLine.GetEndPoint(1).IsAlmostEqualTo(edgeArray[i].GetEndPoint(1)))
					{
						orderedLines.Add(SwapPoints(edgeArray[i]));
						currentLine = edgeArray[i];
						edgeArray.RemoveAt(i);
						foundNextLine = true;
						break;
					}
				}

				if (!foundNextLine)
				{
					// Если не удалось найти следующую линию, меняем местами точки у текущей линии и продолжаем поиск
					currentLine = SwapPoints(currentLine);
				}
			}

			return orderedLines;
		}
		static Line SwapPoints(Line edge)
		{
			return Line.CreateBound(edge.GetEndPoint(1), edge.GetEndPoint(0));
		}
	}
}
