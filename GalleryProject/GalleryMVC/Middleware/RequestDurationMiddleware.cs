namespace GalleryMVC.Middleware
{
    public class RequestDurationMiddleware
    {
        private readonly RequestDelegate _next;
        public RequestDurationMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            await _next(context);

            watch.Stop();
            Console.WriteLine($"== [LATENCY] {context.Request.Path} took {watch.ElapsedMilliseconds}ms");
        }
    }
}
