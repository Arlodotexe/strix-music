// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;

namespace StrixMusic.Sdk.AppModels;

/// <summary>
/// Thrown to indicate that an object originating from one version of the Strix Music SDK was provided to a newer version which is not backwards compatible. 
/// </summary>
public class IncompatibleSdkVersionException : InvalidOperationException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IncompatibleSdkVersionException"/> class.
    /// </summary>
    /// <param name="providedSdkVersion"> The version of the SDK provided which isn't compatible with the current running version.</param>
    /// <param name="hintName">A name or identifier associated with the object which caused the exception to be thrown.</param>
    public IncompatibleSdkVersionException(Version providedSdkVersion, string hintName)
    {
        ProvidedSdkVersion = providedSdkVersion;
        HintName = hintName;
    }

    /// <summary>
    /// The version of the SDK provided which isn't compatible with the current running version.
    /// </summary>
    public Version ProvidedSdkVersion { get; }
    
    /// <summary>
    /// A name or identifier associated with the object which caused the exception to be thrown.
    /// </summary>
    public string HintName { get; }
}
