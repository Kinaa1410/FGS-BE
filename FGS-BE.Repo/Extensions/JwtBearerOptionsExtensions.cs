using FGS_BE.Repo.Exceptions;
using FGS_BE.Repo.Resources;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace FGS_BE.Repo.Extensions;

public static class JwtBearerOptionsExtensions
{
    public static void HandleEvents(this JwtBearerOptions options)
    {
        options.Events = new JwtBearerEvents
        {
            OnForbidden = context =>
            {
                throw new ForbiddenAccessException();
            },

            OnChallenge = context =>
            {
                throw new UnauthorizedAccessException(Resource.Unauthorized);
            },

            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                if (!string.IsNullOrEmpty(accessToken))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    }
}