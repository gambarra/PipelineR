using Microsoft.Extensions.Caching.Memory;
using PipelineR.GettingStarted.Domain;

namespace PipelineR.GettingStarted.Repositories
{
    public class BankRepository : IBankRepository
    {
        private readonly IMemoryCache _repository;

        public BankRepository(IMemoryCache repository)
        {
            _repository = repository;
        }

        public Account Get(int id)
        {
            return _repository.Get<Account>(id);
        }

        public void Insert(Account account)
        {
            _repository.Set(account.Id, account);
        }

        public void Remove(int id)
        {
            _repository.Remove(id);
        }
    }

    public interface IBankRepository
    {
        Account Get(int id);

        void Insert(Account account);

        void Remove(int id);
    }
}