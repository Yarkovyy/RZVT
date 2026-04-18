using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gallery.DataAccess.Models
{
    public class Image
    {
        [Key]
        public int ImgId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ContentType { get; set; }
        public DateTime UploadDate { get; set; }
        public byte[] ImageData { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
