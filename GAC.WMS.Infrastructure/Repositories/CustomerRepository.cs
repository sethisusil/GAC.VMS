using GAC.WMS.Domain.Entities;
using GAC.WMS.Domain.Repositories;
using GAC.WMS.Infrastructure.Data;

namespace GAC.WMS.Infrastructure.Repositories
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(GacDbContext context) : base(context) { }
    }
}
