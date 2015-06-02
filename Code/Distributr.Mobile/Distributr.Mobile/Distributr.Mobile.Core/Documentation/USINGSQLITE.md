## Using SQLite Effectively

Apart from network IO, the biggest bottleneck in an application is usually the database or file I/O. The runtime and compiler can do magic with poorly written code, but databases require a bit more effort from the developer. This section serves as guide of how to use SQLite effectively on resource-limited mobile applications. 

#### Threading
Older versions of SQLite did not have full multi-threading support, leaving that down to the developer. For the Android OS versions that we are targeting, they all have SQLite versions that support multi-threading. As such, SQLite will be used in multi-threaded mode. That is, multiple threads can share a single connection safely.

#### Transactions
As well as for consistency (see above), transactions should be used for performance reasons. This is particularly important for bulk inserts such as applying a full Master Data download. For every `INSERT`, SQLite rebuilds any indices on the table each time. However, if you use a transaction, the indices are only rebuilt once on commit. Using a transaction can reduce 15 seconds worth of `INSERT`s down to less than one second.

#### Pagination & Limiting Result Sets
SQLite supports pagination by allowing you to specify a `LIMIT` at the end of your query. Pagination will be used for various lists of items in the app, rather than loading all data, which can lead to the app running out of memory or a sluggish UI. 

`LIMIT` can be used in the following ways:

//Limit Only. Limit results to a maximum of 50 records

`LIMIT 50`

//Offset and Limit. Limit results to a maximum of 50 records, starting from record 100

`LIMIT 100, 50`

Comments:

1. You should **always use ORDER BY with LIMIT**. Otherwise your data will be random. 
2. When using the offset (second example above) you **should always be querying on an index**. The way offset works is by processing each row and then discarding them until it reaches the row number you specified as the offset. This means that **it scales very poorly**. An offset of 1000000 would read and discard one million rows. Using an index avoids this problem and means you will get O(log n) performance instead of O(n) performance. One handy indexed column, which can be used for insertion order (which will probably be the most common `ORDER BY` in the app) is `ROWID` - a special column that SQLite adds to tables. It is a 64-bit integer and so makes a good index value.  

The Android Framework in Mobile.Common already provides a general purpose `FixedSizeListAdapter` class that handles pagination for you. It will be documented as part of the framework but a brief outline is as follows

- Supports an unlimited amount of records, adding and removing records depending on the Lists scroll position (ie the Infinite Scrolling pattern like you have on the Twitter app)
- Pre-loads next and previous pages on a background thread. This keeps the UI responsive.
- Only ever holds a maximum of 200 records in memory: 100 in the List and 50 for the previous and next page, as required. It is easy to tweak these numbers but they should be sensible defaults for all devices. 
- Maintains the precise scroll position in the List after modification (which Android annoyingly resets when you remove items)
- Enforces the always use `ORDER BY` constraint mentioned above. 
- Adds the LIMIT and OFFSET automatically to the query you provide
- Can be re-initialised with a new query (when the user applies sorting or filters)

The `FixedSizeListAdapter` should be used for any lists that can have potentially large (ie wont all fit in memory in one go) amounts of data. 

#### ORM (SQLite.Net.Extensions)

SQLiteExtensions is a set of C# Extensions methods that bring more ORM-Like capabilities to SQLite.Net. They fix an issue present in the old Android code where by there is one query per table when building up a complex object that is spread across several tables. These extensions issue one query for the complete object graph. However, **be careful with this feature**. When displaying large lists, you should query only the columns required, creating an new class if necessary to hold these columns, rather than bring back a complete object graph of 20+ fields when you only require three of them. 

#### Documentation

- [SQLite.NET PCL](https://github.com/oysteinkrog/SQLite.Net-PCL)
- [SQLite.NET.Extensions](https://bitbucket.org/twincoders/sqlite-net-extensions)
