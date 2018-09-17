﻿using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Acb.WebApi.Test.OAuth
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        public ResourceOwnerPasswordValidator()
        {

        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            //根据context.UserName和context.Password与数据库的数据做校验，判断是否合法
            if (context.UserName == "admin" && context.Password == "123456")
            {
                context.Result = new GrantValidationResult(
                    subject: context.UserName,
                    authenticationMethod: "custom",
                    claims: GetUserClaims());
            }
            else
            {

                //验证失败
                context.Result =
                    new GrantValidationResult(TokenRequestErrors.InvalidGrant, "invalid custom credential");
            }
        }
        //可以根据需要设置相应的Claim
        private Claim[] GetUserClaims()
        {
            return new Claim[]
            {
                new Claim("UserId", 1.ToString()),
                new Claim(JwtClaimTypes.Name,"admin"),
                new Claim(JwtClaimTypes.GivenName, "jaycewu"),
                new Claim(JwtClaimTypes.FamilyName, "yyy"),
                new Claim(JwtClaimTypes.Email, "977865769@qq.com"),
                new Claim(JwtClaimTypes.Role,"admin")
            };
        }
    }
}
