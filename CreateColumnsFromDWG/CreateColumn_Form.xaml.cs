using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Linq;
using System.Windows;
using ComboBoxNow = System.Windows.Controls.ComboBox;

namespace CreateColumnsFromDWG
{
    public partial class CreateColumn_Form : Window
    {
        Document Doc;
        SelectObject select = new SelectObject();

        private ExternalEvent _event;
        private ExternalEventCreate _eventCreate;

        public static string dwg;
        public static string level;
        public static string layer;
        public static string columns;

        public CreateColumn_Form(Document doc, ExternalEvent exEvent, ExternalEventCreate exCreate)
        {
            InitializeComponent();
            Doc = doc;
            _event = exEvent;
            _eventCreate = exCreate;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _event.Dispose();
            _event = null;
            _eventCreate = null;
        }
        private void Start_Loaded(object sender, RoutedEventArgs e)
        {
            combo_DWG.ItemsSource = select.IsSelectDWG(Doc).Distinct().ToList();
            combo_DWG.SelectedIndex = 0;

            combo_Level.ItemsSource = select.IsSelectLevel(Doc).Distinct().ToList();
            combo_Level.SelectedIndex = 0;
        }

        private void cb_SelectDWG(object sender, EventArgs e)
        {
            ComboBoxNow combo = sender as ComboBoxNow;

            dwg = combo.SelectedItem as string;
            if (dwg != null)
            {
                combo_Layer.ItemsSource = select.IsSelectLayer(Doc).Distinct().ToList();
                combo_Layer.SelectedIndex = 0;

                combo_Column.ItemsSource = select.IsSelectColumn(Doc).Distinct().ToList();
                combo_Column.SelectedIndex = 0;
            }
        }

        private void cb_SelectLevel(object sender, EventArgs e)
        {
            ComboBoxNow combo = sender as ComboBoxNow;
            level = combo.SelectedItem as string;
        }

        private void cb_SelectLayer(object sender, EventArgs e)
        {
            ComboBoxNow combo = sender as ComboBoxNow;
            layer = combo.SelectedItem as string;
        }

        private void cb_SelectColumn(object sender, EventArgs e)
        {
            ComboBoxNow combo = sender as ComboBoxNow;

            columns = combo.SelectedItem as string;
        }

        private void btn_Create_Click(object sender, RoutedEventArgs e)
        {
            _event.Raise();
        }
    }
}
