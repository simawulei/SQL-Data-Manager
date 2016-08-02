# SQL Data Manager
Lightweight database tool for data access &amp; manipulation using classes mapped to table in MS SQL Server

This is a class library that allows simple classes to be defined. Once defined, an instance can be created
by simply calling a GetRecord static method to pull in data from the database. GetRecords methods also
exist to return a List of records.

## Why

I wrote this initially as a proof-of-concept for VB. I liked the idea that EF made it simple to use classes
that mapped to database tables, but I didn't like the learning curve required. It worked well as a proof-of-concept
and it remained so until I thought it was about time to learn C#. So I used this project as a learning
platform by porting it from VB and making many improvements along the way.

It's still very much a work-in-progress, but I am starting to use it on my own projects, which is a great way
to facilitate developments.

## What this library will do

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

Primary keys are queried on startup and cached for future use.

Applications that use this library must call SQLDMGlobal.RegisterAssemblyForData on initial startup.
Environments (for database connection) must also be configured and can be done so manually or in a web.config file.

## Example class

A simple class example that will provide all the tools for loading and saving data might look like this:

    public class News : DataLoader<News>
    {

        public long NewsID { get; set; }
        public string Headline { get; set; }
        public string Image { get; set; }
        public DateTime ArticleDate { get; set; }
        public string Summary { get; set; }
        public string Story { get; set; }
       
    }

Note the inheritance - it must inherit from DataLoader<*class-name*>. Also note the types, which must match the
equivelant data type in SQL Server.

## Attributes
Attributes can also be applied. Currently, a class can be defined as ReadOnly, which will prevent saving of data
changes. Any property can be defined as NotDatabaseField, which will prevent the code from attempting to populated
it when loading data.
