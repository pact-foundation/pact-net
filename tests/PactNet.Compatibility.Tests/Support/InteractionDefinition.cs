using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Reqnroll;

namespace PactNet.Compatibility.Tests.Support
{
    /// <summary>
    /// Request/response body described by a compatibility suite table cell
    /// </summary>
    internal sealed class BodyDefinition
    {
        public BodyDefinition(string contentType, byte[] bytes)
        {
            this.ContentType = contentType;
            this.Bytes = bytes;
        }

        public string ContentType { get; }

        public byte[] Bytes { get; }

        public string Text => Encoding.UTF8.GetString(this.Bytes);

        /// <summary>
        /// Whether the body can only be expressed as raw bytes (it cannot round-trip through a C string)
        /// </summary>
        public bool IsBinary
        {
            get
            {
                if (this.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase) ||
                    this.ContentType.StartsWith("application/octet-stream", StringComparison.OrdinalIgnoreCase) ||
                    this.ContentType.StartsWith("application/pdf", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                if (Array.IndexOf(this.Bytes, (byte)0) >= 0)
                {
                    return true;
                }

                try
                {
                    _ = new UTF8Encoding(false, true).GetString(this.Bytes);
                    return false;
                }
                catch (ArgumentException)
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// Parse a body cell: <c>JSON: ...</c>, <c>XML: ...</c>, <c>file: name</c>, <c>EMPTY</c> or literal text
        /// </summary>
        public static BodyDefinition Parse(string cell, string fallbackContentType)
        {
            string value = cell.Trim();

            if (value.StartsWith("JSON:", StringComparison.Ordinal))
            {
                return new BodyDefinition("application/json", Encoding.UTF8.GetBytes(value.Substring(5).Trim()));
            }

            if (value.StartsWith("XML:", StringComparison.Ordinal))
            {
                return new BodyDefinition("application/xml", Encoding.UTF8.GetBytes(value.Substring(4).Trim()));
            }

            if (value.StartsWith("file:", StringComparison.Ordinal))
            {
                return FromFixture(value.Substring(5).Trim(), fallbackContentType);
            }

            if (value == "EMPTY")
            {
                return new BodyDefinition(fallbackContentType ?? "text/plain", Array.Empty<byte>());
            }

            return new BodyDefinition(fallbackContentType ?? "text/plain", Encoding.UTF8.GetBytes(value));
        }

        public static BodyDefinition FromFixture(string name, string fallbackContentType = null)
        {
            string path = CompatPaths.Fixture(name);

            // *-body.xml fixtures are an envelope: <body><contentType/><contents eol="CRLF"/></body>
            if (name.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
            {
                XDocument doc = XDocument.Load(path);
                XElement root = doc.Root;

                if (root?.Name.LocalName == "body" && root.Element("contents") is { } contents)
                {
                    string contentType = root.Element("contentType")?.Value.Trim() is { Length: > 0 } ct ? ct : "text/plain";
                    string text = contents.Value.Trim().Replace("\r\n", "\n");

                    if (string.Equals((string)contents.Attribute("eol"), "CRLF", StringComparison.OrdinalIgnoreCase))
                    {
                        text = text.Replace("\n", "\r\n");
                    }

                    return new BodyDefinition(contentType, Encoding.UTF8.GetBytes(text));
                }
            }

            string inferred = Path.GetExtension(name).ToLowerInvariant() switch
            {
                ".json" => "application/json",
                ".xml" => "application/xml",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".pdf" => "application/pdf",
                _ => fallbackContentType ?? "text/plain"
            };

            return new BodyDefinition(inferred, File.ReadAllBytes(path));
        }
    }

    /// <summary>
    /// One row of a "the following HTTP interactions have been defined" table
    /// </summary>
    internal sealed class InteractionDefinition
    {
        private static readonly HashSet<string> SupportedColumns = new(StringComparer.Ordinal)
        {
            "No", "method", "path", "query", "headers", "body", "response", "response headers", "response content", "response body"
        };

        public int No { get; set; }

        public string Method { get; set; } = "GET";

        public string Path { get; set; } = "/";

        public string Query { get; set; }

        public List<KeyValuePair<string, string>> Headers { get; set; } = new();

        public BodyDefinition Body { get; set; }

        public ushort Status { get; set; } = 200;

        public List<KeyValuePair<string, string>> ResponseHeaders { get; set; } = new();

        public string ResponseContentType { get; set; }

        public BodyDefinition ResponseBody { get; set; }

        public InteractionDefinition Clone() => new()
        {
            No = this.No,
            Method = this.Method,
            Path = this.Path,
            Query = this.Query,
            Headers = this.Headers.ToList(),
            Body = this.Body,
            Status = this.Status,
            ResponseHeaders = this.ResponseHeaders.ToList(),
            ResponseContentType = this.ResponseContentType,
            ResponseBody = this.ResponseBody
        };

        public static List<InteractionDefinition> FromTable(Table table)
        {
            EnsureSupported(table.Header);

            return table.Rows.Select(row =>
            {
                var interaction = new InteractionDefinition();
                interaction.Apply(row);
                return interaction;
            }).ToList();
        }

        /// <summary>
        /// Apply a changes table (header row + one value row) to a copy of this interaction
        /// </summary>
        public InteractionDefinition WithChanges(Table changes)
        {
            EnsureSupported(changes.Header);

            if (changes.RowCount != 1)
            {
                throw new NotSupportedException($"Expected exactly one row of changes, got {changes.RowCount}");
            }

            InteractionDefinition copy = this.Clone();
            copy.Apply(changes.Rows[0]);
            return copy;
        }

        private static void EnsureSupported(IEnumerable<string> columns)
        {
            string[] unknown = columns.Where(c => !SupportedColumns.Contains(c)).ToArray();

            if (unknown.Length > 0)
            {
                throw new NotSupportedException($"Interaction table column(s) not supported by this runner: {string.Join(", ", unknown)}");
            }
        }

        private void Apply(DataTableRow row)
        {
            foreach (KeyValuePair<string, string> cell in row)
            {
                string value = cell.Value?.Trim() ?? string.Empty;

                switch (cell.Key)
                {
                    case "No":
                        this.No = int.Parse(value);
                        break;
                    case "method":
                        this.Method = value;
                        break;
                    case "path":
                        this.Path = value;
                        break;
                    case "query":
                        this.Query = value.Length == 0 ? null : value;
                        break;
                    case "headers":
                        this.Headers = ParseHeaders(value);
                        break;
                    case "body":
                        if (value.Length > 0)
                        {
                            this.Body = BodyDefinition.Parse(value, this.Body?.ContentType);
                        }
                        break;
                    case "response":
                        if (value.Length > 0)
                        {
                            this.Status = ushort.Parse(value);
                        }
                        break;
                    case "response headers":
                        this.ResponseHeaders = ParseHeaders(value);
                        break;
                    case "response content":
                        this.ResponseContentType = value.Length == 0 ? null : value;
                        break;
                    case "response body":
                        if (value.Length > 0)
                        {
                            this.ResponseBody = BodyDefinition.Parse(value, this.ResponseContentType ?? this.ResponseBody?.ContentType);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Parse <c>'X-A: 1', 'X-B: 2'</c>
        /// </summary>
        public static List<KeyValuePair<string, string>> ParseHeaders(string cell)
        {
            var headers = new List<KeyValuePair<string, string>>();

            if (string.IsNullOrWhiteSpace(cell))
            {
                return headers;
            }

            foreach (string part in cell.Split(','))
            {
                string header = part.Trim().Trim('\'', '"').Trim();
                int colon = header.IndexOf(':');

                if (colon <= 0)
                {
                    throw new FormatException($"Invalid header '{header}'");
                }

                headers.Add(new KeyValuePair<string, string>(header.Substring(0, colon).Trim(), header.Substring(colon + 1).Trim()));
            }

            return headers;
        }

        /// <summary>
        /// Parse a raw query string into ordered, decoded pairs
        /// </summary>
        public static List<KeyValuePair<string, string>> ParseQuery(string query)
        {
            var pairs = new List<KeyValuePair<string, string>>();

            if (string.IsNullOrEmpty(query))
            {
                return pairs;
            }

            foreach (string part in query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries))
            {
                int eq = part.IndexOf('=');
                string key = eq < 0 ? part : part.Substring(0, eq);
                string value = eq < 0 ? string.Empty : part.Substring(eq + 1);
                pairs.Add(new KeyValuePair<string, string>(Uri.UnescapeDataString(key.Replace('+', ' ')), Uri.UnescapeDataString(value.Replace('+', ' '))));
            }

            return pairs;
        }
    }
}
