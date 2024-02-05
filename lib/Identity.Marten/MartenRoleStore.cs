using Marten;
using Microsoft.AspNetCore.Identity;

namespace Identity.Marten;

public class MartenRoleStore<TRole> : IRoleStore<TRole> where TRole : IdentityRole
{
    private readonly IDocumentSession _documentSession;

    public MartenRoleStore(IDocumentSession documentSession)
    {
        _documentSession = documentSession;
    }

    public async Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken)
    {

        ArgumentNullException.ThrowIfNull(role, nameof(role));

        _documentSession.Store(role);
        await _documentSession.SaveChangesAsync(cancellationToken);
        return IdentityResult.Success;
    }

    public async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(role, nameof(role));

        _documentSession.Delete(role);
        await _documentSession.SaveChangesAsync(cancellationToken);
        return IdentityResult.Success;
    }

    public void Dispose()
    {

    }

    public async Task<TRole?> FindByIdAsync(string roleId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(roleId, nameof(roleId));

        var role = await _documentSession.LoadAsync<TRole>(roleId, cancellationToken);

        return role;

    }

    public async Task<TRole?> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(normalizedRoleName, nameof(normalizedRoleName));

        var role = await _documentSession.Query<TRole>()
            .FirstOrDefaultAsync(a => a.NormalizedName == normalizedRoleName, token: cancellationToken);

        return role;
    }

    public Task<string?> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken)
    {
        return Task.FromResult(role.NormalizedName);
    }

    public Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken)
    {
        return Task.FromResult(role.Id);
    }

    public Task<string?> GetRoleNameAsync(TRole role, CancellationToken cancellationToken)
    {
        return Task.FromResult(role.Name);
    }

    public Task SetNormalizedRoleNameAsync(TRole role, string? normalizedName, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(role, nameof(role));


        role.NormalizedName = normalizedName;
        //_documentSession.Store(role);

        //await _documentSession.SaveChangesAsync(cancellationToken);

        return Task.CompletedTask;
    }

    public Task SetRoleNameAsync(TRole role, string? roleName, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(role, nameof(role));

        
        role.Name = roleName;
        //_documentSession.Store(role); 

        //await _documentSession.SaveChangesAsync(cancellationToken);
        return Task.CompletedTask;
    }

    public async Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(role, nameof(role));

        _documentSession.Update(role);
        await _documentSession.SaveChangesAsync(cancellationToken);
        return IdentityResult.Success;
    }
}