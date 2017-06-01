﻿// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices.Platforms.uwp.Authentication;

namespace Microsoft.WindowsAzure.MobileServices
{
    internal class MobileServiceUIAuthentication : MobileServicePKCEAuthentication
    {
        internal static AuthenticationHelper CurrentAuthenticator;

        /// <summary>
        /// Instantiates a new instance of <see cref="MobileServiceUIAuthentication"/>.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        /// <param name="provider">
        /// The authentication provider.
        /// </param>
        /// <param name="parameters">
        /// Provider specific extra parameters that are sent as query string parameters to login endpoint.
        /// </param>
        public MobileServiceUIAuthentication(MobileServiceClient client, string provider, string uriScheme, IDictionary<string, string> parameters)
            : base(client, provider, uriScheme, parameters)
        {
        }

        /// <summary>
        /// Provides Login logic by showing a login UI.
        /// </summary>
        /// <returns>
        /// Task that will complete with the response string when the user has finished authentication.
        /// </returns>
        protected override Task<string> GetAuthorizationCodeAsync()
        {
            var tcs = new TaskCompletionSource<string>();

            if (CurrentAuthenticator != null)
            {
                tcs.TrySetException(new InvalidOperationException("Authentication is already in progress."));
                return tcs.Task;
            }

            CurrentAuthenticator = new AuthenticationHelper();

            CurrentAuthenticator.Completed += (o, e) =>
            {
                if (!e.IsAuthenticated)
                {
                    tcs.TrySetException(new InvalidOperationException("Authentication was cancelled by the user."));
                }
                else
                {
                    tcs.TrySetResult(e.AuthorizationCode);
                }
            };

            CurrentAuthenticator.Error += (o, e) =>
            {
                tcs.TrySetException(new Exception(e.Message));
            };

            var browserLaunched = Windows.System.Launcher.LaunchUriAsync(LoginUri);
            //if (!browserLaunched.) {
            //    tcs.TrySetException(new Exception("Could not start browser."));
            //}
            return tcs.Task;
        }
    }
}
