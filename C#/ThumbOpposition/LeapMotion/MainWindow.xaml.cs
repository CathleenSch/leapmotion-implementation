using Leap;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LeapMotion {
    public partial class MainWindow : Window {
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
            debugTextDisplay.AppendText("Goes through here 1\n");
            fpsDisplay.Content = frame.CurrentFramesPerSecond.ToString();
            if (frame.Hands.Count > 0) {
                Hand hand = frame.Hands[0];

                // fingers
                Finger thumb = hand.Fingers[0];
                Finger pointer = hand.Fingers[1];
                Finger middle = hand.Fingers[2];
                Finger ring = hand.Fingers[3];
                Finger pinky = hand.Fingers[4];

                debugTextDisplay.AppendText("Goes through here 2\n");
                pinchingDisplay.Content = (hand.PinchStrength).ToString();
                debugTextDisplay.AppendText("Goes through here 3\n");
                pinchingFingerDisplay.Content = getPinchingFinger(hand);
                
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
            debugTextDisplay.AppendText("Goes through here 4\n");
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

            debugTextDisplay.AppendText("Goes through here 5\n");
            return fingers[index];
        }
    }
}
