using System;
using OwlCore.AbstractUI.Models;

namespace StrixMusic.Cores.OneDrive.ConfigPanels
{
    internal class DeviceCodeLoginPanel : AbstractUICollection
    {
        /// <summary>
        /// Creates a new instance of <see cref="DeviceCodeLoginPanel"/>.
        /// </summary>
        /// <param name="verificationUri">The Uri that should be navigated to for logging in and entering the code.</param>
        /// <param name="code">The code that should be entered to complete the login process.</param>
        public DeviceCodeLoginPanel(Uri verificationUri, string code)
            : base(nameof(DeviceCodeLoginPanel))
        {
            Title = "Let's login";
            Subtitle = "You'll need your phone or computer";

            AuthenticateButton = new AbstractButton("codeButton", verificationUri.OriginalString)
            {
                Title = $"Go to this URL and enter the code {code}",
                IconCode = "\xE8A7"
            };

            Add(AuthenticateButton);
        }

        /// <summary>
        /// A button that displays the URI to go to for authentication. When clicked, an attempt should be make to redirect the user to this URL.
        /// </summary>
        public AbstractButton AuthenticateButton { get; set; }
    }
}
