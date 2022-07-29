using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Markdig.Renderers;
using Markdig.Syntax.Inlines;

namespace Unity.Markdown.ObjectRenderers
{
    public class LinkInlineRenderer : MarkdownObjectRenderer<UIMarkdownRenderer, LinkInline>
    {
        protected override void Write(UIMarkdownRenderer renderer, LinkInline obj)
        {
            string link = obj.GetDynamicUrl != null ? obj.GetDynamicUrl() ?? obj.Url : obj.Url;
            if (!obj.IsImage)
            {
                renderer.OpenLink(link);
                renderer.WriteChildren(obj);
                renderer.CloseLink();
            }
            else
            {
                var imgElem = renderer.AddImage();

                if (link.Contains("http://") || link.Contains("https://"))
                {
                    var uwr = new UnityWebRequest(link, UnityWebRequest.kHttpVerbGET);

                    uwr.downloadHandler = new DownloadHandlerTexture();
                    var asyncOp = uwr.SendWebRequest();

                    asyncOp.completed += operation =>
                    {
                        imgElem.image = DownloadHandlerTexture.GetContent(uwr);
                        imgElem.tooltip = "This is a test";
                        uwr.Dispose();
                    };
                }
                else
                {
                    Texture2D tex = null;
                    byte[] fileData;

                    var path = Path.Combine(Application.dataPath, link);

                    if (File.Exists(path))
                    {
                        fileData = File.ReadAllBytes(path);
                        tex = new Texture2D(2, 2);
                        tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.

                        imgElem.image = tex;
                        imgElem.tooltip = "This is a test";
                    }
                    else
                    {
                        Debug.Log("File does not exist");
                    }
                }
            }
        }
    }
}