// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;

namespace StrixMusic.Sdk.AdapterModels;

/// <summary>
/// Provides configuration options for a <see cref="MergedCollectionMap"/>.
/// </summary>
public class MergedCollectionConfig
{
    private MergedCollectionSorting _mergedCollectionSorting;
    private IReadOnlyList<string> _coreRanking = new List<string>();

    /// <summary>
    /// The user's preference for how items in a collection from multiple sources are sorted. 
    /// </summary>
    public MergedCollectionSorting MergedCollectionSorting
    {
        get => _mergedCollectionSorting;
        set => _mergedCollectionSorting = value;
    }

    /// <summary>
    /// The user's preferred ranking for each core, stored as the core's instance ID. Highest ranking first.
    /// </summary>
    public IReadOnlyList<string> CoreRanking
    {
        get => _coreRanking;
        set => _coreRanking = value;
    }

    /// <summary>
    /// Raised when <see cref="CoreRanking"/> is changed.
    /// </summary>
    public event EventHandler<IReadOnlyList<string>>? CoreRankingChanged;

    /// <summary>
    /// Raised when <see cref="MergedCollectionSorting"/> is changed.
    /// </summary>
    public event EventHandler<MergedCollectionSorting>? MergedCollectionSortingChanged;
}
