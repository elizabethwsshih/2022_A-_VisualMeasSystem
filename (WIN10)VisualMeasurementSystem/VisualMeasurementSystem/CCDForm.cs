using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using Basler.Pylon;
using System.Drawing.Imaging;

namespace VisualMeasurementSystem
{
    public partial class CCDForm : Form
    {
        private Camera camera = null;
        private PixelDataConverter converter = new PixelDataConverter();
        private Stopwatch stopWatch = new Stopwatch();


        private Bitmap shotBitmap1 = null;
        public object imageLock = new object();
        public EventHandler imageHandler;
        Bitmap PICTUREBOXIMAGE; // form1 原本影像

        public CCDForm()
        {
            InitializeComponent();
        }


        //==============================EVENTS==============================
        // Occurs when an image has been acquired and is ready to be processed.
        public Bitmap SHOTbitmap
        {
            get
            {
                lock (imageLock)
                {
                    if (shotBitmap1 == null)
                    {
                        return new Bitmap(10, 10);
                    }
                    else
                    {
                        return (Bitmap)shotBitmap1.Clone();
                    }

                }
            }

            private set
            {
                lock (imageLock)
                {
                    Bitmap tmp = shotBitmap1;
                    shotBitmap1 = null;
                    shotBitmap1 = value;
                    if (tmp != null)
                    {
                        //shotBitmap1.Dispose();
                        tmp.Dispose();
                    }
                }
            }
        }
        private void OnImageGrabbed(Object sender, ImageGrabbedEventArgs e)
        {
            if (InvokeRequired)
            {
                // If called from a different thread, we must use the Invoke method to marshal the call to the proper GUI thread.
                // The grab result will be disposed after the event call. Clone the event arguments for marshaling to the GUI thread.
                BeginInvoke(new EventHandler<ImageGrabbedEventArgs>(OnImageGrabbed), sender, e.Clone());
                return;
            }

            try
            {
                // Acquire the image from the camera. Only show the latest image. The camera may acquire images faster than the images can be displayed.

                // Get the grab result.
                IGrabResult grabResult = e.GrabResult;

                // Check if the image can be displayed.
                if (grabResult.IsValid)
                {
                    // Reduce the number of displayed images to a reasonable amount if the camera is acquiring images very fast.
                    if (!stopWatch.IsRunning || stopWatch.ElapsedMilliseconds > 33)
                    {



                        stopWatch.Restart();

                        Bitmap bitmap = new Bitmap(2048, 2048, PixelFormat.Format32bppRgb);
                        // Lock the bits of the bitmap.
                        BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
                        // Place the pointer to the buffer of the bitmap.
                        converter.OutputPixelFormat = PixelType.BGRA8packed;
                        IntPtr ptrBmp = bmpData.Scan0;
                        converter.Convert(ptrBmp, bmpData.Stride * bitmap.Height, grabResult);
                        bitmap.UnlockBits(bmpData);

                        SHOTbitmap = (Bitmap)bitmap.Clone();

                        bitmap.Dispose();
                        // Assign a temporary variable to dispose the bitmap after assigning the new bitmap to the display control.
                        //Bitmap bitmapOld = pictureBox1.Image as Bitmap;
                        //// Provide the display control with the new bitmap. This action automatically updates the display.
                        //pictureBox1.Image = bitmap;
                        //if (bitmapOld != null)
                        //{
                        //    // Dispose the bitmap.
                        //    bitmapOld.Dispose();
                        //}
                    }
                }
            }
            catch (Exception exception)
            {
                ShowException(exception);
            }
            finally
            {
                // Dispose the grab result if needed for returning it to the grab loop.
                e.DisposeGrabResultIfClone();
            }
        }

        // Occurs when a camera has stopped grabbing.
        private void OnGrabStopped(Object sender, GrabStopEventArgs e)
        {
            if (InvokeRequired)
            {
                // If called from a different thread, we must use the Invoke method to marshal the call to the proper thread.
                BeginInvoke(new EventHandler<GrabStopEventArgs>(OnGrabStopped), sender, e);
                return;
            }

            // Reset the stopwatch.
            stopWatch.Reset();

            // Re-enable the updating of the device list.
            //updateDeviceListTimer.Start();

            // The camera stopped grabbing. Enable the grab buttons. Disable the stop button.
            //EnableButtons(true, false);

            // If the grabbed stop due to an error, display the error message.
            if (e.Reason != GrabStopReason.UserRequest)
            {
                MessageBox.Show("A grab error occured:\n" + e.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnGrabStarted(Object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                // If called from a different thread, we must use the Invoke method to marshal the call to the proper thread.
                BeginInvoke(new EventHandler<EventArgs>(OnGrabStarted), sender, e);
                return;
            }

            // Reset the stopwatch used to reduce the amount of displayed images. The camera may acquire images faster than the images can be displayed.

            stopWatch.Reset();

            // Do not update the device list while grabbing to reduce jitter. Jitter may occur because the GUI thread is blocked for a short time when enumerating.
            // updateDeviceListTimer.Stop();

            // The camera is grabbing. Disable the grab buttons. Enable the stop button.
            //EnableButtons(false, true);
        }
        private void OnCameraClosed(Object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                // If called from a different thread, we must use the Invoke method to marshal the call to the proper thread.
                BeginInvoke(new EventHandler<EventArgs>(OnCameraClosed), sender, e);
                return;
            }

            // The camera connection is closed. Disable all buttons.
            // EnableButtons(false, false);
        }
        private void OnConnectionLost(Object sender, EventArgs e)
        {

            if (InvokeRequired)
            {
                // If called from a different thread, we must use the Invoke method to marshal the call to the proper thread.
                BeginInvoke(new EventHandler<EventArgs>(OnConnectionLost), sender, e);
                return;
            }

            // Close the camera object.
            DestroyCamera();
            // Because one device is gone, the list needs to be updated.
            //UpdateDeviceList();
        }
        // Occurs when the connection to a camera device is opened.
        private void OnCameraOpened(Object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                // If called from a different thread, we must use the Invoke method to marshal the call to the proper thread.
                BeginInvoke(new EventHandler<EventArgs>(OnCameraOpened), sender, e);
                return;
            }

            // The image provider is ready to grab. Enable the grab buttons.
            // EnableButtons(true, false);
        }

        //==============================FUNCTIONS==============================
        //public void UpdateOLDBMP(Bitmap OLDBMP)
        //{
        //    PICTUREBOXIMAGE = OLDBMP;

        //}
        public void OpenCamera()//(ICameraInfo selectedCamera)
        {
            try
            {
                UpdateDeviceList();
                ListViewItem item = deviceListView.Items[0];
                //// Get the attached device data.

                ICameraInfo selectedCamera = item.Tag as ICameraInfo;


                //// Create a new camera object.
                camera = new Camera(selectedCamera);
                camera.CameraOpened += Configuration.AcquireContinuous;
                // Register for the events of the image provider needed for proper operation.
                camera.ConnectionLost += OnConnectionLost;
                camera.CameraOpened += OnCameraOpened;
                camera.CameraClosed += OnCameraClosed;
                camera.StreamGrabber.GrabStarted += OnGrabStarted;
                camera.StreamGrabber.ImageGrabbed += OnImageGrabbed;
                camera.StreamGrabber.GrabStopped += OnGrabStopped;

                //// Open the connection to the camera device.
                camera.Open();
            }
            catch (Exception exception)
            {
                ShowException(exception);
            }
        }
        public void ContinuousShot()
        {
            //OpenCamera(selectedCamera);
            try
            {
                camera.Parameters[PLCamera.AcquisitionMode].SetValue(PLCamera.AcquisitionMode.Continuous);
                camera.StreamGrabber.Start(GrabStrategy.OneByOne, GrabLoop.ProvidedByStreamGrabber);
            }
            catch (Exception exception)
            {
                ShowException(exception);
            }
        }

        public Bitmap ReturnBMP()
        {
            // NEWIMAGE = (Bitmap)SHOTbitmap.Clone();
            return SHOTbitmap;

        }
        private void ShowException(Exception exception)
        {
            MessageBox.Show("Exception caught:\n" + exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public void DestroyCamera()
        {
            // Disable all parameter controls.
            try
            {

            }
            catch (Exception exception)
            {
                ShowException(exception);
            }

            // Destroy the camera object.
            try
            {
                if (camera != null)
                {
                    camera.Close();
                    camera.Dispose();
                    camera = null;
                }
            }
            catch (Exception exception)
            {
                ShowException(exception);
            }

            // Destroy the converter object.
            if (converter != null)
            {
                converter.Dispose();
                converter = null;
            }
        }
        // Updates the list of available camera devices.
        public void UpdateDeviceList()
        {
            try
            {
                // Ask the camera finder for a list of camera devices.
                List<ICameraInfo> allCameras = CameraFinder.Enumerate();

                ListView.ListViewItemCollection items = deviceListView.Items;

                // Loop over all cameras found.
                foreach (ICameraInfo cameraInfo in allCameras)
                {
                    // Loop over all cameras in the list of cameras.
                    bool newitem = true;
                    foreach (ListViewItem item in items)
                    {
                        ICameraInfo tag = item.Tag as ICameraInfo;

                        // Is the camera found already in the list of cameras?
                        if (tag[CameraInfoKey.FullName] == cameraInfo[CameraInfoKey.FullName])
                        {
                            tag = cameraInfo;
                            newitem = false;
                            break;
                        }
                    }

                    // If the camera is not in the list, add it to the list.
                    if (newitem)
                    {
                        // Create the item to display.
                        ListViewItem item = new ListViewItem(cameraInfo[CameraInfoKey.FriendlyName]);

                        // Create the tool tip text.
                        string toolTipText = "";
                        foreach (KeyValuePair<string, string> kvp in cameraInfo)
                        {
                            toolTipText += kvp.Key + ": " + kvp.Value + "\n";
                        }
                        item.ToolTipText = toolTipText;

                        // Store the camera info in the displayed item.
                        item.Tag = cameraInfo;

                        // Attach the device data.
                        deviceListView.Items.Add(item);
                    }
                }



                // Remove old camera devices that have been disconnected.
                foreach (ListViewItem item in items)
                {
                    bool exists = false;

                    // For each camera in the list, check whether it can be found by enumeration.
                    foreach (ICameraInfo cameraInfo in allCameras)
                    {
                        if (((ICameraInfo)item.Tag)[CameraInfoKey.FullName] == cameraInfo[CameraInfoKey.FullName])
                        {
                            exists = true;
                            break;
                        }
                    }
                    // If the camera has not been found, remove it from the list view.
                    if (!exists)
                    {
                        deviceListView.Items.Remove(item);
                    }
                }
            }
            catch (Exception exception)
            {
                ShowException(exception);
            }
        }
















    }
}
