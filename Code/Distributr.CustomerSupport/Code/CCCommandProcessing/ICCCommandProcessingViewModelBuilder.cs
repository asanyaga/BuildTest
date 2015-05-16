using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Distributr.CustomerSupport.Code.CCCommandProcessing
{
    public interface ICCCommandProcessingViewModelBuilder
    {
        CCCommandProcessingSummaryViewModel GetCommandProcessingSummary();
        CCCommandProcessingDetailViewModel GetCommandDetail(int dayofYear, int year, Guid costCentreId);
        decimal GetUnQueuedCommands();
        void QueueCommands();
        void Test();
    }
}