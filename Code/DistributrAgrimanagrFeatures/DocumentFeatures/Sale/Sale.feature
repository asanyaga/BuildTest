Feature: Sale
	In order to do a sale
	As a user
	I want to be able to create an order of type sale
	Then i should be able to create an line item and add it to the sale document
	When the sale has been confirmed
	And i jave submitted the sale to its respective workflow
	Then the sale should be sent asynchronuously to ther server
	And when the sale is processed on the server, the sale should have the corrent payment amounts and line item quantities.

@documenthook
Scenario: Create a local sale
	Given I create an order of type sale
	And I create a line item
	When I submit the sale to its respective workflow
	Then there should be a saved sale
	And the sale document should have a line item
	And there should be a corresponding order of type sale command envelope on the outgoing command queue

	@documenthook
Scenario: Create a server sale
	Given I create an order of type sale [server]
	And I create a sale line item [server]
	When I submit the sale to its respective workflow [server]
	And I trigger a server sync from hub (sale) [server]
	Then There should be a saved sale on the server [server]
	And the amount of the sale line item should be the same as in the hub [server]
