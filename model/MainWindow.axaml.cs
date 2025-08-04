using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using painel_controle_micro_ondas.controller;
using painel_controle_micro_ondas.data;
using painel_controle_micro_ondas.enums;

namespace painel_controle_micro_ondas.model;

public partial class MainWindow : Window
{
    private readonly MicroOndasController _microondasController;
    private string _inputBuffer = string.Empty;
    private int _power = 10;
    private InputState _currentMode = InputState.Time;
    private HeatingProgram? _selectedProgram;
    private readonly DispatcherTimer _carouselText;
    private string _fullInstructionsText = "";

    public MainWindow()
    {
        InitializeComponent();

        _microondasController = new MicroOndasController();

        _microondasController.TimeChanged += OnTimeChanged;
        _microondasController.HeatingProgressChanged += OnHeatingProgressChanged;
        _microondasController.HeatingFinished += (s, e) => UpdateDisplayAfterHeating();
        _microondasController.HeatingCancelled += (s, e) => UpdateDisplayAfterHeating();

        _carouselText = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(200)
        };
        _carouselText.Tick += CarouselText_Tick;

        DisplayTime.Text = "00:00";
        DisplayProgress.Text = "";

    }

    private void OnNumberClick(object? sender, RoutedEventArgs e)
    {
        if (_microondasController.Status == StatusMicroOndas.Running || sender is not Button button)
        {
            return;
        }

        if (_currentMode == InputState.Power && _inputBuffer.Length == 0)
        {
            DisplayTime.Text = "";
        }

        int maxLength = _currentMode == InputState.Power ? 2 : 4;
        if (_inputBuffer.Length >= maxLength)
        {
            return;
        }

        _inputBuffer += button.Content?.ToString();

        if (_currentMode == InputState.Power)
        {
            DisplayTime.Text = _inputBuffer;
        }
        else
        {
            string paddedDigits = _inputBuffer.PadLeft(4, '0');
            string maskedText = $"{paddedDigits.Substring(0, 2)}:{paddedDigits.Substring(2, 2)}";
            DisplayTime.Text = maskedText;
        }
    }

    private void OnPowerClick(object? sender, RoutedEventArgs e)
    {
        _currentMode = InputState.Power;
        _inputBuffer = "";
        DisplayTime.Text = _power.ToString();
        DisplayProgress.Text = "Digite a Nova Potência";

    }


    private void OnStartOrAddTimeClick(object sender, RoutedEventArgs e)
    {
        int timeToHeat;
        if (_selectedProgram != null)
        {

            _microondasController.StartHeating(_selectedProgram);
            return;
        }

        if (_microondasController.Status != StatusMicroOndas.Idle)
        {
            _microondasController.StartOrAddTime();
            return;
        }

        if (_currentMode == InputState.Power)
        {
            if (!string.IsNullOrEmpty(_inputBuffer))
            {
                if (int.TryParse(_inputBuffer, out int newPower) && newPower >= 1 && newPower <= 10)
                {
                    _power = newPower;
                    DisplayProgress.Text = $"Potência Definida: {_power}";
                }
                else
                {
                    DisplayProgress.Text = "Erro: Potência Inválida (1-10)";
                    _inputBuffer = "";
                    return;
                }
            }
            _currentMode = InputState.Time;
            _inputBuffer = "";
            DisplayTime.Text = "00:00";

            return;
        }

        if (string.IsNullOrEmpty(_inputBuffer))
        {
            timeToHeat = 30;
        }
        else
        {
            timeToHeat = ParseTime(_inputBuffer);
        }

        try
        {
            _microondasController.StartHeating(timeToHeat, _power);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            DisplayProgress.Text = ex.Message;
        }

        _inputBuffer = "";
        _currentMode = InputState.Time;
    }

    private void OnPauseOrCancelClick(object sender, RoutedEventArgs e)
    {
        if (_microondasController.Status == StatusMicroOndas.Idle)
        {
            _inputBuffer = "";
            _power = 10;
            DisplayTime.Text = "00:00";
            DisplayProgress.Text = "";
        }
        else
        {
            _microondasController.PauseOrCancel();
        }

        NumericKeypad.IsEnabled = true;
        PowerButton.IsEnabled = true;
        _selectedProgram = null;
    }


    private void OnTimeChanged(object? sender, int time)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            DisplayTime.Text = $"{time / 60:00}:{time % 60:00}";
        });
    }

    private void OnHeatingProgressChanged(object? sender, string progress)
    {
        _carouselText.Stop();

        Dispatcher.UIThread.InvokeAsync(() =>
        {
            DisplayProgress.Text = progress;
        });
    }
    private void UpdateDisplayAfterHeating()
    {
        _carouselText.Stop();

        Dispatcher.UIThread.InvokeAsync(() =>
        {
            _inputBuffer = "";
            _power = 10;
            DisplayTime.Text = "00:00";

            NumericKeypad.IsEnabled = true;
            PowerButton.IsEnabled = true;
            _selectedProgram = null;
        });
    }

    private int ParseTime(string input)
    {
        string paddedInput = input.PadLeft(4, '0');

        // Extrai os minutos e segundos
        int minutes = int.Parse(paddedInput.Substring(0, 2));
        int seconds = int.Parse(paddedInput.Substring(2, 2));

        if (seconds >= 60 && seconds <= 99)
        {
            minutes += 1;
            seconds -= 60;
        }

        int totalSeconds = (minutes * 60) + seconds;

        return totalSeconds;
    }

    private void OnPresetProgramClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button clickedButton) return;

        string? programNameToFind = clickedButton.Content as string;

        if (string.IsNullOrEmpty(programNameToFind)) return;

        var programs = PresetPrograms.GetPrograms();
        var foundProgram = programs.FirstOrDefault(p => p.Name == programNameToFind);

        if (foundProgram != null)
        {
            ApplyProgramSettings(foundProgram);
        }

    }
    private void ApplyProgramSettings(HeatingProgram program)
    {
        _selectedProgram = program;

        DisplayTime.Text = $"{program.TimeInSeconds / 60:00}:{program.TimeInSeconds % 60:00}";
        DisplayProgress.Text = program.Instructions;

        _carouselText.Stop();
        _fullInstructionsText = program.Instructions + "   ---   ";
        DisplayProgress.Text = _fullInstructionsText;

        if (_fullInstructionsText.Length > 50) 
        {
            _carouselText.Start();
        }

        NumericKeypad.IsEnabled = false;
        PowerButton.IsEnabled = false;
    }
    
    private void CarouselText_Tick(object? sender, EventArgs e)
    {
        var currentText = DisplayProgress.Text ?? "";
        if (currentText.Length > 1)
        {
            DisplayProgress.Text = currentText.Substring(1) + currentText[0];
        }
    }
    
}


