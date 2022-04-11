using System;
using Microsoft.Identity.Client;

/// <summary>
/// Used to propogate a created instance of <see cref="MsalPublicClientApplicationBuilderCreated" /> via an event handler and allow for modifications before the application uses it.
/// </summary>
public class MsalPublicClientApplicationBuilderCreatedEventArgs : EventArgs
{
    /// <summary>
    /// Creates a new instance of <see cref="MsalPublicClientApplicationBuilderCreatedEventArgs" />.
    /// </summary>
    public MsalPublicClientApplicationBuilderCreatedEventArgs(PublicClientApplicationBuilder builder)
    {
        Builder = builder;
    }

    /// <summary>
    /// The created instance of <see cref="PublicClientApplicationBuilder" /> that will be used by the application.
    /// </summary>
    /// <remarks>
    /// Can be read and reassigned. The assigned value will be used by the application when all synchronous event handlers have finished being raised.
    /// </remarks>
    public PublicClientApplicationBuilder Builder { get; set; }
}