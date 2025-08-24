﻿using URL_Shortener.Application.Exceptions;

namespace URL_Shortener.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                context.Response.StatusCode = e switch
                {
                    InvalidArgumentException => StatusCodes.Status422UnprocessableEntity,
                    ArgumentNullException => StatusCodes.Status422UnprocessableEntity,
                    NotFoundException => StatusCodes.Status404NotFound,
                    AlreadyExistsException => StatusCodes.Status409Conflict,
                    MethodAccessException => StatusCodes.Status405MethodNotAllowed,
                    _ => StatusCodes.Status500InternalServerError
                };
                await context.Response.WriteAsJsonAsync(new { error = e.Message });
            }
        }
    }
}
