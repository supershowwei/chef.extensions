using System;
using System.Data;

namespace Chef.Extensions.Dapper
{
    public interface IRowParserProvider
    {
        Func<IDataReader, T> GetRowParser<T>(string discriminator, IDataReader reader);
    }
}