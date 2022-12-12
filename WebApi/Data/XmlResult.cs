namespace WebApi.Data
{
    public class XmlResult<T> : IResult
    {
        private static readonly XmlSerializer _xmlSerializer = new(typeof(T));
        private readonly T _result;
        public XmlResult(T result)
        {
            _result = result;
        }

        public Task ExecuteAsync(HttpContext httpContext)
        {
           using var ms = new MemoryStream();
           _xmlSerializer.Serialize(ms, _result);
            ms.Position = 0;
            return ms.CopyToAsync(httpContext.Response.Body);
        }
    }
}
