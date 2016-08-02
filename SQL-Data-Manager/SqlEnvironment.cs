namespace SQLDM
{

    using System.Data.SqlClient;

    /// <summary>
    /// Contains all information required to connect to an instance of SQL Server
    /// </summary>
    public class SqlEnvironment
    {

        private SqlConnectionStringBuilder ConnectionStringBuilder { get; set; } = new SqlConnectionStringBuilder();

        /// <summary>
        /// Creates a new SQLEnvironment
        /// </summary>
        /// <param name="environmentName">The name of the environment to be created</param>
        public SqlEnvironment(string environmentName)
        {
            EnvironmentName = environmentName;
        }

        /// <summary>
        /// Creates a new SQLEnvironment
        /// </summary>
        /// <param name="environmentName">The name of the environment to be created</param>
        /// <param name="connectionString">The connection string that this environment will use</param>
        public SqlEnvironment(string connectionString, string environmentName)
        {
            ConnectionStringBuilder.ConnectionString = connectionString;
            EnvironmentName = environmentName;
        }

        /// <summary>
        /// Gets or sets the name by which to identify the environment
        /// </summary>
        public string EnvironmentName { get; set; }

        /// <summary>
        /// Gets or sets a flag to define is this is the current environment in use
        /// </summary>
        public bool IsCurrent { get; set; } = false;

        /// <summary>
        /// Gets the connection string this environment uses
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return ConnectionStringBuilder.ConnectionString;
            }
        }

        /// <summary>
        /// Connection timeout limit in seconds
        /// </summary>
        public int ConnectionTimeout
        {
            get
            {
                return ConnectionStringBuilder.ConnectTimeout;
            }
            set
            {
                ConnectionStringBuilder.ConnectTimeout = value;
            }
        }

        /// <summary>
        /// Gets or sets the name or network address of the instance of SQL Server to connect to
        /// </summary>
        public string DataSource
        {
            get
            {
                return ConnectionStringBuilder.DataSource;
            }
            set
            {
                ConnectionStringBuilder.DataSource = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the database associated with the connection
        /// </summary>
        public string InitialCatalog
        {
            get
            {
                return ConnectionStringBuilder.InitialCatalog;
            }
            set
            {
                ConnectionStringBuilder.InitialCatalog = value;
            }
        }

        /// <summary>
        /// Gets or sets a Boolean value that indicates whether User ID and Password are
        /// specified in the connection (when false) or whether the current Windows account
        /// credentials are used for authentication (when true)
        /// </summary>
        public bool IntegratedSecurity
        {
            get
            {
                return ConnectionStringBuilder.IntegratedSecurity;
            }
            set
            {
                ConnectionStringBuilder.IntegratedSecurity = value;
            }
        }

        /// <summary>
        /// Gets or sets the password for the SQL Server account
        /// </summary>
        public string Password
        {
            get
            {
                return ConnectionStringBuilder.Password;
            }
            set
            {
                ConnectionStringBuilder.Password = value;
            }
        }

        /// <summary>
        /// Gets or sets the default schema used by this connection
        /// </summary>
        public string Schema { get; set; }

        /// <summary>
        /// Gets or sets the user ID to be used when connecting to SQL Server
        /// </summary>
        public string UserID
        {
            get
            {
                return ConnectionStringBuilder.UserID;
            }
            set
            {
                ConnectionStringBuilder.UserID = value;
            }
        }




    }
}
