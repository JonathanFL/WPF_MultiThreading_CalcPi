using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CalcPi;
using CalcPiAlgoritm;

namespace SyncCalcPi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int digits;
        BackgroundWorker backgroundWorker;

        DateTime startTime;
        DateTime endTime;

        public MainWindow()
        {
            InitializeComponent();
            backgroundWorker = ((BackgroundWorker)this.FindResource("BackgroundWorker"));
            btnCancel.IsEnabled = false;
        }
        private void miFileExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnCalculate_Click(object sender, RoutedEventArgs e)
        {
            btnCalculate.IsEnabled = false;
            btnCancel.IsEnabled = true;
            // Update statusbar
            sbiStatus.Content = "Calculating...";
            progressBar.Visibility = Visibility.Visible;

            // Convert control's decimal Value to an integer

            backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += new DoWorkEventHandler(BackgroundWorker_OnDoWork);
            backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(BackgroundWorker_OnProgressChanged);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundWorker_OnRunWorkerCompleted);
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.WorkerSupportsCancellation = true;

            // Convert control's decimal Value to an integer
            //CalculateDigitsOfPiInput input = new CalculateDigitsOfPiInput(int.Parse(this.tbxDigits.Text));
            digits = int.Parse(this.tbxDigits.Text);
            //FindPrimesInput input = new FindPrimesInput(from, to);
            startTime = DateTime.Now;
            backgroundWorker.RunWorkerAsync(digits);

            // Update statusbar
            sbiStatus.Content = "Ready";
        }
        

        private void BackgroundWorker_OnDoWork(object sender, DoWorkEventArgs e)
        {
            
            //Debug.WriteLine(input.Digits);

            StringBuilder pi = new StringBuilder("3", digits + 2);

            if (digits > 0)
            {
                pi.Append(".");

                for (int i = 0; i < digits; i += 9)
                {
                    int nineDigits = NineDigitsOfPi.StartingAt(i + 1);
                    int digitCount = Math.Min(digits - i, 9);
                    string ds = $"{nineDigits:D9}";
                    pi.Append(ds.Substring(0, digitCount));

                    backgroundWorker.ReportProgress((i * 100) / digits, pi);
                    if (backgroundWorker.CancellationPending)
                    {
                        break;
                    }
                }


            }

            if (backgroundWorker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

        }

        private void BackgroundWorker_OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            StringBuilder pi = e.UserState as StringBuilder;
            tblkResults.Text = pi.ToString();// ?? throw new InvalidOperationException();
            progressBar.Maximum = 100;
            progressBar.Value = e.ProgressPercentage;
            progressBar.Visibility = Visibility.Visible;
            sbiStatus.Content = "Calculating";
        }

        private void BackgroundWorker_OnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            endTime = DateTime.Now;
            TimeSpan calcTime = endTime - startTime;
            tblCalcTime.Text = calcTime.ToString();
                // Reset UI
            sbiStatus.Content = "Completed and Ready";
            progressBar.Visibility = Visibility.Hidden;

            btnCalculate.IsEnabled = true;
            btnCancel.IsEnabled = false;

            if (e.Cancelled)
            {
                MessageBox.Show("Search cancelled.");
                sbiStatus.Content = "Cancelled but ready";
            }
            else if (e.Error != null)
            {
                // An error was thrown by the DoWork event handler.
                MessageBox.Show(e.Error.Message, "An Error Occurred");
            }

        }

        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            backgroundWorker.CancelAsync();
        }
    }
}
