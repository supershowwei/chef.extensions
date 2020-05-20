namespace Chef.Extensions.DbAccess
{
    public interface IDataAccessFactory
    {
        IDataAccess<T> Create<T>();

        IDataAccess<T> Create<T>(string nameOrConnectionString);

        void AddConnectionString(string name, string value);
    }
}