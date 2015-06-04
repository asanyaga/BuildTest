Release Notes
==============

### Version 0.1 

APK File [Distributr.v0.1.apk](https://github.com/nutshellit/VirtualCity-Distributr/blob/master/Distributr.Mobile/Distributr.Mobile/Releases/Distributr.v0.1.apk)

Delivery Date **31st May 2015**

Android Versions Supported **JellyBean+**

#### Version 0.1 Feature List 

#####Login
- User can login to system with valid account
- Performs remote login for new user
- Downloads master data for new user
- Allows offline login for existing user
- Reports error for invalid username or password or when server unreachable (remote login only)
- Allows user to change server URL

#####Orders
- Make an order for items in product list
- Allow full or partial payment(s) by cheque or cash
- Sends order to Hub including item returnables
- User can sort product list by name
- User can search for product by name
- Loads a fixed amount of products to avoid exhausting memory (updating list as user scrolls back and forth)

#####Deliveries
- Deliver orders which have been approved and dispatched by Hub
- Optionally sell returnable items
- Allow full or partial payment(s) by cheque or cash
- Issue credit note for unsold returnable items

#####Sale
- Sell items from inventory
- Optionally sell returnable items
- Allow full or partial payment(s) by cheque or cash
- User can sort product list by name, or stock count
- User can search for inventory item by name
- Loads a fixed amount of products to avoid exhausting memory (updating list as user scrolls back and forth)

#####Hub Actions
- Hub can add item to order submitted by device
- Hub can delete item to order submitted by device
- Hub can change quantity of item to order submitted by device
- Hub can create order for device user, which can then be delivered by device user (once approved and dispatched)
- Hub can create a sale for device user (device receives and processes inventory adjustment note when sale is confirmed)
- Hub can reject order (Note: nothing is sent from the Hub for a rejection. However, when building a new DB from envelopes the order is included and shown in order list as rejected)

#####Inventory
- User can receive inventory as part of master data update
- User can receive arbitrary inventory for sale transactions 
- User can receive fixed inventory for deliveries

#####Payments
- User can currently pay by cheque or cash when making an order, sale or delivery. M-Money will be delivered in next release
- User can send payments after submitting sale or delivery

#####Data Sync
- Master data is synced automatically on login or manually via sync tab in side panel (Note: new server API currently does not filter by time stamp - this will require an code change on the server side)
- Inbound transactions (approved orders, inventory transfer etc) are synced on demand via sync tab. 
- Outbound transactions are synced automatically after submitting a sale or order. 
- Sync UI shows status of sync tasks in real time, including when paused due to the network being unavailable. Errors communicating with server are shown in dialog.
- Errors encountered when  processing downloaded transactions are logged in error log screen
- Errors encountered when uploading transactions are logged in error log screen

#####Routes
- List routes by name ordered by priority
- List outlets by visit day ordered by priority

#####Outlets
- Perform actions for outlet including: make sale, make order, make delivery
- Get directions to outlet from current location or view on map (launches Google Maps with outlets coordinates)

#####Order History
- View orders in reverse chronological order
- Filter orders by status or date
- Loads a fixed amount of orders to avoid exhausting memory (updating list as user scrolls back and forth)

#####Side Panel
- Navigate through app via side panel
- View and control sync via side panel
- Settings tab (Nothing on here at the moment but user will change password here)

#####Error Log
- Log errors related to transactions
- Allow user to report error via email which is created automatically with address, subject and body populated. This can be improved so that it includes more details about the error, user and device state (disk space and last sync time etc)
- View error details in UI. Currently this info is technical only but can include more user friendly stuff as well as additional content (see line above)
- Replay all failed transactions: try to upload outgoing commands, try to reprocess downloaded commands
- Replay single transaction











