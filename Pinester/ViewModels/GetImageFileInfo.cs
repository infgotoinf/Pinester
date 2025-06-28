using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pinester.DataBase;
using Pinester.Models;

namespace Pinester.ViewModels
{
    internal class GetImageFileInfo
    {
        private ImageFileInfo _imageFileInfo;
        private ImageInfo dbInfo;
        public GetImageFileInfo(ImageInfo dbData, DatabaseService db) 
        { 
            dbInfo = dbData;
            _imageFileInfo = new();

            Dictionary<string, int> colorDict = PictureMainColor.GetInfo(dbData.ImageSource);
            
            InfoColor color = new(
                colorDict["R"],
                colorDict["G"],
                colorDict["B"]);

            


            _imageFileInfo.MainColor = color;
            _imageFileInfo.Date = db.GetPictureInfoDate(dbInfo);

        }
    }
}
