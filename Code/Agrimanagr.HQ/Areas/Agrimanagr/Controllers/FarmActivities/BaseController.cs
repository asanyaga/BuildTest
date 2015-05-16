using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Data.EF;
using Distributr.Core.Utility.Mapping;
using log4net;

namespace Agrimanagr.HQ.Areas.Agrimanagr.Controllers.FarmActivities
{
    public abstract class BaseController : Controller
    {
        protected IDTOToEntityMapping _dtoToEntityMapping;
        protected IMasterDataToDTOMapping _masterDataToDtoMapping;
        protected CokeDataContext _dataContext;
        protected static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        protected int DefaultPageCount = 2;
      
        protected BaseController(IDTOToEntityMapping dtoToEntityMapping, IMasterDataToDTOMapping masterDataToDtoMapping, CokeDataContext context)
        {
            _dtoToEntityMapping = dtoToEntityMapping;
            _masterDataToDtoMapping= masterDataToDtoMapping;
            _dataContext = context;
        }

       
        
    }

    
}
