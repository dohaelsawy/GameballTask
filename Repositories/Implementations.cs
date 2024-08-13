using Microsoft.EntityFrameworkCore;
using GameballTask.Repositories.Interfaces;
using GameballTask.Models;
using GameballTask.Data;



namespace GameballTask.Repositories.Implementations
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApiDbContext _context;

        public CustomerRepository(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<Customer?> GetCustomerByIdAsync(int id)
        {
            return await _context.Customers.Include(c => c.Transactions).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddCustomerAsync(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
             _context.Customers.Update(customer);
        }
        public async Task DeleteCustomerAsync(Customer customer)
        {
            _context.Customers.Remove(customer);
        }

        public async Task<bool> SaveAsync()
        {
            var changes = await _context.SaveChangesAsync();
            return changes > 0;
        }
    }
}
