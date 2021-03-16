using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Web;

namespace SmartOrderService.Services
{
    public class ImageCreatorService
    {

        public Image createImage(string html) {

            TheArtOfDev.HtmlRenderer.WinForms.HtmlPanel htmlPanel = new TheArtOfDev.HtmlRenderer.WinForms.HtmlPanel();

            Image image = TheArtOfDev.HtmlRenderer.WinForms.HtmlRender.RenderToImage(html, new Size(380, 200), new Size(380, 100000));

            return image;
        }

    }
}