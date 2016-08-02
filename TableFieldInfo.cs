using System.Reflection;

namespace SQLDM
{
    internal class TablePropertyInfo
    {
        public string SchemaName { get; set; }
        public string TableName { get; set; }
        public PropertyInfo PropertyInfo { get; set; }
    }
}
