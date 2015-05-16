Feature: InventoryTransferNote	
	In order to transfer invetory to  a costcentre
	As a user
	I want to be able to tranfer inventory from the hub to  a salesman  existing stock  using a Inventory Tranfer Note
    And when the Inventory tranfer Note on the costcentre client application is confirmed
    Then ITN document should be sent asynchronously to the server 
    And after the document is processed on the server the document should be on the server and the inventory level for that salesman should be adjusted


@documenthook
Scenario: Adjust local salesman inventory level using ITN
	Given I have inventory on the hub with a current stock level
	And I create an ITN to adjust its level by ten
	When I submit the ITN to its respective workflow
	Then then the inventory level on the hub should have decreased by ten and increased the salesman inventory by ten
	And there should be a saved ITN document
	And there should be a corresponding ITN command envelope on the outgoing command queue 

@documenthook
Scenario: issue as salesman stock using ITN  to be synced to server
	Given I have a product stock on the hub  that i want to issue to as a salesman [server]
	And I create an ITN to issue as  salesman ten unit of stock   [server]
	When I submit the ITN to its respective workflow [server]
	And I trigger a server sync send ITN to the server
	Then there should be a saved ITN document on the server
	And the salesman product stock level on the server should be the same as the hub  