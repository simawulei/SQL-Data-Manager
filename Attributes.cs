using System;

namespace SQLDM
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ReadOnly : Attribute { }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class NotDatabaseField : Attribute { }

}
