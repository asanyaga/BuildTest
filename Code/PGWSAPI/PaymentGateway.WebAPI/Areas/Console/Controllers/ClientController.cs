using System.Web.Mvc;
using PaymentGateway.WSApi.Lib.Domain.MasterData;
using PaymentGateway.WSApi.Lib.Paging;
using PaymentGateway.WSApi.Lib.Repository.MasterData.Clients;
using PaymentGateway.WSApi.Lib.Util;
using System.Linq;

namespace PaymentGateway.WebAPI.Controllers
{
    public class ClientController : Controller
    {
        private IClientRepository _clientRepository;

        public ClientController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public ActionResult Index(int? page)
        {
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            int take = 20;
            int skip = currentPageIndex * take;
            var query = new PGQueryMasterData() { Skip = skip, Take = take };
            var result = _clientRepository.Query(query);
            var item = result.Result.OfType<Client>();


            int? total = result.Count < take ? take : result.Count;
            var data = item.ToPagedList(currentPageIndex, take, total);


            return View(data);
        }

    }
}
