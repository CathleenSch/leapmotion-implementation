using Leap;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LeapMotion {
    public partial class MainWindow : Window {
        bool firstFrame = true;
        bool recording = false;
        DateTime startRecording;

        double[][] ranges = new double[20][] {
            new double[2] {0, 2.5},
            new double[2] {2.5, 7.5},
            new double[2] {7.5, 12.5},
            new double[2] {12.5, 17.5},
            new double[2] {17.5, 22.5},
            new double[2] {22.5, 27.5},
            new double[2] {27.5, 32.5},
            new double[2] {32.5, 37.5},
            new double[2] {37.5, 42.5},
            new double[2] {42.5, 47.5},
            new double[2] {47.5, 52.5},
            new double[2] {52.5, 57.5},
            new double[2] {57.5, 62.5},
            new double[2] {62.5, 67.5},
            new double[2] {67.5, 72.5},
            new double[2] {72.5, 77.5},
            new double[2] {77.5, 82.5},
            new double[2] {82.5, 87.5},
            new double[2] {87.5, 92.5},
            new double[2] {92.5, 97.5}
        };

        double[] currentRange = new double[2] { 0, 5 };

        double currentAngle;
        double currentAverageAngle;

        private byte[] imagedata = new byte[1];
        private Controller controller = new Controller();
        WriteableBitmap bitmap;

        public MainWindow() {
            InitializeComponent();

            //set greyscale palette for WriteableBitmap object
            List<Color> grayscale = new List<Color>();
            for (byte i = 0; i < 0xff; i++) {
                grayscale.Add(Color.FromArgb(0xff, i, i, i));
            }

            BitmapPalette palette = new BitmapPalette(grayscale);
            bitmap = new WriteableBitmap(640, 480, 72, 72, PixelFormats.Gray8, palette);
            frameDisplay.Source = bitmap;

            controller.EventContext = SynchronizationContext.Current;
            controller.FrameReady += newFrameHandler;
            controller.ImageReady += onImageReady;
            controller.ImageRequestFailed += onImageRequestFailed;
        }

        void newFrameHandler(object sender, FrameEventArgs eventArgs) {
            Frame frame = eventArgs.frame;
            fpsDisplay.Content = frame.CurrentFramesPerSecond.ToString();
            if (frame.Hands.Count > 0) {
                Hand hand = frame.Hands[0];

                string handType = hand.IsLeft ? "Left" : "Right";


                // fingers
                Finger thumb = hand.Fingers[0];
                Finger pointer = hand.Fingers[1];
                Finger middle = hand.Fingers[2];
                Finger ring = hand.Fingers[3];
                Finger pinky = hand.Fingers[4];

                handTypeDisplay.Content = hand.IsLeft ? "left" : "right";

                Arm arm = hand.Arm;
                Leap.Vector armDirection = arm.Direction;
                Leap.Vector handDirection = hand.Direction;

                double angleRadians = armDirection.AngleTo(handDirection);
                double angleDegrees = Math.Round(angleRadians * 180 / Math.PI);
                currentAngle = angleDegrees;
                calculateAverageAngle();
                this.angleDisplay.Content = currentAverageAngle.ToString();

                if (recording == true) {
                    DateTime currentTime = DateTime.Now;
                    int elapsedTime = Convert.ToInt32(((TimeSpan)(currentTime - startRecording)).TotalMilliseconds);
                    timeDisplay.Content = elapsedTime.ToString();
                    string line = "timestamp: " + elapsedTime.ToString() + " | hand: " + handType + " | angle: " + currentAverageAngle.ToString();
                    if (firstFrame == true) {
                        using (StreamWriter recordFile = new StreamWriter(@"C:\Users\casch\Documents\Work\Uni\Studienarbeit\Implementierung\C#\urd.txt"))
                        {
                            recordFile.WriteLine(line);
                        };

                        firstFrame = false;
                    } else {
                        using (StreamWriter recordFile = new StreamWriter(@"C:\Users\casch\Documents\Work\Uni\Studienarbeit\Implementierung\C#\urd.txt", true))
                        {
                            recordFile.WriteLine(line);
                        };
                    }
                }
            }
            
            controller.RequestImages(frame.Id, Leap.Image.ImageType.DEFAULT, imagedata);
        }

        void onImageRequestFailed(object sender, ImageRequestFailedEventArgs e) {
            if (e.reason == Leap.Image.RequestFailureReason.Insufficient_Buffer) {
                imagedata = new byte[e.requiredBufferSize];
            }
            debugTextDisplay.AppendText("Image request failed: " + e.message + "\n");
        }

        void onImageReady(object sender, ImageEventArgs e) {
            bitmap.WritePixels(new Int32Rect(0, 0, 640, 480), imagedata, 640, 0);
        }

        void calculateAverageAngle()
        {
            if (!(currentAngle >= currentRange[0] && currentAngle <= currentRange[1]))
            {
                for (int i = 0; i < ranges.Length; i++)
                {
                    if (currentAngle >= ranges[i][0] && currentAngle <= ranges[i][1])
                    {
                        currentRange = ranges[i];
                        break;
                    }
                }
            }

            currentAverageAngle = currentRange[1] - 2.5;
        }

        private void StartRecordingButton_Click(object sender, RoutedEventArgs e)
        {
            recording = true;
            this.isRecordingDisplay.Content = "Recording";
            startRecording = DateTime.Now;
        }

        private void StopRecordingButton_Click(object sender, RoutedEventArgs e)
        {
            recording = false;
            this.isRecordingDisplay.Content = "Not recording";
        }
    }
}
