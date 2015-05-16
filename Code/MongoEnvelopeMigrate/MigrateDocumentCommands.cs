using Akavache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using Distributr.WSAPI.Lib.Services.CommandAudit;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using MongoDB.Bson.Serialization;
using MongoDB.Driver.Linq;
using Distributr.Core.Commands.DocumentCommands;
using Newtonsoft.Json;
using Distributr.Core.Utility;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Commands.CommandPackage;
using System.Diagnostics;
using System.Threading;
using Distributr.Core.Utility.Command;
using log4net;
using Distributr.WSAPI.Lib.Services.Routing;
using Distributr.MongoDB.CommandRouting;



namespace MongoEnvelopeMigrate
{
    public class MigrateDocumentCommands
    {
        private static readonly ILog log = LogManager.GetLogger
                                  (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        AppSettings settings = null;
        public MigrateDocumentCommands()
        {
            settings = new AppSettings();
        }
        public async Task<int> Go()
        {
            log.Info("Begin migrating mongo commands to envelope");
            await Task.Delay(100);
            log.Info("Get all saved documentIds from local cache");
            List<MapReduceResult> mrResults = await GetAllDocumentIdsFromCache();
            int loop = 0;
            log.InfoFormat("Begin processing {0} documentIds ", mrResults.Count());
            Action<int> act1 = i =>
            {
                try
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    var r = mrResults[i];
                    int threadId = Thread.CurrentThread.ManagedThreadId;
                    log.DebugFormat("[{1}] # Count {2} #  Processing  document id {0}", r.Key, threadId, i);
                    if (r.Key == Guid.Empty)
                        return;
                    log.DebugFormat("[{1}] Fetch commands for documentid {0}", r.Key, threadId);
                    List<CommandProcessingAudit> commandsForDocumentId = GetCommandProcessingAuditsByDocumentId(r.Key);
                    log.DebugFormat("[{0}] Resolve execution grouping", threadId);
                    log.DebugFormat("[{0}] Map to command envelope processing audit", threadId);
                    Tuple<CommandEnvelopeProcessingAudit, long?> envelopeAuditT = MapAuditAndResolveRORId(r.Key, commandsForDocumentId);
                    long? rorId = envelopeAuditT.Item2;
                    CommandEnvelopeProcessingAudit envelopeAudit = envelopeAuditT.Item1;
                    log.DebugFormat("[{0}] Map to ROR CC", threadId);
                    List<CommandEnvelopeRouteOnRequestCostcentre> rorCCs = MapRORCC(rorId, envelopeAudit);
                    log.DebugFormat("[{0}] Map to Command Envelope Routing Status", threadId);
                    List<CommandEnvelopeRoutingStatus> envRoutingStatus = MapRoutingStatuses(rorId, rorCCs);
                    log.DebugFormat("[{0}] Save......", threadId);
                    Save(envelopeAudit, rorCCs, envRoutingStatus);
                    sw.Stop();
                    log.DebugFormat("DocumentId {0} processed in {1} milliseconds", r.Key, sw.Elapsed.TotalMilliseconds);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
                //Thread.Sleep(200);
            };

            //--- Single thread looping
            //foreach (var result in mrResults)
            //{
            //    loop++;
            //    act1(loop);
            //}
            //---- Parallel loopinng
            Parallel.For(1, mrResults.Count(), act1);

            return mrResults.Count();
        }

        void Save(CommandEnvelopeProcessingAudit envelopeAudit, List<CommandEnvelopeRouteOnRequestCostcentre> rorCCs, List<CommandEnvelopeRoutingStatus> envRoutingStatus)
        {
            ICommandEnvelopeProcessingAuditRepository auditRepository = settings.MongoCommandEnvelopeProccessingAuditRepository;
            CommandEnvelopeRouteOnRequestCostcentreRepository rorRepository = settings.MongoCommandEnvelopeRORCostCentreRepository as CommandEnvelopeRouteOnRequestCostcentreRepository; ;
            MongoCollection<CommandEnvelopeRouteOnRequestCostcentre> repCERORCC = rorRepository._commandEnvelopeRouteOnRequestCostcentreCollection;
            MongoCollection<CommandEnvelopeRoutingStatus> routingStatus = rorRepository._commandEnvelopeRoutingStatusCollection;
           
            auditRepository.AddCommand(envelopeAudit);
            
            if (rorCCs != null)
            {
                foreach (var rorCC in rorCCs)
                {
                    repCERORCC.Save(rorCC);
                }
            }

            if (envRoutingStatus != null)
            {
                foreach (var stat in envRoutingStatus)
                {
                    routingStatus.Save(stat);
                }
            }
            
        }

        private List<CommandEnvelopeRoutingStatus> MapRoutingStatuses(long? rorId, List<CommandEnvelopeRouteOnRequestCostcentre> rorEnvCCs)
        {
            if (rorId == null) return null;
            //* Lookup CommandRoutingStatus by RORId
            List<CommandRoutingStatus> ccAppIdsInCommandRoutingStatusColl = settings.MongoCollection_Command_CommandRoutingStatus
                .AsQueryable()
                .Where(n => n.CommandRouteOnRequestId == rorId.Value)
                .ToList();

            //* Get distinct ccappids
            List<Guid> ccAppIds = ccAppIdsInCommandRoutingStatusColl.Select(n => n.DestinationCostCentreApplicationId).Distinct().ToList();
            //* Lookup sql db cdids to get ccid
            var cc_ccappidsG = settings.GetContext().tblCostCentreApplication
                .Where(n => ccAppIds.Contains(n.id))
                .Select(n => new { ccappid = n.id, n.costcentreid })
                .ToList()
                .GroupBy(n => n.costcentreid);

            List<CommandEnvelopeRoutingStatus> envelopeStatuss = new List<CommandEnvelopeRoutingStatus>();
            //* Lookup CommandEnvelopeRouteOnRequestCostcentre by ccid
            foreach (var ccidG in cc_ccappidsG)
            {
                Guid ccid = ccidG.Key;
                if(!rorEnvCCs.Any(n => n.CostCentreId == ccid))
                {
                    log.WarnFormat("Unable to reverse lookup ccid in CommandEnvelopeRouteOnRequestCostCentre {0}", ccid);
                    continue;
                }
                //* generate CommandEnvelopeRoutingStatus's based on above ccappids
                CommandEnvelopeRouteOnRequestCostcentre ceRORCC = rorEnvCCs.First(n => n.CostCentreId == ccid);
                foreach (var ccid_appid in ccidG)
                {
                    CommandRoutingStatus crs = ccAppIdsInCommandRoutingStatusColl
                        .Single(n => n.DestinationCostCentreApplicationId == ccid_appid.ccappid);
                    envelopeStatuss.Add(new CommandEnvelopeRoutingStatus { 
                        Id = Guid.NewGuid(),
                        EnvelopeArrivalAtServerTick = crs.DateAdded.Ticks,
                        EnvelopeDeliveredAtServerTick = crs.DateAdded.Ticks,
                        EnvelopeId = ceRORCC.EnvelopeId,
                        DestinationCostCentreApplicationId = ccid_appid.ccappid,
                        Delivered = crs.Delivered,
                        DateDelivered = crs.DateDelivered,
                        DateExecuted = crs.DateExecuted,
                        DateAdded = crs.DateAdded,
                        CostCentreId = ccid_appid.costcentreid
                    });
                }
            }
            return envelopeStatuss;
        }

        private List<CommandEnvelopeRouteOnRequestCostcentre> MapRORCC(long? rorId, CommandEnvelopeProcessingAudit envelopeAudit)
        {
            if (rorId == null)
                return null;
            var rorcc = settings.MongoCollection_Command_CommandRouteOnRequestCostCentre
                .AsQueryable()
                .Where(n => n.CommandRouteOnRequestId == rorId.Value)
                .ToList();
            var rorEnvCCs = rorcc.Select(n => new CommandEnvelopeRouteOnRequestCostcentre
            {
                Id = Guid.NewGuid(),
                EnvelopeId = envelopeAudit.Id,
                EnvelopeArrivalAtServerTick = n.DateAdded.Ticks,
                CostCentreId = n.CostCentreId,
                IsValid = true,
                DateAdded = n.DateAdded,
                IsRetired = n.IsRetired,
                DocumentType = envelopeAudit.DocumentType.ToString(),
                EnvelopeRoutedAtServerTick = n.DateAdded.Ticks,
                EnvelopeRoutePriority = EnvelopeRoutePriority.Level1

            });
            if (rorEnvCCs.Count() == 0)
                log.WarnFormat("No CommandEnvelopeRouteOnRequestCostcentres added for RORId {0}", 0);
            return rorEnvCCs.ToList();
        }



        Tuple<CommandEnvelopeProcessingAudit, long?> MapAuditAndResolveRORId(Guid documentId, List<CommandProcessingAudit> commands)
        {
            CommandProcessingAudit initialCommandAudit = commands.OrderBy(n => n.CostCentreCommandSequence).First();
            DocumentCommand initialCommand = JsonConvert.DeserializeObject<DocumentCommand>(initialCommandAudit.JsonCommand);
            IResolveCommand resolver = new ResolveCommand();
            ResolveCommandItem rci = resolver.Get(initialCommand);
            DocumentType dt = rci.DocumentType;
            Guid envelopeId = Guid.NewGuid();
            long generatedTick = initialCommandAudit.DateInserted.Ticks;
            long? rorId = GetCommandRORId(initialCommandAudit.Id);
            Guid? recipientCostCentreId = null;
            if (rorId != null)
            {
                recipientCostCentreId = ResolveReceipientCostCentre(initialCommandAudit.Id, initialCommand.CommandGeneratedByCostCentreId, rorId.Value);
            }
            if (recipientCostCentreId == null)
            {
                log.WarnFormat("Failed to resolve recipient cost centre for document id {0} command id {1}, saving envelope without recipient cost centre ", documentId, initialCommandAudit.Id);
            }
            DocumentCommand[] deserialisedCommands = commands.OrderBy(n => n.CostCentreCommandSequence).Select(n => JsonConvert.DeserializeObject<DocumentCommand>(n.JsonCommand)).ToArray();
            CommandEnvelope ce = CommandsToEnvelope(envelopeId, documentId, dt, generatedTick, initialCommandAudit.ParentDocumentId, recipientCostCentreId, deserialisedCommands);
            var cpa = new CommandEnvelopeProcessingAudit
            {
                Id = envelopeId,
                DocumentId = documentId,
                DocumentTypeName = dt.ToString(),
                DocumentType = dt,
                LastExecutedCommand = 0,
                NumberOfCommand = commands.Count(),
                GeneratedByCostCentreApplicationId = initialCommand.CommandGeneratedByCostCentreApplicationId,
                GeneratedByCostCentreId = initialCommand.CommandGeneratedByCostCentreId,
                RecipientCostCentreId = recipientCostCentreId ?? Guid.Empty,
                ParentDocumentId = initialCommandAudit.ParentDocumentId,
                JsonEnvelope = JsonConvert.SerializeObject(ce)
            };
            return Tuple.Create(cpa, rorId);
        }

        public Guid? ResolveReceipientCostCentre(Guid commandId, Guid generatedByCCId, long rorId)
        {

            var rorcc = settings.MongoCollection_Command_CommandRouteOnRequestCostCentre
                .AsQueryable()
                .Where(n => n.CommandRouteOnRequestId == rorId)
                .ToList()
                .Where(n => n.CostCentreId != generatedByCCId)
                .Select(n => new { Tick = n.DateAdded.Ticks, CCId = n.CostCentreId })
                .OrderBy(n => n.Tick)
                .FirstOrDefault();
            if (rorcc == null)
            {
                log.WarnFormat("Failed to get ROR for commandId {0} and RORId {1} from CommandRouteOnRequestCostCentre", commandId, rorId);
                return null;
            }
            return rorcc.CCId;
        }

        long? GetCommandRORId(Guid commandId)
        {
            var rors = settings.MongoCollection_Command_CommandRouteOnRequest
                .AsQueryable()
                .Where(n => n.CommandId == commandId)
                .ToList();
            if (rors.Count() == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                log.WarnFormat("Failed to get ROR for commandId {0}", commandId);
                Console.ForegroundColor = ConsoleColor.White;
                return null;
            }
            return rors.First().Id;
        }

        CommandEnvelope CommandsToEnvelope(Guid envelopeId, Guid documentId, DocumentType documentType, long generatedTick, Guid parentId, Guid? recipientCCId, DocumentCommand[] commands)
        {
            DocumentCommand refC = commands.First();
            List<CommandEnvelopeItem> cei = commands.Select((n, i) => new CommandEnvelopeItem(i + 1, n)).ToList();
            var ce = new CommandEnvelope
            {
                Id = envelopeId,
                DocumentId = documentId,
                DocumentTypeId = (int)documentType,
                GeneratedByCostCentreId = refC.CommandGeneratedByCostCentreId,
                RecipientCostCentreId = recipientCCId ?? Guid.Empty,
                GeneratedByCostCentreApplicationId = refC.CommandGeneratedByCostCentreApplicationId,
                ParentDocumentId = parentId,
                CommandsList = cei,
                EnvelopeGeneratedTick = generatedTick,
                EnvelopeArrivedAtServerTick = generatedTick
            };

            return ce;
        }

        List<CommandProcessingAudit> GetCommandProcessingAuditsByDocumentId(Guid documentId)
        {
            var audit = settings.MongoCollection_Command_CommandProcessingAudit.AsQueryable().Where(n => n.DocumentId == documentId).ToList();
            return audit;
        }

        async Task<List<MapReduceResult>> GetAllDocumentIdsFromCache()
        {
            var items = await BlobCache.UserAccount.GetAllObjects<MapReduceResult>();
            return items.ToList();
        }
    }
}
