using System.IO;
using System.Web;

namespace VotacionesWebSite.Utilities
{
    public class Utilities
    {
        public static void UploadPhoto(HttpPostedFileBase file)
        {
            ////Upload photo
            //string path = string.Empty;
            //string pic = string.Empty;

            //if (file != null)
            //{
            //    pic = Path.GetFileName(file.FileName);
            //    path = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/Photos"), pic);
            //    file.SaveAs(path);
            //    using (MemoryStream ms = new MemoryStream())
            //    {
            //        file.InputStream.CopyTo(ms);
            //        byte[] array = ms.GetBuffer();
            //    }
            //}

        }
    }
}
