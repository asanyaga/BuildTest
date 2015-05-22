Feature: Commodity Storage Note
	In order to store a commodity
	As a user
	I want to be able to generate a commodity storage note
	Then i should be able  to create a commodity storage line item and add it to the credit note
	When the commodity storage note has been confirmed
	And i have submitted the commodity storage note to its respective workflow
	Then the commodity storage note should be sent asynchronuously to the server
	And when the commodity storage note is processed on the server, the commodity storage note should have the correct sale amounts

@documenthook
Scenario: Create a local commodity storage note
	Given I create a commodity storage note
	And I create a commodity storage line item
	When I submit the commodity storage note to its respective workflow
	Then there should be a saved commodity storage note
	And the commodity storage note should have a line item
	And there should be a corresponding commodity storage note command envelope on the outgoing command queue
