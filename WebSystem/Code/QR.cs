using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataSystem;
using DataSystem.DB;
using System.IO;
using System.Web.Hosting;

namespace WebSystem.Code
{
    public static class QR
    {
        private static ZXing.BarcodeWriter _BarcodeWriter;
        private static string ImagePath = HostingEnvironment.MapPath("~/Image/");

        private static ZXing.BarcodeWriter BarcodeWriter
        {
            get
            {
                if (_BarcodeWriter == null)
                {
                    _BarcodeWriter = new ZXing.BarcodeWriter()
                    {
                        Format = ZXing.BarcodeFormat.QR_CODE,
                        Options = new ZXing.Common.EncodingOptions
                        {
                            Height = 200,
                            Width = 200,
                            Margin = 1
                        },
                    };
                    _BarcodeWriter.Options.Hints.Add(ZXing.EncodeHintType.CHARACTER_SET, "utf-8");
                    DirectoryInfo directoryInfo = new DirectoryInfo(ImagePath);
                    if (!directoryInfo.Exists) directoryInfo.Create();
                }
                return _BarcodeWriter;
            }
        }

        /// <summary>
        /// 学生信息保存为QR
        /// </summary>
        /// <param name="student"></param>
        public static void StudentToQR(this Student student)
        {
            var img= BarcodeWriter.Write(new { student.Id, student.Name, student.StudentName }.ToJsonForWeb());
            img.Save($"{ImagePath}{student.Id}.jpg");
        }

    }
}