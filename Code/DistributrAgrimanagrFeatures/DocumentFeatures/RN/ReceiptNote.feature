Feature: ReceiptNote
	In order to acknowledge receipt of a payment
	As a user
	I want to be able to generate a receipt note
	Then i should be able  to create a receipt line item and add it to the receipt note
	When the receipt has been confirmed
	And i have submitted the receipt note to its respective workflow
	Then the receipt should be sent asynchronuously to the server
	And when the receipt is processed on the server, the receipt note should have the correct payment amounts

@documenthook
Scenario: Create a local receipt
	Given I create a receipt note
	And I create a receipt line item
	When I submit the receipt to its respective workflow
	Then there should be a saved receipt note
	And the receipt note should have a line item
	And there should be a corresponding receipt note command envelope on the outgoing command queue

@documenthook
Scenario: Create a server receipt
	Given I create a receipt note [server]
	And I create a receipt line item [server]
	When I submit the receipt to its respective workflow [server]
	And I trigger a server sync [server]
	Then There should be a saved receipt note on the server
	And the receipt amount in should be the same as in the hub
