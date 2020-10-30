using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace WindowsGallaryApp.Scripts
{
    class ImageGenerator
    {
        public string Title { get; set; }
        public string DateCreated { get; set; }
        public string Type { get; set; }

        public ImageGenerator(string Title, string ImageLocation, string fileType, DateTimeOffset DateCreated)
        {
            var lastDash = Title.LastIndexOf('-');
            this.Title = Title.Substring(0, lastDash);

            this.Type = fileType;

            this.DateCreated = DateCreated.UtcDateTime.ToString();
        }

        public static async Task<List<ImageGenerator>> GenerateImages()
        {
            String filePath = @"Assets\PagerControlSampleImages";
            StorageFolder mediaFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(filePath);
            var files = await mediaFolder.GetFilesAsync();

            List<ImageGenerator> images = new List<ImageGenerator>();

            foreach (var file in files)
            {
                images.Add(new ImageGenerator(file.DisplayName, file.Path, file.FileType, file.DateCreated));
            }

            return images;
        }

        public static async Task<int> GetNumberOfPages(int itemsPerPage, List<ImageGenerator> totalList)
        {
            int numberOfPages = 0;

            numberOfPages = (int)Math.Ceiling((double)totalList.Count / itemsPerPage);

            return numberOfPages;
        }

        public static async Task<List<ImageGenerator>> GetImagesInPage(int selectedIndex, int imagesPerPage, List<ImageGenerator> totalList)
        {
            if ((selectedIndex + 1) * imagesPerPage > totalList.Count)
            {
                return totalList.GetRange(selectedIndex * imagesPerPage, totalList.Count - (selectedIndex * imagesPerPage));
            }

            return totalList.GetRange(selectedIndex * imagesPerPage, imagesPerPage);
        }


    }
}

