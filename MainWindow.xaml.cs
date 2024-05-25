using NAudio.Wave;
using System;
using System.Runtime;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SilentSOund
{
    public partial class MainWindow : Window
    {
        private NotifyIcon notifyIcon;
        private AudioFileReader audioPlayer;
        private WaveOutEvent waveOut;

        private const string soundName = "silent_audio.mp3";

        public MainWindow()
        {
            InitializeComponent();
            InitializeNotifyIcon();
            InitializeAudioPlayer();

            Hide();
            Closing += MainWindow_Closing;
        }

        private void InitializeNotifyIcon()
        {
            string iconPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mute.ico");
            this.Icon = new BitmapImage(new Uri(iconPath));

            notifyIcon = new NotifyIcon
            {
                Icon = new System.Drawing.Icon("mute.ico"),
                Visible = true,
                BalloonTipText = "Maximize"
            };
            notifyIcon.Click += (sender, e) => Maximize();
        }

        private void InitializeAudioPlayer()
        {
            using (new LowLatencyMode())
            {
                audioPlayer = new AudioFileReader(soundName);
                waveOut = new WaveOutEvent();

                waveOut.Init(audioPlayer);
                waveOut.Play();
                waveOut.PlaybackStopped += OnPlaybackStopped;
            }
        }

        private void OnPlaybackStopped(object sender, StoppedEventArgs e)
        {
            using (new LowLatencyMode())
            {
                // Reset the audio player to the start
                audioPlayer.Position = 0;
                // Reinitialize and play
                waveOut.Play();
            }
        }

        private void Maximize()
        {
            Show();
            Focus();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Properly dispose the resources
            waveOut?.Stop();
            waveOut.PlaybackStopped -= OnPlaybackStopped;
            waveOut?.Dispose();
            audioPlayer?.Dispose();

            notifyIcon.Click -= (sender, eventArgs) => Maximize();
            notifyIcon.Dispose();
            Closing -= MainWindow_Closing;

            // Force garbage collection
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
                notifyIcon.ShowBalloonTip(2000);
            }
        }
    }

    public class LowLatencyMode : IDisposable
    {
        private readonly GCLatencyMode _oldMode;

        public LowLatencyMode()
        {
            _oldMode = GCSettings.LatencyMode;
            GCSettings.LatencyMode = GCLatencyMode.LowLatency;
        }

        public void Dispose()
        {
            GCSettings.LatencyMode = _oldMode;
        }
    }
}
