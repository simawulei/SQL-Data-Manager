namespace SQLDM
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Collections.Generic;
    using System.Reflection;

    public delegate void ParentFieldValueChangedEventHandler(object sender, EventArgs e);

    /// <summary>
    /// Represents a single piece of data in a data record
    /// </summary>
    [Serializable]
    public class DataField : IComparable<DataField>, IEquatable<DataField>
    {

        private bool _isPrimaryKey;
        private PropertyInfo _linkValue = null;
        private SqlParameter _originalParam = new SqlParameter();
        private SqlParameter _param;
        private DataRecord _parentRecord = null;


        /// <summary>
        /// Creates a new Field instance
        /// </summary>
        public DataField()
        {
            _param = new SqlParameter();
        }

        /// <summary>
        /// Creates a new Field instance
        /// </summary>
        /// <param name="dbType">The SqlDbType of this field</param>
        /// <param name="sourceColumn">The column nmae as it is in the database</param>
        public DataField(SqlDbType dbType, string sourceColumn)
        {
            _param = new SqlParameter(SQLDMGlobal.PARAMFIRSTCHAR + sourceColumn, dbType);
            _param.SourceColumn = sourceColumn;
        }

        public DataField(PropertyInfo linkedValue, DataRecord parent)
        {
            _param = new SqlParameter();
            _linkValue = linkedValue;
            _parentRecord = parent;
            _param.SourceColumn = _linkValue.Name;
            _param.ParameterName = "@" + _linkValue.Name;
        }



        public event ParentFieldValueChangedEventHandler ParentFieldValue_Changed;

        protected virtual void OnParentFieldValueChanged(EventArgs e)
        {
            if(ParentFieldValue_Changed != null) { ParentFieldValue_Changed(this, e); }
        }



        /// <summary>
        /// A list of child table types that reference this field as their parent
        /// </summary>
        public List<Type> ChildTableTypes { get; set; } = new List<Type>();

        /// <summary>
        /// Gets or sets the flag to indicate if data is present in the field
        /// </summary>
        public bool DataPresent { get; set; }

        /// <summary>
        /// Gets or sets the System.Data.SqlDbType
        /// </summary>
        public DbType DbType
        {
            get
            {
                return _param.DbType;
            }
            set
            {
                _param.DbType = value;
            }
        }

        /// <summary>
        /// Gets a flag indicating if the value is different from its original value
        /// </summary>
        public bool IsDirty
        {
            get
            {
                if (Value == null) return OriginalValue != null;
                if (OriginalValue == null) return Value != null;

                return !Value.ToString().Equals(OriginalValue.ToString());
            }
        }

        /// <summary>
        /// Gets or sets the flag indicating if this field is an identity field in the database
        /// </summary>
        public bool IsIdentity { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the field accepts null values
        /// </summary>
        public bool IsNullable
        {
            get
            {
                return _param.IsNullable;
            }
            set
            {
                _param.IsNullable = value;
            }
        }

        /// <summary>
        /// Gets or sets a flag that indicates if this field is the parent reference for other tables
        /// </summary>
        public bool IsParentField { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the column is the primary key column for its table
        /// </summary>
        public bool IsPrimaryKey
        {
            get
            {
                return _isPrimaryKey;
            }

            set
            {
                _isPrimaryKey = value;
                if (value) { IsNullable = false; }
            }
       }

        /// <summary>
        /// Gets or sets the original value stored in the database prior to any changes
        /// </summary>
        public object OriginalValue
        {
            get
            {
                return _originalParam.SqlValue;
            }
            set
            {
                _originalParam.SqlValue = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the underlying SqlParameter
        /// </summary>
        public string ParameterName
        {
            get
            {
                if (_param.ParameterName.Substring(0, 1) != "@") { _param.ParameterName = "@" + _param.ParameterName; }
                if (_param.ParameterName == "") { _param.ParameterName = "@" + _param.SourceColumn; }
                return _param.ParameterName;
            }
            set
            {
                if (value.Substring(0, 1) != "@") { value = "@" + value; }
                if (value == "") { value = "@" + _param.SourceColumn; }
                _param.ParameterName = value;
            }
        }

        /// <summary>
        /// The table type that this field references as its parent
        /// </summary>
        public Type ParentTableType { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of digits used
        /// </summary>
        public byte Precision
        {
            get
            {
                return _param.Precision;
            }
            set
            {
                _param.Precision = value;
            }
        }

        /// <summary>
        /// This field's property name in code
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Gets the field name enclosed within square brackets []
        /// </summary>
        public string SafeFieldName
        {
            get
            {
                return string.Format("[{0}]", SourceColumn);
            }
        }

        /// <summary>
        /// Gets or sets the number of decimal places to which the value is resolved
        /// </summary>
        public byte Scale
        {
            get
            {
                return _param.Scale;
            }
            set
            {
                _param.Scale = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum size, in bytes, of the data within the column
        /// </summary>
        public int Size
        {
            get
            {
                return _param.Size;
            }
            set
            {
                _param.Size = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the source column in the database table
        /// </summary>
        public string SourceColumn
        {
            get
            {
                return _param.SourceColumn;
            }
            set
            {
                _param.SourceColumn = value;
                if (_param.ParameterName == "") { _param.ParameterName = "@" + value; }
            }
        }

        /// <summary>
        /// Gets or sets the System.Data.SqlDbType
        /// </summary>
        public SqlDbType SqlDbType
        {
            get
            {
                return _param.SqlDbType;
            }
            set
            {
                _param.SqlDbType = value;
            }
        }

        /// <summary>
        /// Gets or sets the value of the field
        /// </summary>
        public object Value
        {
            get
            {
                if (_linkValue != null) return _linkValue.GetValue(_parentRecord);
                return _param.Value;
            }
            set
            {
                if (_linkValue != null) _linkValue.SetValue(_parentRecord, value);

                _param.Value = value;
                if (ParentFieldValue_Changed!= null)  ParentFieldValue_Changed(_parentRecord, null);
            }
        }



        public void ResetDbType()
        {
            _param.ResetDbType();
        }

        public void ResetSqlDbType()
        {
            _param.ResetSqlDbType();
        }

        public override string ToString()
        {
            if (_param.Value == null){ return null; }
            return Convert.ToString(_param.Value);
        }



        public static implicit operator DataField(SqlParameter value)
        {
            DataField newCol = new DataField();
            newCol._param = (SqlParameter)((ICloneable)value).Clone();
            return newCol;
        }

        public static explicit operator SqlParameter(DataField value)
        {
            if (value.Value == null)
                return new SqlParameter
                {
                    ParameterName = value._param.ParameterName,
                    SourceColumn = value.SourceColumn
                };
            SqlParameter newParam = (SqlParameter)((ICloneable)value._param).Clone();
            newParam.Value = value.Value;
            return newParam;
        }

        public static implicit operator int? (DataField value)
        {
            return Convert.ToInt32(value.Value);
        }

        public static implicit operator int (DataField value)
        {
            if (value == null) { return 0; }
            return Convert.ToInt32(value.Value);
        }

        public static implicit operator string(DataField value)
        {
            return Convert.ToString(value.Value);
        }

        public static implicit operator long? (DataField value)
        {
            return Convert.ToInt64(value.Value);
        }

        public static implicit operator long (DataField value)
        {
            if (value.Value == null) { return 0; }
            return Convert.ToInt64(value.Value);
        }

        public static implicit operator byte? (DataField value)
        {
            return Convert.ToByte(value.Value);
        }

        public static implicit operator byte (DataField value)
        {
            if (value.Value == null) { return 0; }
            return Convert.ToByte(value.Value);
        }

        public static implicit operator DateTime? (DataField value)
        {
            return Convert.ToDateTime(value.Value);
        }

        public static implicit operator DateTime (DataField value)
        {
            if (value.Value == null) { return DateTime.MinValue; }
            return Convert.ToDateTime(value.Value);
        }

        public static implicit operator decimal? (DataField value)
        {
            return Convert.ToDecimal(value.Value);
        }

        public static implicit operator decimal (DataField value)
        {
            if (value.Value == null) { return 0; }
            return Convert.ToDecimal(value.Value);
        }

        public static implicit operator bool? (DataField value)
        {
            return Convert.ToBoolean(value.Value);
        }

        public static implicit operator bool (DataField value)
        {
            if (value.Value == null) { return false; }
            return Convert.ToBoolean(value.Value);
        }

        public static implicit operator Single? (DataField value)
        {
            return Convert.ToSingle(value.Value);
        }

        public static implicit operator Single(DataField value)
        {
            if (value.Value == null) { return 0; }
            return Convert.ToSingle(value.Value);
        }

        public static implicit operator double? (DataField value)
        {
            return Convert.ToDouble(value.Value);
        }

        public static implicit operator double (DataField value)
        {
            if (value.Value == null) { return 0; }
            return Convert.ToDouble(value.Value);
        }

        public static implicit operator Guid? (DataField value)
        {
            return new Guid(Convert.ToString(value.Value));
        }

        public static implicit operator Guid (DataField value)
        {
            if (value.Value == null) { return Guid.Empty; }
            return new Guid(Convert.ToString(value.Value));
        }



        public int CompareTo(DataField other)
        {
            return DataField.Compare(this, other);
        }

        public static int Compare(DataField first, DataField second)
        {
            if (Object.ReferenceEquals(first, null))
                return (Object.ReferenceEquals(second, null) ? 0 : -1);

            return first.CompareTo(second);
        }

        public bool Equals(DataField other)
        {
            if (Object.ReferenceEquals(other, null))
                return false;
            if (Object.ReferenceEquals(other, this))
                return true;

            return String.Equals(this, other);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as DataField);
        }

        public override int GetHashCode()
        {
            return this == null ? 0 : this.GetHashCode();
        }

        public static bool operator ==(DataField first, DataField second)
        {
            return Object.Equals(first, second);
        }

        public static bool operator !=(DataField first, DataField second)
        {
            return !Object.Equals(first, second);
        }

        public static bool operator <(DataField first, DataField second)
        {
            return DataField.Compare(first, second) < 0;
        }

        public static bool operator >(DataField first, DataField second)
        {
            return DataField.Compare(first, second) > 0;
        }

        public static bool operator <=(DataField first, DataField second)
        {
            return DataField.Compare(first, second) <= 0;
        }

        public static bool operator >=(DataField first, DataField second)
        {
            return DataField.Compare(first, second) >= 0;
        }


    }
}
