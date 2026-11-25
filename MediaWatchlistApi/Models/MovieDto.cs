using FluentValidation;

namespace MediaWatchlistApi.Dtos
{
    public record MovieCreateDto (
        bool Watched,
        double Rating,
        string Comment = "",
        string Name = ""
    );
    public class MovieCreateDtoValidator : AbstractValidator<MovieCreateDto>
    {
        public MovieCreateDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MinimumLength(2).MaximumLength(100);
            RuleFor(x => x.Comment).NotNull().MinimumLength(2).MaximumLength(500);
            RuleFor(x => x.Rating).NotEmpty().InclusiveBetween(0, 5);
        }
    }
    public record MovieUpdateDto(
        string? Name = null,
        bool? Watched = null,
        double? Rating = null,
        string? Comment = null
    );
    public class MovieUpdateDtoValidator : AbstractValidator<MovieUpdateDto>
    {
        public MovieUpdateDtoValidator()
        {
            RuleFor(x => x.Name).MinimumLength(2).MaximumLength(100);
            RuleFor(x => x.Comment).MinimumLength(2).MaximumLength(500);
            RuleFor(x => x.Rating).InclusiveBetween(0, 5);
        }
    }
    public record ApiError(string Code, string Message);

    public record ApiResponseSuccess<T>(
        bool Success = true,
        string? Message = null,
        T? Data = default
    )
    {
        public static ApiResponseSuccess<T> Ok(T data, string? message = null) =>
            new(true, message, data);
    }
    public record ApiResponseError(
        bool Success = false,
        string? Message = null,
        IReadOnlyList<ApiError>? Errors = null,
        string? TraceId = null)
    {
        public static ApiResponseError Fail(string message, IReadOnlyList<ApiError>? errors = null, string? traceId = null) =>
            new(false, message, errors, traceId);
    }
}