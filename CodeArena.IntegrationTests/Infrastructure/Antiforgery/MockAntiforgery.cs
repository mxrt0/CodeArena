using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.IntegrationTests.Infrastructure.Antiforgery;

public class MockAntiforgery : IAntiforgery
{
    public AntiforgeryTokenSet GetAndStoreTokens(HttpContext httpContext)
     => new AntiforgeryTokenSet("test", "test", "test", "test");

    public AntiforgeryTokenSet GetTokens(HttpContext httpContext)
     => new AntiforgeryTokenSet("test", "test", "test", "test");

    public Task<bool> IsRequestValidAsync(HttpContext httpContext)
    => Task.FromResult(true);

    public void SetCookieTokenAndHeader(HttpContext httpContext)
    {
        
    }
    public Task ValidateRequestAsync(HttpContext httpContext)
    => Task.CompletedTask;
}
