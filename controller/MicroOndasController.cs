using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using painel_controle_micro_ondas.enums;

namespace painel_controle_micro_ondas.controller;

public class MicroOndasController
{
    public int TimeInSec { get; private set; }
    public int Power { get; private set; }
    public StatusMicroOndas Status { get; private set; }
    public string HeatingProgress { get; private set; }

    private readonly Timer _timer;

    public event EventHandler<int>? TimeChanged;
    public event EventHandler<string>? HeatingProgressChanged;
    public event EventHandler? HeatingFinished;
    public event EventHandler? HeatingCancelled;

    public MicroOndasController()
    {
        Power = 10;
        Status = StatusMicroOndas.Idle;
        HeatingProgress = string.Empty;

        _timer = new Timer(1000);
        _timer.Elapsed += OnTimerElapsed;
    }
    
     public void StartOrAddTime()
    {
        if (Status == StatusMicroOndas.Running)
        {
            TimeInSec += 30;
            if (TimeInSec > 120) TimeInSec = 120;
            TimeChanged?.Invoke(this, TimeInSec);
        }
        else if (Status == StatusMicroOndas.Paused)
        {
            Status = StatusMicroOndas.Running;
            _timer.Start();
        } 
    }

    public void PauseOrCancel()
    {
        if (Status == StatusMicroOndas.Running)
        {
            Status = StatusMicroOndas.Paused;
            _timer.Stop();
        }
        else if (Status == StatusMicroOndas.Paused)
        {
            Clear();
            HeatingProgress = "";
            HeatingProgressChanged?.Invoke(this, HeatingProgress);
            HeatingCancelled?.Invoke(this, EventArgs.Empty);
        }
    }

    public void StartHeating(int seconds, int power)
    {
        if (seconds < 1 || seconds > 120)
        {
            throw new ArgumentOutOfRangeException(nameof(seconds), "Erro: Segundos inválida (1-120)");
        }
        if (power < 1 || power > 10)
        {
            throw new ArgumentOutOfRangeException(nameof(power), "Erro: Potência inválida (1-10)");
        }

        TimeInSec = seconds;
        Power = power;

        HeatingProgress = GenerateGroupedString(TimeInSec, Power);

        Status = StatusMicroOndas.Running;
        _timer.Start();

        TimeChanged?.Invoke(this, TimeInSec);
        HeatingProgressChanged?.Invoke(this, HeatingProgress);
    }
    
    private string GenerateGroupedString(int totalDots, int power)
    {
        if (totalDots <= 0) return "";

        int groupSize = power + 1;

        var sb = new StringBuilder();
        for (int i = 1; i <= totalDots; i++)
        {
            sb.Append('.');
            if (i % groupSize == 0 && i < totalDots)
            {
                sb.Append(' ');
            }
        }
        return sb.ToString();
    }

    private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        if (TimeInSec > 0)
        {
            TimeInSec--;

            HeatingProgress = GenerateGroupedString(TimeInSec, Power);
            TimeChanged?.Invoke(this, TimeInSec);
            HeatingProgressChanged?.Invoke(this, HeatingProgress);
        }
        if (TimeInSec == 0)
        {
            _timer.Stop();
            HeatingProgress = "Aquecimento Concluído";
            HeatingProgressChanged?.Invoke(this, HeatingProgress);
            HeatingFinished?.Invoke(this, EventArgs.Empty);
            Clear();
        }
    }

    private void Clear()
    {
        Status = StatusMicroOndas.Idle;
        TimeInSec = 0;
        Power = 10;
        _timer.Stop();

        TimeChanged?.Invoke(this, TimeInSec);
    }
}