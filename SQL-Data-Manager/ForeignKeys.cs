using System.Collections.Generic;
using System.Data.SqlClient;

namespace SQLDM
{
    class ForeignKeys : DataLoader<ForeignKeys>
    {

        public string ParentSchema { get; set; }
        public string ParentTable { get; set; }
        public string ParentColumn { get; set; }
        public string ChildSchema { get; set; }
        public string ChildTable { get; set; }
        public string ChildColumn { get; set; }

        [NotDatabaseField]
        public override bool IsVirtualTable { get { return true; } }

        public static List<ForeignKeys> GetForeignKeys()
        {
            return GetList(@"
                                select
	                                Rsch.name as ParentSchema
	                                , Rtab.name as ParentTable
	                                , Rcol.name as ParentColumn
	                                , Psch.name as ChildSchema
	                                , Ptab.name as ChildTable
	                                , PCol.name as ChildColumn
                                from
	                                sys.foreign_key_columns fk
	                                inner join sys.columns Pcol on fk.parent_object_id = Pcol.object_id and fk.parent_column_id = Pcol.column_id
	                                inner join sys.columns Rcol on fk.referenced_object_id = Rcol.object_id and fk.referenced_column_id = Rcol.column_id
	                                inner join sys.tables Ptab on Ptab.object_id = fk.parent_object_id
	                                inner join sys.tables Rtab on Rtab.object_id = fk.referenced_object_id
	                                inner join sys.schemas Psch on Psch.schema_id = Ptab.schema_id
	                                inner join sys.schemas Rsch on Rsch.schema_id = Rtab.schema_id
                        ", new List<SqlParameter>(), false);
        }
    }
}
