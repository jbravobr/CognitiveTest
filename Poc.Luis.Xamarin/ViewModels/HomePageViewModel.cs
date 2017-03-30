﻿using Prism.Commands;
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
                return new Action(async () =>
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

                    var ocrResult = await CallVisionCognitiveService(imgStream);
                    ReadTextFromImage(ocrResult);
                });
            }
        }

        public Action PickImg
        {
            get
            {
                return new Action(async () =>
                {
                    try
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

                        if (file == null)
                            return;
                        
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            _userDialogsService.ShowLoading(null, MaskType.Gradient);
                        });

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

                        Device.BeginInvokeOnMainThread(() =>
                        {
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

            foreach (var region in ocrResult.Regions)
            {
                foreach (var line in region.Lines)
                {
                    foreach (var word in line.Words)
                    {
                        if (word.Text.Contains(""))
                        {
                            _productService.Add(new Product
                            {
                                Name = "Produto Encontado",
                                RecordedDate = DateTime.Now,
                                Value = 1
                            });
                        }
                    }
                }
            }
        }

        public async Task<OcrResults> CallVisionCognitiveService(Stream imgStream)
        {
            var client = new VisionServiceClient("4e0ff94636434833bdf313f7e605ba85");

            try
            {
                using (var photoStream = imgStream)
                {
                    return await client.RecognizeTextAsync(photoStream);
                }

                //var httpClient = new HttpClient();
                //httpClient.BaseAddress = new Uri(https://westus.api.cognitive.microsoft.com/vision/v1.0/analyze
            }
            catch (Exception ex)
            {
                throw ex;
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