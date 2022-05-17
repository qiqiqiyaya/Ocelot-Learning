// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Models;
using System.Collections.Generic;
using IdentityModel;
using IdentityServer4;

namespace Idp
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            //IdentityResource资源
            //UserInfo 端点身份认证信息
            new IdentityResource[]
            {
                //如果想要下面的预设的claims，就必须加上OpenId（表示openid connect请求的协议）
                new IdentityResources.OpenId(),

                //下面这些就是标准的预设claims
                //想要获取这些claims，用户就需要设置该属性，不设置默认null
                new IdentityResources.Profile(),
                new IdentityResources.Phone(),
                new IdentityResources.Email(),
                new IdentityResources.Address(),
                new IdentityResource("roles","角色",new List<string>(){JwtClaimTypes.Role}),
                new IdentityResource("locations","地点",new List<string>(){"location"}),
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("api1","我的api1资源"),
                //new List<string>{"location"} 意思是，如果api1资源需要location claim,那么这种方式就会在访问api1资源时带上location claim
                //new ApiScope("api1","我的api1资源",new List<string>{"location"}),
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                // m2m client credentials flow client
                new Client
                {
                    ClientId = "console client",
                    ClientName = "Client Credentials Client",

                    //OAutho2.0 中的
                    //准许 类型是 客户端证书，一般来说没有UI界面
                    //简单来说就是自己的其他程序 访问授权服务器 

                    //注意：这种授权方式不代表任务用户，所以发送请求时，你不能获取用户资源，也就是IdentityResource（用户信息资源）
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    //这个就是客户端凭据511536EF-F270-4058-80CA-1C89C192F69A
                    ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                    AllowedScopes = { "api1" }
                },
                new Client
                {
                    ClientId = "winform client",

                    //OAutho2.0 中的
                    //资源所有者的密码凭证（例如用户名和密码）直接被用来请求Access Token
                    //通常用于遗留的应用
                    //资源所有者和客户端应用之间必须保持高度信任
                    //其他授权方式不可用的时候才使用，尽量不用
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    //这个就是客户端凭据511536EF-F270-4058-80CA-1C89C192F69A
                    ClientSecrets = { new Secret("winform secret".Sha256()) },

                    AllowedScopes = { "api1", 
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Phone,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Address,
                        IdentityServerConstants.StandardScopes.Profile
                    }
                }
                ,
                new Client
                {
                    ClientId = "mvc client",
                    ClientName = "Asp.Net Core Mvc Client",
                    //OAutho2.0 中的,然后openid connect 在此之上做了修改或扩展
                    //1.适用于保密客户端（Confidential Client）
                    //   1.1Confidential 机密的
                    //      这种客户端有能力维护其凭据的机密性   （服务器端比较机密，例如asp.net core MVC）
                    //   1.2pubilc
                    //      这种客户端无法维护其凭据的机密性     （位于客户端设备，例如JavaScript、Angular应用、移动端应用等等）
                    //2.服务器端的web应用
                    //3.对用户和客户端进行身份认证
                    
                    AllowedGrantTypes = GrantTypes.Code,
                    //这个就是客户端凭据511536EF-F270-4058-80CA-1C89C192F69A
                    ClientSecrets = { new Secret("mvc secret".Sha256()) },

                    // where to redirect to after login
                    //我tm服了，这里注意http与https
                    //解决方案：配置好客户端的返回地址，
                    //保证AllowedRedirectUris：中的值与Invalid redirect_uri: http://localhost:5000/signin-oidc，相同。
                    RedirectUris = { "http://localhost:5000/signin-oidc" },

                    FrontChannelLogoutUri="http://localhost:5000/signout-oidc",
                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "http://localhost:5000/signout-callback-oidc" },

                    //允许离线访问
                    AllowOfflineAccess=true,
                    //accessToken  过期时间 单位秒
                    AccessTokenLifetime=60,

                    AllowedScopes = { "api1", 
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Phone,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Address,
                    }
                },

                //angular ,implicit flow
                new Client
                {
                    ClientId = "angular-client",
                    ClientName = "Angular SPA 客户端",
                    ClientUri="http://localhost:4200",
                    
                    AllowedGrantTypes=GrantTypes.Implicit,
                    //是否通过浏览器返回
                    AllowAccessTokensViaBrowser=true,
                    //是否需要用户点击同意
                    RequireConsent=true,
                    //token有效期
                    AccessTokenLifetime=60*60*5,

                    //是否需要验证客户端id
                    //RequirePkce = true,
                    //是否需要客户端机密，因为客户端不被信任所以false
                    //RequireClientSecret = false,

                    RedirectUris=
                    {
                        //登录成功有跳转回地址
                        "http://localhost:4200/signin-oidc",
                        //刷新access token地址
                        "http://localhost:4200/redirect-silentrenew"
                    },

                    PostLogoutRedirectUris=
                    {
                        //登出之后跳转地址
                        "http://localhost:4200"
                    },

                    AllowedCorsOrigins=
                    {
                        //允许跨域地址
                        "http://localhost:4200"
                    },

                    AllowedScopes={ "api1", 
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Address,
                        IdentityServerConstants.StandardScopes.Phone,
                        IdentityServerConstants.StandardScopes.Profile
                    }
                }

                ,//mvc Hybrid Flow  
                new Client {
                    ClientId="hybrid client",
                    ClientName="ASP.NET Core",
                    ClientSecrets = { new Secret("hybrid secret".Sha256()) },

                    AllowedGrantTypes=GrantTypes.Hybrid,
                    
                    RedirectUris           = { "http://localhost:5001/signin-oidc" },
                    PostLogoutRedirectUris = { "http://localhost:5001/signout-callback-oidc" },
                    //刷新token
                    AllowOfflineAccess=true,
                    //将UserClaims 加入到IdToken中
                    AlwaysIncludeUserClaimsInIdToken=true,
                    //Mvc Client端ResponseType无论设置“code id_token”, "code token", "code id_token token"中的哪一种，都会报如下错误：
                    //invalid_request
                    //code challenge required
                    //抓包后，发现访问connect/authorize的参数中比Authorization Code Flow时少了一个code_challenge相关的两个参数。
                    //注释掉Config中的RequirePkce即可。这样服务端便不在需要客户端提供code challeng。
                    //这里默认值是true
                    RequirePkce=false,
                    AllowedScopes=
                    { "api1", 
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Address,
                        IdentityServerConstants.StandardScopes.Phone,
                        IdentityServerConstants.StandardScopes.Profile,
                        "roles",
                        "locations"
                    }
                }
            };
    }
}