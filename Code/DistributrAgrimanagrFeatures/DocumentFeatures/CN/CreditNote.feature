Feature: Credit
	In order to acknowledge a sale on creadit
	As a user
	I want to be able to generate a credit note
	Then i should be able  to create a credit note line item and add it to the credit note
	When the credit note has been confirmed
	And i have submitted the credit note to its respective workflow
	Then the credit note should be sent asynchronuously to the server
	And when the credit note is processed on the server, the credit note should have the correct sale amounts

@documenthook
Scenario: Create a local credit note
	Given I create a credit note
	And I create a credit note line item
	When I submit the credit note to its respective workflow
	Then there should be a saved credit note
	And the credit note should have a line item
	And there should be a corresponding credit note command envelope on the outgoing command queue

@documenthook
Scenario: Create a server credit note
	Given I create a credit note [server]
	And I create a credit note line item [server]
	When I submit the credit note to its respective workflow [server]
	And I trigger a server sync (credit note) [server]
	Then There should be a saved credit note on the server
	And the sale amount for the credit note should be the same as in the hub
