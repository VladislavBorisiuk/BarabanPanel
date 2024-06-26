﻿using BarabanPanel.Models;
using BarabanPanel.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace BarabanPanel.Services
{
    class JsonReader : IJsonReader
    {
        private static readonly string filePath = Path.Combine(Directory.GetCurrentDirectory(), "TextFile1.json");
        private readonly string jsonContent = File.ReadAllText(filePath);
        public IEnumerable<string> GetDictionaryNames()
        {
            try
            {
                List<Melody> melodies = JsonConvert.DeserializeObject<List<Melody>>(jsonContent);
                List<string> names = new List<string>();
                foreach (var item in melodies)
                {
                    names.Add(item.Name);
                }
                IEnumerable<string> n = names;
                return n;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка чтения JSON файла: {ex.Message}");
                return null;
            }
        }

        public Melody GetMelody(string key)
        {
            try
            {

                List<Melody> melodies = JsonConvert.DeserializeObject<List<Melody>>(jsonContent);

                foreach(var item in melodies)
                {
                    if(item.Name == key)
                    {
                        return item;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка чтения JSON файла: {ex.Message}");
                return null;
            }
        }
        public JsonReader() { }
    }
}
