
Coding Guidelines
=================

These guidelines should be followed when adding new code to the project. They should also be used during code reviews. 

In a nutshell, the guidelines are as follows

* Code should be easy to read
* Code should not contain duplication
* Code should be tested


### General Stuff
- _DRY_ (Don't Repeat Yourself). Avoid duplication. The project is already well set up to share code so there's no excuse for the blatant copy/pastes in the existing Java Android project. Duplication means that refactoring takes longer, and that bugs also get duplicated, and that [performance issues](http://programmer.97things.oreilly.com/wiki/index.php/WET_Dilutes_Performance_Bottlenecks) become diluted.

- Keep everything as loosely-coupled as possible. The Android Framework in Mobile.Common provides a simple EventBus (ie the Observer pattern) which is used for inter-component-communication. Components don't have to depend on each other directly and instead send messages (events) to the Bus where they are delivered to interested parties (other components which have registered with the Bus to received certain event types). This has two nice benefits: it means that components can be changed in isolation and it also means that components can be tested eaiser (it's easy to listen for or send dummy events during tests). Components in the Android framework are automatically registered with the Bus when they define `OnEvent(EventType event)` methods.

- Favour immutable objects where possible. Immutable objects can be safely passed around in a multi-threaded environment. They are also easier to reason about. At the very least, all dependencies should go through the constructor - that is: the object should be in a usable state once the constructor returns. One exception is for entities - SQLite requires a default no-args constructor (which sucks!).

- Follow the [Single Responsibility Principle](http://en.wikipedia.org/wiki/Single_responsibility_principle). Give every class a single, clear responsibility which is obvious from the class name. We don't want any of those massive classes full of spaghetti code like exists in the Java Android projects. In the existing Java Android code there are some classes that contain logic for several independent features. Add the fact that variable naming was extremely poor and it makes it impossible to see where one feature starts and the other ends. 

- [Package By Feature, Not Layer](http://www.javapractices.com/topic/TopicAction.do?Id=205). Or put another way, code that works together should live together. Don't put all services in a folder called Services or all repositories in a folder Repositories, ie package by layer. Instead have a top-level folder per feature e.g. MakeSale, which then contains MakeSaleService and MakeSaleRepository. It is *much* easier to read and understand a project organised in this way. It also organises the project in a modular way making it easier to break-out features into separate libraries in future.

- Write Self-Documenting Code. Don't add unnecessary comments in the code. Comments quickly fall out of sync with what the actual code does. Also, they often don't tell you anything that you can't already see from the code (providing it is well written) and so are a form of duplication.  Comments should reserved for exceptional cases (or libraries and frameworks). If you follow the Single Responsibility Principle, and keep your classes and methods small, and give everything a meaningful name, and Package By Feature, it should be easy to understand the code. Try to avoid acronyms and short meaningless names when naming variables, and also don't just use the type e.g `List<User> list = ...` should be `List<User> allUsers = ...`. Naming is a *major* issue in the old code base. 

- Identify Atomic Operations and ensure they are wrapped in a transaction. This is a general remark that is particularly important when using SQLite. The existing Java Android code does not use transactions. One example is when creating a new Outlet and Outlet Contact. The steps are as follows:
  1. `Save Outlet in Database` 
  2. `Save CreateOutletCommand (that will be sent to the server) in Database`
  3. `Save Outlet Contact in Database`
  4. `Save CreateOutletContactCommand (that will be sent to the server) in Database`

 In the Java Android code base these operations are all independent. If one were to fail the system would be in an inconsistent state which could break the UI and even cause issues on the server. These four operations should be made atomic by wrapping them in a database transaction. Either they all succeed or none of them do. 
 
- Handle Exceptions where appropriate. The most obvious place is when dealing with the network. Also, writing messages to the SD Card or just logging them into a Black Hole is not proper error handling. For user-actioned operations that lead to Exceptions, the user should be made aware of the issue and given an option to retry if appropriate. Exceptions will be stored by the framework (an interceptor will be added to the EventBus that listens for Error events and saves them in the Database on a background thread). Later versions of Distributr might send errors to the server automatically or give the user an option to send them when reporting an issue - this still needs to be discussed along with a wider-monitoring solution. 
