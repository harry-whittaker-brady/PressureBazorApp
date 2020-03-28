using Domain.Abstract;
using Domain.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace Domain.Models
{
    public class Account : Entity
    {
        public string Name { get; set; }
        public Bank Bank { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public TransactionType AccountType { get; set; }
        public decimal Balance { get; set; }
        public IEnumerable<Transaction> Transactions { get; set; }
    }
}
