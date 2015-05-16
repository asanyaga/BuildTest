using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PaymentGateway.WSApi.Lib.Domain.MasterData;
using PaymentGateway.WSApi.Lib.Paging;
using PaymentGateway.WSApi.Lib.Repository.MasterData.Clients;
using PaymentGateway.WSApi.Lib.Services.DistributrWSProxy;
using PaymentGateway.WSApi.Lib.Util;
using PaymentGateway.WSApi.Lib.Validation;

namespace PaymentGateway.WebAPI.Areas.Console.Controllers
{
    public class SmsClientController : Controller
    {
        //
        // GET: /Console/SmsClient/
         private IClientRepository _clientRepository;
         private IClientMemberRepository _clientMemberRepository;
         private IDistributorWebApiProxy _distributorWebApiProxy;

        public SmsClientController(IClientRepository clientRepository, IClientMemberRepository clientMemberRepository, IDistributorWebApiProxy distributorWebApiProxy)
        {
            _clientRepository = clientRepository;
            _clientMemberRepository = clientMemberRepository;
            _distributorWebApiProxy = distributorWebApiProxy;
        }

        public ActionResult Index(int? page, bool showInactive = false)
        {

            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            int take = 20;
            int skip = currentPageIndex * take;
            var query = new PGQuery() { Skip = skip, Take = take, ShowInactive = showInactive};
            var result = _clientRepository.Query(query);
            var item = result.Result.OfType<Client>();


            int? total = result.Count < take ? take : result.Count;
            var data = item.ToPagedList(currentPageIndex, take, total);


            return View(data);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Client model)
        {
            try
            {
                model.ExternalId = "";
                _clientRepository.Save(model);

            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DisplayDomainValidationResult(dve, ModelState);
                return View(model);
            }

           
            catch (Exception ex)
            {
                ValidationSummary.DisplayValidationResult(ex.Message, ModelState);
                return View(model);
            }

            return RedirectToAction("Index");
        }
        public ActionResult Edit(int id)
        {
            var client = _clientRepository.GetById(id);
            return View(client);
        }
        [HttpPost]
        public ActionResult Edit(Client model)
        {
            try
            {
                model.ExternalId = "";
                _clientRepository.Save(model);

            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DisplayDomainValidationResult(dve, ModelState);
                return View(model);
            }


            catch (Exception ex)
            {
                ValidationSummary.DisplayValidationResult(ex.Message, ModelState);
                return View(model);
            }

            return RedirectToAction("Index");
        }
        public ActionResult Delete(int id)
        {
            try
            {
                _clientRepository.Delete(id);
            }
            catch (Exception)
            {
                throw;
            }
            return RedirectToAction("Index");
        }
        public ActionResult Members(int clientId,int? page)
        {
            ViewBag.ClientId = clientId;
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            int take = 20;
            int skip = currentPageIndex * take;
            var query = new PGQueryClientMember() { Skip = skip, Take = take, ClientId = clientId };
            var result = _clientMemberRepository.Query(query);
            var item = result.Result.OfType<ClientMember>();


            int? total = result.Count < take ? take : result.Count;
            var data = item.ToPagedList(currentPageIndex, take, total);


            return View(data);
        }
        [HttpPost]
        public async Task<ActionResult> GetMembers(int clientId)
        {
            var client = _clientRepository.GetById(clientId);
            var farmers = await _distributorWebApiProxy.GetClientMember(client.Path);
            _clientMemberRepository.Delete(clientId);
            foreach (var clientMember in farmers)
            {
                if (clientMember.Code == null) clientMember.Code = "_";
                clientMember.Client = client;
                _clientMemberRepository.Save(clientMember);
            }
            return RedirectToAction("Members",new {clientId=2});
        }
    }
}
