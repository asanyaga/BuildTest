using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.DataImporter.Lib.Utils;
using Distributr.WPF.Lib.Services.Service.Sync;
using Distributr.WPF.Lib.Services.Service.Utility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using StructureMap;

namespace Distributr.DataImporter.Lib.ViewModel
{
    public class CommandsUploadActivityViewModel : MasterdataImportViewModelBase
    {
       public CommandsUploadActivityViewModel()
        {
            ExportActivityMessage = "";
          UploadAllCommand= new RelayCommand(Upload);
            
        }

       public RelayCommand UploadAllCommand { get; set; }

        internal void ReceiveMessage(string msg)
        {
            ExportActivityMessage += "\n" + msg;
            FileUtility.LogCommandActivity(msg);
        }
        void Upload()
        {
           base.BeginCommandsUpload();
        }

        protected async override void Load(System.Windows.Controls.Page page)
        {
            base.BeginCommandsUpload(); 
        }
    }
}
