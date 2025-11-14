using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using DAL.Entities;
using DAL.Interfaces;

namespace DAL.DataProvider
{
    public class JsonDocumentDataProvider : IDataProvider<Document>
    {
        private readonly string documentsFilePath;

        public JsonDocumentDataProvider()
        {
            // Базова директорія проєкту (де запускається PL)
            string baseDirectory = Directory.GetCurrentDirectory();

            // Папка Result всередині PL
            string resultFolder = Path.Combine(baseDirectory, "Result");

            // Створюємо папку, якщо її немає
            if (!Directory.Exists(resultFolder))
                Directory.CreateDirectory(resultFolder);

            // Файл документів
            documentsFilePath = Path.Combine(resultFolder, "documents.json");

            // Якщо файл не існує — створюємо порожній JSON
            if (!File.Exists(documentsFilePath))
                File.WriteAllText(documentsFilePath, "[]");
        }

        public void Save(List<Document> documents)
        {
            string json = JsonSerializer.Serialize(documents, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(documentsFilePath, json);
        }

        public List<Document> Load()
        {
            if (!File.Exists(documentsFilePath)) return new List<Document>();
            string json = File.ReadAllText(documentsFilePath);
            return JsonSerializer.Deserialize<List<Document>>(json) ?? new List<Document>();
        }
    }
}