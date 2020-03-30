using AspNetScaffolding.Controllers;
using Microsoft.AspNetCore.Mvc;
using PipelineR.GettingStarted.Models;
using PipelineR.GettingStarted.Workflows.Bank;
using System;

namespace PipelineR.GettingStarted.Controllers
{
    [Route("bank")]
    public class BankController : BaseController
    {
        private readonly IBankPipelineBuilder _bankPipelineBuilder;

        public BankController(IBankPipelineBuilder bankPipelineBuilder)
        {
            _bankPipelineBuilder = bankPipelineBuilder;
        }

        [HttpGet("deposit/{accountKey}/{destinationAccountKey}/{value}")]
        public IActionResult GetDeposit([FromRoute] int accountKey, [FromRoute] int destinationAccountKey, [FromRoute] int value)
        {
            var model = new DepositModel(value, accountKey, destinationAccountKey);
            var response = _bankPipelineBuilder.Deposit(model);
            return new ObjectResult(response.Errors ?? response.Result()) { StatusCode = response.StatusCode };
        }

        [HttpGet("account/{accountKey}")]
        public IActionResult GetAccount([FromRoute] int accountKey)
        {
            var model = new CreateAccountModel()
            {
                Id = accountKey,
                BalanceInCents = 0,
                OwnerName = "Yuri Pereira"
            };

            var response = _bankPipelineBuilder.CreateAccount(model);
            return new ObjectResult(response.Errors ?? response.Result()) { StatusCode = response.StatusCode };
        }
    }
}