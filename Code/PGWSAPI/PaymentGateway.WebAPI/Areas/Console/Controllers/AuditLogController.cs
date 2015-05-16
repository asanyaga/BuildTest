using System.Linq;
using System.Web.Mvc;
using PaymentGateway.WSApi.Lib.Repository.Payments.Utility;
using PaymentGateway.WSApi.Lib.Paging;

namespace PaymentGateway.WebAPI.Areas.Console.Controllers
{
    [Authorize(Roles = "ROLE_ADMIN")]
    public class AuditLogController : Controller
    {
        private IAuditLogRepository _auditLogRepository;

        public AuditLogController(IAuditLogRepository auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }

        public ActionResult Index(int? page)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            var logs = _auditLogRepository.GetAll().OrderByDescending(n => n.DateCreated);
            return View(logs.ToPagedList(currentPageIndex, PagerSettings.defaultPageSize));
        }

    }
}
