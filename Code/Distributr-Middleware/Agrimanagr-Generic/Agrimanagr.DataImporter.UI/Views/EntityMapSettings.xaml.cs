using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Agrimanagr.DataImporter.Lib.Utils;
using Agrimanagr.DataImporter.Lib.ViewModels;

namespace Agrimanagr.DataImporter.UI.Views
{
    /// <summary>
    /// Interaction logic for EntityMapSettings.xaml
    /// </summary>
    public partial class EntityMapSettings : Window, IMapEntityColumnPosition
    {
        private ImportSettingsViewModel _vm;
        public EntityMapSettings()
        {
            InitializeComponent();
            MouseDown += delegate { DragMove(); };
            _vm = DataContext as ImportSettingsViewModel;
            _vm.SetupCommand.Execute(null);
            _vm.RequestClose += (s, e) => this.Close();
        }


        public Dictionary<int, string> GetEntityMapping(ImportEntity entity)
        {
            _vm.SelectedImportEntity = entity;
            ShowDialog();
            return _vm.GetMappings();
        }

     
    }
}
