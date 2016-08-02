using System;
using System.Data;
using System.Reflection;

namespace SQLDM
{
    public class DataLink
    {

        public DataLink(DataRecord parent, DataField parentLinkField, Type childType, string childField)
        {
            ParentRecord = parent;
            ParentField = parentLinkField;
            ChildRecordType = childType;
            ChildFieldName = childField;
            Validate();
        }

        public DataRecord ParentRecord { get; set; }
        public DataField ParentField { get; set; }
        public Type ChildRecordType { get; set; }
        public string ChildFieldName { get; set; }

        private void Validate()
        {
            foreach(DataRecordTypeAssembly t in SQLDMGlobal.DataRecordTypes)
            {
                if (t.DataRecordType == ChildRecordType)
                {
                    foreach(MemberInfo m in t.DataRecordType.GetMembers())
                    {
                        if (m.Name == ChildFieldName) return;
                    }
                }
            }

            throw new DataException("Invalid Link Data");
        }

    }
}
