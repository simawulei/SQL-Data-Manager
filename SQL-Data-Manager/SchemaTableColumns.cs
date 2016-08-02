using System.Collections.Generic;
using System.Data.SqlClient;

namespace SQLDM
{
    class SchemaTableColumns : DataLoader<SchemaTableColumns>
    {

        public string SchemaName;
        public string TableName;
        public string ColumnName;

        public override bool IsVirtualTable { get { return true; } }

        public static List<SchemaTableColumns> GetPrimaryKeys()
        {
            return GetRecords(@"
                                select
	                                tc.TABLE_SCHEMA SchemaName
                                    , ku.table_name as TableName
	                                , column_name as ColumnName
                                from
	                                INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS tc
	                                inner join INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS ku ON tc.CONSTRAINT_TYPE = 'PRIMARY KEY' and tc.CONSTRAINT_NAME = ku.CONSTRAINT_NAME
                        ", new List<SqlParameter>(), false);
        }

        public static List<SchemaTableColumns> GetIdentityColumns()
        {
            return GetRecords(@"
                                select
	                                s.name SchemaName
	                                , t.name TableName
	                                , c.name ColumnName
                                from
	                                sys.columns c
	                                inner join sys.tables t on t.object_id = c.object_id
	                                inner join sys.schemas s on s.schema_id = t.schema_id
                                where
	                                is_identity = 1
                        ", new List<SqlParameter>(), false);
        }

        public static List<SchemaTableColumns> GetNullableColumns()
        {
            return GetRecords(@"
                                select
	                                s.name SchemaName
	                                , t.name TableName
	                                , c.name ColumnName
                                from
	                                sys.columns c
	                                inner join sys.tables t on t.object_id = c.object_id
	                                inner join sys.schemas s on s.schema_id = t.schema_id
                                where
	                                is_nullable = 1
                        ", new List<SqlParameter>(), false);
        }

    }
}
