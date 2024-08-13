using System.Security.Claims;
using GameballTask.Models;
using GameballTask.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Customer")]
[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CustomerController(ICustomerRepository customerRepository, IHttpContextAccessor httpContextAccessor)
    {
        _customerRepository = customerRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpGet("getMyPoints")]
    public async Task<IActionResult> MyPoints()
    {
        var customerId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var customer = await _customerRepository.GetCustomerByIdAsync(customerId);
        if (customer == null)
        {
            return NotFound();
        }
        return Ok(customer.Points);
    }

    [HttpPost("redeemMyPoints")]
    public async Task<IActionResult> RedeemPoints(int points)
    {
        var customerId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var customer = await _customerRepository.GetCustomerByIdAsync(customerId);
        if (customer == null || customer.Points < points)
        {
            return BadRequest("Not enough points.");
        }

        customer.Points -= points;
        var transaction = new Transaction
        {
            CustomerId = customerId,
            Customer = customer,
            PointsBefore = customer.Points,
            PointsAfter = customer.Points - points,
            Date = DateTime.UtcNow,
            Status = "Pending",
        };
        customer.Points -= points;
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
}