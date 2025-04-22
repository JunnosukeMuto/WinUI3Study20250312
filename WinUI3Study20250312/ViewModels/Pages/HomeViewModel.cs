using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Data.Pdf;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.Storage;
using WinRT.Interop;
using System.Collections.ObjectModel;

namespace WinUI3Study20250312.ViewModels.Pages
{
    internal partial class HomeViewModel : ObservableObject
    {
        private ObservableCollection<string> hoge = new ObservableCollection<string>() { "hoge" };
        public ObservableCollection<string> Hoge
        {
            get => hoge;
            set
            {
                SetProperty(ref hoge, value);
            }
        }

        private ObservableCollection<BitmapImage> _pdfImages = [];
        public ObservableCollection<BitmapImage> PdfImages
        {
            get => _pdfImages;
            set => SetProperty(ref _pdfImages, value);
        }

        [RelayCommand]
        private async Task LoadPdfAsync()
        {
            Debug.WriteLine("LoadPdfAsync()");
            var filePicker = new FileOpenPicker();
            filePicker.SuggestedStartLocation = PickerLocationId.Downloads;
            filePicker.FileTypeFilter.Add(".pdf");

            var mainWindow = (App.Current as App)?.m_window;
            var hwnd = WindowNative.GetWindowHandle(mainWindow);
            InitializeWithWindow.Initialize(filePicker, hwnd);

            StorageFile file = await filePicker.PickSingleFileAsync();
            PdfDocument pdfDoc = await PdfDocument.LoadFromFileAsync(file);

            PdfImages.Clear();

            for (uint i = 0; i < pdfDoc.PageCount; i++)
            {
                var image = new BitmapImage();
                var page = pdfDoc.GetPage(i);
                using var stream = new InMemoryRandomAccessStream();
                await page.RenderToStreamAsync(stream);
                await image.SetSourceAsync(stream);
                PdfImages.Add(image);
            }
        }

        [RelayCommand]
        private async Task PlusHogeAsync()
        {
            var newHoge = Hoge;
            newHoge.Add("hoge");
            Hoge = newHoge;
        }
    }
}
