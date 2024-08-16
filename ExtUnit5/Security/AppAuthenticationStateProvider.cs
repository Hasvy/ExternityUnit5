using Database;
using ExtUnit5.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static Plotly.NET.StyleParam.DrawingStyle;

namespace ExtUnit5.Security
{
    public class AppAuthenticationStateProvider<TUser> : RevalidatingServerAuthenticationStateProvider
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

        public AppAuthenticationStateProvider(ILoggerFactory loggerFactory, IDbContextFactory<AppDbContext> dbContextFactory) : base(loggerFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        private static ClaimsPrincipal AnonymousPrincipal => new ClaimsPrincipal(new ClaimsIdentity(Array.Empty<Claim>(), string.Empty));

        protected override TimeSpan RevalidationInterval => TimeSpan.FromSeconds(60);

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var state = await base.GetAuthenticationStateAsync();

            if (!await ValidateAuthenticationStateAsync(state, CancellationToken.None))
            {
                return await Task.FromResult(new AuthenticationState(AnonymousPrincipal));
            }

            return await Task.FromResult(new AuthenticationState(state.User));
        }

        protected override async Task<bool> ValidateAuthenticationStateAsync(AuthenticationState authenticationState, CancellationToken cancellationToken)
        {
            var dbContext = await _dbContextFactory.CreateDbContextAsync();

            var userName = authenticationState.User.FindFirst(ClaimTypes.Name)?.Value;
            if (userName != null)
                return dbContext.Users.Any(u => u.UserName == userName);

            return false;
        }
    }
}
