namespace EDennis.AspNetUtils
{
    /// <summary>
    /// The type of DbContext to use.
    /// </summary>
    public enum DbContextType
    {
        /// <summary>
        /// This is a normal SQL Server DbContext instance with regular (framework-managed)
        /// transactions.  This type of DbContext is fine for testing read-only methods.
        /// </summary>
        SqlServer,

        /// <summary>
        /// This is a SQL Server DbContext with an open transaction that is automatically
        /// rolled back after a test.  This type of DbContext is better for testing methods that
        /// modify data.
        /// </summary>
        SqlServerOpenTransaction,

        /// <summary>
        /// >This is a SQLite in-memory DbContext.  This type of DbContext is suitable for 
        /// testing methods that modify data.
        /// </summary>
        SqliteInMemory
    }
}
