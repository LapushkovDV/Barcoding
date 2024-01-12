using ERP_Admin_Panel.Services.Token;
using Microsoft.AspNetCore.Components.ProtectedBrowserStorage;

namespace ERP_Admin_Panel.Services.Account
{
    public class AccountManager : IAccountManager
    {
        private readonly ProtectedSessionStorage _sessionStorage;

        public AccountManager(ProtectedSessionStorage protectedSessionStorage) => _sessionStorage = protectedSessionStorage;

        public async Task<int> GetRoleId()
        {
            if (_sessionStorage != null)
            {
                var token = await _sessionStorage.GetAsync<SecurityToken>(nameof(SecurityToken));

                if (token.Value != null)
                {
                    return token.Value.RoleId;
                }
            }

            return -1;
        }

        public async Task<string> GetRoleName()
        {
            if (_sessionStorage != null)
            {
                var token = await _sessionStorage.GetAsync<SecurityToken>(nameof(SecurityToken));

                if (token.Value != null)
                {
                    return token.Value.Role;
                }
            }

            return string.Empty;
        }

        public async Task<int> GetUserId()
        {
            if (_sessionStorage != null)
            {
                var token = await _sessionStorage.GetAsync<SecurityToken>(nameof(SecurityToken));

                if (token.Value != null)
                {
                    return token.Value.UserId;
                }
            }

            return -1;
        }

        public async Task<string> GetUserName()
        {
            if (_sessionStorage != null)
            {
                var token = await _sessionStorage.GetAsync<SecurityToken>(nameof(SecurityToken));

                if (token.Value != null)
                {
                    return token.Value.UserName;
                }
            }

            return string.Empty;
        }
    }
}
