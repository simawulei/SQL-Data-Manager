# SQL-Data-Manager
Lightweight database tool for data access &amp; manipulation using classes mapped to table in MS SQL Server

This is a class library that allows simple classes to be defined. Once defined, an instance can be created
by simply calling a GetRecord static method to pull in data from the database. GetRecords methods also
exist to return a List of records.

Multiple overloads exist of GetRecord and GetRecords to cover several scenarios. These can easily be called
from within classes to cover specific scenarios.

GetRecord and GetRecords methods all utilise GetDataTable and GetRecordDataRow methods which simply return
a DataTable or DataRow.

Where foreign keys are defined in the database, GetRecord and GetRecords methods can also pull in all child
records from the database. These will be stored i each instance under a ChildRecords list - which may also
contain ChildRecords. Currently, children are loaded on an all or nothing basis. Future developments will
allow defining of maximum child levels and specific child types to be loaded.

Save methods of a class instance will save changes or create new records in the database. Child records
loaded will also be save recursively.

