using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using painel_controle_micro_ondas.service;

namespace painel_controle_micro_ondas.model
{
    public partial class CreateProgramWindow : Window
    {
        public CreateProgramWindow()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object? sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                string.IsNullOrWhiteSpace(FoodTextBox.Text) ||
                string.IsNullOrWhiteSpace(TimeTextBox.Text) ||
                string.IsNullOrWhiteSpace(PowerTextBox.Text)||
                string.IsNullOrWhiteSpace(HeatingCharTextBox.Text))
            {
                ErrorTextBlock.Text = "Os campos marcados com (*) são obrigatórios.";
                return;
            }

            if (!int.TryParse(TimeTextBox.Text, out int time) || time <= 0)
            {
                ErrorTextBlock.Text = "O tempo deve ser um número inteiro positivo.";
                return;
            }

            if (!int.TryParse(PowerTextBox.Text, out int power) || power < 1 || power > 10)
            {
                ErrorTextBlock.Text = "Erro: Potência Inválida (1-10)";
                return;
            }

            char heatingChar = HeatingCharTextBox.Text[0];

            var allPrograms = ProgramService.GetAllPrograms();
            var usedChars = new HashSet<char>(allPrograms.Select(p => p.HeatingChar));

            if (usedChars.Contains(heatingChar))
            {
                ErrorTextBlock.Text = $"O caractere '{heatingChar}' já está em uso. Por favor, escolha outro.";
                return;
            }

            var newProgram = new HeatingProgram
            {
                Name = NameTextBox.Text,
                Food = FoodTextBox.Text,
                TimeInSeconds = time,
                Power = power,
                Instructions = InstructionsTextBox.Text ?? "",
                HeatingChar = heatingChar,
                IsCustom = true 
            };

            try
            {
                ProgramService.SaveCustomProgram(newProgram);
                this.Close(true);
            }
            catch (Exception ex)
            {
                ErrorTextBlock.Text = $"Erro ao salvar o arquivo: {ex.Message}";
            }
        }

        private void CancelButton_Click(object? sender, RoutedEventArgs e)
        {
            this.Close(false);
        }

    }
}