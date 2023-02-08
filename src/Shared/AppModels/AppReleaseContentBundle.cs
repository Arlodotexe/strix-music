using System;
using System.Collections.Generic;
using OwlCore.Storage;

namespace StrixMusic.AppModels;

/// <summary>
/// Designed to be deserialized from JSON, this class describes a "release bundle" of relative paths from the root to files that should be saved or a common purpose.
/// </summary>
/// <example>
/// A bundle named "docs" that includes all the content needed to view the documentation.
/// </example>
/// <example>
/// A bundle named "web app" that includes all the content needed to run the web app offline.
/// </example>
public class AppReleaseContentBundle
{
    /// <summary>
    /// A unique identifier for this content bundle. Allows for updating the other properties without creating a new bundle.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// A user-friendly display name for this content bundle.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// A user-friendly description of what this bundle is and what it's used for.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// A list of the relative paths from the root release folder. The folders that should be included in this bundle 
    /// </summary>
    public List<string>? RelativePathsToRoot { get; set; }
}
