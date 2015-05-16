Feature: InventoryAdjustmentNote
	In order to adjust existing stock levels in a costcentre
	As a user
	I want to be able to adjust existing stock levels on the costcentre client application using a Inventory Adjustment Note
    And when the Inventory Adjustment Note on the costcentre client application is confirmed
    Then IAN document should be sent asynchronously to the server 
    And after the document is processed on the server the document should be on the server and the stock levels for that cost centre should be adjusted


@documenthook
Scenario: Adjust local product stock level using IAN
	Given I have a product on the hub with a current stock level
	And I create an IAN to adjust its stock level by ten
	When I submit the IAN to its respective workflow
	Then then the product stock level on the hub should have increased by ten
	And there should be a saved IAN document
	And there should be a corresponding command envelope on the outgoing command queue

@documenthook
Scenario: Adjust local product stock level using IAN causes product stock level to be synced to server
	Given I have a product on the hub with a current stock level [server]
	And I create an IAN to adjust its stock level by ten [server]
	When I submit the IAN to its respective workflow [server]
	And I trigger a server sync
	Then there should be a saved IAN document on the server
	And the product stock level on the server should be the same as the hub  