using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using painel_controle_micro_ondas.model;
using painel_controle_micro_ondas.service; 

namespace painel_controle_micro_ondas.model
{
    public partial class DeleteProgramWindow : Window
    {
        public DeleteProgramWindow()
        {
            InitializeComponent();
            LoadCustomProgramsToDelete();
        }

        private void LoadCustomProgramsToDelete()
        {
            var customPrograms = ProgramService.GetAllPrograms().Where(p => p.IsCustom).ToList();
            ProgramsListBox.ItemsSource = customPrograms;
        }

        private void DeleteButton_Click(object? sender, RoutedEventArgs e)
        {
            var selectedProgram = ProgramsListBox.SelectedItem as HeatingProgram;

            if (selectedProgram == null)
            {
                ErrorTextBlock.Text = "Por favor, selecione um programa da lista para deletar.";
                return;
            }

            ProgramService.DeleteCustomProgram(selectedProgram.Name);

            this.Close(true);
        }

        private void CancelButton_Click(object? sender, RoutedEventArgs e)
        {
            this.Close(false);
        }
    }
}