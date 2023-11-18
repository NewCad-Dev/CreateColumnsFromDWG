using Autodesk.Revit.UI;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace CreateColumnsFromDWG
{
    public class Application : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            string assemblyLocation = Assembly.GetExecutingAssembly().Location,
                iconsDirectoryPath = Path.GetDirectoryName(assemblyLocation) + @"\Resources\",
                panelName = "NewCad";

            RibbonPanel panel = application.CreateRibbonPanel(panelName);

            PushButtonData buttonData = new PushButtonData(nameof(ColumnCommand), "Create Columns", assemblyLocation, typeof(ColumnCommand).FullName)
            {
                LargeImage = new BitmapImage(new Uri(iconsDirectoryPath + "add_columns.png"))
            };

            panel.AddItem(buttonData);

            return Result.Succeeded;
        }
    }
}
