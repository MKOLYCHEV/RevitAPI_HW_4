using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPI_HW_4
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class CreationModel : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            List<Level> listLevel = GetLevelList(doc);

            Level level1 = GetLevel(listLevel, 1);

            Level level2 = GetLevel(listLevel, 2);

            double width = 10000;
            double depth = 5000;

            WallsCreation(doc, level1, level2, width, depth);

            return Result.Succeeded;
        }

        public List<Level> GetLevelList(Document doc)
        {
            List<Level> listLevel = new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .OfType<Level>()
                .ToList();

            return listLevel;
        }

        public Level GetLevel(List<Level> levelList, int levelNum)
        {
            Level level = levelList
                .Where(x => x.Name.Equals($"Уровень {levelNum}"))
                .FirstOrDefault();

            return level;
        }

        public void WallsCreation(Document doc, Level level1, Level level2, double width, double depth)
        {
            double Width = UnitUtils.ConvertToInternalUnits(width, UnitTypeId.Millimeters);
            double Depth = UnitUtils.ConvertToInternalUnits(depth, UnitTypeId.Millimeters);

            double dx = Width / 2;
            double dy = Depth / 2;

            List<XYZ> points = new List<XYZ>();
            points.Add(new XYZ(-dx, -dy, 0));
            points.Add(new XYZ(dx, -dy, 0));
            points.Add(new XYZ(dx, dy, 0));
            points.Add(new XYZ(-dx, dy, 0));
            points.Add(new XYZ(-dx, -dy, 0));

            Transaction transaction = new Transaction(doc, "Построение стен");
            transaction.Start();

            for (int i = 0; i < 4; i++)
            {
                Line line = Line.CreateBound(points[i], points[i + 1]);
                Wall wall = Wall.Create(doc, line, level1.Id, false);

                wall.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).Set(level2.Id);
            }

            transaction.Commit();
        }
    }
}
