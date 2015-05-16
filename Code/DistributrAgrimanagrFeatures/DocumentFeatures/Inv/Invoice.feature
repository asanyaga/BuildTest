Feature: Invoice
	In order to acknowledge a sale
	As a user
	I want to be able to generate an invoice
	Then i should be able  to create a invoice line item and add it to the invoice
	When the invoice has been confirmed
	And i have submitted the invoice to its respective workflow
	Then the invoice should be sent asynchronuously to the server
	And when the invoice is processed on the server, the invoice should have the correct sale amounts

@documenthook
Scenario: Create a local invoice
	Given I create an invoice
	And I create an invoice line item
	When I submit the invoice to its respective workflow
	Then there should be a saved invoice
	And the invoice should have a line item
	And there should be a corresponding invoice command envelope on the outgoing command queue

@documenthook
Scenario: Create a server invoice
	Given I create an invoice [server]
	And I create an invoice line item [server]
	When I submit the invoice to its respective workflow [server]
	And I trigger a server sync from hub [server]
	Then There should be a saved invoice on the server
	And the sale amount in should be the same as in the hub
