using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

namespace CreateColumnsFromDWG
{
    public class SelectObject
    {
        private IList<ImportInstance> dwgForLayer;

        public static IList<PolyLine> pLines = new List<PolyLine>();
        public static IList<Arc> pArc = new List<Arc>();

        public IList<string> IsSelectDWG(Document doc)
        {
            IList<string> result = new List<string>();

            IList<ImportInstance> dwg = new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .OfType<ImportInstance>()
                .ToList();

            dwgForLayer = dwg;

            foreach (ImportInstance imp in dwg)
            {
                result.Add(imp.Category.Name);
            }

            return result;
        }

        public IList<string> IsSelectLevel(Document doc)
        {
            IList<string> result = new List<string>();

            IList<Level> levels = new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .Cast<Level>()
                .ToList();

            foreach (Level level in levels)
            {
                result.Add(level.Name);
            }

            return result;
        }

        public IEnumerable<string> IsSelectLayer(Document doc)
        {
            IList<string> result = new List<string>();

            if (dwgForLayer.Count > 0)
            {
                foreach (ImportInstance imp in dwgForLayer)
                {
                    if (imp.Category.Name == CreateColumn_Form.dwg)
                    {
                        pLines.Clear();
                        pArc.Clear();

                        GeometryElement geoElem = imp.get_Geometry(new Options());

                        foreach (GeometryObject geoObj in geoElem)
                        {
                            if (geoObj is GeometryInstance)
                            {
                                GeometryInstance geoInst = (GeometryInstance)geoObj;
                                GeometryElement geoElement = geoInst.GetInstanceGeometry();

                                if (geoElement != null)
                                {
                                    foreach (GeometryObject obj in geoElement)
                                    {
                                        GraphicsStyle gStyle = doc.GetElement(obj.GraphicsStyleId) as GraphicsStyle;
                                        string layer = gStyle.GraphicsStyleCategory.Name;

                                        if (obj is PolyLine)
                                        {
                                            result.Add(layer);
                                            pLines.Add(obj as PolyLine);
                                        }

                                        if (obj is Arc)
                                        {
                                            result.Add(layer);
                                            pArc.Add(obj as Arc);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            IEnumerable<string> res = result.Where(s => s.Contains("C_"));

            return res;
        }

        public IList<string> IsSelectColumn(Document doc)
        {
            IList<string> result = new List<string>();

            IList<Element> columns = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_StructuralColumns)
                .WhereElementIsElementType()
                .ToElements();

            foreach (Element element in columns)
            {
                result.Add(element.Name);
            }

            return result;
        }
    }
}
