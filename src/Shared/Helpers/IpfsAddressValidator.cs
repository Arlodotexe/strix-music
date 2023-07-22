using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace StrixMusic.Helpers
{
    /// <summary>
    /// Validates ipfs addresses.
    /// </summary>
    public class IpfsAddressValidator
    {
        /// <summary>
        /// Identifies whether ipns address is valid or not.
        /// </summary>
        /// <param name="ipnsAddress">The potential ipns address</param>
        /// <returns>The flag representing the state of validation</returns>
        public static bool IsValidIPNS(string ipnsAddress)
        {
            var ipnsPattern = @"^\/ipns\/[a-z0-9]{62}$";

            var regex = new Regex(ipnsPattern);

            return regex.IsMatch(ipnsAddress);
        }

        /// <summary>
        /// Identifies whether ipfs cid address is valid or not.
        /// </summary>
        /// <param name="cid">The potential CID</param>
        /// <returns>The flag representing the state of validation</returns>
        public static bool IsValidCID(string cid)
        {
            var cidPattern = @"^Qm[1-9A-HJ-NP-Za-km-z]{44}$";

            var regex = new Regex(cidPattern);

            return regex.IsMatch(cid);
        }
    }
}
