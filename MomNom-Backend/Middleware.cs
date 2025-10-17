namespace MomNom_Backend
{
    public class CustomMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Console.WriteLine("Custom Middleware Executing...");
            //Console.WriteLine(context.Request.Body.Read())
            await _next(context);
            Console.WriteLine("Custom Middleware Finished.");
        }
    }
}
