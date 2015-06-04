Distributr
=========================

This project contains the source code for the new version of Distributr - a Xamarin.Android application. It uses a copy of the Distributr Core Shared code which is currently based on Subversion revision 3136 (hosted on Assembla).

This project will target Android versions 15 (Ice Cream Sandwich) and above. It will  use the latest SDK (API 21) and so will be Lollipop ready. 

Project Structure
=================
### Top-Level Projects

| Name | Description |
|------|-------------|
|Distributr.Core.Shared|The shared code library which has been generated from the existing code base used by the Server, HQ and Hub. Entities, some DTOs, and the workflows are being reused in the app|
|Distributr.Mobile| The new app project root. This project is further broken down into sub-projects to allow better reuse across apps and workaround some limitations with Xamarin. See below.|

### Distrubr.Mobile Sub-Projects

| Name | Type | Project Dependencies | Description |
|------|------|----------------------|-------------|
|Distributr.Mobile| Xamarin.Android Application|Distributr.Core.Shared, Distributr.Mobile.Core, Mobile.Common| This is the main project for the app which is built into the final APK file. Xamarin does not provide good support for testing Android code, as a result, this project should be kept as light as possible with the main business logic residing in Distributr.Mobile.Core instead|
|Distributr.Mobile.Core| C# Shared Code Library| Distributr.Core.Shared | This is where most of the app business logic, specific to Distributr, is contained. Keeping code in this project means it an be tested easier using standard C# methods and libraries|
|Mobile.Common| Xamarin.Android Shared Library| None.| This project contains reusable Android code and an Android framework that makes common tasks simpler. Before adding code to the Distributr.Mobile project you should look to add it here if is common enough to be reused|
|Distributr.Mobile.Core.Test| NUnit Library Project| Distributr.Mobile.Core, Distributr.Core.Shared| This project tests the code in Distributr.Mobile.Core|

Project Documentation
=====================
* [System Overview](Distributr.Mobile/Distributr.Mobile.Core/Documentation/SYSTEMOVERVIEW.md)
* [Server API](Distributr.Mobile/Distributr.Mobile.Core/Documentation/SERVERAPI.md)
* [Using the Android Framework inside Mobile.Common](Distributr.Mobile/Distributr.Mobile.Core/Documentation/MOBILECOMMON.md)
* [Coding Guidelines](Distributr.Mobile/Distributr.Mobile.Core/Documentation/CODINGGUIDELINES.MD)
* [Using SQLite Effectively](Distributr.Mobile/Distributr.Mobile.Core/Documentation/USINGSQLITE.md)
* [Adding a New Feature](Distributr.Mobile/Distributr.Mobile.Core/Documentation/NEWFEATURE.md)
* [Testing](Distributr.Mobile/Distributr.Mobile.Core/Documentation/TESTING.md)
* [Tools](Distributr.Mobile/Distributr.Mobile.Core/Documentation/TOOLS.md)
* [Definition of Done](Distributr.Mobile/Distributr.Mobile.Core/Documentation/Documentation/DOD.md)
* [Release Notes](Distributr.Mobile/Distributr.Mobile.Core/Documentation/RELEASENOTES.md)


