using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pinewood.Api.Common;
using Pinewood.Api.Common.Exceptions;
using Pinewood.Api.Entities;
using Pinewood.Api.Infrastructure.Persistence;

namespace Pinewood.Api.Features.Commands;

public class UpdateCustomerController : ApiControllerBase
{
    [HttpPut("/api/customers/{customerId}")]
    public async Task<ActionResult> Update(int customerId, UpdateCustomerCommand command)
    {
        if (customerId != command.CustomerId)
        {
            return BadRequest();
        }

        await Mediator.Send(command);

        return NoContent();
    }
}

public class UpdateCustomerCommand : IRequest<Unit>
{
    public int CustomerId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public DateTime DateOfBirth { get; init; } = DateTime.MinValue;
}

public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerCommandValidator()
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

internal sealed class UpdateCustomerCommandHandler(DatabaseContext databaseContext) : IRequestHandler<UpdateCustomerCommand, Unit>
{
    public async Task<Unit> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var entity = await databaseContext.Customers.Where(c => c.Id == request.CustomerId)
            .SingleOrDefaultAsync(cancellationToken) ?? throw new NotFoundException(nameof(Customer), request.CustomerId);

        entity.Name = request.Name;
        entity.Email = request.Email;
        entity.PhoneNumber = request.PhoneNumber;
        entity.DateOfBirth = request.DateOfBirth;

        await databaseContext.SaveChangesAsync(cancellationToken);
        
        return Unit.Value;
    }
}