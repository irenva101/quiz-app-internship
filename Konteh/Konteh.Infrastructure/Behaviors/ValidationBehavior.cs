using FluentValidation;
using MediatR;

namespace Konteh.Infrastructure.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : class, IRequest
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResult = await Task.WhenAll(validators.Select(v =>
            v.ValidateAsync(context, cancellationToken))).ConfigureAwait(false);

            var failures = validationResult
                .Where(r => r.Errors.Count > 0)
                .SelectMany(r => r.Errors).ToList();

            if (failures.Count > 0)
                throw new ValidationException(failures);
        }
        return await next().ConfigureAwait(false);
    }
}