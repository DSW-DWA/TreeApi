using TreeApi.Data;
using TreeApi.Models;

namespace TreeApi.Errors
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _next = next;
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (SecureException ex)
            {
                await HandleSecureExceptionAsync(context, ex);
            }
            catch (Exception ex)
            {
                await HandleGeneralExceptionAsync(context, ex);
            }
        }

        private async Task HandleSecureExceptionAsync(HttpContext context, SecureException ex)
        {
            var eventId = Guid.NewGuid();
            await LogExceptionAsync(ex, eventId, context);

            var response = new
            {
                type = "Secure",
                id = eventId.ToString(),
                data = new { message = ex.Message }
            };

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(response);
        }

        private async Task HandleGeneralExceptionAsync(HttpContext context, Exception ex)
        {
            var eventId = Guid.NewGuid();
            await LogExceptionAsync(ex, eventId, context);

            var response = new
            {
                type = "Exception",
                id = eventId.ToString(),
                data = new { message = $"Internal server error ID = {eventId}" }
            };

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(response);
        }

        private async Task LogExceptionAsync(Exception ex, Guid eventId, HttpContext context)
        {
            var queryParams = context.Request.QueryString.Value;
            var bodyParams = await new StreamReader(context.Request.Body).ReadToEndAsync();

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var exceptionLog = new ExceptionJournal
                {
                    Id = eventId,
                    Timestamp = DateTime.UtcNow,
                    QueryParams = queryParams,
                    BodyParams= bodyParams,
                    StackTrace = ex.StackTrace ?? string.Empty,
                    ExceptionType = ex.GetType().Name,
                    ExceptionMessage = ex.Message
                };

                dbContext.ExceptionJournals.Add(exceptionLog);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
