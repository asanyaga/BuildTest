using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Domain.InventoryEntities;
using Distributr.Core.Domain.Master.BankEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Mobile.Core.Data;
using Distributr.Mobile.Core.OrderSale;
using Distributr.Mobile.Core.Test.Data;
using Distributr.Mobile.Core.Test.OrderSale;
using Distributr.Mobile.Envelopes;
using Distributr.Mobile.Login;
using Newtonsoft.Json;
using NUnit.Framework;
using SQLiteNetExtensions.Extensions;

namespace Distributr.Mobile.Core.Test.Support
{
    //Loads the local copy of Master Data. This can be used to Test Workflows in a headless, production-like manner, rather than relying on Mocks. 
    //However, this means that it is not so fast, so try to unit the number of tests low and use unit-tests when there are many variations on a scenario. 
    [Category("Integration")]
    public class WithFullDatabaseTest : WithEmptyDatabaseTest
    {
        [SetUp]
        public void LoadDatabase()
        {
            //This reuses some of the actual application code, which I am not 100% comfortable with. At least this code is tested by MasterDataUpdaterTest
            //when running on a CI server I hope to make MasterDataUpdaterTest run first, and then not run any WithFullDatbaseTest child tests if it is unsuccesful
            var zipStreamProcesser = new FakeZipStreamProcesser(MasterDataUpdaterTest.FullMasterUpdatePath);
            var masterDataUpdater = new MasterDataUpdater(Database, zipStreamProcesser);

            var result = masterDataUpdater.ApplyUpdate(false, default(Stream));
            
            if (!result.WasSuccessful())
            {
                throw result.Exception;
            }
        }

        public static List<CommandEnvelope> ExtractCommandEnvelopes(List<LocalCommandEnvelope> envelopes)
        {
            var commandEnvelopes = new List<CommandEnvelope>();

            foreach (var commandEnvelope in envelopes)
            {
                var envelope = JsonConvert.DeserializeObject<CommandEnvelope>(commandEnvelope.Contents);
                commandEnvelopes.Add(envelope);
            }

            return commandEnvelopes;
        }

        //MD5 Hash of string "12345678"
        protected const string HashedPassword = "25d55ad283aa400af464c76d713c07ad";

        public SaleProduct ASaleProduct()
        {
            return Database.GetWithChildren<SaleProduct>(new Guid("4b5bc620-9e3a-44ce-99e4-cf023e17d2e6"), recursive: true);        
        }

        public Inventory AnInventoryProduct()
        {
            return Database.GetWithChildren<Inventory>(new Guid("9974ebb1-e274-4c6b-b257-606d79b92549"), recursive: true);
        }

        public Inventory AnotherInventoryProduct()
        {
            return Database.GetWithChildren<Inventory>(new Guid("6074ebb1-e274-4c6b-b257-606d79b92549"), recursive: true);
        }

        public SaleProduct AnotherSaleProduct()
        {
            return Database.GetWithChildren<SaleProduct>(new Guid("c8a07a84-fd98-44cf-86e2-e8983dc81ed6"), recursive: true);
        }

        public User AUser()
        {
            var user = Database.GetAll<User>().FirstOrDefault();
            user.CostCentreApplicationId = Guid.NewGuid().ToString(); // Generate a Fake ID for tests
            return user;                                    
        }

        public User AnUnknownUser()
        {
            return new User()
            {
                CostCentre = Guid.NewGuid(),
                CostCentreApplicationId = Guid.NewGuid().ToString(),
                Username = "Ander",
                Password = HashedPassword
            };            
        }

        public DistributorSalesman AUsersCostCentre()
        {
            return Database.GetWithChildren<DistributorSalesman>(AUser().CostCentre);
        }

        public Outlet AnOutlet()
        {
            return Database.GetAll<Outlet>().FirstOrDefault();
        }

        public Bank ABank()
        {
            return Database.GetAll<Bank>().FirstOrDefault();
        }

        public OrderSaleContextBuilder AnOrderAndContextBuilder()
        {
            var outlet = AnOutlet();
            var order = new Order(Guid.NewGuid(), outlet);
            var bank = ABank();
            return new OrderAndContextBuilder(outlet, AUsersCostCentre(), AUser(), order, bank, bank.Branches.First());
        }

        public OrderSaleContextBuilder AnSaleAndContextBuilder()
        {
            var outlet = AnOutlet();
            var order = new Order(Guid.NewGuid(), outlet);
            var bank = ABank();
            return new SaleAndContextBuilder(outlet, AUsersCostCentre(), AUser(), order, bank, bank.Branches.First());
        }

        public OrderSaleContextBuilder AnUnpaidOrderForOneItem()
        {
            var inventoryProduct = AnInventoryProduct();

            var aProduct = Database.GetWithChildren<SaleProduct>(inventoryProduct.ProductMasterID, recursive: true);

            return AnOrderAndContextBuilder()
                .AddLineItem(aProduct, 1, 0);
        }

        public OrderSaleContextBuilder AnUnpaidOrderForTwoItems()
        {
            var anotherInventoryProduct = AnotherInventoryProduct();
            var anotherProduct = Database.GetWithChildren<SaleProduct>(anotherInventoryProduct.ProductMasterID, recursive: true);

            return AnUnpaidOrderForOneItem()
                .AddLineItem(anotherProduct, 1, 0);
        }

        public OrderSaleContextBuilder AFullyPaidCashOrderForOneItem()
        {
            return AnUnpaidOrderForOneItem().PaidInfFullByCash();
        }

        public OrderSaleContextBuilder AnUnpaidSaleForOneItem()
        {
            var anInventoryProduct = AnInventoryProduct();
            var aProduct = Database.GetWithChildren<SaleProduct>(anInventoryProduct.ProductMasterID, recursive: true);

            return AnSaleAndContextBuilder()
                .AddLineItem(aProduct, 1, 0);
        }

        public OrderSaleContextBuilder AnUnpaidSaleForTwoItems()
        {            
            var anotherInventoryProduct = AnotherInventoryProduct();
            var anotherProduct = Database.GetWithChildren<SaleProduct>(anotherInventoryProduct.ProductMasterID, recursive: true);

            return AnUnpaidSaleForOneItem()
                .AddLineItem(anotherProduct, 1, 0);
        }

        public OrderSaleContextBuilder ASaleFullyPaidByChequeWithOneItem()
        {
            return AnUnpaidSaleForOneItem().PaidInFullByCheque();
        }

        public OrderSaleContextBuilder AFullyPaidCashSaleWithTwoItems()
        {
            return AnUnpaidSaleForTwoItems()
                .PaidInfFullByCash();
        }
    }
}