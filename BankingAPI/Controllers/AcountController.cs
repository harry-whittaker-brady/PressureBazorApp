using BankingAPI.Abstract;
using BankingAPI.Validation;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace BankingAPI.Controllers
{
    public class AccountController : GenericController<Account, AccountValidator>
    {
        public AccountController(BankingDbContext context, AccountValidator validator)
            : base(context, validator)
        {
        }

        public override IActionResult Post([FromBody] Account entity)
        {
            var bankId = entity?.Bank?.Id ?? default;

            if (bankId == default)
                return BadRequest("Invalid bank Id");

            entity.Bank = Context.Set<Bank>().Where(x => x.Id == bankId).FirstOrDefault();
            return base.Post(entity);
        }
    }
}
