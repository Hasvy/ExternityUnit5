using Database;
using ExtUnit5.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ExtUnit5.Security
{
    public class AppAuthenticationStateProvider<TUser> : RevalidatingServerAuthenticationStateProvider
    {
        private readonly UserService _userService;
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

        public AppAuthenticationStateProvider(ILoggerFactory loggerFactory, UserService userService, IDbContextFactory<AppDbContext> dbContextFactory) : base(loggerFactory)
        {
            _userService = userService;
            _dbContextFactory = dbContextFactory;
        }

        private static ClaimsPrincipal AnonymousPrincipal => new ClaimsPrincipal(new ClaimsIdentity(Array.Empty<Claim>(), string.Empty));

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var state = await base.GetAuthenticationStateAsync();
            var dbContext = await _dbContextFactory.CreateDbContextAsync();

            if (state == null)
            {
                return new AuthenticationState(AnonymousPrincipal);
            }
            else
            {
                var userId = state.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId != null && dbContext.Users.Any(u => u.Id == userId))
                {
                    return state;
                }
            }

            return state;
        }

        protected override async Task<bool> ValidateAuthenticationStateAsync(AuthenticationState authenticationState, CancellationToken cancellationToken)
        {
            var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var userId = authenticationState.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return dbContext.Users.Any(u => u.Id == userId);

            return false;
        }

        protected override TimeSpan RevalidationInterval => TimeSpan.FromSeconds(60);
    }
}
