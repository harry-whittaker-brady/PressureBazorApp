using BlazorApp.Abstract;
using BlazorApp.Models;
using Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorApp.Services
{
    public class TransactionService : GenericService<Transaction>
    {
        public TransactionService(HostConfiguration hostConfiguration) : base(hostConfiguration)
        {
        }

        public async Task<List<string>> GetClassificationsAsync()
        {
            var odataQueryBuilder = new OdataQueryBuilder(ApiUrl);
            odataQueryBuilder.Select.Add(nameof(Transaction.Classification));

            var response = await base.GetEntitiesAsync();

            var classifications = new List<string>() { "" };

            foreach (var category in response.Entities.Select(x => x.Classification))
            {
                if (!classifications.Contains(category))
                    classifications.Add(category);
            }
            return classifications;
        }

        public override OdataQueryBuilder GetQueryBuilder()
        {
            var builder = base.GetQueryBuilder();
            builder.Expand.Add(nameof(Transaction.Account));
            builder.Expand.Add(nameof(Transaction.Bank));
            return builder;
        }
    }
}
