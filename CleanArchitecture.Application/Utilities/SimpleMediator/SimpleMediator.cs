using CleanArchitecture.Application.Utilities.SimpleMediator;
using Microsoft.Extensions.DependencyInjection;

public class SimpleMediator(IServiceProvider serviceProvider) : IMediator
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request)
    {
        var handlerType = typeof(IRequestHandler<,>)
            .MakeGenericType(request.GetType(), typeof(TResponse));

        var handler = _serviceProvider.GetRequiredService(handlerType);

        return ((dynamic)handler).Handle((dynamic)request);
    }
}