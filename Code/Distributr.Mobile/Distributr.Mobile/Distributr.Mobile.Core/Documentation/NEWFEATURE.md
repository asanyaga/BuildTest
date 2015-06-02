Implementing a New Feature
=========================

As mentioned above, the project is packaged by feature, that is, there is one top-level folder for each notable feature in the app. The app is made up of multiple sub-projects and as such the code for each feature is spread across projects. 

| Project | Description |
|---------|-------------|
| Distributr.Mobile | This will generally only include code that depends on the Google Android libraries and nothing more, which will mainly be presentation logic. You should not add business logic here due to Xamarin's poor Android testing support. |
| Distributr.Mobile.Core | This is where your business logic should live. Presentation logic in Distributor.Mobile should delegate to the code you add here. |
| Distributr.Mobile.Core.Test | Add your tests to this project. Tests here will test code in the Distributor.Mobile.Core |

#### A Concrete Example

The existing code for the Login Feature serves as an example. 

##### Distributor.Mobile/Login
- LoginActivity - the Android ViewController for the Login Screen
- Settings/LoginSettingsActivity - the Android ViewController for the Login Settings Screen

(additional XML for layouts and Toolbar menus is found in Resources/Layout and Resources/Menu for this feature)

As stated above, we keep very little code in this part of the project. 

##### Distributor.Mobile.Core/Login
- ILoginClient - An interface for the Login Client that talks to the server
- ILoginRepository - An interface for the Login Repository that provides User data
- LastLoggedUser - An entity that tracks the last user who logged in
- LoginClient - A concrete implementation of ILoginClient that uses HTTP to call the server's login API
- LoginModule - Defines dependencies used by this module. These dependencies are registered with the Dependency Injection framework (Ninject). Each feature will have its own Module. 
- LoginRepository - A concrete implementation for the LoginRepository that provides User data from a SQLite database.
- LoginWorkflow - the class that perform login operations. *Workflow is already used as a convention in the Shared Code project so we use it here too. Each feature will have at least one of these. 
- User - A entity that represents a user in the system. It extends a user which is present in the Shared Code, adding CostCentreApplicationID as an additional field/column

The key points are:

1. Have one or more *Workflow objects per feature. (Generally only one, if you have two or more you probably have two or more features)
2. Have one *Module defining the dependencies used by the feature. 
3. Declare an interface for objects that you will need to mock in your tests. For example, you don't always want to call a network server during your test so you mock ILoginClient instead and provided a dummy result.
4. If you add additional entities then put them in the package that they are most closely linked to. 
5. This project should contain most of the code that you add

##### Distributor.Mobile.Core.Test/Login
- LoginWorkflowTest - Integration test - this will test all the scenarios supported by the LoginWorkflow ie the core logic of
the Login feature in this example.
- LoginRepositoryTest - Unit test persistence features offered by LoginRepository
- Settings/LoginSettingsRepositoryTest - Unit test persistence features offered by LoginSettingsRepository

Repository Unit Tests can extend BaseRepository which creates a new DB and cleans up when the test has finished.

#### Wrap Up

Following this coding structure you get something similar to what Uncle Bob calls a [Clean Architecture](http://blog.8thlight.com/uncle-bob/2012/08/13/the-clean-architecture.html), where the core of the application stands alone, and is fully testable without needing the UI, network or database. It can also be easily converted into a Desktop app or a Web app.

Using this blueprint you can easily add and test new features in an organised way. You don't need to know zillions of patterns or write much complex code (and most of the tricky stuff should be in the framework). You just need to follow the principles in the Coding Guidelines section and have a good understanding of the libraries and frameworks that you are using to get good, clean code that is easy to read, change and test. 
