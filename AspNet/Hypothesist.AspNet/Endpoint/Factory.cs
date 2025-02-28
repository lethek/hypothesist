#if NET7_0
using Microsoft.AspNetCore.Http;

namespace Hypothesist.AspNet.Endpoint;

public static class Factory
{
    public static IEndpointFilter When(this IEndpointFilter filter, Predicate<EndpointFilterInvocationContext> when) => 
        new When(when, filter); 
}
#endif