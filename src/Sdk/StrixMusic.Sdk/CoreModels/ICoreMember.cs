// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

namespace StrixMusic.Sdk.CoreModels
{
    /// <summary>
    /// Properties required by all core-based interfaces that live in an <see cref="ICore"/>.
    /// </summary>
    public interface ICoreMember
    {
        /// <summary>
        /// The <see cref="ICore"/> instance which created this object.
        /// </summary>
        /// <remarks>
        /// This is required to be present on all classes implemented by a core.
        /// </remarks>
        public ICore SourceCore { get; }
    }
}
