using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using DAL.Entities;
using DAL.Interfaces;

namespace DAL.DataProvider
{
    public class JsonDocumentDataProvider : IDataProvider<Document>
    {
        private readonly string _fileName = "documents.json";
        private readonly string _basePath = "../../../Result/";

        public JsonDocumentDataProvider()
        {
            string resultFolder = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), _basePath));
            if (!Directory.Exists(resultFolder))
                Directory.CreateDirectory(resultFolder);
        }

        public void Save(List<Document> documents)
        {
            string fullPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), _basePath, _fileName));
            string json = JsonSerializer.Serialize(documents, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(fullPath, json);
        }

        public List<Document> Load()
        {
            string fullPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), _basePath, _fileName));

            if (!File.Exists(fullPath))
            {
                File.WriteAllText(fullPath, "[]");
                return new List<Document>();
            }

            string json = File.ReadAllText(fullPath);
            return JsonSerializer.Deserialize<List<Document>>(json) ?? new List<Document>();
        }
    }
}