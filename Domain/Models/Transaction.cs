using Domain.Abstract;
using Domain.Enums;
using Newtonsoft.Json.Converters;
using System;
using System.Text.Json.Serialization;

namespace Domain.Models
{
    public class Transaction : Entity
    {
        public string Description { get; set; }
        public Account Account { get; set; }
        public Bank Bank { get; set; }
        public DateTimeOffset Date { get; set; }
        public decimal Amount { get; set; }
        public string Classification { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public TransactionType TransactionType { get; set; }
        public string Location { get; set; }
    }
}
