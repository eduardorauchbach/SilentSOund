using NAudio.Wave;
using System;
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
        private WaveOut waveOut;

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

            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = new System.Drawing.Icon("mute.ico");
            notifyIcon.Visible = true;
            notifyIcon.BalloonTipText = "Maximize";
            notifyIcon.Click += (sender, e) => Maximize();
        }

        private void InitializeAudioPlayer()
        {
            audioPlayer = new AudioFileReader(soundName);
            waveOut = new WaveOut();

            waveOut.Init(audioPlayer);
            waveOut.Play();
            waveOut.PlaybackStopped += OnPlaybackStopped;
        }

        private void OnPlaybackStopped(object sender, StoppedEventArgs e)
        {
            // Reset the audio player to the start
            audioPlayer.Position = 0;
            // Reinitialize and play
            waveOut.Play();
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
            waveOut.PlaybackStopped -= OnPlaybackStopped; // Desanexar o manipulador de eventos
            waveOut?.Dispose();
            audioPlayer?.Dispose();

            notifyIcon.Click -= (sender, eventArgs) => Maximize();
            notifyIcon.Dispose();
            Closing -= MainWindow_Closing;
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
}
