using System;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Data.SqlClient;
using System.Linq;

namespace SQLDM
{
    public static class SQLDMGlobal
    {

        public enum GetRecordOperator {Or, And };


        public static string PARAMFIRSTCHAR = "@";


        private static List<DataRecordTypeAssembly> _dataRecordTypes = null;
        private static List<ColumnInfo> _Columns = null;
        private static List<DataRecord> _Dummys = null;
        private static List<ForeignKeys> _ForeignKeysLinks = null;
        private static bool _EnvironmentsCollected = false;


        internal static List<TablePropertyInfo> AllFieldInfoList { get; set; } = new List<TablePropertyInfo>();

        internal static List<ColumnInfo> Columns
        {
            get
            {
                return _Columns;
            }
        }

        internal static List<ForeignKeys> ForeignKeysLinks
        {
            get
            {
                return _ForeignKeysLinks;
            }
        }

        internal static List<DataRecordTypeAssembly> DataRecordTypes
        {
            get
            {
                if(_dataRecordTypes == null)
                {
                    Assembly ass = Assembly.GetEntryAssembly();
                    RegisterAssemblyForData(ass);
                }

                return _dataRecordTypes;
            }
        }

        internal static List<DataRecord> Dummys
        {
            get
            {
                return _Dummys;
            }
        }

        internal static DataRecord FindDummy(Type type)
        {
            return Dummys.Find(d => d.GetType() == type);
        }



        public static void RegisterAssemblyForData(Assembly ass)
        {
            if(_dataRecordTypes == null) { _dataRecordTypes = new List<DataRecordTypeAssembly>(); }
            foreach(Type t in ass.GetTypes())
            {
                DataRecordTypeAssembly newRec = new DataRecordTypeAssembly();
                newRec.DataRecordType = t;
                newRec.Assembly = ass;
                if(t.BaseType.BaseType != null && t.BaseType != null && t.BaseType.BaseType.FullName == typeof(DataRecord).FullName && !_dataRecordTypes.Contains(newRec))
                {
                    _dataRecordTypes.Add(newRec);
                }
            }

            GetEnvironments();
        }

        private static void GetEnvironments()
        {
            if (_EnvironmentsCollected) return;

            _EnvironmentsCollected = true;

           foreach (Configuration.SQLDataManagerElement envElement in Configuration.SQLDataManager.GetEnvironments())
            {
                SqlEnvironment env = new SqlEnvironment(envElement.Name);
                ConnectionManager cm = ConnectionManager.GetConnectionManager();
                env.IsCurrent = envElement.IsCurrent;
                env.InitialCatalog = envElement.InitialCatalogue;
                env.IntegratedSecurity = envElement.IntegratedSecurity;
                env.ConnectionTimeout = envElement.ConnectionTimeout;
                env.DataSource = envElement.DataSource;
                env.Schema = envElement.Schema;
                env.UserID = envElement.UserID;
                env.Password = envElement.Password;
                cm.Add(env);
            }
        }

        public static void RegisterAssemblyForData()
        {
            RegisterAssemblyForData(Assembly.GetCallingAssembly());
        }



        internal static void Initialise()
        {
            RegisterAssemblyForData();

            LoadDummys();

            _Columns = ColumnInfo.GetAllColumnInfo();
            _ForeignKeysLinks = ForeignKeys.GetForeignKeys();

            foreach (DataRecord dummy in Dummys)
            {
                dummy.LoadRelationships();
            }
        }

        internal static void LoadDummys()
        {
            _Dummys = new List<DataRecord>();

            foreach (DataRecordTypeAssembly t in DataRecordTypes)
            {
                DataRecord dummy = (DataRecord)Activator.CreateInstance(t.DataRecordType);
                _Dummys.Add(dummy);
            }

        }

        internal static string ConnectionString()
        {
            return ConnectionManager.GetConnectionManager().ConnectionString();
        }

        internal static string ConnectionString(string environmentName)
        {
            return ConnectionManager.GetConnectionManager().ConnectionString(environmentName);
        }

        /// <summary>
        /// Creates a SqlParameter with a value and ensures the ParameterName begins with @
        /// </summary>
        public static SqlParameter CreateSqlParameter(string parameterName, SqlDbType dbType, object value)
        {
            return CreateSqlParameter(parameterName, dbType, 0, value);
        }

        /// <summary>
        /// Creates a SqlParameter with a value and ensures the ParameterName begins with @
        /// </summary>
        public static SqlParameter CreateSqlParameter(string parameterName, SqlDbType dbType, int size, object value)
        {
            if (!parameterName.StartsWith(PARAMFIRSTCHAR)) parameterName = PARAMFIRSTCHAR + parameterName;
            return new SqlParameter(parameterName, dbType, size) { Value = value };
        }

        internal static Type TableType(string schemaName, string tableName, Type callingType)
        {
            foreach(DataRecordTypeAssembly t in DataRecordTypes)
            {
                if(t.DataRecordType != callingType)
                {
                    DataRecord dummy = SQLDMGlobal.Dummys.Find(d => d.GetType() == t.DataRecordType);
                    if (dummy.SchemaName == schemaName && dummy.TableName == tableName) return dummy.GetType();
                }
            }
            return null;
        }

        /// <summary>
        /// Gets a SQL WHERE clause from the parameters and comparison type
        /// </summary>
        internal static string WhereClauseFromParameters(SqlParameter[] sqlParams, GetRecordOperator comparison)
        {
            if (sqlParams.FirstOrDefault(p => string.IsNullOrWhiteSpace(p.SourceColumn)) != null) throw new DataException("All source columns must be defined");
            foreach(SqlParameter p in sqlParams)
            {
                if (p.ParameterName == "") p.ParameterName = "@" + p.SourceColumn.Replace(" ", "_");
                if (p.ParameterName.Substring(0, 1) != "@") p.ParameterName = "@" + p.ParameterName;
            }

            string sql = "where ";
            string clause = (comparison == GetRecordOperator.And) ? " and ": " or ";

            bool first = true;
            foreach(SqlParameter p in sqlParams)
            {
                if (!first) sql += clause;
                sql += "[" + p.SourceColumn + "] ";
                if(p.Value == null)
                {
                    sql += "is null";
                }else
                {
                    sql += "= " + p.ParameterName;
                }
                first = false;
            }

            return sql;
        }

        /// <summary>
        /// Gets a SQL WHERE clause from the parameter
        /// </summary>
        internal static string WhereClauseFromParameters(SqlParameter sqlParam)
        {
            return WhereClauseFromParameters(new SqlParameter[] { sqlParam }, GetRecordOperator.And);
        }

        /// <summary>
        /// Gets a safe to use table name including database and schema if provided
        /// </summary>
        internal static string FullSafeTableName(string databaseName, string schemaName, string tableName)
        {
            if (databaseName != "" && schemaName != "") return string.Format("[{0}].[{1}].[{2}]", databaseName, schemaName, tableName);
            if (databaseName != "" && schemaName == "") return string.Format("[{0}]..[{1}]", databaseName, tableName);
            if (databaseName == "" && schemaName != "") return string.Format("[{0}].[{1}]", schemaName, tableName);
            return string.Format("[{0}]", tableName);
        }

    }
}
