// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Collections.ObjectModel;
using System.ComponentModel;
using OwlCore.AbstractUI.Models;
using OwlCore.AbstractUI.ViewModels;

namespace StrixMusic.Sdk.ViewModels.Notifications
{
    /// <summary>
    /// Wraps an <see cref="AbstractUICollection"/> intended for Notifications and parses supported elements for rendering.
    /// </summary>
    [Bindable(true)]
    public sealed class AbstractUINotificationViewModel : AbstractUICollectionViewModel
    {
        private const int MAX_BUTTONS = 3;
        private readonly AbstractUICollection _model;

        /// <inheritdoc />
        public AbstractUINotificationViewModel(AbstractUICollection model)
            : base(model)
        {
            _model = model;

            EvaluateSupportedItems();
        }

        private void EvaluateSupportedItems()
        {
            foreach (var value in _model)
            {
                if (value is AbstractProgressIndicator progressUIElement && ProgressIndicator is null)
                {
                    ProgressIndicator = new AbstractProgressIndicatorViewModel(progressUIElement);
                }
                else if (value is AbstractButton buttonElement && Buttons.Count < MAX_BUTTONS)
                {
                    Buttons.Add(new AbstractButtonViewModel(buttonElement));
                }
                else
                {
                    UnsupportedItems.Add(value);
                }
            }
        }

        /// <summary>
        /// A list of <see cref="AbstractUIBase"/>s were given to the notification that aren't explicitly supported.
        /// The can be displayed in popout window if desired.
        /// </summary>
        public ObservableCollection<AbstractUIBase> UnsupportedItems { get; set; } = new ObservableCollection<AbstractUIBase>();

        /// <summary>
        /// An <see cref="AbstractProgressIndicator"/> that may appear in a notification.
        /// </summary>
        public AbstractProgressIndicatorViewModel? ProgressIndicator { get; set; }

        /// <summary>
        /// A list of <see cref="AbstractButton"/>s that may appear in a notification
        /// </summary>
        public ObservableCollection<AbstractButtonViewModel> Buttons { get; set; } = new ObservableCollection<AbstractButtonViewModel>();
    }
}
