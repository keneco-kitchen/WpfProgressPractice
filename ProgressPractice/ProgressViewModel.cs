using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgressPractice
{
    class ProgressViewModel : INotifyPropertyChanged
    {
        private bool startEnabled = true;

        public bool StartEnabled
        {
            get { return startEnabled; }
            set
            {
                if (this.startEnabled == value) { return; }
                startEnabled = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StartEnabled)));
            }
        }

        private string progressText = "停止中";


        public string ProgressText
        {
            get { return this.progressText; }
            set
            {
                if (this.progressText == value) { return; }
                this.progressText = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProgressText)));
            }
        }

        private double estimatedRemain;

        public double EstimatedRemain
        {
            get { return estimatedRemain; }
            set
            {
                if (this.estimatedRemain == value) { return; }
                this.estimatedRemain = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EstimatedRemain)));
            }
        }

        private void SetProgressValue(ProgressReport p_report)
        {
            this.EstimatedRemain = p_report.Persent;
            if (0 < p_report.Persent && p_report.Persent < 100)
            {
                this.ProgressText = $"あと{p_report.EstimatedRemain.TotalSeconds}";
            }
        }
        public DelegateCommand StartCommand
        {
            get
            {
                return this.startCommand ?? (this.startCommand = new DelegateCommand(this.StartExecute, () => true));
            }

        }
        private DelegateCommand startCommand;

        private async void StartExecute()
        {
            this.StartEnabled = false;
            this.ProgressText = "実行中...";

            var l_progress = new Progress<ProgressReport>(this.SetProgressValue);
            await this.DoworkAsync(l_progress);
            this.ProgressText = "完了";
            this.StartEnabled = true;
        }

        private async Task DoworkAsync(IProgress<ProgressReport> p_progress)
        {
            const int Count = 10;
            const int PreiodMsec = 500;
            for (int l_i = 0; l_i < Count; l_i++)
            {
                await Task.Delay(PreiodMsec);


                p_progress.Report(new ProgressReport()
                {
                    Persent = (l_i + 1) * Count,
                    EstimatedRemain = TimeSpan.FromMilliseconds((Count - 1 - l_i) * PreiodMsec),
                });
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
