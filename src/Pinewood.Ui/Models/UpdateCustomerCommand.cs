namespace Pinewood.Ui.Models;

public record UpdateCustomerCommand(int CustomerId, string Name, string Email, string PhoneNumber, DateTime DateOfBirth);