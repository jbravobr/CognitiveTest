using Prism.Commands;
using Prism.Mvvm;
using System;
using Xamarin.Forms;
using Plugin.Media.Abstractions;
using Acr.UserDialogs;
using System.Linq;
using System.IO;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using System.Threading.Tasks;
using PropertyChanged;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Poc.Luis.Xamarin.ViewModels
{
    [ImplementPropertyChanged]
    public class HomePageViewModel : BindableBase
    {
        readonly IMedia _mediaService;
        readonly IUserDialogs _userDialogsService;
        readonly IApplicationServices<Product> _productService;
        readonly IApplicationServices<Image> _imageService;

        public DelegateCommand PickImgCmd { get; set; }
        public DelegateCommand AnalyseImgCmd { get; set; }
        Stream imgStream { get; set; }
        MediaFile file { get; set; }

        public ImageSource ImgSource
        {
            get
            {
                if (!_imageService.GetAll().Any())
                    return ImageSource.FromFile("noimage.png");

                var img = _imageService.GetAll().Last();
                return ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(img.ImageBase64)));
            }
            set { value = ImgSource; }
        }

        public Action AnalyseImg
        {
            get
            {
                return new Action(() =>
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        if (imgStream == null)
                        {
                            var toastConfig = new ToastConfig("Nenhuma imagem carregada")
                            {
                                BackgroundColor = System.Drawing.Color.Red,
                                Duration = TimeSpan.FromMilliseconds(5000),
                                MessageTextColor = System.Drawing.Color.White
                            };
                            _userDialogsService.Toast(toastConfig);
                            return;
                        }

                        _userDialogsService.ShowLoading();
                        var ocrResult = await CallVisionCognitiveService(imgStream);
                        ReadTextFromImage(ocrResult);
                    });
                });
            }
        }

        public Action PickImg
        {
            get
            {
                return new Action(() =>
                {
                    try
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await _mediaService.Initialize();

                            if (App.OnEmulator || (!_mediaService.IsCameraAvailable || !_mediaService.IsTakePhotoSupported))
                                file = await _mediaService.PickPhotoAsync();
                            else
                            {
                                var cameraOptions = new StoreCameraMediaOptions
                                {
                                    AllowCropping = false,
                                    CompressionQuality = 100,
                                    CustomPhotoSize = 100,
                                    DefaultCamera = CameraDevice.Rear,
                                    SaveToAlbum = true,
                                    Directory = "Sample",
                                    Name = "photoTest.jpg"
                                };
                                file = await _mediaService.TakePhotoAsync(cameraOptions);
                            }

                            _userDialogsService.ShowLoading(null, MaskType.Gradient);

                            if (file == null)
                                return;

                            byte[] bytes;
                            using (var memoryStream = new MemoryStream())
                            {
                                file.GetStream().CopyTo(memoryStream);
                                bytes = memoryStream.ToArray();
                            }

                            _imageService.Add(new Image
                            {
                                ImageBase64 = Convert.ToBase64String(bytes),
                                RecordedDate = DateTime.Now
                            });

                            imgStream = file.GetStream();
                            ImgSource = ImageSource.FromStream(() =>
                            {
                                var img = file.GetStream();
                                file.Dispose();
                                return img;
                            });

                            _userDialogsService.HideLoading();
                        });
                    }
                    catch
                    {
                        Device.BeginInvokeOnMainThread(() =>
                         {
                             _userDialogsService.HideLoading();
                         });

                        var toastConfig = new ToastConfig("Erro ao efetuar conexão com a sua câmera ou galeria de fotos")
                        {
                            Duration = TimeSpan.FromMilliseconds(5000),
                            MessageTextColor = System.Drawing.Color.White,
                            BackgroundColor = System.Drawing.Color.Red

                        };
                        _userDialogsService.Toast(toastConfig);
                    }
                });
            }
        }

        void ReadTextFromImage(OcrResults ocrResult)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (ocrResult == null)
                {
                    var toastConfig = new ToastConfig("Erro na execução do OCR")
                    {
                        BackgroundColor = System.Drawing.Color.Red,
                        Duration = TimeSpan.FromMilliseconds(5000),
                        MessageTextColor = System.Drawing.Color.White
                    };
                    _userDialogsService.Toast(toastConfig);
                }

                _userDialogsService.HideLoading();

                foreach (var region in ocrResult.Regions)
                {
                    foreach (var line in region.Lines)
                    {
                        foreach (var word in line.Words)
                        {
                            _userDialogsService.Confirm(new ConfirmConfig
                            {
                                Message = $"Encontrei a seguinte palavra {word.Text}",
                                OkText = "OK",
                                Title = "OCR - Status"
                            });
                        }
                    }
                }
            });
        }

        public async Task<OcrResults> CallVisionCognitiveService(Stream imgStream)
        {
            try
            {
                OcrResults text;

                var client = new VisionServiceClient("51644637681e498f8dff98b6d69a0c7f");

                using (var stream = imgStream)
                    text = await client.RecognizeTextAsync(stream);

                return text;
            }
            catch (Exception ex)
            {
                var toastConfig = new ToastConfig(ex.InnerException.Message)
                {
                    BackgroundColor = System.Drawing.Color.Red,
                    MessageTextColor = System.Drawing.Color.White
                };
                _userDialogsService.Toast(toastConfig);
                return null;
            }
        }

        public HomePageViewModel(IMedia mediaService, IUserDialogs userDialogsService,
                                 IApplicationServices<Product> productService, IApplicationServices<Image> imageService)
        {
            _imageService = imageService;
            _mediaService = mediaService;
            _productService = productService;
            _userDialogsService = userDialogsService;

            PickImgCmd = new DelegateCommand(PickImg);
            AnalyseImgCmd = new DelegateCommand(AnalyseImg);
        }

        //public HomePageViewModel() { }
    }
}