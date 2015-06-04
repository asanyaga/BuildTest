
# Mobile Common Documentation

The Mobile Common project is a thin layer that sits on top of the Android Framework. It's goal is to make life easier for future Virtual City projects and reduce the massive amount of boiler plate code that you need to write to do even trivial tasks in Android. 

You should try to add code to the Mobile Common project first before adding it to the Distributr.Mobile. This way the code can be reused in other Android projects. 

The key features that it provides are as follows

* Resolving dependencies required by the App
* Automatically setting Fragment layout and menu resources file based on a naming convention
* Automatically calling base methods that are required by the Android Framework 
* Storing Objects between screen changes
* Monitoring network status for services that require network access
* An Event Bus that allows background services to update the UI
* Various UI features, such as Tabs, Search, Floating Action Button, Side Panel Navigation
* Fixed Size list adapter with infinite scroll capability
* Fragment back-stack management


## Feature Documentation

Most of the features listed above are provided by base classes that extend the main android component types such as BaseFragment, BaseActivity and BaseIntentService. 

### Resolving Dependencies

The Android project Distributr.Mobile is kept as light as possible. It delegates to Distributr.Mobile.Core which contains the core business logic for the app. When you need to access something from Distributr.Mobile.Core from Distributr.Mobile you can can call Resolve like so

```c#
var myClass = Resolve<IMyClass>();
```

The `Resolve` method is available in the following classes

* BaseActivity
* BaseFragment
* BaseIntentService 

The dependencies that you want to resolve must have been registered with the container. For full detail on this see TODO

### Automatically Setting Resource Files

When you create a new Fragment you will also need to provide a layout resource file, unless you are inheriting from a base class that provides its own resources. You may optionally create a menu resource file if this is required by your Fragment. The framework will automatically set this for you if you follow the naming convention. 

Fragments names should end in *Fragment e.g. `OutletFragment` and resource files should match the name of the Fragment, while also following Android's naming convention of lowercase and underscores e.g   `outlet_fragment.axml`. If you follow this convention, the framework will automatically call `SetLayout(Resource.Layout.outlet_fragment);` for you when OutletFragment is created.  If you provide a menu, you should use the same name for your menu resource file e.g. `menu/outlet_fragment.xml` to have this automatically set by the framework. 

This also works with inheritance. If you were to extend OutletFragment e.g. `AnotherOutletFragment`, you can either provide a new layout called `another_outlet_fragment.axml` or inherit the layout `outlet_fragment.axml` from the base class. The framework will look for the most specific layout first (another_outlet_fragment.axml) and if this layout is not found, it will try the base class (outlet_fragment.axml). 

### Automatically Calling Base Methods for Android Components

One of the annoying things about the Android Framework is the need to always call the base classes method first when overriding it in in your code. When you don't do this you get a runtime exception and it is not always obvious what the cause of this exception is. Given that calling the base class must always happen, it should have been taken care of by the Android developers. The base classes in Mobile.Common do this automatically for the main lifecycle methods on Fragments and Activities.

Standard way of doing things with Android Framework
```c# 
     protected override void OnPause()
        {
            base.OnPause(); // Call OnPause in Android framework first - or runtime exception
            //Do work required by your app
        }
```

When using a component in mobile common, you do not override `OnPaused()`, but instead override `Paused()`. The component, e.g. BaseActivity will automatically call `base.OnPaused()` for you before calling `Paused()`. 

A full list of overridden methods is as follows

| Android        | Mobile Common Equivalent|
| ------------- |:-------------:|
| OnCreated      | Created |
| OnPaused      | Paused      |
| OnResumed | Resumed      |
| OnStarted | Started      |
| OnStopped | Stopped      |

See [BaseActivity](../../Mobile.Common/Core/BaseActivity.cs) and [BaseFragment](../../Mobile.Common/Core/BaseFragment.cs) for more details. 

### Storing Object Between Screen Changes

Android does not allow you to easily pass objects between Activities or Fragments during a screen change. You can only provide primitive values in Intents and you can not use any other constructor other than the default no-args constructor. The framework in Mobile.Common allows you to store complex objects between screen changes by calling `App.Put(myObject)` and `App.Get<MyObject>()`. These objects are stored by the `BaseApplication` class which is available to all components, and is equivalent to a session cache in a web framework. To avoid leaking memory the BaseApplication class will only allow one copy of of a given type to be stored at any one time. If you call App.Put twice with the same object type, the second call will overwrite the value set by the first. You can see an example of Put/Get in [OutletFragment](../../Distributr.Mobile/Outlets/OutletFragment.cs). 

*Do not call Put/Get from a background service*. This feature is intended for use only by code running on the Main UI thread. We don't want multiple threads reading and writing to this storage which will cause problems and make the state difficult to reason about. 

### Monitoring Network Status 

Most background services will be dependent on a network connection so that they can either pull or push data to the server. Mobile.Common contains code that can tell you the current state of the network and also put the calling thread to sleep until the network comes available. When the network is unavailable, the framework registers a `BroadcastReceiver` with the Android framework so that it can receive events about the state of the network. Once it receives an event to say the network is available, it wakes up any waiting threads. It also unregisters the `BroadcastReceiver` to avoid wasting resources. 

The class that contains most of the code is [ConnectivityMonitor](../../Mobile.Common/Core/Net/ConnectivityMonitor.cs) however, you don't need to use this class directly. Instead you can either extend [NetworkAwareSyncService](../Sync/NetworkAwareSyncService.cs) in Distributr.Mobile.Core or you can use the [BaseApplication](../../Mobile.Common/Core/BaseApplication.cs) class which will call `ConnectivityMonitor` for you. The general pattern for using this code is as follows

```c#
while (hasWorkToDo)
  {
      try
      {
          // This method will block if the network is currently unavailable
          connectivityMonitor.WaitForNetwork();
          DoSomeWork();
      }
      catch (Exception e)
      {
          if (!connectivityMonitor.IsNetworkAvailable())
          {
              // 1) Do not throw exception or perform any application error handling
              // 2) Move to top of loop i.e. block until network becomes available
              continue;
          }
          //Not a network error - so need to handle this
          throw e;
      }
 }
```

To best understand this code you should check [NetworkAwareSyncService]../Sync/NetworkAwareSyncService.cs) and one of its decendants such as [CommandEnvelopeUploader](../Sync/Outgoing/CommandEnvelopeUploader.cs). 

To understand what `ConnectivityMonitor` is doing, you can follow [this link](http://developer.android.com/training/monitoring-device-state/connectivity-monitoring.html) to the Android documentation. 

## Event Bus 

The BaseApplication class contains an EventBus which can be used to send event messages to/from the following framework components

* BaseApplication
* BaseActivity
* BaseFragment
* BaseIntentService

The main use for this is to be able to provide updates from background services that are reflected in the UI. The above components are automatically registered and unregistered on the EventBus. If you wish to subscribe to a particular event you should define an `OnEvent(EventType event)` method in your class. The EventType must be the only parameter of the OnEvent method; it can be any type of Object. The [LoginActivity](../../Distributr.Mobile/Login/LoginActivity.cs) serves as an example as this. When a User logs in for the first time the application downloads their Master Data. The background service `MasterDataDownloadService` publishes events on the EventBus to reflect the progress of the Master Data Download. All sync services use the following events

* SyncUpdateEvent<T>
* SyncPausedEvent<T>
* SyncFailedEvent<T>
* SyncCompletedEvent<T>

By subscribing to these events you can provide feedback to the User about the current state of a given task. For example, to subscribe to a Sync Completed event for Master Data you would add the following code to you class

```c#
 public void OnEvent(SyncCompletedEvent<MasterDataUpdate> completed)
 {
     //Do work required once sync is completed, such as updating the UI and moving to the next screen
 }
```

[NetworkAwareSyncService](../Sync/NetworkAwareSyncService.cs) automatically publishes events for you during processing. 

##UI Features

Mobile.Common also provides support for a number of UI features which are labelled on the following screens:

Screen 1:
![alt text](Component%20Overview%201.png "Screen 1 ")

Screen 2:
![alt text](Component%20Overivew%202.png "Screen 2")


###Header View
The Header is the top part of the app that contains the toolbar, tabs and search. If you want to display an additional layout as part of the Header View than you can do this by calling `AddHeaderView(View yourView)` from inside of your Fragment. The AddHeaderView method is part of BaseFragment. Header views are automatically removed when the user switches to a new screen. 

###Tabs
If your Fragment needs to display tabs then you can extend TabbedFragment instead of BaseFragment. The only difference between these two classes is that TabbedFragment adds and removes tabs depending on the fragment’s visibility. TabbedFragment extends BaseFragment. 

In your TabbedFragment, you can add tabs to be displayed as follows:

```c#
//Add a Tab with the following name
AddTab(new TabModel(Resources.GetString(Resource.String.cash))
      {
          //Provide an action to be performed when the tab is selected, in this case show a fragment
          //but in other cases it might be to update the existing fragment with the result of a new SQL query
          OnTabSelected = () => { ShowNestedFragment(viewId, receiveCashFragment); }
      }
 );
```

When using tabs, you usually show a `NestedFragmment`. A nested fragment is one that can be displayed inside of another fragment. When you switch between screens, the framework in Mobile.Common resets state (remove tabs and search widget, for example) so that the new screen has a clean environment that it can configure itself. For NestedFragments the framework does not reset the environment, so they inherit the same environment as their parents. 

##FAB - Floating Action Button

The so-called Floating Action Button is becoming more common in Android applications. You can use this on screens that provide a single action such as confirming a payment. You can show or hide the FAB based on the current screen state. For example, if the User has not entered all required information then hide the button and show it only when the input is valid. It is usually shown in the bottom right hand corner of the screen (to make it easier for 90% of the World's population who are right-handed). 

By default this button is hidden, but you can use it in your Fragment by adding the following code:

```c#
//Pass in a resource ID that points to the image icon that you want to display
SetupFab(Resource.Drawable.ic_check);

//Call this to make the FAB visible
ShowFab();

//Call this to make the FAB invisible
HideFab();

...

protected override void OnFabClicked()
{
     //Override this method in your Fragment to handle FAB clicks
}
```

The FAB is automatically hidden again when the User switches screens. 

##Search Widget

Like the FAB above, you can enable Search in your fragment in the following way

```c#
//Tell the framework to show he Search Widget in the header
SetupSearch();

...
protected override void OnSearch(string text)
{
  //Override this method in your Fragment to handle search queries
}
```

As with the other features, the Search Widget is automatically hidden when the User switches screens. 

##Fixed Size List Adapter 

For any lists that appear in the app that are backed by tables that potentially contain a large amount of data, such a products, you should use the [FixedSizeListAdapter](../../Mobile.Common/Core/Views/FixedSizeListAdapter.cs). You simply extend this adapter like you would with one of Android's built-in adapters, such as `ArrayAdapter`. This adapter will fetch enough data to fill the list/screen and also load previous and next pages, if any, on a background thread. As the User scrolls through the list, the adapter will add and remove items as necessary. This avoids running out of memory due to trying to load too much data in one go. 

You can see this feature in action in the [ProductFragment](../../Distributr.Mobile/Products/ProductFragment.cs) class. 

Any queries that you use with this adapter must specify an `ORDER BY` clause. The Adapter adds the correct `LIMIT` and `OFFSET` values for you, depending on the User's scroll position. 

You use this adapter as follows

```c#
//product list adapter extends FixedSizeListAdapter
var productListAdapter = new ProductsListAdapter<Product>(Activity);

//Initialise with a data source
productListAdapter.Initialise(new SqliteDataSource<P>(Resolve<Database>(), query));
```

In the above example, query is the following SQL

```sql

 public static string AllProductsByNameAscending
   {
       get
       {
           return @" SELECT 
                     SaleProduct.Description, SaleProduct.MasterId
                   FROM 
                     SaleProduct
                   ORDER BY
                     SaleProduct.Description";
       }
   }
```

We only load two columns from SaleProduct here, rather than the whole row or complete object graph. This helps keep the UI responsive. When the user selects a product from the list we then load the entire product which includes information about returnables, prices and VAT etc. 

### Fragment Back Stack Management

The App UI is built on Fragments rather than Activities which are heavyweight and difficult to reuse. As the User navigates from screen-to-screen, you call `BaseActivity.Show(Type fragmentTypeToShow)` which is implemented by the [FragmentHostActivity](../../Mobile.Common/Core/FragmentHostActivity.cs) - the main Activity used in the app, which displays the Fragments that make up the current screen. When the User moves through the App Fragments are stacked on top of each other. When the User clicks the Back button, the last Fragment is removed from the stack, revealing the previous fragment. When the back stack contains only a single Fragment, the logout dialog is shown. 

Nested Fragments, Fragments that are displayed inside other Fragments are not added to the back stack (for these you call `ShowNestedFragment(containerId, fragment)` on `BaseFragment`. 

You shouldn't need to add any special code to handle navigation. However, you might need to jump back more than one fragment in some cases. To do this you can call  `GoBackTo(Type fragmentToGoBackTo)` on `BaseFragment`, which will call FragmentHostActivity and tell it to keep removing Fragments from the stack until it reaches `fragmentToGoBackTo`. 














