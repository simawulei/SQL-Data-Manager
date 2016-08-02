using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using static SQLDM.SQLDMGlobal;

namespace SQLDM
{
    public abstract class DataLoader<T> : DataRecord where T : DataRecord, new()
    {
        public static bool LOADCHILDRENDEFAULT = false;



        #region GetRecord Methods (returns 1 record of Type T)

        protected static T GetRecord(DataField field) { return GetRecord(field, LOADCHILDRENDEFAULT); }

        protected static T GetRecord(SqlParameter parameter) { return GetRecord(parameter, LOADCHILDRENDEFAULT); }

        protected static T GetRecord(DataField field, bool loadChildren) { return GetRecord(field.Value, loadChildren); }

        protected static T GetRecord(object value) { return GetRecord(value, LOADCHILDRENDEFAULT); }

        protected static T GetRecord(object value, bool loadChildren)
        {
            DataField pk = FindDummy(typeof(T)).IdentityField;
            if(pk != null) return GetRecord(pk.SourceColumn, value, loadChildren);
            return null;
        }

        protected static T GetRecord(string fieldName, object value) { return GetRecord(fieldName, value, LOADCHILDRENDEFAULT); }

        protected static T GetRecord(string fieldName, object value, bool loadChildren)
        {
            SqlParameter p = new SqlParameter("@P", value);
            p.SourceColumn = fieldName;
            return GetRecord(p, loadChildren);
        }

        protected static T GetRecord(SqlParameter parameter, bool loadChildren)
        {
            string sql = "select * from " + FindDummy(typeof(T)).FullSafeTableName + " " + WhereClauseFromParameters(parameter);
            return GetRecord(sql, new SqlParameter[] { parameter }, loadChildren);
        }

        protected static T GetRecord(List<SqlParameter> parameters, GetRecordOperator comparison) { return GetRecord(parameters.ToArray(), comparison, LOADCHILDRENDEFAULT); }

        protected static T GetRecord(List<SqlParameter> parameters, GetRecordOperator comparison, bool loadChildren) { return GetRecord(parameters.ToArray(), comparison, loadChildren); }

        protected static T GetRecord(SqlParameter[] parameters, GetRecordOperator comparison) { return GetRecord(parameters,comparison, LOADCHILDRENDEFAULT); }

        protected static T GetRecord(SqlParameter[] parameters, GetRecordOperator comparison, bool loadChildren)
        {
            string sql = "select * from " + FindDummy(typeof(T)).FullSafeTableName + " " + WhereClauseFromParameters(parameters, comparison);
            return GetRecord(sql, parameters, loadChildren);
        }

        protected static T GetRecord(string sql, List<SqlParameter> parameters) { return GetRecord(sql, parameters.ToArray(), LOADCHILDRENDEFAULT); }

        protected static T GetRecord(string sql, List<SqlParameter> parameters, bool loadChildren) { return GetRecord(sql, parameters.ToArray(), loadChildren); }
            
        protected static T GetRecord(string sql, SqlParameter[] parameters) { return GetRecord(sql, parameters, LOADCHILDRENDEFAULT); }

        protected static T GetRecord(string sql, SqlParameter[] parameters, bool loadChildren)
        {
            return GetRecord(sql, parameters, CommandType.Text, loadChildren);
        }

        protected static T GetRecord(string sql, SqlParameter[] parameters, CommandType commandType, bool loadChildren)
        {
            return GetRecord(GetRecordDataRow(sql, parameters, commandType, loadChildren),loadChildren);
        }

        protected static T GetRecord(DataRow row)
        {
            return GetRecord(row, LOADCHILDRENDEFAULT);
        }

        protected static T GetRecord(DataRow row, bool loadChildren)
        {
            if (row == null) return null;

            DataTable dt = row.Table;
            List<T> l = ToList(dt, loadChildren);
            if (l.Count > 0) return l[0];

            return null;
        }

        #endregion

        #region GetRecordDataRow Methods (returns a single DataRow)

        protected static DataRow GetRecordDataRow(DataField field) { return GetRecordDataRow(field, LOADCHILDRENDEFAULT); }

        protected static DataRow GetRecordDataRow(SqlParameter parameter) { return GetRecordDataRow(parameter, LOADCHILDRENDEFAULT); }

        protected static DataRow GetRecordDataRow(DataField field, bool loadChildren) { return GetRecordDataRow(field.Value, loadChildren); }

        protected static DataRow GetRecordDataRow(object value) { return GetRecordDataRow(value, LOADCHILDRENDEFAULT); }

        protected static DataRow GetRecordDataRow(object value, bool loadChildren)
        {
            DataField pk = FindDummy(typeof(T)).IdentityField;
            if (pk != null) return GetRecordDataRow(pk.SourceColumn, value, loadChildren);
            return null;
        }

        protected static DataRow GetRecordDataRow(string fieldName, object value) { return GetRecordDataRow(fieldName, value, LOADCHILDRENDEFAULT); }

        protected static DataRow GetRecordDataRow(string fieldName, object value, bool loadChildren)
        {
            SqlParameter p = new SqlParameter("@P", value);
            p.SourceColumn = fieldName;
            return GetRecordDataRow(p, loadChildren);
        }

        protected static DataRow GetRecordDataRow(SqlParameter parameter, bool loadChildren)
        {
            string sql = "select * from " + FindDummy(typeof(T)).FullSafeTableName + " " + WhereClauseFromParameters(parameter);
            return GetRecordDataRow(sql, new SqlParameter[] { parameter }, loadChildren);
        }

        protected static DataRow GetRecordDataRow(List<SqlParameter> parameters, GetRecordOperator comparison) { return GetRecordDataRow(parameters.ToArray(), comparison, LOADCHILDRENDEFAULT); }

        protected static DataRow GetRecordDataRow(List<SqlParameter> parameters, GetRecordOperator comparison, bool loadChildren) { return GetRecordDataRow(parameters.ToArray(), comparison, loadChildren); }

        protected static DataRow GetRecordDataRow(SqlParameter[] parameters, GetRecordOperator comparison) { return GetRecordDataRow(parameters, comparison, LOADCHILDRENDEFAULT); }

        protected static DataRow GetRecordDataRow(SqlParameter[] parameters, GetRecordOperator comparison, bool loadChildren)
        {
            string sql = "select * from " + FindDummy(typeof(T)).FullSafeTableName + " " + WhereClauseFromParameters(parameters, comparison);
            return GetRecordDataRow(sql, parameters, loadChildren);
        }

        protected static DataRow GetRecordDataRow(string sql, List<SqlParameter> parameters) { return GetRecordDataRow(sql, parameters.ToArray(), LOADCHILDRENDEFAULT); }

        protected static DataRow GetRecordDataRow(string sql, List<SqlParameter> parameters, bool loadChildren) { return GetRecordDataRow(sql, parameters.ToArray(), loadChildren); }

        protected static DataRow GetRecordDataRow(string sql, SqlParameter[] parameters) { return GetRecordDataRow(sql, parameters, LOADCHILDRENDEFAULT); }

        protected static DataRow GetRecordDataRow(string sql, SqlParameter[] parameters, bool loadChildren) { return GetRecordDataRow(sql, parameters, CommandType.Text, loadChildren); }

        protected static DataRow GetRecordDataRow(string sql, SqlParameter[] parameters, CommandType commandType, bool loadChildren)
        {
            DataTable dt = GetDataTable(sql, parameters, commandType, loadChildren);
            if (dt != null && dt.Rows.Count > 0) return dt.Rows[0];
            return null;
        }

        #endregion



        #region GetRecords Methods (returns Lists of Type T)

        protected static List<T> GetList() { return GetList(LOADCHILDRENDEFAULT); }

        protected static List<T> GetList(bool loadChildren)
        {
            string sql = "select * from " + FindDummy(typeof(T)).FullSafeTableName;
            return GetList(sql, new List<SqlParameter>(), loadChildren);
        }

        protected static List<T> GetList(string fieldName, object value) { return GetList(fieldName, value, LOADCHILDRENDEFAULT); }

        protected static List<T> GetList(string fieldName, object value, bool loadChildren)
        {
            SqlParameter p = new SqlParameter("@P", value);
            p.SourceColumn = fieldName;
            return GetList(p, loadChildren);
        }

        protected static List<T> GetList(SqlParameter parameter) { return GetList(parameter, LOADCHILDRENDEFAULT); }

        protected static List<T> GetList(SqlParameter parameter, bool loadChildren)
        {
            string sql = "select * from " + FindDummy(typeof(T)).FullSafeTableName + " " + WhereClauseFromParameters(parameter);
            return GetList(sql, new SqlParameter[] { parameter }, loadChildren);
        }

        protected static List<T> GetList(List<SqlParameter> parameters, GetRecordOperator comparison) { return GetList(parameters.ToArray(), comparison, LOADCHILDRENDEFAULT); }

        protected static List<T> GetList(List<SqlParameter> parameters, GetRecordOperator comparison, bool loadChildren) { return GetList(parameters.ToArray(), comparison, loadChildren); }

        protected static List<T> GetList(SqlParameter[] parameters, GetRecordOperator comparison) { return GetList(parameters, comparison, LOADCHILDRENDEFAULT); }

        protected static List<T> GetList(SqlParameter[] parameters, GetRecordOperator comparison, bool loadChildren)
        {
            string sql = "select * from " + FindDummy(typeof(T)).FullSafeTableName + " " + WhereClauseFromParameters(parameters, comparison);
            return GetList(sql, parameters, loadChildren);
        }

        protected static List<T> GetList(string sql, List<SqlParameter> parameters) { return GetList(sql, parameters.ToArray(), LOADCHILDRENDEFAULT); }

        protected static List<T> GetList(string sql, List<SqlParameter> parameters, bool loadChildren) { return GetList(sql, parameters.ToArray(), loadChildren); }

        protected static List<T> GetList(string sql, SqlParameter[] parameters) { return GetList(sql, parameters, LOADCHILDRENDEFAULT); }

        protected static List<T> GetList(string sql, SqlParameter[] parameters, bool loadChildren)
        {
            DataTable dt = GetDataTable(sql, parameters, CommandType.Text, loadChildren);
            return ToList(dt, loadChildren);
        }

        #endregion

        #region GetDataTable Methods (returns DataTable)

        protected static DataTable GetDataTable() { return GetDataTable(LOADCHILDRENDEFAULT); }

        protected static DataTable GetDataTable(bool loadChildren)
        {
            string sql = "select * from " + FindDummy(typeof(T)).FullSafeTableName;
            return GetDataTable(sql, new List<SqlParameter>(), loadChildren);
        }

        protected static DataTable GetDataTable(string fieldName, object value) { return GetDataTable(fieldName, value, LOADCHILDRENDEFAULT); }

        protected static DataTable GetDataTable(string fieldName, object value, bool loadChildren)
        {
            SqlParameter p = new SqlParameter("@P", value);
            p.SourceColumn = fieldName;
            return GetDataTable(p, loadChildren);
        }

        protected static DataTable GetDataTable(SqlParameter parameter) { return GetDataTable(parameter, LOADCHILDRENDEFAULT); }

        protected static DataTable GetDataTable(SqlParameter parameter, bool loadChildren)
        {
            string sql = "select * from " + FindDummy(typeof(T)).FullSafeTableName + " " + WhereClauseFromParameters(parameter);
            return GetDataTable(sql, new SqlParameter[] { parameter }, CommandType.Text, loadChildren);
        }

        protected static DataTable GetDataTable(List<SqlParameter> parameters, GetRecordOperator comparison) { return GetDataTable(parameters.ToArray(), comparison, LOADCHILDRENDEFAULT); }

        protected static DataTable GetDataTable(List<SqlParameter> parameters, GetRecordOperator comparison, bool loadChildren) { return GetDataTable(parameters.ToArray(), comparison, loadChildren); }

        protected static DataTable GetDataTable(SqlParameter[] parameters, GetRecordOperator comparison) { return GetDataTable(parameters, comparison, LOADCHILDRENDEFAULT); }

        protected static DataTable GetDataTable(SqlParameter[] parameters, GetRecordOperator comparison, bool loadChildren)
        {
            string sql = "select * from " + FindDummy(typeof(T)).FullSafeTableName + " " + WhereClauseFromParameters(parameters, comparison);
            return GetDataTable(sql, parameters, CommandType.Text, loadChildren);
        }

        protected static DataTable GetDataTable(string sql, List<SqlParameter> parameters) { return GetDataTable(sql, parameters.ToArray(), CommandType.Text, LOADCHILDRENDEFAULT); }

        protected static DataTable GetDataTable(string sql, List<SqlParameter> parameters, bool loadChildren) { return GetDataTable(sql, parameters.ToArray(), CommandType.Text, loadChildren); }

        protected static DataTable GetDataTable(string sql, SqlParameter[] parameters) { return GetDataTable(sql, parameters, CommandType.Text, LOADCHILDRENDEFAULT); }

        protected static DataTable GetDataTable(string sql, SqlParameter[] parameters, CommandType commandType, bool loadChildren)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(SQLDMGlobal.ConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandType = commandType;
                cmd.Parameters.AddRange(parameters);
                cmd.CommandText = sql;

                try
                {

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    cmd.Parameters.Clear();
                }
#pragma warning disable CS0168 // Variable is declared but never used
                catch (Exception ex)
#pragma warning restore CS0168 // Variable is declared but never used
                {
                    throw;
                }
                finally
                {
                    cmd.Parameters.Clear();
                    conn.Close();
                }
            }

            return dt;
        }

        #endregion



        #region ToList Methods - takes a DataTable and converts it to a List<T>

        public static List<T> ToList(DataTable dt)
        {
            return ToList(dt, LOADCHILDRENDEFAULT);
        }

        public static List<T> ToList(DataTable dt, bool loadChildren)
        {
            List<T> records = new List<T>();

            if(dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    T record = new T();
                    if (!FillRecord(row, record, loadChildren)) throw new DataException("Unable to load record");
                    record.IsNew = false;
                    records.Add(record);
                }
            }

            return records;
        }

        #endregion

    }
}
