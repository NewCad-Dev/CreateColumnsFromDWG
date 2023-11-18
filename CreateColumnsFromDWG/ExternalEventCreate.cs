using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CreateColumnsFromDWG
{
    public class ExternalEventCreate : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            UIDocument uidoc = app.ActiveUIDocument;
            Document doc = uidoc.Document;

            IList<PolyLine> pLines = SelectObject.pLines;
            IList<Arc> pArc = SelectObject.pArc;

            string selectedLayer = CreateColumn_Form.layer;
            string levelName = CreateColumn_Form.level;
            string selectedColumnType = CreateColumn_Form.columns;

            Level colLevel = FindLevelByName(doc, levelName);
            FamilySymbol fs = FindColumnSymbolByName(doc, selectedColumnType);

            CreateColumnsFromPolylines(doc, pLines, selectedLayer, colLevel, fs);
            CreateColumnsFromArcs(doc, pArc, selectedLayer, colLevel, fs);
        }

        public string GetName()
        {
            return "Create Columns";
        }

        private static XYZ MidPoint(double x1, double x2, double y1, double y2, double z1, double z2)
        {
            XYZ midPoint = new XYZ((x1 + x2) / 2, (y1 + y2) / 2, (z1 + z2) / 2);
            return midPoint;
        }

        private Level FindLevelByName(Document doc, string levelName)
        {
            return new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .Cast<Level>()
                .FirstOrDefault(level => level.Name == levelName);
        }

        private FamilySymbol FindColumnSymbolByName(Document doc, string columnName)
        {
            return new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_StructuralColumns)
                .WhereElementIsElementType()
                .Cast<FamilySymbol>()
                .FirstOrDefault(symbol => symbol.Name == columnName);
        }

        private void CreateColumnsFromPolylines(Document doc, IList<PolyLine> pLines, string selectedLayer, Level colLevel, FamilySymbol fs)
        {
            foreach (PolyLine line in pLines)
            {
                GraphicsStyle gStyle = doc.GetElement(line.GraphicsStyleId) as GraphicsStyle;
                string layer = gStyle.GraphicsStyleCategory.Name;

                if (layer == selectedLayer)
                {
                    Outline pOutLine = line.GetOutline();
                    XYZ firstP = pOutLine.MaximumPoint;
                    XYZ secondP = pOutLine.MinimumPoint;
                    XYZ lineMid = MidPoint(firstP.X, secondP.X, firstP.Y, secondP.Y, firstP.Z, secondP.Z);

                    using (Transaction tr = new Transaction(doc, "Create Columns"))
                    {
                        tr.Start();
                        try
                        {
                            if (!fs.IsActive)
                                fs.Activate();

                            doc.Create.NewFamilyInstance(lineMid, fs, colLevel, StructuralType.Column);
                        }
                        catch (Exception ex)
                        {
                            TaskDialog.Show(ex.Message, ex.ToString());
                        }
                        tr.Commit();
                    }
                }
            }
        }

        private void CreateColumnsFromArcs(Document doc, IList<Arc> pArc, string selectedLayer, Level colLevel, FamilySymbol fs)
        {
            foreach (Arc arc in pArc)
            {
                GraphicsStyle gStyle = doc.GetElement(arc.GraphicsStyleId) as GraphicsStyle;
                string layer = gStyle.GraphicsStyleCategory.Name;

                if (layer == selectedLayer)
                {
                    using (Transaction tr = new Transaction(doc, "Create Columns"))
                    {
                        tr.Start();
                        try
                        {
                            if (!fs.IsActive)
                                fs.Activate();

                            doc.Create.NewFamilyInstance(arc.Center, fs, colLevel, StructuralType.Column);
                        }
                        catch (Exception ex)
                        {
                            TaskDialog.Show(ex.Message, ex.ToString());
                        }
                        tr.Commit();
                    }
                }
            }
        }
    }
}
