using System;
using Microsoft.Identity.Client;

/// <summary>
/// Used to propogate a created instance of <see cref="AcquireTokenInteractiveParameterBuilder" /> via an event handler and allow for modifications before the application uses it.
/// </summary>
public class AcquireTokenInteractiveParameterBuilderCreatedEventArgs : EventArgs
{
    /// <summary>
    /// Creates a new instance of <see cref="AcquireTokenInteractiveParameterBuilderCreatedEventArgs" />.
    /// </summary>
    public AcquireTokenInteractiveParameterBuilderCreatedEventArgs( AcquireTokenInteractiveParameterBuilder builder)
    {
        Builder = builder;
    }

    /// <summary>
    /// The created instance of <see cref=" AcquireTokenInteractiveParameterBuilder" /> that will be used by the application.
    /// </summary>
    /// <remarks>
    /// Can be read and reassigned. The assigned value will be used by the application when all synchronous event handlers have finished being raised.
    /// </remarks>
    public  AcquireTokenInteractiveParameterBuilder Builder { get; set; }
}