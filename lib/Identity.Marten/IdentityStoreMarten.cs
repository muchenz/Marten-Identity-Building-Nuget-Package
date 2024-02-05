using JasperFx.CodeGeneration.Frames;
using Marten;
using Marten.Linq.MatchesSql;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace Identity.Marten;

public class UserRole<TKey>
{
    public int Id { get; set; }
    public TKey UserId { get; set; }
    public string RoleId { get; set; }
}

public class MartenUserStore<TUser, TRole, TKey> : IUserStore<TUser>, IUserRoleStore<TUser>
    where TUser : IdentityUser<TKey>
    where TKey : IEquatable<TKey>
    where TRole : IdentityRole
{
    private readonly IDocumentSession _documentSession;
    private readonly IRoleStore<TRole> _roleStore;

    public MartenUserStore(IDocumentSession documentSession, IRoleStore<TRole> roleStore)
    {
        _documentSession = documentSession;
        _roleStore = roleStore;
    }

    public async Task AddToRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(user));
        ArgumentNullException.ThrowIfNull(roleName, nameof(roleName));

        var role = await _roleStore.FindByNameAsync(roleName, cancellationToken);
        if (role is null)
        {
            throw new Exception($"Role name {roleName} doesn't exist");
        }

        _documentSession.Store(new UserRole<TKey>
        {
            UserId = user.Id,
            RoleId = role.Id,

        });

        await _documentSession.SaveChangesAsync(cancellationToken);

    }

    public async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(user));


        _documentSession.Insert(user);
        await _documentSession.SaveChangesAsync(cancellationToken);
        return IdentityResult.Success;
    }

    public async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(user));

        _documentSession.Delete(user);
        await _documentSession.SaveChangesAsync(cancellationToken);
        return IdentityResult.Success;
    }

    public void Dispose()
    {
    }

    public async Task<TUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        if (typeof(TKey) == typeof(string))
        {
            return await _documentSession.Query<TUser>()
                .FirstOrDefaultAsync(a => a.MatchesSql("data ->> 'Id' = ?", userId), token: cancellationToken);

            //    return await _documentSession.Query<TUser>()
            //       .FirstOrDefaultAsync(a => a.Id.Equals(userId), token: cancellationToken);

        }

        if (typeof(TKey) == typeof(Guid))
        {

            if (!Guid.TryParse(userId, out var guidId))
            {
                return null;
            }

            //return await _documentSession.Query<TUser>()
            //    .FirstOrDefaultAsync(a => a.Id.Equals(guidId), token: cancellationToken);

            return await _documentSession.Query<TUser>()
               .FirstOrDefaultAsync(a => a.MatchesSql("data ->> 'Id' = ?", guidId), token: cancellationToken);
        }

        if (typeof(TKey) == typeof(Guid))
        {

            if (!int.TryParse(userId, out var intId))
            {
                return null;
            }

            //return await _documentSession.Query<TUser>()
            //    .FirstOrDefaultAsync(a => a.Id.Equals(intId), token: cancellationToken);

            return await _documentSession.Query<TUser>()
               .FirstOrDefaultAsync(a => a.MatchesSql("data ->> 'Id' = ?", intId), token: cancellationToken);
        }

        if (typeof(TKey) == typeof(Guid))
        {

            if (!long.TryParse(userId, out var longId))
            {
                return null;
            }

            //return await _documentSession.Query<TUser>()
            //    .FirstOrDefaultAsync(a => a.Id.Equals(longId), token: cancellationToken);

            return await _documentSession.Query<TUser>()
               .FirstOrDefaultAsync(a => a.MatchesSql("data ->> 'Id' = ?", longId), token: cancellationToken);

        }

        throw new ArgumentException($"unsuported type key type: {typeof(TKey).FullName}");
    }

    public async Task<TUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {

        return await _documentSession.Query<TUser>()
            .FirstOrDefaultAsync(a => a.NormalizedUserName == normalizedUserName, token: cancellationToken);
    }

    public Task<string?> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.NormalizedUserName);
    }

    public async Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(user));

        var identityRoles = new List<IdentityRole>();

        var userRoles = await _documentSession.Query<UserRole<TKey>>()
            .Include<IdentityRole>(a => a.RoleId, identityRoles.Add)
            .Where(a => a.MatchesSql("data ->> 'UserId' = ?", user.Id))
            .ToListAsync();

        var roleNames = identityRoles.Select(a => a.Name).ToList();

        return roleNames!;

    }

    public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.Id.ToString()!);
    }

    public Task<string?> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.UserName);
    }

    public async Task<IList<TUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(roleName, nameof(roleName));


        var role = await _roleStore.FindByNameAsync(roleName, cancellationToken);
        if (role is null)
        {
            throw new Exception($"Role name {roleName} doesn't exist");
        }

        var userList = new List<TUser>();

        var userRoles = await _documentSession.Query<UserRole<TKey>>()
           .Include<TUser>(a=>a.UserId, userList.Add)
           .Where(a => a.RoleId == role.Id)
           .ToListAsync();

        return userList;

    }

    public async Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(user));
        ArgumentNullException.ThrowIfNull(roleName, nameof(roleName));

        var role = await _roleStore.FindByNameAsync(roleName, cancellationToken);
        
        if (role is null)
        {
            return false;
        }

        var isUserRole = await _documentSession.Query<UserRole<TKey>>()
            .AnyAsync(a => a.RoleId == role.Id && a.MatchesSql("data ->> 'UserId' = ?", user.Id));

        return isUserRole;

    }

    public async Task RemoveFromRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(user));
        ArgumentNullException.ThrowIfNull(roleName, nameof(roleName));

        var role = await _roleStore.FindByNameAsync(roleName, cancellationToken);
        if (role is null)
        {
            throw new Exception($"Role name {roleName} doesn't exist");
        }

        var userRole = await _documentSession.Query<UserRole<TKey>>()
            .FirstOrDefaultAsync(a => a.RoleId == role.Id && a.MatchesSql("data ->> 'UserId' = ?", user.Id));


        if (userRole is null)
        {
            return;
        }

        _documentSession.Delete(userRole);

        await _documentSession.SaveChangesAsync(cancellationToken);
    }

    public Task SetNormalizedUserNameAsync(TUser user, string? normalizedName, CancellationToken cancellationToken)
    {
        user.NormalizedUserName = normalizedName;
        return Task.CompletedTask;
    }

    public Task SetUserNameAsync(TUser user, string? userName, CancellationToken cancellationToken)
    {
        user.UserName = userName;
        return Task.CompletedTask;
    }

    public async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(user));

        _documentSession.Update(user);
        await _documentSession.SaveChangesAsync(cancellationToken);
        return IdentityResult.Success;
    }
}
