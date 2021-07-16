
using DataAccessLibrary.Models;
using ImageMagick;
using Microsoft.AspNetCore.Hosting;
using System;

using System.IO;


namespace Diary.WebUI.Helpers
{
    public static class Compressor
    {

        public static MagickImage CompressImage(Record recordModel)
        {
            var imgSize = recordModel.ImageFile.Length;
            MagickImage image = new MagickImage(recordModel.ImageFile.OpenReadStream());
            image.Format = image.Format; 

            if (imgSize > 1048576)
            {

                image.Resize(256, 256);
            }
            return image;
            }
        }
    }

