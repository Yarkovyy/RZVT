using Gallery.BusinessLogic.Models;
using Microsoft.AspNetCore.SignalR;

namespace GalleryMVC.SignalR
{
    public class GalleryHub: Hub
    {
        public async Task NewImageUploaded(ImageInfo image)
        {
            await Clients.All.SendAsync("ReceiveNewImage", image);
        }
    }
}
