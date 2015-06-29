using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using Distributr.WPF.Lib.Data.Migrations;
using Distributr.WPF.Lib.Impl.Model.Transactional;
using Distributr.WPF.Lib.Impl.Model.Transactional.AuditLog;
using Distributr.WPF.Lib.Impl.Model.Utility;
using Distributr.WPF.Lib.Services.Service.CommandQueues;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client.Request;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client.Response;
using log4net;

namespace Distributr.WPF.Lib.Data.EF
{
    public class DistributrLocalContext : DbContext
    {
        public DistributrLocalContext(string connectionString) : base(connectionString)
        {
        }

        private ILog _logger = LogManager.GetLogger("DistributrLocalContext");
        public DbSet<LogLocal> LogLocals { get; set; }
        public DbSet<ErrorLogLocal> ErrorLogLocals { get; set; }
        public DbSet<IncomingCommandQueueItemLocal> IncomingCommandQueueItemLocals { get; set; }
        public DbSet<InComingCommandEnvelopeQueueItemLocal> InComingCommandEnvelopeQueueItems { get; set; }
        public DbSet<OutgoingCommandQueueItemLocal> OutgoingCommandQueueItemLocals { get; set; }
        public DbSet<OutGoingMasterDataQueueItemLocal> OutGoingMasterDataQueueItemLocals { get; set; }
        public DbSet<BuyGoodsNotificationResponse> BuyGoodsNotificationResponse { get; set; }
        public DbSet<ConfigLocal> ConfigLocal { get; set; }
        public DbSet<GeneralSettingLocal> GeneralSettingLocal { get; set; }
        public DbSet<UnExecutedCommandLocal> UnExecutedCommandLocal { get; set; }
        public DbSet<PaymentNotificationRequest> AsynchronousPaymentNotificationRequest { get; set; }
        public DbSet<PaymentNotificationResponse> AsynchronousPaymentNotificationResponse { get; set; }
        public DbSet<PaymentRequest> AsynchronousPaymentRequest { get; set; }
        public DbSet<PaymentResponse> AsynchronousPaymentResponse { get; set; }
        public DbSet<ReceivedCommandLocal> ReceivedCommand { get; set; }
        public DbSet<ReceivedCommandEnvelopeId> ReceivedCommandEnvelopeIds { get; set; }
        public DbSet<ClientApplicationLocal> ClientApplicationLocal { get; set; }
        public DbSet<SyncTrackerLocal> SyncTrackerLocal { get; set; }
        public DbSet<PaymentNotificationListItem> PaymentNotificationListItems { get; set; }
        public DbSet<OutGoingNotificationQueueItemLocal> OutGoingNotificationQueueItemLocals { get; set; }
        public DbSet<ReceiptPrintTracker> ReceiptPrintTrackers { get; set; }
        public DbSet<AppTempTransaction> AppTempTransactions { get; set; }
        public DbSet<OutGoingCommandEnvelopeQueueItemLocal> OutGoingCommandEnvelopeQueueItems { get; set; }

        

        public override int SaveChanges()
        {
            try
            {
                _logger.Info("SaveChanges");
                return base.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    _logger.ErrorFormat("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                   
                    foreach (var ve in eve.ValidationErrors)
                    {
                        _logger.ErrorFormat("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                        
                    }
                }
                throw;
            }
           
        }
        
      

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<PaymentNotificationListItem>()
                .HasRequired(x => x.PaymentNotificationResponse)
                .WithMany(x => x.PaymentNotificationDetails)
                .HasForeignKey(x => x.ResponseId)
                .WillCascadeOnDelete(false);
        }
    }
}
