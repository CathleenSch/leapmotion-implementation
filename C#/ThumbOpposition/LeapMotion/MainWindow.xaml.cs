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
        float endDistance;

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

                pinchingDisplay.Content = (hand.PinchStrength).ToString();
                pinchingFingerDisplay.Content = getPinchingFinger(hand);

                if (recording == true) {
                    DateTime currentTime = DateTime.Now;
                    int elapsedTime = Convert.ToInt32(((TimeSpan) (currentTime - startRecording)).TotalMilliseconds);
                    timeDisplay.Content = elapsedTime.ToString();
                    string line = "timestamp: " + elapsedTime.ToString() + " | hand: " + handType + " | pinchStrength: " + hand.PinchStrength.ToString() + " | finger: " + getPinchingFinger(hand) + " | distance: " + endDistance;
                    if (firstFrame == true) {
                        using (StreamWriter recordFile = new StreamWriter(@"C:\Users\casch\Documents\Work\Uni\Studienarbeit\Implementierung\C#\opp.txt"))
                        {
                            recordFile.WriteLine(line);
                        };

                        firstFrame = false;
                    } else {
                        using (StreamWriter recordFile = new StreamWriter(@"C:\Users\casch\Documents\Work\Uni\Studienarbeit\Implementierung\C#\opp.txt", true)) {
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

        string getPinchingFinger(Hand hand) {
            string[] fingers = new string[5] { "thumb", "index", "middle", "ring", "pinky" };

            float distance = 500;
            Finger pinchingFinger;
            int index = 0;
            Leap.Vector thumbPos = hand.Fingers[0].TipPosition;

            for (int i = 1; i < hand.Fingers.Count; i++) {
                Finger currentFinger = hand.Fingers[i];
                float currentDistance = currentFinger.TipPosition.DistanceTo(thumbPos);

                if (currentDistance < distance) {
                    distance = currentDistance;
                    pinchingFinger = currentFinger;
                    index = i;
                }
            }

            endDistance = distance;

            return fingers[index];
        }

        private void StartRecordingButton_Click(object sender, RoutedEventArgs e) {
            recording = true;
            this.isRecordingDisplay.Content = "Recording";
            startRecording = DateTime.Now;
        }

        private void StopRecordingButton_Click(object sender, RoutedEventArgs e) {
            recording = false;
            this.isRecordingDisplay.Content = "Not recording";
        }
    }
}
