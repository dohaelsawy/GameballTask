namespace GameballTask.Models;

public class User
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }

    public required string Password { get; set; }
    public string Role { get; set; }  = "customer" ;
}


public class Customer : User {
    public int Points {get;set;}= 0;
    public required List<Transaction?> Transactions { get; set; }
}


public class Transaction {
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public required Customer Customer { get; set; }
    public int PointsBefore { get; set; }
    public int PointsAfter { get; set; }
    public string Status { get; set; } = ""; 
    public DateTime Date { get; set; }
}