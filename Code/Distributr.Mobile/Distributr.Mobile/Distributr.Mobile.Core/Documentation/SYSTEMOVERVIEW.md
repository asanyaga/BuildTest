
System Overview
=========================

The two images below show how the projects are built for the production and test environments. In production, we use the real web server and an SQLite instance on the Android filesystem. In test we use a fake in-process web server that imitates the real server and also an SQLite instance that is on our local (Windows) filesystem. In both deployments, Distributr.Mobile.Core is the same. 

![alt text](https://github.com/nutshellit/VirtualCity-Distributr/blob/master/Distributr.Mobile/Distributr.Mobile.Core/Documentation/System%20Overview.png "Production and Test overview")

Most of what the application does can be grouped into two types of component: background and foreground. Both of these component types depend on SQLite and Distributr.Mobile.Core to do their work, but they don't know anything about each other. A simplified view is shown below. The foreground services, which follow Android's standard model-view-controller pattern, mainly do CRUD stuff - reading and writing to a local database. Fragments and Activities are really just view controllers that delegate to Distributr.Mobile.Core to do any work e.g. generating command envelopes to be sent to the server. The envelopes are stored in SQLite which is then read by the background services which forward them to the server. Background services can publish events about their processing status but they do not know who is receiving these events. Background services fetch new data from the server which is stored in SQLite and read by the foreground services and displayed in the UI. 

![alt text](https://github.com/nutshellit/VirtualCity-Distributr/blob/master/Distributr.Mobile/Distributr.Mobile.Core/Documentation/background_foreground.png "Background and Foreground services")
