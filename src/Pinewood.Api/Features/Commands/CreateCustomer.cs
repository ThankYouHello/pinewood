using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pinewood.Api.Common;
using Pinewood.Api.Entities;
using Pinewood.Api.Infrastructure.Persistence;

namespace Pinewood.Api.Features.Commands;

public class CreateCustomerController : ApiControllerBase
{
    [HttpPost("/api/customers")]
    public async Task<int> Create(CreateCustomerCommand command) => await Mediator.Send(command);
}

public class CreateCustomerCommand : IRequest<int>
{
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public DateTime DateOfBirth { get; init; } = DateTime.MinValue;
}

public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Name is required.");

        RuleFor(c => c.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.");

        RuleFor(c => c.PhoneNumber)
            .NotEmpty().WithMessage("Phone Number is required.")
            .Matches(@"^\d{10}$").WithMessage("Phone number must be 10 digits.");
        
        RuleFor(c => c.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required.")
            .LessThan(DateTime.Now).WithMessage("Date of birth cannot be in the future.")
            .GreaterThan(DateTime.Now.AddYears(-120)).WithMessage("Date of birth cannot be more than 120 years ago.");
    }
}

internal sealed class CreateCustomerCommandHandler(DatabaseContext databaseContext)
    : IRequestHandler<CreateCustomerCommand, int>
{
    public async Task<int> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var entity = new Customer
        {
            Name = request.Name,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            DateOfBirth = request.DateOfBirth
        };

        await databaseContext.Customers.AddAsync(entity, cancellationToken);
        await databaseContext.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}