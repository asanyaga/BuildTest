using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Mobile.Core.Envelopes;
using Distributr.Mobile.Core.Net;
using Distributr.Mobile.Core.Sync.Outgoing;
using Distributr.Mobile.Core.Test.Support;
using Distributr.Mobile.Envelopes;
using Distributr.Mobile.Sync.Outgoing;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Distributr.Mobile.Core.Test.Sync.Outgoing
{    
    [TestFixture]
    public class CommandEnvelopeUploaderTest : WithFakeServerTest
    {
        private static readonly LocalCommandEnvelope Envelope1 = new LocalCommandEnvelope()
        {
            Id = Guid.NewGuid(),
            Contents = "{value = 1 }",
            RoutingStatus = RoutingStatus.Pending, 
            DocumentType = DocumentType.Order,
            RoutingDirection = RoutingDirection.Outgoing,
            ParentDoucmentGuid = Guid.NewGuid()
        };

        private static readonly LocalCommandEnvelope Envelope2 = new LocalCommandEnvelope()
        {
            Id = Guid.NewGuid(),
            Contents = "{value = 2 }",
            RoutingStatus = RoutingStatus.Pending,
            DocumentType = DocumentType.DispatchNote,
            RoutingDirection =  RoutingDirection.Outgoing,
            ParentDoucmentGuid = Guid.NewGuid()
        };

        private static readonly List<LocalCommandEnvelope> Envelopes = new List<LocalCommandEnvelope>()
        {
            Envelope1,
            Envelope2
        };

        private readonly UploadEnvelopeResponse positiveResponse = new UploadEnvelopeResponse()
        {
            Result = UploadEnvelopeResponse.EnvelopeProcessed
        };

        private readonly UploadEnvelopeResponse negativeResponse = new UploadEnvelopeResponse()
        {
            Result = UploadEnvelopeResponse.ProcessingFailed
        };

        private ILocalCommandEnvelopeRepository localCommandEnvelopeRepository;
        private CommandEnvelopeUploader commandEnvelopeUploader;

        [SetUp]
        public void InsertEnvelopes()
        {
            localCommandEnvelopeRepository = Resolve<ILocalCommandEnvelopeRepository>();
            commandEnvelopeUploader = Resolve<CommandEnvelopeUploader>();
            commandEnvelopeUploader.StatusUpdate += (e) => { }; //Events are already tested in NetworkAwareServiceTest
            Database.InsertAll(Envelopes);
        }

        [Test]
        public void CanUploadPendingEnvelopesWhenConnectionIsAvailable()
        {
            //Given            
            AddFakePostResponse(CommandEnvelopeUploadClient.OutgoingEnvelopeEndpoint, 
                new HttpParams(), 
                JsonConvert.SerializeObject(positiveResponse),
                Envelope1.Contents);
                        
            AddFakePostResponse(CommandEnvelopeUploadClient.OutgoingEnvelopeEndpoint, 
                new HttpParams(), 
                JsonConvert.SerializeObject(positiveResponse),
                Envelope2.Contents);
          
            //When
            commandEnvelopeUploader.UploadPendingEnvelopes();
            
            //Then
            AssertFakeServerIsSatisfied();

            var pendingEnvelopes = localCommandEnvelopeRepository.GetNextOutgoingBatch();

            Assert.AreEqual(0, pendingEnvelopes.Count, "pending envelopes");
        }

        [Test]
        public void ErrorResponseLeadsToCommandBeingMarkedAsFailed()
        {
            //Given 
            AddFakePostResponse(CommandEnvelopeUploadClient.OutgoingEnvelopeEndpoint, 
                new HttpParams(), 
                JsonConvert.SerializeObject(negativeResponse),
                Envelope1.Contents);            

            AddFakePostResponse(CommandEnvelopeUploadClient.OutgoingEnvelopeEndpoint, 
                new HttpParams(), 
                JsonConvert.SerializeObject(negativeResponse),
                Envelope2.Contents);            

            //When
            commandEnvelopeUploader.UploadPendingEnvelopes();

            //Then
            AssertFakeServerIsSatisfied();

            var pendingEnvelopes = localCommandEnvelopeRepository.GetNextOutgoingBatch();

            Assert.AreEqual(0, pendingEnvelopes.Count, "pending envelopes");

            var count =
                Database.GetAll<LocalCommandEnvelope>()
                    .Count(
                        l => l.RoutingStatus == RoutingStatus.Error && l.RoutingDirection == RoutingDirection.Outgoing);

            Assert.AreEqual(2, count, "error count");
        }
    }
}