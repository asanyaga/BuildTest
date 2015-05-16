Feature: PaymentNote
	In order to make a payment in a costcentre
	As a user
	I want to be able to generate a payment note
	Then i should be able to create a payment line item  and add it to the payment note
	And when the payment note in the cost centre has been confirmed
	Then the payment note should be sent asynchronuously to the server
	And after the document is processed on the server the payment note should be on the server with the correct payment amounts

@documenthook
Scenario: Create local payment
	Given I generate a payment note
	And I add a payment line item to the payment
	When I submit the payment to its workflow
	Then there should be a saved payment note
	And the payment note should have a line item
	And there should be a corresponding payment note command envelop on the outgoing command queue

@documenthook
Scenario: Create local payment and sync to the server
	Given I generate a payment note [server]
	And I add a payment line item to the payment [server]
	When I submit the payment to its workflow [server]
	And I trigger a sync to the server
	Then There should be saved payment note on the server
	And The line item amount should be the same as in the hub
