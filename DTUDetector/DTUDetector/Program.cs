using System;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace DTUDetector
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var root = @"C:Projects"; //Update this path

            var files = Directory
                .GetFiles(root, "appsettings*.json", SearchOption.AllDirectories)
                .Where(f => !f.Contains(@"\bin\") && !f.Contains(@"\obj\"));

            foreach (var file in files)
            {
                try
                {
                    var json = File.ReadAllText(file);

                    var options = new JsonDocumentOptions
                    {
                        AllowTrailingCommas = true
                    };

                    using (var doc = JsonDocument.Parse(json, options))
                    {
                        if (HasSkuInfo(doc.RootElement))
                        {
                            var projectName = new DirectoryInfo(Path.GetDirectoryName(file)).Name;
                            Console.WriteLine($"{projectName} | {file}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {file}");
                }
            }
        }

        public static bool HasSkuInfo(JsonElement element)
        {
            foreach (var prop in element.EnumerateObject())
            {
                if (prop.Value.ValueKind == JsonValueKind.Object)
                {
                    if (HasSkuInfo(prop.Value))
                        return true;
                }
                else
                {
                    var name = prop.Name.ToLower();

                    if (name.Contains("sku"))
                        return true;
                }
            }

            return false;
        }
    }
}
