@masterdatahook
Feature: HubSetup
	In order to use a hub application on the distributr platform
	As a hub application
	I want to be able to setup the hub application so that it can communicate with the distributr platform
	And synchronise the master data on the distributr platform

@masterdatahook
Scenario: Client Application Id Setup
	Given I want to setup the client application id on a hub application
	When i run the appid setup
	Then the configuration should have the saved client application

@masterdatahook
Scenario: Set Config WebService Url
	Given I want to setup the distributr server url on a hub application
	When i run initial setup config webservice setup
	And I set the url on the configuration 
	Then the configuration should have the saved server url

@masterdatahook
Scenario: Do initial login to get cost centre id
	Given I want to establish the cost centre id of my hub
	And I have a valid username and password
	When I run prelogin setup
	And I login to the server
	Then I should get a valid response
	And the configuration should have the saved cost centre id for the hub

@masterdatahook
Scenario: Get cost centre application id and application initialise
	Given I want to initialise the hub application for this cost centre by getting the cost centre application id
	When I run the setup required for getting ccappid
	And I get cost centre application id
	Then i should get a valid response
	And  the application should be initialized

@masterdatahook
Scenario: Can Sync
	Given I want to sync data
	And I run the setup required [stephook one]
	When I run Can Sync
	Then the client application can sync should be set to true

@masterdatahook
Scenario: Can sync master data
	Given I want to sync master data
	And I run set setup required [stephook two]
	When I run sync master data on the hub
	Then the hub application should have valid master data
	

