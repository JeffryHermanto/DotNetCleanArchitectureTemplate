namespace CleanArchitecture.Application.Utilities.SimpleMediator;

public interface IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request);
}