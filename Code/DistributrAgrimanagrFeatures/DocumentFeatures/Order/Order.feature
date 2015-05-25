Feature: Order
	In order to create an order
	As a user
	I want to be able to create an order
	Then i should be able  to create an order line item and add it to the order
	When the purchase order has been confirmed
	And i have submitted the order to its respective workflow
	Then the order should be sent asynchronuously to the server
	And when the order is processed on the server, the order should have the correct sale amounts

@documenthook
Scenario: Create an order
	Given I create an order
	And I create an order line item
	When I submit the order to its respective workflow
	Then there should be a saved order
	And the order should have a line item
	And there should be a corresponding order command envelope on the outgoing command queue

@documenthook
Scenario: Create a server order
	Given I create an order [server]
	And I create an order line item [server]
	When I submit the order to its respective workflow [server]
	And I trigger a server sync from hub (order) [server]
	Then There should be a saved order on the server [server]
	And the amount of the order line item should be the same as in the hub [server]
	When I approve the order in the client application [server]
	And I trigger a server sync (order) in the client application
	Then I should be able to fetch the order in server with a status of approved