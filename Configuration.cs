using System.Configuration;

namespace SQLDM.Configuration
{
    public class SQLDataManagerElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("isCurrent", IsRequired = false, DefaultValue = false)]
        public bool IsCurrent
        {
            get { return (bool)this["isCurrent"]; }
            set { this["isCurrent"] = value; }
        }

        [ConfigurationProperty("initialCatalogue", IsRequired = true)]
        public string InitialCatalogue
        {
            get { return (string)this["initialCatalogue"]; }
            set { this["initialCatalogue"] = value; }
        }

        [ConfigurationProperty("integratedSecurity", IsRequired = false, DefaultValue = false)]
        public bool IntegratedSecurity
        {
            get { return (bool)this["integratedSecurity"]; }
            set { this["integratedSecurity"] = value; }
        }

        [ConfigurationProperty("connectionTimeout", IsRequired = false, DefaultValue = 30)]
        [IntegerValidator(ExcludeRange = false, MaxValue = 7200, MinValue = 1)]
        public int ConnectionTimeout
        {
            get { return (int)this["connectionTimeout"]; }
            set { this["connectionTimeout"] = value; }
        }

        [ConfigurationProperty("dataSource", IsRequired = false, DefaultValue = "localhost")]
        public string DataSource
        {
            get { return (string)this["dataSource"]; }
            set { this["dataSource"] = value; }
        }

        [ConfigurationProperty("schema", IsRequired = false, DefaultValue = "dbo")]
        public string Schema
        {
            get { return (string)this["schema"]; }
            set { this["schema"] = value; }
        }

        [ConfigurationProperty("userID", IsRequired = false)]
        public string UserID
        {
            get { return (string)this["userID"]; }
            set { this["userID"] = value; }
        }

        [ConfigurationProperty("password", IsRequired = false)]
        public string Password
        {
            get { return (string)this["password"]; }
            set { this["password"] = value; }
        }

    }

    [ConfigurationCollection(typeof(SQLDataManagerElement))]
    public class SQLDataManagerCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new SQLDataManagerElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SQLDataManagerElement)element).Name;
        }
    }

    public class SQLDataManagerSection : ConfigurationSection
    {
        [ConfigurationProperty("Environments", IsDefaultCollection = true)]
        public SQLDataManagerCollection Environments
        {
            get { return (SQLDataManagerCollection)this["Environments"]; }
            set { this["Environments"] = value; }
        }
    }

    public class SQLDataManager
    {
        public static SQLDataManagerSection _Config = ConfigurationManager.GetSection("SQLDataManager") as SQLDataManagerSection;

        public static SQLDataManagerCollection GetEnvironments() 
        {
            return _Config.Environments;
        }
    }

}