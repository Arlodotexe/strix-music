using System;
using OwlCore.AbstractUI.Models;

namespace StrixMusic.Cores.OneDrive.ConfigPanels
{
    internal class DeviceCodeLoginPanel : AbstractUICollection
    {
        private AbstractProgressIndicator _loadingIndicator = new(nameof(DeviceCodeLoginPanel), true)
        {
            Title = "Please wait..."
        };

        private Uri? _verificationUri;
        private string? _code;

        /// <summary>
        /// Creates a new instance of <see cref="DeviceCodeLoginPanel"/>.
        /// </summary>
        /// <param name="verificationUri">The Uri that should be navigated to for logging in and entering the code.</param>
        /// <param name="code">The code that should be entered to complete the login process.</param>
        public DeviceCodeLoginPanel(Uri? verificationUri = null, string? code = null)
            : base(nameof(DeviceCodeLoginPanel))
        {
            Title = "Let's login";
            Subtitle = "You'll need your phone or computer";
            AuthenticateButton = new AbstractButton("codeButton", verificationUri?.OriginalString ?? "...")
            {
                Title = string.IsNullOrWhiteSpace(code) ? "..." : $"Go to this URL and enter the code {code}",
                IconCode = "\xE8A7"
            };

            VerificationUri = verificationUri;
            Code = code;

            if (VerificationUri is null && Code is null)
                Add(_loadingIndicator);
            else
                Add(AuthenticateButton);
        }

        /// <summary>
        /// The Uri that should be navigated to for logging in and entering the code.
        /// </summary>
        public Uri? VerificationUri
        {
            get => _verificationUri;
            set
            {
                _verificationUri = value;
                AuthenticateButton.Text = value?.OriginalString ?? "...";

                if (value is not null && Contains(_loadingIndicator))
                    Remove(_loadingIndicator);

                if (value is not null && !Contains(AuthenticateButton))
                    Add(AuthenticateButton);
            }
        }

        /// <summary>
        /// The code that should be entered to complete the login process.
        /// </summary>
        public string? Code
        {
            get => _code;
            set
            {
                _code = value;
                AuthenticateButton.Title = $"Go to this URL and enter the code {value}";

                if (value is not null && Contains(_loadingIndicator))
                    Remove(_loadingIndicator);

                if (value is not null && !Contains(AuthenticateButton))
                    Add(AuthenticateButton);
            }
        }

        /// <summary>
        /// A button that displays the URI to go to for authentication. When clicked, an attempt should be make to redirect the user to this URL.
        /// </summary>
        public AbstractButton AuthenticateButton { get; set; }
    }
}
