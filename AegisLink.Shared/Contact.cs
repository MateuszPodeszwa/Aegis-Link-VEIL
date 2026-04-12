using System;
using System.Collections.Generic;
using System.Text;

namespace AegisLink.Shared
{
    public class Contact
    {
        public string AegisId { get; set; } = string.Empty;
        public string PublicKey { get; set; } = string.Empty;
        public string Nickname { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public string Colour { get; set; } = string.Empty;
    }
}
