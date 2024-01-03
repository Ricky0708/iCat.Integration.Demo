using Demo.Shared.Constants;
using iCat.DB.Client.Factory.Interfaces;
using iCat.DB.Client.Implements;
using iCat.DB.Client.Models;
using iCat.Worker.Interfaces;
using Microsoft.Data.SqlClient;
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
            tempDic.Add(DBName.CompanyA, () => new DBClient(new DBClientInfo(DBName.CompanyA, new SqlConnection("your connection string for companyA"))));
            tempDic.Add(DBName.CompanyB, () => new DBClient(new DBClientInfo(DBName.CompanyA, new SqlConnection("your connection string for companyB"))));
            tempDic.Add(DBName.MainDB, () => new DBClient(new DBClientInfo(DBName.CompanyA, new SqlConnection("server=192.168.1.3\\SQL2019;user id=sa;password= P@ssw0rd;initial catalog=A"))));
            _dbClients = tempDic;
            return await Task.FromResult("");
        }

        public Func<DBClient> GetDBClientCreator(string key)
        {
            return _dbClients[key];
        }
    }
}
