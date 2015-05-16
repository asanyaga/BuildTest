Feature: Stockist Purchase Order
	In order to create a stockist purchase order
	As a user
	I want to be able to create a stockist purchase order
	Then i should be able  to create an order line item and add it to the stockist purchase order
	When the stockist purchase order has been confirmed
	And i have submitted the stockist purchase order to its respective workflow
	Then the stockist purchase order should be sent asynchronuously to the server
	And when the stockist purchase order is processed on the server, the purchase order should have the correct sale amounts

@documenthook
Scenario: Create a stockist purchase order
	Given I create a stockist purchase order
	And I create a stockist purchase order line item
	When I submit the stockist purchase order to its respective workflow
	Then there should be a saved purchase stockist order
	And the stockist purchase order should have a line item
	And there should be a corresponding stockist purchase order command envelope on the outgoing command queue

@documenthook
Scenario: Create a server stockist purchase order
	Given I create a stockist purchase order [server]
	And I create a stockist purchase order line item [server]
	When I submit the stockist purchase order to its respective workflow [server]
	And I trigger a server sync from hub ( stockist purchase order) [server]
	Then There should be a saved stockist purchase order on the server [server]
	And the amount of the stockist purchase order line item should be the same as in the hub [server]
	When I approve the stockist purchase order in the client application [server]
	And I trigger a server sync (stockist purchase order) in the client application
	Then I should be able to fetch the stockist purchase order in server with a status of approved