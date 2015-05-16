using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.WSAPI.Lib.CommandResults;

namespace Distributr.WSAPI.Controllers
{
    public class GUIDController : BaseController
    {
        //
        // GET: /GUID/

        public JsonResult GetGuid()
        {
            string guidString = Guid.NewGuid().ToString();
            guidString=guidString.Substring(0, guidString.Length-4);

            ResponseBasic response = new ResponseBasic()
            {
                Result = guidString,
                ErrorInfo="Success"
            };
            return Json(response, JsonRequestBehavior.AllowGet);
        }

    }
}
