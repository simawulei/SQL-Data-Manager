using System;
using System.Reflection;

namespace SQLDM
{
    class DataRecordTypeAssembly
    {

        public Type DataRecordType { get; set; }
        public Assembly Assembly { get; set; }

        public static DataRecordTypeAssembly Find(string typeFullName)
        {
            foreach(DataRecordTypeAssembly d in SQLDMGlobal.DataRecordTypes)
            {
                if (d.DataRecordType.FullName == typeFullName) { return d; }
            }

            return null;
        }
    }
}
