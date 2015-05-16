using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.AssetEntities;
using Distributr.Core.Domain.Master.CoolerEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.MasterDataDTO;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Assets;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.User;
using Distributr.Core.Repository.Master.AssetRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Mapping;
using Distributr.Core.Utility.Security;
using Distributr.WSAPI.Lib.Services.Bus;
using Distributr.WSAPI.Lib.Services.WebService.CommandDeserialization;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Distributr.WSAPI.Controllers
{
    [Obsolete("No longer used?")]
    public class CnUMasterDataController : Controller
    {
        private IMasterDataDTODeserialize _masterDataDTODeserialize;
        private IPublishMasterData _publishMasterData;
        private ICostCentreRepository _costCenterRepository;
        private IMasterDataToDTOMapping _mapping;
        private IContactRepository _contactRepository;
        private IAssetRepository _assetRepository;
        private IDistributrFileRepository _imageRepository;
        private IUserRepository _userRepository;
        ILog _log = LogManager.GetLogger("CnUMasterDataController");

        public CnUMasterDataController(IMasterDataDTODeserialize masterDataDtoDeserialize, IPublishMasterData publishMasterData, ICostCentreRepository costCenterRepository, IMasterDataToDTOMapping mapping, IContactRepository contactRepository, IAssetRepository assetRepository, IDistributrFileRepository imageRepository, IUserRepository userRepository)
        {
            _masterDataDTODeserialize = masterDataDtoDeserialize;
            _publishMasterData = publishMasterData;
            _costCenterRepository = costCenterRepository;
            _mapping = mapping;
            _contactRepository = contactRepository;
            _assetRepository = assetRepository;
            _imageRepository = imageRepository;
            _userRepository = userRepository;
        }

        [HttpPost]
        public JsonResult Procces(string masterDataCollective, string jsonDTO)
        {
            MasterBaseDTO data = _masterDataDTODeserialize.DeserializeMasterDataDTO(masterDataCollective, jsonDTO);
            ResponseBasic response = null;
            if (data == null)
            {
                response = new ResponseBasic { Result = "Invalid", ErrorInfo = "Failed" };
            }
            else
            {
                try
                {
                    MasterDataDTOSaveCollective ct = GetMasterDataCollective(masterDataCollective);
                    _publishMasterData.Publish(data, ct);
                    response = new ResponseBasic { Result = "OK", ErrorInfo = "Success" };
                }
                catch (Exception ex)
                {
                    _log.InfoFormat("ERROR Processing MasterData : {0} with {1} Exception {2} ", masterDataCollective, jsonDTO,ex.Message);
                }
            }
            return Json(response);
           
        }

        [HttpPost]
        [JsonFilter]
        public JsonResult SLProcess(string masterDataCollective, string jsonDTO)
        {
            MasterBaseDTO data = _masterDataDTODeserialize.DeserializeMasterDataDTO(masterDataCollective, jsonDTO);
            ResponseBasic response = new ResponseBasic { Result = "Invalid", ErrorInfo = "Failed" };
            if (data == null)
            {
                response = new ResponseBasic { Result = "Invalid", ErrorInfo = "Failed" };
            }
            else
            {
                try
                {
                    MasterDataDTOSaveCollective ct = GetMasterDataCollective(masterDataCollective);
                    _publishMasterData.Publish(data, ct);
                    response = new ResponseBasic { Result = "OK", ErrorInfo = "Success" };
                }
                catch (Exception ex)
                {
                    _log.InfoFormat("ERROR Processing MasterData : {0} with {1} Exception {2} ", masterDataCollective, jsonDTO, ex.Message);
                }
            }
            return Json(response);

        }

        private MasterDataDTOSaveCollective GetMasterDataCollective(string masterDataCollective)
        {
            MasterDataDTOSaveCollective _masterDataCollective;
            Enum.TryParse(masterDataCollective, out _masterDataCollective);
            return _masterDataCollective;
        }

        public ActionResult TestShow()
        {
            DistributrFile imagex = _imageRepository.GetAll().OrderByDescending(p=>p._DateCreated).FirstOrDefault();

            byte[] image=Convert.FromBase64String(imagex.FileData);
            return File(image, "image/"+imagex.FileExtension);

        }

        [HttpGet]
         public ActionResult TestOutlet()
         {
             Outlet o = _costCenterRepository.GetAll().OfType<Outlet>().FirstOrDefault();
             OutletDTO dto = _mapping.Map(o);
             string mssg = "";
             string type = "Outlet";
            mssg = JsonConvert.SerializeObject(dto, new IsoDateTimeConverter());
            Send(mssg, type);
            return Json(dto, JsonRequestBehavior.AllowGet);
         }

        [HttpGet]
        public ActionResult TestContact()
        {
            Contact o = _contactRepository.GetAll().First() as Contact;
            ContactDTO dto = _mapping.Map(o);
            string mssg = "";
            string type = "Contact";
            //MasterDataCollective.Contact
            mssg = JsonConvert.SerializeObject(dto, new IsoDateTimeConverter());
            Send(mssg, type);
            return Json(dto, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TestAsset()
        {
           
           

            Asset o = _assetRepository.GetAll().First() ;
            AssetDTO dto = _mapping.Map(o);
            string mssg = "";
            string type = "Asset";
            //MasterDataCollective.Contact
            mssg = JsonConvert.SerializeObject(dto, new IsoDateTimeConverter());
            Send(mssg, type);
            return Json(dto, JsonRequestBehavior.AllowGet);
        }

         public ActionResult TestUserChangePwd()
         {
             User u = _userRepository.GetAll().First();
             ChangePasswordDTO dto = new ChangePasswordDTO
                                         {
                                             NewPassword = EncryptorMD5.GetMd5Hash("123456"),
                                             OldPassword = u.Password,
                                             MasterId = u.Id
                                         };

             string mssg = "";
             string type = "PasswordChange";
             mssg = JsonConvert.SerializeObject(dto, new IsoDateTimeConverter());

             Send(mssg, type);

             return Json(dto, JsonRequestBehavior.AllowGet);
         }

        public ActionResult TestFiles()
        {//D:\Project\distributr\Distributr.WSAPI\Content\themes\base\images\Penguins.jpg
           string imageFile = Path.Combine(Server.MapPath("~/Content/themes/base/images"), "Penguins.jpg");
            //byte[] buffer =System.IO.File.ReadAllBytes("foo.png");

           byte[] buffer = System.IO.File.ReadAllBytes(imageFile);
           string returnValue = System.Convert.ToBase64String(buffer);

            //byte[] encodedDataAsBytes = System.Convert.FromBase64String(returnValue);
            //string returnValue1 =System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);

          
          
            //DistributrFile o = _assetRepository.GetAll().First();
            DistributrFileDTO dto = new DistributrFileDTO
                                       {
                                           Description = "test",
                                           FileData = returnValue,
                                           FileExtension = "jpg",
                                           FileTypeMasterId = 1,
                                           MasterId=Guid.NewGuid()
                                       };//_mapping.Map(o);
            string mssg = "";
            string type = "DistributrFile";
            //MasterDataCollective.Contact
            mssg = JsonConvert.SerializeObject(dto, new IsoDateTimeConverter());
            Send(mssg, type);
            return Json(dto, JsonRequestBehavior.AllowGet);
        }

        private void Send(string mssg, string type)
        {
           
            NameValueCollection param = new NameValueCollection();
            param.Add("masterDataCollective", type);
            param.Add("jsonDTO", mssg);
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            string hsenidUrl = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"));
            Uri uri = new Uri(hsenidUrl + "CnUMasterData/Procces", UriKind.Absolute);
            wc.UploadValuesAsync(uri, "POST", param);//.UploadStringAsync(uri, "POST", param.ToString());
            wc.UploadStringCompleted += (sender, e) =>
                                            {
                                                try
                                                {
                                                    if (e.Error != null)
                                                    {
                                                        string error = e.Error.Message;
                                                        return;
                                                    }
                                                    string jsonResult = e.Result;

                                                }
                                                catch (Exception ex)
                                                {

                                                }
                                            };
          
        }

        public class JsonFilter : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                var incomingData = new StreamReader(filterContext.HttpContext.Request.InputStream).ReadToEnd();
                filterContext.ActionParameters["jsonDTO"] = incomingData.Replace("jsonDTO=", "");
                base.OnActionExecuting(filterContext);
            }
        }
    }
}
