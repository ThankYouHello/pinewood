using MediatR;

namespace Pinewood.Api.Common.Behaviours;

public class UnhandledExceptionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception e)
        {
            var requestName = typeof(TRequest).Name;
            
            Console.WriteLine($"Unhandled Exception for Request {requestName}: {e.Message}");
            Console.WriteLine($"Request details: {request}");
            
            throw;
        }
    }
}