using System;
using System.Text.Json.Serialization;

namespace painel_controle_micro_ondas.model
{
    public record HeatingProgram
    {
        public string Name { get; init; } = string.Empty;
        public string Food { get; init; } = string.Empty;
        public int TimeInSeconds { get; init; }
        public int Power { get; init; }
        public char HeatingChar { get; init; }
        public string Instructions { get; init; } = string.Empty;

        [JsonIgnore] 
        public bool IsCustom { get; set; } = false;
    }
}