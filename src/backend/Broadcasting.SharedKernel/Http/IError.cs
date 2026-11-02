namespace Broadcasting.SharedKernel.Http;

public interface IError<TError> where TError : IError<TError>
{
    static abstract TError Create(int statusCode, string title, string detail);

    static abstract Task<TError?> CreateAsync(HttpResponseMessage response, CancellationToken ct = default);
}
