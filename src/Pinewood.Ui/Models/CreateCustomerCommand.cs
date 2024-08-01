namespace Pinewood.Ui.Models;

public record CreateCustomerCommand(string Name, string Email, string PhoneNumber, DateTime DateOfBirth);