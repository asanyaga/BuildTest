Testing 
============

Xamarin does not have great support for end-to-end testing or UI testing. As a result, we keep as much logic as possible out of Distributr.Mobile and put it in Distributr.Mobile.Core instead. Distributr.Mobile.Core is a plain C# project that has no dependencies on Xamarin or Android, meaning that it can be tested in isolation. 

##Writing Tests

All code inside Distributr.Mobile.Core needs to be tested. Testing of most features will consist of unit tests and integration tests. Unit tests should focus on one class, testing all possible uses of the class's API. Integration tests should focus on how components work together and should treat the components under test as black-box, ignoring the things happen inside and instead verifying external events, such as that a web service has been called appropriately or that the database is in the correct state after the test has run. Integration tests are slower so we want to avoid testing things that are better tested by unit tests, such as command field mappings and arithmetic. All tests should be added to Distributr.Mobile.Core.Test. 

### Test Categories
NUnit - the testing framework that we are using - allows us to group tests into categories. There are currently three categories of tests

* \<Uncategorized\> - this is the default category and contains unit tests
* Database - tests that work exclusively with the Database, such as some DAO tests and the MasterDataUpdaterTest which verifies that the Master Data CSV files provided by the server devs can be applied OK
* Integration - tests that also read and write to the database as well as make HTTP calls to the fake web server
 
The reason for the additional database category is so that we can run this before the integration tests. The integration tests depend on a fully functional DB (they use the data loaded from the CSV files that are tested by MasterDataUpdaterTest). If the database test do not work, there's no point running the integration tests. 

### Test Support
There are four test base classes that you can extend to make your life easier when writing new tests. Some of these base classes contain factories and builders for generating test data which is either from mocks, or loaded from a local SQLite database running on Windows. 

* [WithDependenciesTest](https://github.com/nutshellit/VirtualCity-Distributr/blob/master/Distributr.Mobile/Distributr.Mobile.Core.Test/Support/WithDependenciesTest.cs) - You can extend this class if need access to the Dependency Container inside the App. Just like in your application code you can call `Resolve<MyObject>();` to resolve a dependency. This will also verify that the Dependency Container is setup correctly. 
* [WithEmptyDatabaseTest](https://github.com/nutshellit/VirtualCity-Distributr/blob/master/Distributr.Mobile/Distributr.Mobile.Core.Test/Support/WithEmptyDatabaseTest.cs) - This class extends WithDependenciesTest giving you access to the Dependency Container and also creates an empty SQLite database, which is cleaned after each test.
* [WithFullDatabaseTest](https://github.com/nutshellit/VirtualCity-Distributr/blob/master/Distributr.Mobile/Distributr.Mobile.Core.Test/Support/WithFullDatabaseTest.cs) - This class extends WithEmptyDatabaseTest and loads the CSV files from disk into SQLite given you access to real data in your test. This class also contains a number of factories that you can use to reduce test data setup noise. 

 e.g. 
 ```c# 
 var anUnpaidOrderForOneItem = AnUnpaidOrderForOneItem(); 
 ```

 We want to avoid duplication across our tests and reduce the noise that test data setup code adds, so we use factories and builders which are present on base classes. 
* [WithFakeWebServerTest](https://github.com/nutshellit/VirtualCity-Distributr/blob/master/Distributr.Mobile/Distributr.Mobile.Core.Test/Support/WithFakeServerTest.cs) - Extend this test if you want to test code that makes web service calls, such as login or command uploading. It allows you to prime the fake server with fake responses that imitate real web server responses. As far as the application knows it is talking to the real production web server. 

 Usage of this class is as follows
 1. Add Fake Responses which map to real requests made by the App
 2. Exercise application code
 3. Call AssertFakeServerIsSatisfied() to verify that the server delivered all responses specified and did not receive unexpected requests. 

 Here is a real test from [CommandEnvelopeUploaderTest](https://github.com/nutshellit/VirtualCity-Distributr/blob/master/Distributr.Mobile/Distributr.Mobile.Core.Test/Sync/Outgoing/CommandEnvelopeUploaderTest.cs)
 ```c#
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

            var pendingEnvelopes = localCommandEnvelopeRepository.GetNextIncomingEnvelopeBatch();

            Assert.AreEqual(0, pendingEnvelopes.Count, "pending envelopes");
        }
```
Make sure that you understand the code inside all of these bases classes so that you can reuse it rather than duplicate it. You should add new factories and builders as necessary when adding new features. Ideally all tests should be clean and easy to understand like the example above, though some of them will be longer due to the Byzantine server API that we have to deal with. 
