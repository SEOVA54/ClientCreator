using System.IO;
using System.Text.RegularExpressions;

namespace MulticlientCreator.Helpers
{
    /// <summary>
    /// Outcome of a patch attempt. The IP and the port live in two unrelated places in the client,
    /// so they must be reported separately: patching only one of them yields a client that silently
    /// dials the wrong endpoint while the tool claims success.
    /// </summary>
    public class PatchResult
    {
        public bool IpPatched { get; set; }

        public bool PortPatched { get; set; }

        public int PortTablesPatched { get; set; }

        public bool Success => IpPatched && PortPatched;
    }

    public class HexFinder
    {
        /// <summary>
        /// The client stores its login ports as a table of 7 consecutive little-endian dwords,
        /// framed by a leading 00 byte and 12 trailing zero bytes.
        ///
        /// Matching that structure rather than one hardcoded table matters: retail builds ship
        /// different values (some 4000,4001,4002,4000,4000,4000,4003, others 4000 x7), and an
        /// exact-match pattern silently leaves the port untouched on any build it does not know.
        /// Ports 4000-4008 are 0x0FA0-0x0FA8, i.e. A00F0000..A80F0000 little-endian.
        /// </summary>
        private const string PortTablePattern = "00(?:A[0-8]0F0000){7}000000000000000000000000";

        private readonly string _nostalePath;
        private readonly string _newIp;
        private readonly string _newPort;

        public HexFinder(string nostalePath, string newIp, string newPort)
        {
            _nostalePath = nostalePath;
            _newIp = newIp;
            _newPort = newPort;
        }

        public PatchResult ReplaceIpPattern(string ipPattern)
        {
            byte[] byteData;
            using (var fileStream = new FileStream(_nostalePath, FileMode.Open, FileAccess.Read))
                byteData = DeserializationHelper.ReadFully(fileStream);

            string oldHex = HexHelper.ToHexString(byteData);

            string withIp = oldHex.Replace(ipPattern, _newIp);
            var result = new PatchResult { IpPatched = withIp != oldHex };

            int tables = Regex.Matches(withIp, PortTablePattern).Count;
            string newHex = Regex.Replace(withIp, PortTablePattern, _newPort);

            result.PortTablesPatched = tables;
            result.PortPatched = tables > 0 && newHex != withIp;

            if (newHex == oldHex)
                return result;

            byte[] newByteData = HexHelper.ToByteArray(newHex);
            using (var writer = new FileStream(_nostalePath, FileMode.Create, FileAccess.Write))
                writer.Write(newByteData, 0, newByteData.Length);

            return result;
        }
    }
}
