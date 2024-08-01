using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pinewood.Api.Common;
using Pinewood.Api.Entities;
using Pinewood.Api.Infrastructure.Persistence;

namespace Pinewood.Api.Features.Queries;

public class GetCustomersController : ApiControllerBase
{
    [HttpGet("/api/customers")]
    public async Task<List<Customer>> Get([FromQuery] GetCustomerQuery query) => await Mediator.Send(query);
}

public class GetCustomerQuery : IRequest<List<Customer>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetCustomerQueryValidator : AbstractValidator<GetCustomerQuery>
{
    public GetCustomerQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("PageNumber at least greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("PageSize at least greater than or equal to 1.");
    }
}

internal sealed class GetCustomerQueryHandler(DatabaseContext databaseContext)
    : IRequestHandler<GetCustomerQuery, List<Customer>>
{
    public async Task<List<Customer>> Handle(GetCustomerQuery request, CancellationToken cancellationToken)
    {
        return await databaseContext.Customers
            .OrderBy(x => x.Id)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);
    }
}