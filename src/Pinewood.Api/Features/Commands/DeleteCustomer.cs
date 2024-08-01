using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pinewood.Api.Common;
using Pinewood.Api.Common.Exceptions;
using Pinewood.Api.Entities;
using Pinewood.Api.Infrastructure.Persistence;

namespace Pinewood.Api.Features.Commands;

public class DeleteCustomerController : ApiControllerBase
{
    [HttpDelete("/api/customers/{customerId}")]
    public async Task<ActionResult> Delete(int customerId)
    {
        await Mediator.Send(new DeleteCustomerCommand { CustomerId = customerId });
        return NoContent();
    }
}

public class DeleteCustomerCommand : IRequest<Unit>
{
    public int CustomerId { get; init; }
}

public class DeleteCustomerCommandValidator : AbstractValidator<DeleteCustomerCommand>
{
    public DeleteCustomerCommandValidator()
    {
        RuleFor(v => v.CustomerId)
            .NotEmpty().WithMessage("Customer Id is required.");
    }
}

internal sealed class DeleteCustomerCommandHandler(DatabaseContext databaseContext)
    : IRequestHandler<DeleteCustomerCommand, Unit>
{
    public async Task<Unit> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        var entity = await databaseContext.Customers.Where(c => c.Id == request.CustomerId)
                .SingleOrDefaultAsync(cancellationToken) ?? throw new NotFoundException(nameof(Customer), request.CustomerId);

        databaseContext.Customers.Remove(entity);
        await databaseContext.SaveChangesAsync(cancellationToken);
        
        return Unit.Value;
    }
}