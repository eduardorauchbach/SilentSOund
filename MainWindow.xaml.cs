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
        private AudioFileReader currentAudioPlayer;
        private AudioFileReader nextAudioPlayer;
        private WaveOut waveOut;

        private const string soundName = "silent_audio.mp3";

        public MainWindow()
        {
            InitializeComponent();
            InitializeNotifyIcon();
            InitializeAudioPlayers();

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

        private void InitializeAudioPlayers()
        {
            nextAudioPlayer = new AudioFileReader(soundName);
            currentAudioPlayer = nextAudioPlayer;
            waveOut = new WaveOut();

            waveOut.Init(currentAudioPlayer);
            waveOut.Play();
            waveOut.PlaybackStopped += OnPlaybackStopped;

            void OnPlaybackStopped(object? sender, StoppedEventArgs e)
            {
                waveOut.Dispose();
                currentAudioPlayer.Dispose();

                currentAudioPlayer = nextAudioPlayer;
                nextAudioPlayer.Dispose();

                waveOut = new WaveOut();
                waveOut.Init(currentAudioPlayer);
                waveOut.Play();
                waveOut.PlaybackStopped += OnPlaybackStopped;

                nextAudioPlayer = new AudioFileReader(soundName);
            }
        }

        private void Maximize()
        {
            Show();
            Focus();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            waveOut.Stop();
            waveOut.Dispose();
            currentAudioPlayer.Dispose();
            nextAudioPlayer.Dispose();

            notifyIcon.Click -= (sender, eventArgs) => Maximize();
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
