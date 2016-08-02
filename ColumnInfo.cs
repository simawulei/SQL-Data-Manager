using System.Collections.Generic;
using System.Data.SqlClient;

namespace SQLDM
{
    class ColumnInfo : DataLoader<ColumnInfo>
    {

        public string Schema { get; set; }
        public string Table { get; set; }
        public string Column { get; set; }
        public bool IsIdentity { get; set; }
        public bool IsNullable { get; set; }
        public bool IsPrimaryKey { get; set; }
        public string DataType { get; set; }
        public int? MaxLength { get; set; }

        [NotDatabaseField]
        public override bool IsVirtualTable { get { return true; } }

        public static List<ColumnInfo> GetAllColumnInfo()
        {
            return GetList(@"
                                select
	                                tc.TABLE_SCHEMA SchemaName
                                    , ku.table_name as TableName
	                                , column_name as PrimaryKey
	                                , ku.*
                                into
	                                #PrimaryKeys
                                from
	                                INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS tc
	                                inner join INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS ku ON tc.CONSTRAINT_TYPE = 'PRIMARY KEY' and tc.CONSTRAINT_NAME = ku.CONSTRAINT_NAME

                                select
	                                s.name [Schema]
	                                , t.name [Table]
	                                , c.name [Column]
	                                , c.is_identity IsIdentity
	                                , c.is_nullable IsNullable
	                                , cast(case when pk.PrimaryKey is null then 0 else 1 end as bit) IsPrimaryKey
	                                , ic.DATA_TYPE DataType
	                                , ic.CHARACTER_MAXIMUM_LENGTH MaxLength
                                from
	                                sys.columns c
	                                inner join sys.tables t on t.object_id = c.object_id
	                                inner join sys.schemas s on s.schema_id = t.schema_id
	                                inner join INFORMATION_SCHEMA.COLUMNS ic on ic.TABLE_SCHEMA = s.name and ic.TABLE_NAME = t.name and ic.COLUMN_NAME = c.name
	                                left outer join #PrimaryKeys pk on pk.SchemaName = s.name and pk.TableName = t.name and pk.PrimaryKey = c.name
                                order by
	                                s.name
	                                , t.name
	                                , c.column_id
	
                                drop table #PrimaryKeys
                        ", new List<SqlParameter>(), false);
        }

        public static ColumnInfo FindColumnInfo(string schemaName, string tableName, string columnName)
        {
            return SQLDM.SQLDMGlobal.Columns.Find(c => c.Schema == schemaName && c.Table == tableName && c.Column == columnName);
        }

    }
}
