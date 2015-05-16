using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using GalaSoft.MvvmLight;
using Distributr.SL.Lib.Service.WSProxies;

namespace Distributr.SL.Lib.ViewModels.MasterData
{
    //public class SyncMasterDataViewModel : ViewModelBase
    //{
    //    IPullMasterDataProxy _pullMasterDataProxy;
    //    public SyncMasterDataViewModel(IPullMasterDataProxy pullMasterDataProxy )
    //    {
    //        _pullMasterDataProxy = pullMasterDataProxy;
    //    }

    //    public const string TestNamePropertyName = "TestName";
    //    private string _testName = "";
    //    public string TestName
    //    {
    //        get
    //        {
    //            return _testName;
    //        }

    //        set
    //        {
    //            if (_testName == value)
    //            {
    //                return;
    //            }

    //            var oldValue = _testName;
    //            _testName = value;

    //            // Update bindings, no broadcast
    //            RaisePropertyChanged(TestNamePropertyName);

    //        }
    //    }



    //}
}
