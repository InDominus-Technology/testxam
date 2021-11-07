using System;
using System.IO;
using System.Threading.Tasks;
using AVFoundation;
using Foundation;
using Plugin.Media.Abstractions;
using Xamarin.Essentials;

namespace test.iOS
{
    public class EmptyClass
    {
        public EmptyClass()
        {
        }
    }

    public class VideoConverter : IVideoConverter
    {
        public VideoConverter()
        {
        }

        //public async Task<Stream> StoreVideo(MediaFile file)
        //{
        //    string exportPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        //    string exportFilePath = Path.Combine(exportPath, "stored_video.mp4");

        //}

        public async Task SaveVideo(Stream file)
        {
            System.Diagnostics.Debug.WriteLine("Starting...");

            string exportPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string exportFilePath = Path.Combine(exportPath, "saved_video.mov");


            System.Diagnostics.Debug.WriteLine("Export path: " + exportPath);
            System.Diagnostics.Debug.WriteLine("Export file path: " + exportFilePath);

            byte[] bArray = new byte[file.Length];
            using (FileStream fs = new FileStream(exportFilePath, FileMode.OpenOrCreate))
            {
                using (file)
                {
                    file.Read(bArray, 0, (int)file.Length);
                }
                int length = bArray.Length;
                fs.Write(bArray, 0, length);
            }
        }

        public async Task<Stream> CompressVideo(MediaFile file, int presetNumber)
        {
            AVAssetExportSessionPreset preset = AVAssetExportSessionPreset.LowQuality;
            string title;

            switch (presetNumber)
            {
                case 1:
                    {
                        preset = AVAssetExportSessionPreset.HighestQuality;
                        title = "compressed_video_hq.mp4";
                        break;
                    }
                case 2:
                    {
                        preset = AVAssetExportSessionPreset.MediumQuality;
                        title = "compressed_video_mq.mp4";
                        break;
                    }
                default:
                    title = "compressed_video.mp4";
                    break;
            }

            System.Diagnostics.Debug.WriteLine("Starting with title: " + title);

            string exportPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string exportFilePath = Path.Combine(exportPath, title);

            System.Diagnostics.Debug.WriteLine("Export path: " + exportPath);
            System.Diagnostics.Debug.WriteLine("Export file path: " + exportFilePath);

            var asset = AVAsset.FromUrl(NSUrl.FromFilename(file.Path));
            AVAssetExportSession export = new AVAssetExportSession(asset, preset);

            System.Diagnostics.Debug.WriteLine("Created export object");

            export.OutputUrl = NSUrl.FromFilename(exportFilePath);
            export.OutputFileType = AVFileType.Mpeg4;
            export.ShouldOptimizeForNetworkUse = true;

            System.Diagnostics.Debug.WriteLine("Preparing to export");

            await RunExportAsync(export);

            Stream exportStream = File.OpenRead(exportFilePath);

            System.Diagnostics.Debug.WriteLine("Stream read from file");

            return exportStream;
        }

        private async Task RunExportAsync(AVAssetExportSession exp)
        {
            await exp.ExportTaskAsync();
            if (exp.Status == AVAssetExportSessionStatus.Completed)
            {
                System.Diagnostics.Debug.WriteLine("Finished export");
            }
        }

        public async Task ConvertVideo(string inputpath, string outputpath)
        {
            /// string OutputFilePath = Path.ChangeExtension(outputpath, "mp4");
            ///



            string downloadPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string downloadFilePath = Path.Combine(downloadPath, "compressed_video.mp4");

            await CheckAndRequestCameraPermission();
            await CheckAndRequestMediaPermission();
            await CheckAndRequestPhotosPermission();

            //AVAsset asset = AVAsset.FromUrl(NSUrl.FromFilename(inputpath));
            var asset = AVAsset.FromUrl(NSUrl.FromFilename(inputpath));
            AVAssetExportSession export = new AVAssetExportSession(asset, AVAssetExportSession.PresetLowQuality);

            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var cache = Path.Combine(documents, "..", "Library", "Caches");
            var dest = Path.Combine(documents, "..", "tmp");

            export.OutputUrl = NSUrl.FromFilename(downloadFilePath);
            export.OutputFileType = AVFileType.Mpeg4;
            export.ShouldOptimizeForNetworkUse = true;

            var results = export.DetermineCompatibleFileTypesAsync();
            try
            {
                try
                {
                    await export.ExportTaskAsync();

                    //if (!File.Exists(dest))
                    //    throw new Exception("aaaaarrrrghhhhhhh");

                    System.Diagnostics.Debug.WriteLine(export.Error?.LocalizedDescription);
                }
                catch (Exception ex)
                {

                    System.Diagnostics.Debug.WriteLine(ex);
                }
                //export.ExportAsynchronously(() =>
                //    {
                //        if (export.Error != null)
                //        {
                //            System.Diagnostics.Debug.WriteLine(export.Error.LocalizedDescription);
                //        }

                //        System.Diagnostics.Debug.WriteLine(export.Error.LocalizedDescription);
                //    });

                //await results;
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        public async Task<PermissionStatus> CheckAndRequestCameraPermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();

            if (status == PermissionStatus.Granted)
                return status;

            if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
            {
                // Prompt the user to turn on in settings
                // On iOS once a permission has been denied it may not be requested again from the application
                return status;
            }

            if (Permissions.ShouldShowRationale<Permissions.Camera>())
            {
                // Prompt the user with additional information as to why the permission is needed
            }

            status = await Permissions.RequestAsync<Permissions.Camera>();

            return status;
        }

        public async Task<PermissionStatus> CheckAndRequestMediaPermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Media>();

            if (status == PermissionStatus.Granted)
                return status;

            if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
            {
                // Prompt the user to turn on in settings
                // On iOS once a permission has been denied it may not be requested again from the application
                return status;
            }

            if (Permissions.ShouldShowRationale<Permissions.Media>())
            {
                // Prompt the user with additional information as to why the permission is needed
            }

            status = await Permissions.RequestAsync<Permissions.Media>();

            return status;
        }

        public async Task<PermissionStatus> CheckAndRequestPhotosPermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Photos>();

            if (status == PermissionStatus.Granted)
                return status;

            if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
            {
                // Prompt the user to turn on in settings
                // On iOS once a permission has been denied it may not be requested again from the application
                return status;
            }

            if (Permissions.ShouldShowRationale<Permissions.Photos>())
            {
                // Prompt the user with additional information as to why the permission is needed
            }

            status = await Permissions.RequestAsync<Permissions.Photos>();

            return status;
        }
    }

 }

