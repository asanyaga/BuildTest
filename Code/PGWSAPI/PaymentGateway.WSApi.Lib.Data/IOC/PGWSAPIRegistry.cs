using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PaymentGateway.WSApi.Lib.Data.EF;
using PaymentGateway.WSApi.Lib.Data.Report.Services;
using PaymentGateway.WSApi.Lib.Data.Repository;
using PaymentGateway.WSApi.Lib.Data.Repository.Clients;
using PaymentGateway.WSApi.Lib.Data.Repository.MasterData.ServiceProviders;
using PaymentGateway.WSApi.Lib.Data.Repository.MasterData.Users;
using PaymentGateway.WSApi.Lib.Data.Repository.Payments;
using PaymentGateway.WSApi.Lib.Data.Repository.Payments.Request;
using PaymentGateway.WSApi.Lib.Data.Repository.Payments.Response;
using PaymentGateway.WSApi.Lib.Data.Repository.SMS;
using PaymentGateway.WSApi.Lib.Data.Util.Caching;
using PaymentGateway.WSApi.Lib.Data.Util.Caching.Impl;
using PaymentGateway.WSApi.Lib.Report.Services;
using PaymentGateway.WSApi.Lib.Repository;
using PaymentGateway.WSApi.Lib.Repository.MasterData.Clients;
using PaymentGateway.WSApi.Lib.Repository.MasterData.ServiceProviders;
using PaymentGateway.WSApi.Lib.Repository.MasterData.Users;
using PaymentGateway.WSApi.Lib.Repository.Payments.Request;
using PaymentGateway.WSApi.Lib.Repository.Payments.Response;
using PaymentGateway.WSApi.Lib.Repository.Payments.Utility;
using PaymentGateway.WSApi.Lib.Repository.SMS;
using PaymentGateway.WSApi.Lib.Security;
using PaymentGateway.WSApi.Lib.Security.Impl;
using PaymentGateway.WSApi.Lib.Services.DistributrWSProxy;
using PaymentGateway.WSApi.Lib.Services.Payment;
using PaymentGateway.WSApi.Lib.Services.Payment.Impl;
using PaymentGateway.WSApi.Lib.Services.Webservice;
using PaymentGateway.WSApi.Lib.Services.Webservice.Impl;
using StructureMap.Configuration.DSL;

namespace PaymentGateway.WSApi.Lib.Data.IOC
{
    public class PGWSAPIRegistry : Registry
    {
        public PGWSAPIRegistry()
        {
            For<PGDataContext>()
                .HybridHttpOrThreadLocalScoped()
                .Use<PGDataContext>()
                 .Ctor<string>("connectionString")
                .EqualToAppSetting("PaymentgatewayConnectionstring");
            For<INotificationDeserialize>().Use<NotificationDeserialize>();
            For<INotificationValidation>().Use<NotificationValidation>();
            For<ISmscNotificationRepository>().Use<SmscNotificationRepository>();
            For<IRequestResponseRepository>().Use<RequestResponseRepository>();
            For<IResolveRequestService>().Use<ResolveRequestService>();

            For<IMessageSerializer>().Use<MessageSerializer>();
            For<IMessageValidation>().Use<MessageValidation>();
            For<IResolveMessageService>().Use<ResolveMessageService>();
            For<IPaymentRequestRepository>().Use<PaymentRequestRepository>();
            For<IPaymentResponseRepository>().Use<PaymentResponseRepository>();
            For<IAuditLogRepository>().Use<AuditLogRepository>();
            For<ISmsQueryResolverService>().Use<SmsQueryResolverService>();

            For<IDocSMSRepository>().Use<DocSMSRepository>();

            //cache provider
            For<ICacheProvider>().Use(DefaultCacheProvider.GetInstance());

            //master data repository
            For<IUserRepository>().Use<UserRepository>();
            For<IServiceProviderRepository>().Use<ServiceProviderRepository>();
            For<ISecurityService>().Use<SecurityService>();

            //reports
            For<IReportService>().Use<ReportService>();

            For<IDistributorWebApiProxy>().Use<DistributorWebApiProxy>();

            For<IClientMemberRepository>().Use<ClientMemberRepository>();
            For<IClientRepository>().Use<ClientRepository>();
            
        }
    }
}
