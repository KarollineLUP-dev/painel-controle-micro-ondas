using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using painel_controle_micro_ondas.model;

namespace painel_controle_micro_ondas.service
{
    public static class ProgramService
    {
        private static readonly string PresetsFilePath = "data/presets.json";
        private static readonly string CustomFilePath = "data/custom_programs.json";
        private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

        private static List<HeatingProgram> GetCustomPrograms()
        {
            if (!File.Exists(CustomFilePath))
            {
                return new List<HeatingProgram>();
            }

            string customJson = File.ReadAllText(CustomFilePath);
            if (string.IsNullOrWhiteSpace(customJson))
            {
                return new List<HeatingProgram>();
            }

            return JsonSerializer.Deserialize<List<HeatingProgram>>(customJson) ?? new List<HeatingProgram>();
        }

        public static List<HeatingProgram> GetAllPrograms()
        {
            var allPrograms = new List<HeatingProgram>();

            if (File.Exists(PresetsFilePath))
            {
                string presetsJson = File.ReadAllText(PresetsFilePath);
                var presetPrograms = JsonSerializer.Deserialize<List<HeatingProgram>>(presetsJson);
                if (presetPrograms != null)
                {
                    presetPrograms.ForEach(p => p.IsCustom = false);
                    allPrograms.AddRange(presetPrograms);
                }
            }

            var customPrograms = GetCustomPrograms();

            customPrograms.ForEach(p => p.IsCustom = true);
            allPrograms.AddRange(customPrograms);

            return allPrograms;
        }

        public static void SaveCustomProgram(HeatingProgram newProgram)
        {
            var customPrograms = GetCustomPrograms();

            customPrograms.Add(newProgram);

            string jsonString = JsonSerializer.Serialize(customPrograms, _jsonOptions);
            File.WriteAllText(CustomFilePath, jsonString);
        }
        
    }
}