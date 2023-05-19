// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal
{
    internal class FakeAccessTokenProviderAccessor : IAccessTokenProviderAccessor
    {
        private readonly IServiceProvider _provider;
        private IAccessTokenProvider _tokenProvider;

        public FakeAccessTokenProviderAccessor(IServiceProvider provider) => _provider = provider;

        public IAccessTokenProvider TokenProvider => _tokenProvider ??= _provider.GetRequiredService<IAccessTokenProvider>();
    }
}