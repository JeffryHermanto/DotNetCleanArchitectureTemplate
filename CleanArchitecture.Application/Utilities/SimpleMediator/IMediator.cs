namespace CleanArchitecture.Application.Utilities.SimpleMediator;

public interface IMediator
{
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request);
}