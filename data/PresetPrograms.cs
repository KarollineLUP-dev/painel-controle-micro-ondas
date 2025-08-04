using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using painel_controle_micro_ondas.model;

namespace painel_controle_micro_ondas.data
{
    public static class PresetPrograms
    {
        private static readonly string FilePath = "data/presets.json";

        public static List<HeatingProgram> GetPrograms()
        {
            if (!File.Exists(FilePath))
            {
                return new List<HeatingProgram>();
            }

            string jsonString = File.ReadAllText(FilePath);
            
            List<HeatingProgram> programs = JsonSerializer.Deserialize<List<HeatingProgram>>(jsonString)!;

            return programs;
        }
    }
}