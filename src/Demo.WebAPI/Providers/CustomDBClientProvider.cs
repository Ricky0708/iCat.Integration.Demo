using Demo.Shared.Constants;
using iCat.DB.Client.Factory.Interfaces;
using iCat.DB.Client.Implements;
using iCat.DB.Client.Models;
using iCat.Worker.Interfaces;
using Org.BouncyCastle.Crypto.Tls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.WebAPI.Providers
{
    public class CustomDBClientProvider : IDBClientProvider, IJob
    {
        private Dictionary<string, Func<DBClient>> _dbClients = [];

        public string Category => "UpdateDBClients";

        public async Task<object?> DoJob(object? obj)
        {
            var tempDic = new Dictionary<string, Func<DBClient>>();
            tempDic.Add(DBName.CompanyA, () => new iCat.DB.Client.MSSQL.DBClient(new DBClientInfo(DBName.CompanyA, "your connection string for companyA")));
            tempDic.Add(DBName.CompanyB, () => new iCat.DB.Client.MSSQL.DBClient(new DBClientInfo(DBName.CompanyA, "your connection string for companyB")));
            tempDic.Add(DBName.MainDB, () => new iCat.DB.Client.MSSQL.DBClient(new DBClientInfo(DBName.CompanyA, "your connection string for MainDB")));
            _dbClients = tempDic;
            return await Task.FromResult("");
        }

        public Func<DBClient> GetDBClientCreator(string key)
        {
            return _dbClients[key];
        }
    }
}
