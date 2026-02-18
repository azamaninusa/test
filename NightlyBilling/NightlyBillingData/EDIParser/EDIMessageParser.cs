using EdiFabric.Core.Model.Edi;
using EdiFabric.Framework.Readers;
using EdiFabric.Framework.Writers;
using System.Text;
using VaxCare.EdiFabric.Templates.Hipaa5010;

namespace NightlyBillingData
{
    public static class EDIMessageParser
    {
        /// <summary>
        /// Parses the <paramref name="inputMessage"/> to an EdiFabric <see cref="TS837P"/> object.
        /// </summary>
        /// <param name="inputMessage">An 837 message in raw string form.</param>
        /// <returns>An EdiFabric 837 message.</returns>
        /// <remarks>837 messages that are sent here need to include the envelope segments (ISA, GS, etc.).</remarks>
        public static IEnumerable<TS837P> Parse837Message(string inputMessage)
        {
            return ParseX12Message<TS837P>(inputMessage);
        }

        /// <summary>
        /// Parses the <paramref name="inputMessage"/> to an EdiFabric <see cref="TS835"/> object.
        /// </summary>
        /// <param name="inputMessage">An 835 message in raw string form.</param>
        /// <returns>An EdiFabric 835 message.</returns>
        /// <remarks>835 messages that are sent here need to include the envelope segments (ISA, GS, etc.).</remarks>
        public static IEnumerable<TS835> Parse835Message(string inputMessage)
        {
            return ParseX12Message<TS835>(inputMessage);
        }

        /// <summary>
        /// Parses the <paramref name="inputMessage"/> to an EdiFabric <see cref="EdiMessage"/> object.
        /// </summary>
        /// <param name="inputMessage">An x12 message in raw string form.</param>
        /// <returns>An EdiFabric x12 message.</returns>
        /// <remarks>x12 messages that are sent here need to include the envelope segments (ISA, GS, etc.).</remarks>
        public static IEnumerable<T> ParseX12Message<T>(string inputMessage) where T : EdiMessage
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputMessage);
            MemoryStream messageStream = new MemoryStream(inputBytes);
            List<IEdiItem> items;
            using (var ediReader = new X12Reader(messageStream, "VaxCare.EdiFabric.Templates"))
            {
                items = ediReader.ReadToEnd().ToList();
            }

            var message = items.OfType<T>();
            return message;
        }

        /// <summary>
        /// Parses the <paramref name="inputMessage"/> to an EdiFabric <see cref="EdiMessage"/> object.
        /// </summary>
        /// <param name="inputMessage">An HL7 message in raw string form.</param>
        /// <returns>An EdiFabric HL7 message.</returns>
        /// <remarks>HL7 messages that are sent here need to include the envelope segments (ISA, GS, etc.).</remarks>
        public static IEnumerable<T> ParseHL7Message<T>(string inputMessage) where T : EdiMessage
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputMessage);
            MemoryStream messageStream = new MemoryStream(inputBytes);
            List<IEdiItem> items;
            using (var ediReader = new Hl7Reader(messageStream, "VaxCare.EdiFabric.Templates"))
            {
                items = ediReader.ReadToEnd().ToList();
            }

            var message = items.OfType<T>();
            return message;
        }

        /// <summary>
        /// Parses the <paramref name="inputMessage"/> to an EdiFabric <see cref="TS837P"/> object.
        /// </summary>
        /// <param name="inputMessage">An x12 message in raw string form.</param>
        /// <returns>An EdiFabric x12 message.</returns>
        /// <remarks>x12 messages that are sent here need to include the envelope segments (ISA, GS, etc.).</remarks>
        public static string WriteX12Message<T>(this T inputMessage) where T : EdiMessage
        {
            var ediString = string.Empty;

            using (MemoryStream memStream = new MemoryStream())
            {
                var settings = new X12WriterSettings
                {
                    Separators =
                {
                    ComponentDataElement = ':'
                }
                };
                using (var ediWriter = new X12Writer(memStream, settings))
                {
                    ediWriter.Write(inputMessage);
                }

                ediString = memStream.LoadToString();
            }

            return ediString;
        }

        public static string LoadToString(this Stream stream)
        {
            stream.Position = 0;
            using (var reader = new StreamReader(stream, Encoding.Default))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
