using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.Media.Abstractions;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace test
{
	public interface IVideoConverter
	{
		Task ConvertVideo(string inputpath, string outputpath);
		Task<Stream> CompressVideo(MediaFile file, int presetNumber);
		Task SaveVideo(Stream file);
    }

	public class VideoContent
	{
		public string description { get; set; }
		/// <summary>
		/// Example: https://storage-video.b-cdn.net/video-space/vids/compressed_vid.mp4
		/// </summary>
		public string source { get; set; }
		public string fileSource { get; set; }
		public string imageSource { get; set; }
		public string imageFileSource { get; set; }
		public int likedCount { get; set; }
		public string region { get; set; }
		public string language { get; set; }
		public string createdTime { get; set; }
		public int status { get; set; }
		public int reportedCount { get; set; }
		public string createdAt { get; set; }
		public string validUntil { get; set; }
		public List<string> hashTags { get; set; }
		public bool IsTestVideo { get; set; }
	}

	public partial class MainPage : ContentPage
    {

		public bool isNice = true;
		public bool test { get; set; }

		public string description { get; set; } = "Hallo Universum!";

		public MainPage()
        {
            InitializeComponent();
			MainThread.BeginInvokeOnMainThread(() => sinan.Text = description);
			sinan.Text = description;

			//UploadCommand = new AsyncReactiveCommand()
			//	.WithSubscribe(async () =>
			//	{
			//		var source = previewVideo.Source;
			//		await DependencyService.Get<Network>().UploadVideo(source);
			//	});

			cameraView.CaptureMode = CameraCaptureMode.Video;
			cameraView.CameraOptions = CameraOptions.Front;
			uploadButton.IsEnabled = false;
			timer.Text = "00:00";
		}

		public bool isShit = false;

		private string _videoFilePath;

		public ICommand UploadCommand { get; private set; }
		public Timer _timer { get; private set; }

		bool isRecording = false;

		
		int sec = 0;

		void ZoomSlider_ValueChanged(object? sender, ValueChangedEventArgs e)
		{
			//	cameraView.Zoom = (float)zoomSlider.Value;
			//	zoomLabel.Text = string.Format("Zoom: {0}", Math.Round(zoomSlider.Value));
		}

		void VideoSwitch_Toggled(object? sender, ToggledEventArgs e)
		{
			var captureVideo = e.Value;

			if (captureVideo)
				cameraView.CaptureMode = CameraCaptureMode.Video;
			else
				cameraView.CaptureMode = CameraCaptureMode.Photo;

			//previewPicture.IsVisible = !captureVideo;

			doCameraThings.Text = e.Value ? "Start Recording"
				: "Snap Picture";
		}

		// You can also set it to Default and External
		void FrontCameraSwitch_Toggled(object? sender, ToggledEventArgs e)
		{
			if (isRecording)
				return;

			cameraView.CameraOptions = e.Value ? CameraOptions.Back : CameraOptions.Front;
		}

		// You can also set it to Torch (always on) and Auto
		void FlashSwitch_Toggled(object? sender, ToggledEventArgs e)
			=> cameraView.FlashMode = e.Value ? CameraFlashMode.On : CameraFlashMode.Off;

		void DoCameraThings_Clicked(object? sender, EventArgs e)
		{
			//sec = 0;
			//camState.BackgroundColor = Color.DarkGreen;
			//cameraView.Shutter();

			//if (cameraView.CaptureMode == CameraCaptureMode.Video)
			//{
			//	isRecording = true;
			//	doCameraThings.Text = "Stop Recording";
			//	return;
			//}


			//await DependencyService.Get<IVideoConverter>().CompressVideo();

		}

		void CameraView_OnAvailable(object? sender, bool e)
		{
			//if (e)
			//{
			//	zoomSlider.Value = cameraView.Zoom;
			//	var max = cameraView.MaxZoom;
			//	if (max > zoomSlider.Minimum && max > zoomSlider.Value)
			//		zoomSlider.Maximum = max;
			//	else
			//		zoomSlider.Maximum = zoomSlider.Minimum + 1; // if max == min throws exception
			//}

			//doCameraThings.IsEnabled = e;
			//zoomSlider.IsEnabled = e;
		}

		void CameraView_MediaCaptured(object? sender, MediaCapturedEventArgs e)
		{
			switch (cameraView.CaptureMode)
			{
				default:
				case CameraCaptureMode.Default:
				case CameraCaptureMode.Photo:
					//previewVideo.IsVisible = false;
					//previewPicture.IsVisible = true;
					//previewPicture.Rotation = e.Rotation;
					//previewPicture.Source = e.Image;
					doCameraThings.Text = "Snap Picture";
					break;
				case CameraCaptureMode.Video:
					//previewPicture.IsVisible = false;
					//previewVideo.IsVisible = true;
					//previewVideo.Source = e.Video;
					//var video = e.Video;
					_videoFilePath = e.Video.File;
					isRecording = false;
					uploadButton.IsEnabled = true;
					camState.BackgroundColor = Color.Transparent;
					doCameraThings.Text = "Start Recording";
					break;
			}
		}

		//public async Task UploadVideo()
		//      {
		//	//var source = previewVideo.Source;
		//	//await DependencyService.Get<Network>().UploadVideo(source);
		//      }

		async void Button_Clicked(System.Object sender, System.EventArgs e)
		{
			var source = _videoFilePath;

			await DependencyService.Get<IVideoConverter>().ConvertVideo(source, source.Split('.')[0] + ".mp4");
			var videoContent = new VideoContent();
			videoContent.fileSource = source;
			videoContent.description = desc.Text; //title.Text + " - " +

			//await DependencyService.Get<Network>().UploadVideo(videoContent);
		}
	}
}
