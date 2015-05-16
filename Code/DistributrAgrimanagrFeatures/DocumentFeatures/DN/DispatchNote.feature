Feature: DispatchNote
	In order to dispatch products from a costcentre
	As a user
	I want to be able to dispatch stock from the costcentre client application using a Dispatch Note
    And when the Dispatch Note on the costcentre client application is confirmed
    Then DN document should be sent asynchronously to the server 
    And after the document is processed on the server the document should be on the server and the products  should be dispatched


@documenthook
Scenario: Dispatch local product using DN
	Given I have a product on the hub 
	And I create an DN to dispatch 20 products
	When I submit the DN to its respective workflow	
	Then there should be a saved DN document
	And there should be a corresponding command envelope on the outgoing command queue of the dn


@documenthook
Scenario: Dispatching local product  using DN causes products dispatched  to be synced to server
	Given  I have a product on the hub  [server]
	And I create an DN to dispatch 20 products [server]
	When I submit the DN to its respective workflow[server]
	And I trigger a server sync for DN
	Then there should be a saved DN documenton the server
	