using GameballTask.Models;
using GameballTask.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly ICustomerRepository _customerRepository;

    public AdminController(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    [HttpPost("createCustomer")]
    public async Task<IActionResult> CreateCustomer(Customer customer)
    {
        await _customerRepository.AddCustomerAsync(customer);
        await _customerRepository.SaveAsync();
        return Ok(customer);
    }

    [HttpPost("addPoints")]
    public async Task<IActionResult> AddPoints(int customerId, int points)
    {
        var customer = await _customerRepository.GetCustomerByIdAsync(customerId);
        if (customer == null)
        {
            return NotFound();
        }
        var transaction = new Transaction
        {
            CustomerId = customerId,
            Customer = customer,
            PointsBefore = customer.Points,
            PointsAfter = customer.Points + points,
            Date = DateTime.UtcNow,
            Status = "Pending",
        };
        customer.Points += points;
        customer.Transactions.Add(transaction);

        bool isSaved = await _customerRepository.SaveAsync();

        if (!isSaved)
        {
            transaction.Status = "Failed";
            await _customerRepository.SaveAsync();
            return StatusCode(500, "An error occurred while saving the transaction.");
        }

        transaction.Status = "Completed";
        await _customerRepository.SaveAsync();
        return Ok(customer.Points);
    }

    [HttpGet("getAllPointsFor/{customerId}")]
    public async Task<IActionResult> GetPoints(int customerId)
    {
        var customer = await _customerRepository.GetCustomerByIdAsync(customerId);
        if (customer == null)
        {
            return NotFound();
        }
        return Ok(customer.Points);
    }

    [HttpGet("transactionsHistoryFor/{customerId}")]
    public async Task<IActionResult> GetTransactions(int customerId)
    {
        var customer = await _customerRepository.GetCustomerByIdAsync(customerId);
        if (customer == null)
        {
            return NotFound();
        }
        return Ok(customer.Transactions);
    }
}
