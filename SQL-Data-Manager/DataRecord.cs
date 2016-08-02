using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

namespace SQLDM
{
    public abstract class DataRecord
    {

        protected static List<Type> _AllowablePropertyTypes = null;
        protected static List<string> _ReservedPropertyNames = null;
        private List<DataField> _Fields = null;
        private DataField _IdentityField = null;
        private List<DataLink> _Links = null;
        private List<DataField> _PKFields = null;
        private SqlParameter _OldPKParam = null;
        private DataRecord _Dummy = null;
        private List<TablePropertyInfo> _FieldPropertyList = new List<TablePropertyInfo>();



        public DataRecord()
        {
            AddMyFieldInfoList();
        }



        public event SavedEventHandler Saved;
        public delegate void SavedEventHandler(DataRecord d);

        public event DeletedEventHandler Deleted;
        public delegate void DeletedEventHandler(DataRecord d);

        public event LoadedEventHandler Loaded;
        public delegate void LoadedEventHandler(DataRecord d);



        /// <summary>
        /// Gets or sets all child records of any type
        /// </summary>
        public List<Object> ChildRecords { get; set; } = new List<Object>();

        /// <summary>
        /// Gets all the fields available within the data record
        /// </summary>
        public List<DataField> Fields
        {
            get
            {
                if (_Fields == null) CreateFields();
                return _Fields;
            }
        }

        [NotDatabaseField]
        public string FullSafeTableName
        {
            get
            {
                return SQLDMGlobal.FullSafeTableName(DatabaseName, SchemaName, TableName);
            }
        }

        /// <summary>
        /// Gets the field marked as an identity (if there is one) within the data record
        /// </summary>
        public DataField IdentityField
        {
            get
            {
                if(_IdentityField == null)
                {
                    foreach(DataField f in Fields)
                    {
                        if (f.IsIdentity)
                        {
                            _IdentityField = f;
                            break;
                        }
                    }
                }

                return _IdentityField;
            }
        }

        /// <summary>
        /// Gets a value indicating if the field has changed since the record was last save
        /// </summary>
        [NotDatabaseField]
        public bool IsDirty
        {
            get
            {
                if (IsNew) return true;

                foreach(DataField f in Fields)
                {
                    if (f.IsDirty) return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates the table is virtual and does not exist in the database.
        /// A virtual table will not be tested for field type mismatches and relationships will not be established
        /// </summary>
        [NotDatabaseField]
        public virtual bool IsVirtualTable { get; set; } = false;

        /// <summary>
        /// Gets the primary key field (if there is one) within the data record
        /// </summary>
        public List<DataField> PKFields
        {
            get
            {
                if(_PKFields == null)
                {
                    foreach(DataField f in Fields)
                    {
                        if (f.IsPrimaryKey)
                        {
                            _PKFields.Add(f);
                        }
                    }
                }

                return _PKFields;
            }
        }

        [NotDatabaseField]
        public string SafeTableName
        {
            get
            {
                return string.Format("[{0}]", TableName);
            }
        }

        [NotDatabaseField]
        public virtual string SchemaName { get { return ConnectionManager.GetConnectionManager().GetEnvironment().Schema; } }

        [NotDatabaseField]
        public virtual string TableName
        {
            get
            {
                return GetType().Name;
            }
        }

        [NotDatabaseField]
        internal DataRecord Dummy
        {
            get
            {
                if(_Dummy == null) _Dummy = (SQLDMGlobal.Dummys.Find(d => d.SchemaName == SchemaName && d.TableName == TableName));
                return _Dummy;
            }
        }

        [NotDatabaseField]
        internal bool IsNew { get; set; } = true;

        internal List<DataLink> Links
        {
            get
            {
                if (this == Dummy)
                {
                    if (_Links == null)  SetLinks();
                    return _Links;
                }
                return Dummy.Links;
            }
        }

        internal ValidationResults ValidationErrors { get; set; }

        protected List<SqlParameter> AllParameters
        {
            get
            {
                List<SqlParameter> l = new List<SqlParameter>();
                foreach (DataField f in Fields) l.Add((SqlParameter)f);

                return l;
            }
        }

        [NotDatabaseField]
        protected virtual string DatabaseName { get { return ""; } }

        [NotDatabaseField]
        protected bool IsDeleted { get; set; } = false;

        private List<TablePropertyInfo> PropertyInfoList
        {
            get
            {
                if (_FieldPropertyList.Count == 0) LoadPropertyInfoList();
                return _FieldPropertyList;
            }
        }

        private static List<Type> AllowablePropertyTypes
        {
            get
            {
                if (_AllowablePropertyTypes == null)
                {
                    _AllowablePropertyTypes = new List<Type>();
                    _AllowablePropertyTypes.Add(typeof(int?));
                    _AllowablePropertyTypes.Add(typeof(int));
                    _AllowablePropertyTypes.Add(typeof(long?));
                    _AllowablePropertyTypes.Add(typeof(long));
                    _AllowablePropertyTypes.Add(typeof(string));
                    _AllowablePropertyTypes.Add(typeof(byte?));
                    _AllowablePropertyTypes.Add(typeof(byte));
                    _AllowablePropertyTypes.Add(typeof(DateTime?));
                    _AllowablePropertyTypes.Add(typeof(DateTime));
                    _AllowablePropertyTypes.Add(typeof(decimal?));
                    _AllowablePropertyTypes.Add(typeof(decimal));
                    _AllowablePropertyTypes.Add(typeof(bool?));
                    _AllowablePropertyTypes.Add(typeof(bool));
                    _AllowablePropertyTypes.Add(typeof(Single?));
                    _AllowablePropertyTypes.Add(typeof(Single));
                    _AllowablePropertyTypes.Add(typeof(double?));
                    _AllowablePropertyTypes.Add(typeof(double));
                    _AllowablePropertyTypes.Add(typeof(Guid?));
                    _AllowablePropertyTypes.Add(typeof(Guid));
                    _AllowablePropertyTypes.Add(typeof(short?));
                    _AllowablePropertyTypes.Add(typeof(short));
                }

                return _AllowablePropertyTypes;
            }
        }

        private static List<string> ReservedPropertynames
        {
            get
            {
                if(_ReservedPropertyNames == null)
                {
                    _ReservedPropertyNames = new List<string>();
                    _ReservedPropertyNames.Add(nameof(FullSafeTableName));
                    _ReservedPropertyNames.Add(nameof(SafeTableName));
                    _ReservedPropertyNames.Add(nameof(IsDirty));
                    _ReservedPropertyNames.Add(nameof(IsVirtualTable));
                    _ReservedPropertyNames.Add(nameof(SchemaName));
                    _ReservedPropertyNames.Add(nameof(TableName));
                    _ReservedPropertyNames.Add(nameof(IsNew));
                    _ReservedPropertyNames.Add(nameof(DatabaseName));
                    _ReservedPropertyNames.Add(nameof(IsDeleted));
                }

                return _ReservedPropertyNames;
            }
        }



        public bool Delete()
        {
            return Delete(null);
        }

        public bool Delete(SqlTransaction trans)
        {
            return Delete(trans, null);
        }

        public bool Delete(SqlTransaction trans, SqlConnection conn)
        {
            ValidationErrors.Clear();

            if (IsDeleted) return true;

            DataField idField = IdentityField;
            if (idField == null)
            {
                ValidationErrors.Add("Record does not exist");
                return false;
            }

            if (conn == null) conn = new SqlConnection(SQLDMGlobal.ConnectionString());
            if (conn.State != ConnectionState.Open) conn.Open();

            SqlCommand cmd = conn.CreateCommand();

            bool TransactionExists = true;
            if (trans == null)
            {
                TransactionExists = false;
                trans = conn.BeginTransaction();
            }
            cmd.Transaction = trans;

            try
            {
                foreach(DataRecord r in ChildRecords)
                {
                    if (!r.Delete(trans, conn))
                    {
                        ValidationErrors.AddRange(r.ValidationErrors);
                        throw new DataException("Validation error while saving to " + r.SafeTableName);
                    }
                }

                cmd.CommandText = SQLForDelete(idField);
                cmd.Parameters.Add(idField);
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();

                if (!TransactionExists)
                {
                    trans.Commit();
                    conn.Close();
                }

                IsDeleted = true;

                Deleted(this);
            }
            catch (DataException ex)
            {
                if (!TransactionExists) trans.Rollback();

                ValidationErrors.Add(ex.Message);
            }

            return ValidationErrors.IsValid;
        }

        public DataField FindField(string fieldName)
        {
            return Fields.Find(f => f.SourceColumn == fieldName);
        }

        public bool IsValid()
        {
            ValidationErrors = Validate();
            return ValidationErrors.IsValid;
        }

        public void ResetIdentitySeed() { ResetIdentitySeed(null); }

        public void ResetIdentitySeed(SqlConnection conn)
        {
            DataField f = IdentityField;
            if (f == null) return;

            if (conn == null) conn = new SqlConnection(SQLDMGlobal.ConnectionString());
            using (conn)
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "declare @reseed int set @reseed = (select max(" + f.SafeFieldName + ") from " + FullSafeTableName + ") " +
                                  "dbcc checkident(" + FullSafeTableName + ", reseed, @reseed)";
                cmd.ExecuteNonQuery();
            }
        }

        public string SafeFieldName(string fieldName)
        {
            DataField field = FindField(fieldName);
            if (field != null) return field.SafeFieldName;
            throw new DataException("Table " + SafeTableName + " does not contain the field [" + fieldName + "]");
        }

        /// <summary>
        /// Saves the data record and all its child records, creating a new record if necessary
        /// </summary>
        public virtual bool Save()
        {
            return (Save(null));
        }

        /// <summary>
        /// Saves the data record and all its child records, creating a new record if necessary
        /// </summary>
        public virtual bool Save(SqlTransaction trans)
        {
            return (Save(trans, null));
        }

        /// <summary>
        /// Saves the data record and all its child records, creating a new record if necessary
        /// </summary>
        public virtual bool Save(SqlTransaction trans, SqlConnection conn)
        {
            ReadOnly ro = (ReadOnly)Attribute.GetCustomAttribute(this.GetType(), typeof(ReadOnly));
            if (ro != null) throw new DataException("Data is marked as ReadOnly");

            if (!IsValid()) return false;

            if (!CanSave())
            {
                ValidationErrors.Add("Table " + SafeTableName + " must have either an identity field defined or a primary key field with valid data");
                return false;
            }

            if (conn == null) conn = new SqlConnection(SQLDMGlobal.ConnectionString());
            if (conn.State != ConnectionState.Open) conn.Open();

            SqlCommand cmd = conn.CreateCommand();

            bool TransactionExists = true;
            if(trans == null)
            {
                TransactionExists = false;
                trans = conn.BeginTransaction();
            }
            cmd.Transaction = trans;

            try
            {
                if(IsDirty)
                {
                    if (IsNew)
                    {
                        cmd.CommandText = SQLForCreate();
                        cmd.Parameters.AddRange(AllDirtyParameters(true).ToArray());
                    }
                    else
                    {
                        cmd.CommandText = SQLForUpdate(null);
                        cmd.Parameters.AddRange(AllDirtyParameters(false).ToArray());
                    }

                    SqlDataReader rs = cmd.ExecuteReader();
                    if (rs.Read() && IdentityField != null) IdentityField.Value = rs.GetValue(0);
                    IsNew = false;
                    rs.Close();

                    cmd.Parameters.Clear();

                    foreach (DataField f in Fields) f.OriginalValue = f.Value;
                }

                if (ChildRecords != null)
                {
                    foreach(DataRecord r in ChildRecords)
                    {
                        if(!r.Save(trans, conn))
                        {
                            ValidationErrors.AddRange(r.ValidationErrors);
                            throw new DataException("Validation error while saving to " + r.SafeTableName);
                        }
                    }
                }

                if (!TransactionExists)
                {
                    trans.Commit();
                    conn.Close();
                }

                if(Saved != null) Saved(this);
            }
            catch (DataException ex)
            {
                if (!TransactionExists)
                {
                    ValidationErrors.Add(ex.Message);
                    trans.Rollback();
                } else
                {
                    throw;
                }
            }

            return ValidationErrors.IsValid;
        }

        /// <summary>
        /// Returns the data as a simple data row
        /// </summary>
        private DataRow ToDataRow()
        {
            DataTable dt = GetEmptyDataTable(this);

            DataRow dr = dt.NewRow();
            foreach (TablePropertyInfo pi in PropertyInfoList)
            {
                DataField r = new DataField(pi.PropertyInfo, this);
                dr[r.PropertyName] = r.Value;
            }
            return dr;
        }

        public static DataTable ToDataTable(List<DataRecord> records)
        {
            if (records == null || records.Count == 0) return null;

            DataTable dt = GetEmptyDataTable(records[0]);

            foreach(DataRecord record in records)
            {
                dt.Rows.Add(record.ToDataRow());
            }

            return dt;
        }

        private static DataTable GetEmptyDataTable(DataRecord record)
        {
            DataTable dt = new DataTable(record.TableName);

            foreach (TablePropertyInfo pi in record.PropertyInfoList)
            {
                DataField r = new DataField(pi.PropertyInfo, record);
                dt.Columns.Add(r.PropertyName, typeof(string));
            }

            return dt;
        }

        internal void LoadRelationships()
        {
            if (this == Dummy)
            {
                // Setup relationships and test for field type mismatches
                foreach (DataField f in Fields)
                {
                    // Get list of child types
                    foreach(ForeignKeys fk in SQLDMGlobal.ForeignKeysLinks.FindAll(fk => fk.ParentTable == TableName && fk.ParentSchema == SchemaName && fk.ParentColumn == f.SourceColumn))
                    {
                        f.IsParentField = true;
                        f.ChildTableTypes.Add(SQLDMGlobal.TableType(fk.ChildSchema, fk.ChildTable, GetType()));
                    }

                    // Get parent type
                    foreach(ForeignKeys fk in SQLDMGlobal.ForeignKeysLinks.FindAll(fk => fk.ChildSchema == SchemaName && fk.ChildTable == TableName && fk.ChildColumn == f.SourceColumn))
                    {
                        f.ParentTableType = SQLDMGlobal.TableType(fk.ParentSchema, fk.ParentTable, GetType());
                    }
                }
            }
        }

        internal void SetValue(string fieldName, object Value)
        {
            DataField f = FindField(fieldName);
            if (f != null) throw new DataException("Table " + SafeTableName + " does not contain the field [" + fieldName + "]");
            f.Value = Value;
        }

        internal object Value(string fieldName)
        {
            DataField f = FindField(fieldName);
            if (f != null) return f.Value;
            return null;
        }

        protected List<SqlParameter> AllDirtyParameters(bool forCreate)
        {
            List<SqlParameter> l = new List<SqlParameter>();
            foreach (DataField f in DirtyFields(forCreate)) if (!l.Contains((SqlParameter)f)) l.Add((SqlParameter)f);

            return l;
        }

        /// <summary>
        /// Checks if necessary data is present to allow a record to be saved
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Default Base behaviour is to require a Primary Key or Identity field
        /// and if a Primary Key exists, it must have data or be the Identity field
        /// </remarks>
        protected virtual bool CanSave()
        {
            return !IsDeleted;
        }

        protected List<DataField> DirtyFields(bool forCreate)
        {
            List<DataField> l = new List<DataField>();
            foreach(DataField f in Fields)
            {
                if ((forCreate | f.IsDirty) && !l.Contains(f) && !f.IsIdentity && f.Value != null) l.Add(f);
                if (!forCreate && f.IsIdentity && !l.Contains(f)) l.Add(f);
            }

            return l;
        }

        protected static bool FillRecord(DataRow row, DataRecord record, bool loadChildren)
        {
            foreach (DataField df in record.Fields)
            {
                try
                {
                    if (row[df.SourceColumn] != System.DBNull.Value)
                    {
                        df.Value = row[df.SourceColumn];
                        df.OriginalValue = df.Value;
                    }
                    else
                    {
                        df.Value = null;
                        df.OriginalValue = null;
                    }
                    df.DataPresent = true;
                }
#pragma warning disable CS0168 // Variable is declared but never used
                catch (Exception ex)
#pragma warning restore CS0168 // Variable is declared but never used
                {
                    df.DataPresent = false;
                    return false;
                }
            }

            if (loadChildren) record.LoadChildRecords();
            record.OnLoaded();

            return true;
        }

        protected void LoadChildRecords()
        {
            ChildRecords.Clear();

            foreach(DataLink dl in Links)
            {
                LoadChildRecords(dl.ChildRecordType);
            }
        }

        protected void LoadChildRecords(List<Type> types)
        {
            foreach (DataLink dl in Links)
            {
              if (types.Contains(dl.ChildRecordType))  LoadChildRecords(dl.ChildRecordType);
            }
        }

        protected void LoadChildRecords(Type type)
        {
            using (SqlConnection conn = new SqlConnection(SQLDMGlobal.ConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();

                try
                {
                    foreach(DataLink dl in Links.FindAll(c => c.ChildRecordType == type)){
                        DataRecord childDummy = SQLDMGlobal.FindDummy(type);

                        if(childDummy != null)
                        {
                            cmd.CommandText = string.Format("select * from {0} where [{1}] = {2}", childDummy.FullSafeTableName, dl.ChildFieldName, dl.ParentField.ParameterName);
                            SqlParameter param = GetSqlParameter(dl.ParentField.ParameterName);
                            if(param != null) cmd.Parameters.Add(param);

                            SqlDataAdapter da = new SqlDataAdapter();
                            da.SelectCommand = cmd;
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            foreach(DataRow row in dt.Rows)
                            {
                                DataRecord inst = (DataRecord)System.Activator.CreateInstance(dl.ChildRecordType);
                                FillRecord(row, inst, true);
                                ChildRecords.Add(inst);
                            }

                            cmd.Parameters.Clear();
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (conn.State == ConnectionState.Open) conn.Close();
                    throw ex;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        protected void OnParentFieldValue_Changed(object sender, EventArgs e)
        {
            ResetChildLinks();
        }

        /// <summary>
        /// Generates the SQL script necessary to create a new record.
        /// Allows for Identity insertion if required
        /// </summary>
        /// <returns></returns>
        protected virtual string SQLForCreate()
        {
            bool setIdInsert = (IdentityField != null && IdentityField.Value != null && (Int64)IdentityField.Value != 0);
            StringBuilder sql = new StringBuilder();

            if (setIdInsert) sql.AppendLine("set IDENTITY_INSERT " + FullSafeTableName + " on");

            sql.AppendLine("insert into " + FullSafeTableName + " (");

            string prefix = "    ";
            foreach (DataField f in DirtyFields(true))
            {
                if(f != IdentityField || f == IdentityField && setIdInsert)
                {
                    sql.AppendLine(prefix + f.SafeFieldName);
                    prefix = "    , ";
                }
            }

            sql.AppendLine(") values (");

            prefix = "    ";
            foreach (DataField f in DirtyFields(true))
            {
                if (f != IdentityField || f == IdentityField && setIdInsert)
                {
                    sql.AppendLine(prefix + f.ParameterName);
                    prefix = "    , ";
                }
            }

            sql.AppendLine(")");

            if (setIdInsert) sql.AppendLine("set IDENTITY_INSERT " + FullSafeTableName + " off");

            if (IdentityField != null)
            {
                sql.AppendLine("select cast(SCOPE_IDENTITY() as " + IdentityField.SqlDbType.ToString() + ")");
            }

            return sql.ToString();
        }

        /// <summary>
        /// Generates the SQL script necessary to delete a record
        /// </summary>
        protected virtual string SQLForDelete(DataField idField)
        {
            if (idField == null) throw new DataException("Primary Key or Identity field must exist to delete a record");

            StringBuilder sql = new StringBuilder();
            sql.AppendLine("delete from " + FullSafeTableName);
            sql.AppendLine("where " + idField.SafeFieldName + " = " + idField.ParameterName);
            return sql.ToString();
        }

        /// <summary>
        /// Generates the SQL script necessary to update an existing record. 
        /// </summary>
        /// <param name="whereClause">Where clause to apply to the update - default is to refer to the Primary Key field</param>
        protected virtual string SQLForUpdate(string whereClause)
        {
            if (IdentityField == null && string.IsNullOrWhiteSpace(whereClause)) throw new DataException("Cannot update a table with no Identity without a supplied 'where' clause");

            if (IdentityField != null && IdentityField.IsDirty) throw new DataException("Cannot update an identity field");
            StringBuilder sql = new StringBuilder();

            sql.AppendLine("update " + FullSafeTableName + " set");

            string prefix = "    ";
            foreach (DataField f in Fields)
            {
                if(f.IsDirty)
                {
                    sql.Append(prefix + f.SafeFieldName + " = " + f.ParameterName);
                    prefix = "    , ";
                }
            }

            if (string.IsNullOrWhiteSpace(whereClause))
            {
                SqlParameter paramWhere = (SqlParameter)IdentityField;

                if (string.IsNullOrWhiteSpace(paramWhere.ParameterName)) paramWhere.ParameterName = "@" + paramWhere.SourceColumn;
                if (paramWhere.ParameterName.Substring(0, 1) != "@") paramWhere.ParameterName = "@" + paramWhere.ParameterName;

                sql.AppendLine(" where " + IdentityField.SafeFieldName + " = " + paramWhere.ParameterName);
            }
            else
            {
                sql.AppendLine(whereClause);
            }

            return sql.ToString();
        }

        protected virtual ValidationResults Validate()
        {
            return new ValidationResults();
        }

        private void AddMyFieldInfoList()
        {
            if (PropertyInfoList.Count > 0) return;

            foreach (PropertyInfo pi in GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public))
            {
                if (AllowablePropertyTypes.Contains(pi.PropertyType))
                {
                    NotDatabaseField ca = (NotDatabaseField)pi.GetCustomAttribute(typeof(NotDatabaseField), false);
                    if(ca == null)
                    {
                        TablePropertyInfo newTPI = new TablePropertyInfo();
                        newTPI.SchemaName = SchemaName;
                        newTPI.TableName = TableName;
                        newTPI.PropertyInfo = pi;
                        SQLDMGlobal.AllFieldInfoList.Add(newTPI);
                    }
                }
            }
        }

        private void CreateFields()
        {
            _Fields = new List<DataField>();
            foreach (TablePropertyInfo pi in PropertyInfoList)
            {
                DataField r = new DataField(pi.PropertyInfo, this);
                r.PropertyName = pi.PropertyInfo.Name;
                if (!IsVirtualTable)
                {
                    r.IsIdentity = IsIdentityColumn(r.PropertyName);
                    r.IsPrimaryKey = IsPrimaryKeyColumn(r.PropertyName);
                    r.IsNullable = IsNullableColumn(r.PropertyName);
                    ColumnInfo ci = ColumnInfo.FindColumnInfo(SchemaName, TableName, r.SourceColumn);
                    if (ci != null) r.SqlDbType = (SqlDbType)Enum.Parse(typeof(SqlDbType), ci.DataType, true);
                }
                _Fields.Add(r);
            }
        }

        private SqlParameter GetSqlParameter(string parameterName)
        {
            return (SqlParameter)Fields.Find(f => f.ParameterName == parameterName);
        }

        private bool IsIdentityColumn(string columnName)
        {
            return (SQLDMGlobal.Columns.FindAll(p => p.Schema == SchemaName && p.Table == TableName && p.Column == columnName && p.IsIdentity).Count > 0);
        }

        private bool IsNullableColumn(string columnName)
        {
            return (SQLDMGlobal.Columns.FindAll(p => p.Schema == SchemaName && p.Table == TableName && p.Column == columnName && p.IsNullable).Count > 0);
        }

        private bool IsPrimaryKeyColumn(string columnName)
        {
            return (SQLDMGlobal.Columns.FindAll(p => p.Schema == SchemaName && p.Table == TableName && p.Column == columnName && p.IsPrimaryKey).Count > 0);
        }

        private void LoadPropertyInfoList()
        {
            _FieldPropertyList = SQLDMGlobal.AllFieldInfoList.FindAll(t => t.SchemaName == SchemaName && t.TableName == TableName);
        }

        private void OnLoaded()
        {
           if(Loaded != null) Loaded(this);
        }

        private void ResetChildLinks()
        {
           foreach(DataField df in Fields)
            {
                if (df.IsParentField)
                {
                    foreach(DataLink dl in Links)
                    {
                        if (dl.ParentField.SourceColumn == df.SourceColumn)
                        {
                            foreach(DataRecord child in ChildRecords)
                            {
                                if (child.GetType() == dl.ChildRecordType)
                                {
                                    child.SetValue(dl.ChildFieldName, df.Value);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void SetLinks()
        {
            _Links = new List<DataLink>();

            foreach(DataField field in Dummy.Fields)
            {
                if (field.ChildTableTypes == null) continue;
                foreach(Type childType in field.ChildTableTypes)
                {
                    {
                        DataRecord childDummy = SQLDMGlobal.FindDummy(childType);
                        if (childDummy == null) continue;

                        foreach(DataField childField in childDummy.Dummy.Fields)
                        {
                            if(childField.ParentTableType != null && childField.ParentTableType.FullName == GetType().FullName)
                            {
                                DataLink newLink = new DataLink(this, field, childType, childField.SourceColumn);
                                _Links.Add(newLink);
                                break;
                            }
                        }
                    }
                }
            }
        }

    }
}