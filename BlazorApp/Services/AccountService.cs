using BlazorApp.Abstract;
using BlazorApp.Models;
using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorApp.Services
{
    public class AccountService : GenericService<Account>
    {
        public AccountService(HostConfiguration hostConfiguration) : base(hostConfiguration)
        {
        }

        public override OdataQueryBuilder GetQueryBuilder()
        {
            var builder = base.GetQueryBuilder();
            builder.Expand.Add(nameof(Account.Bank));
            return builder;
        }

        public override Task<APIResponse<Account>> GetEntitiesAsync(OdataQueryBuilder odataQueryBuilder = null)
        {
            var builder = GetQueryBuilder();
            return base.GetEntitiesAsync(builder);
        }

        public override Task<bool> AddEntityAsync(Account entity)
        {
            entity.Bank.Accounts = new List<Account>();
            entity.Transactions = new List<Transaction>();
            return base.AddEntityAsync(entity);
        }
    }
}
