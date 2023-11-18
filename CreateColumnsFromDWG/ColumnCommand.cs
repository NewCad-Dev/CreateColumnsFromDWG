using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CreateColumnsFromDWG
{
    [Transaction(TransactionMode.Manual)]
    public class ColumnCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;

            ExternalEventCreate handler = new ExternalEventCreate();
            ExternalEvent exEvent = ExternalEvent.Create(handler);

            CreateColumn_Form createColumn_Form = new CreateColumn_Form(doc, exEvent, handler);

            createColumn_Form.Show();

            return Result.Succeeded;
        }
    }
}
