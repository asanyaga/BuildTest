Feature: Purchase Order
	In order to create a purchase order
	As a user
	I want to be able to create a purchase order
	Then i should be able  to create an order line item and add it to the purchase order
	When the purchase order has been confirmed
	And i have submitted the purchase order to its respective workflow
	Then the purchase order should be sent asynchronuously to the server
	And when the purchase order is processed on the server, the purchase order should have the correct sale amounts

@documenthook
Scenario: Create a purchase order
	Given I create a purchase order
	And I create a purchase order line item
	When I submit the purchase order to its respective workflow
	Then there should be a saved purchase order
	And the purchase order should have a line item
	And there should be a corresponding purchase order command envelope on the outgoing command queue

@documenthook
Scenario: Create a server purchase order
	Given I create a purchase order [server]
	And I create a purchase order line item [server]
	When I submit the purchase order to its respective workflow [server]
	And I trigger a server sync from hub (purchase order) [server]
	Then There should be a saved purchase order on the server [server]
	And the amount of the purchase order line item should be the same as in the hub [server]
	When I approve the purchase order in the server [server]
	And I trigger a server sync (purchase order) in the client application
	Then I should be able to fetch the purchase order in client application with a status of approved
	