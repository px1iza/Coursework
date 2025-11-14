using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using DAL.Entities;
using DAL.Interfaces;

namespace DAL.DataProvider
{
    public class JsonUserDataProvider : IDataProvider<User>
    {
        private readonly string usersFilePath;

        public JsonUserDataProvider()
        {
            string baseDirectory = Directory.GetCurrentDirectory();
            string resultFolder = Path.Combine(baseDirectory, "Result");

            if (!Directory.Exists(resultFolder))
                Directory.CreateDirectory(resultFolder);

            usersFilePath = Path.Combine(resultFolder, "users.json");

            if (!File.Exists(usersFilePath))
                File.WriteAllText(usersFilePath, "[]");
        }

        public void Save(List<User> users)
        {
            string json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(usersFilePath, json);
        }

        public List<User> Load()
        {
            if (!File.Exists(usersFilePath)) return new List<User>();
            string json = File.ReadAllText(usersFilePath);
            return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
        }
    }
}