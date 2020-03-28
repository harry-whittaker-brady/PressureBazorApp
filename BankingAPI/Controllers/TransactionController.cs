using Domain.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using BankingAPI.Abstract;
using BankingAPI.Validation;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace BankingAPI.Controllers
{
    [EnableCors()]
    public class TransactionController : GenericController<Transaction, TransactionValidator>
    {
        public TransactionController(BankingDbContext context, TransactionValidator validator)
            : base(context, validator)
        {
        }

        public IActionResult Post(List<IFormFile> files)
        {
            Account account;
            try
            {
                account = GetAccount();
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }            

            foreach (var file in files)
            {
                Console.WriteLine($"Importing file : {file.FileName}");
                List<Transaction> transactions = new List<Transaction>();

                using (var stream = file.OpenReadStream())
                {
                    transactions.AddRange(CsvReader.Reader.ReadFileStream(stream));
                }                

                for (int i = 0; i < transactions.Count; i++)
                {
                    transactions[i].Account = account;
                    transactions[i].Bank = account.Bank;
                    transactions[i] = SetCreatedAndUpdatedTimes(transactions[i], DateTimeOffset.Now);

                    if (string.IsNullOrEmpty(transactions[i].Classification))
                        transactions[i].Classification = "NotClassified";
                }
                Repository.AddRange(transactions);
                Context.SaveChanges();
            }
            return Ok();
        }

        Account GetAccount()
        {
            var accountIdHeader = Request.Headers["AccountId"];

            if (string.IsNullOrEmpty(accountIdHeader))
                throw new Exception("Account id header can not be empty");

            if (!long.TryParse(accountIdHeader, out var accountId))
                throw new Exception($"Invalid account id : {accountIdHeader}");

            var account = Context.Set<Account>().Include(x => x.Bank)
                                .Where(x => x.Id == accountId).FirstOrDefault();

            if (account is null)
                throw new Exception($"An account with the id of '{accountId}' count not be found");

            return account;
        }
    }
}
