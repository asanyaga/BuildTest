
Server API 
============

Currently, all communication with the server is initiated by the device over HTTP. This will change with the introduction of Google Cloud Messaging (GCM) which receives cloud messages on the Google Play Store connection (having the Play store installed is a prerequisite for GCM). 

### Base URL

All endpoints listed below are specified after the server base URL. The base URL has the following format

[ http:// ][ `server ip address` ][ /`web service name` ][ /api ]

Example:

`http://192.168.1.73/qa3_ws/api/`


### Login API

When a user logs in for the first time, or when starting with a clean DB, a remote login is performed by the device. This is a two step process. The server initially validates the user's credentials and provides their `Cost Centre`. This Cost Centre is then used in the second request to acquire the user's `Cost Centre Application ID` that is later used to identify the user and device when communicating with the server.

##### Step 1

Initial login to acquire Cost Centre and validate the user's credentials

| Endpoint       | Request Type | 
|----------------|--------------|
| Login/LoginGet | GET          |

| Parameter Name | Value                                                              |
|----------------|--------------------------------------------------------------------|
| Username       | `username` as entered into login screen                            | 
| Password       | `password` as entered into login screen, converted into a MD5 hash |
| usertype       |  always `DistributorSalesman`                                      |  

Example Request

`http://192.168.1.73/qa3_ws/api/Login/LoginGet?username=mike&password=5f4dcc3b5aa765d61d8327deb882cf99&usertype=DistributorSalesman`

Example positive response (JSON)

```json
{
  "CostCentreId" : "0f8fad5b-d9cb-469f-a165-70867728950e",
  "ErrorInfo" : "Success"
}

```

Example negative response (JSON)

```json
{
  "CostCentreId" : "00000000-0000-0000-0000-000000000000",
  "ErrorInfo" : "Invalid user name or password"
}

```

##### Step 2

Acquire user's Cost Centre Application ID

| Endpoint                                             | Request Type | 
|------------------------------------------------------|--------------|
| CostCentreApplication/GetCreateCostCentreApplication | GET          |

| Parameter Name               | Value                                                              |
|------------------------------|--------------------------------------------------------------------|
| costCentreId                 | `Cost Centre ID ` as returned from step 1                          | 
| applicationDescription       | always `Android_Application` for Android Devices                   |


Example Request

`http://192.168.1.73/qa3_ws/api/CostCentreApplication/GetCreateCostCentreApplication?costCentreId=0f8fad5b-d9cb-469f-a165-70867728950e&applicationDescription=Android_Application`

Example positive response (JSON)

```json
{
  "CostCentreApplicationId" : "7c9e6679-7425-40de-944b-e07fc1f90ae7",
  "ErrorInfo" : "Success"
}

```

Example negative response (JSON)

```json
{
  "CostCentreApplicationId" : "00000000-0000-0000-0000-000000000000",
  "ErrorInfo" : "Invalid Cost Centre"
}

```

On receiving a successful response, the user's Cost Centre Application ID is saved in the database on the `Distributr.Mobile.Login.User` entity/table. This ID is then used when communication with the server. 

### Master Data API

Master Data is provided in CSV format. There is one or more CSV files per entity/table. The CSV files are compressed (zip format) and provided as a HTTP File Download rather than a standard GET or POST HTTP Response. Downloads can be resumed which means that no data is wasted if the device's connection is lost temporarily. Android's Download manager is used to download the Master Data CSV file. It automatically pauses and resumes the download depending on the connection availability.


| Endpoint                                                 | Request Type | 
|----------------------------------------------------------|--------------|
| downloadmasterdata/GetZipped/{`CostCentreApplicationId`} | GET          |

Example 

`http://192.168.1.73/qa3_ws/api/downloadmasterdata/GetZipped/0f8fad5b-d9cb-469f-a165-70867728950e`

The response is a HTTP File Download named `MasterDataCSVFiles.zip` which is renamed on the device to avoid collisions. The new name includes the Cost Centre Application ID and a timestamp e.g. masterdata_0f8fad5b-d9cb-469f-a165-70867728950e_200905211035131468.zip. 

The zip file contains one or more CSV files. The format is as follows

| Content Type | Example | Comment                                                             |
|--------------|---------|---------------------------------------------------------------------|
| text         | 'Small' | wrapped in single quotes                                            |
| number       | 1.23    |                                                                     |
| boolean      | 1       | 0 = false, 1 = true                                                 |
| NULL         | NULL    | uppercase null. For text, an empty pair of single quotes can be used|

Example CSV data (for the Route entity)

```csv
Code, DateCreated, DateLastUpdated, MasterId, Name, RegionId, StatusId
'001', '2015-04-09 09:47:19', '2015-04-09 09:47:19', '2dd3cc9a-10a9-499b-848a-8c3aa91adf0e', 'Eastside Route', 'd4b958a5-9eec-4172-86ca-eb354601ef14', 1
```

Each CSV file contained with the zip file has a name based on the entity/table that it represents. Camel Case is used, although some file names also contain underscores

Examples file names

- `PromotionDiscount_Items_0.csv` for Entity ProductDiscountItem 
- `UnderBanking_0.csv` for entity Underbanking

The number after the underscore represents the the file number. Large tables are broken down into multiple files, so it is possible to receive `UnderBanking_0.csv` and `UnderBanking_1.csv` etc. Each file will contain a maximum of 500 records. 

### Command Envelope Download API

| Endpoint                                             | Request Type | 
|------------------------------------------------------|--------------|
| commandenveloperouting/GetNextEnvelopes              | POST         |

A JSON message is POSTed in the request body. 

Example request body when starting from an empty device DB - i.e. need all envelopes)

```json
{
  "DeliveredEnvelopeIds" : [],
  "CostCentreApplicationId" : "7c9e6679-7425-40de-944b-e07fc1f90ae7",
  "BatchSize" : 1
}
```

Example request body when the device has already processed previous envelopes. It sends the ID of the last envelope it received in field "DeliveredEnvelopeIds". 

```json
{
  "DeliveredEnvelopeIds" : [ "0f8fad5b-d9cb-469f-a165-70867728950" ],
  "CostCentreApplicationId" : "7c9e6679-7425-40de-944b-e07fc1f90ae7",
  "BatchSize" : 1
}
```

The response is an array on command envelopes (which contains one item based on the BatchSize parameter in the request. 

Example response when the server has envelopes available for the device

```json
{
  "Id": "893dd638-8d3d-4bb0-91c1-f59794ff7127",
  "DocumentId": "8e6c6ff1-d890-4676-a59f-a25dde5d56e1",
  "DocumentTypeId": 1,
  "GeneratedByCostCentreId": "ffbd9638-1b86-4090-b8b1-78ed8c8f91b6",
  "RecipientCostCentreId": "51a6bde4-d645-43fa-b81b-3708f87a5e6d",
  "GeneratedByCostCentreApplicationId": "9aa4a373-e81a-4767-b689-a797a56cf8f0",
  "ParentDocumentId": "8e6c6ff1-d890-4676-a59f-a25dde5d56e1",
  "CommandsList": [
    {
      "Command": {
        "CommandTypeRef": "CloseOrder",
        "CommandId": "e11692af-37dd-4cb1-aaf5-34f6f64072d8",
        "PDCommandId": "8e6c6ff1-d890-4676-a59f-a25dde5d56e1",
        "DocumentId": "8e6c6ff1-d890-4676-a59f-a25dde5d56e1",
        "CommandGeneratedByUserId": "90173ad5-2e65-488d-94cb-6f350d56f346",
        "CommandGeneratedByCostCentreId": "ffbd9638-1b86-4090-b8b1-78ed8c8f91b6",
        "CostCentreApplicationCommandSequenceId": 1,
        "CommandGeneratedByCostCentreApplicationId": "9aa4a373-e81a-4767-b689-a797a56cf8f0",
        "SendDateTime": "2015-05-28T13:42:04",
        "CommandSequence": 0,
        "CommandCreatedDateTime": "2015-05-28T13:42:04",
        "Longitude": -3.5964775837673559,
        "Latitude": 54.539599620082832,
        "Description": null,
        "IsSystemCommand": false
      },
      "Order": 1
    }
  ],
  "OtherRecipientCostCentreList": [
    "51a6bde4-d645-43fa-b81b-3708f87a5e6d"
  ],
  "EnvelopeGeneratedTick": 1432816928214,
  "EnvelopeArrivedAtServerTick": 635684172039445245,
  "IsSystemEnvelope": false
}

```

Example response when there are no more envelopes to consume

```json
{
  "Envelopes": [],
  "ErrorInfo": "No Pending Download"
}
```

In the first example request above, the JSON array DeliveredEnvelopeIds is empty. In this case, the server will send the oldest known envelope that is associated with the Cost Centre Application ID provided. On downloading this envelope successfully, the device stores the envelopes ID which is then used in the next request. This patterns continues until the server returns "No Pending Download". 

### Command Envelope Upload API

| Endpoint            | Request Type | 
|---------------------|--------------|
| commandenvelope/run | POST         |

A JSON message containing the command envelope is POSTed in the request body. The envelope will contain one or more commands. Envelopes are uploaded one at a time, in the order they were generated. The server will provide an HTTP Response which shows the status of the upload request. 

Example positive response

```json
{
  "ErrorInfo":null, 
  "Result":"Envelope Processed",
  "ResultInfo":null
}
```

Example negative response

```json
{ 
  "ErrorInfo" : "Something bad happened",
  "Result" : "Processing Failed",
  "ResultInfo" : null
}
```
If the device receives a positive response, the envelope is deleted. Otherwise it is marked as failed and isolated from futher processing. The user can view envelopes which have failed in the Errors view in the UI. Currently it is possible to report these (via email) but in future it will be possible to retry processing. 
