using HtmlAgilityPack;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

[Route("widgets")]
[Controller]
public class HtmlWidgetsController(IProxyService proxyService) : Controller
{
    [HttpGet("bettyasfalt-agenda")]
    public async Task<IActionResult> BettyAsfaltAgenda()
    {
        HttpContext.Response.Headers.Append("Content-Security-Policy", "frame-ancestors 'self' https://bettyasfalt.nl");
        HttpContext.Response.Headers.Remove("X-Frame-Options");

        var html = await proxyService.ProxyHtml("https://www.bettyasfaltcomplex.nl/agenda.php");
        
        html = html.Replace("<script type=\"text/javascript\" src=\"v1/scripts/layout.js\"></script>", "");
        html = html.Replace("<script type=\"text/javascript\" src=\"v1/agenda/scripts.js\"></script>", "");

        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);

        var rows = htmlDoc.DocumentNode.SelectNodes("//div[@id='resagenda']//div[contains(@class, 'agendarow')]");

        if (rows != null)
        {
            foreach (var row in rows)
            {
                var prodLink = row.SelectSingleNode(".//div[contains(@class, 'agendaproductie')]//a");
                var targetLink = row.SelectSingleNode(".//div[contains(@class, 'agendakaarten')]//a");

                if (prodLink != null && targetLink != null)
                {
                    targetLink.SetAttributeValue("href", prodLink.GetAttributeValue("href", string.Empty));
                }
            }
        }

        var anchors = htmlDoc.DocumentNode.SelectNodes("//a");
        if (anchors != null)
        {
            foreach (var a in anchors)
            {
                a.SetAttributeValue("target", "_blank");
            }
        }

        return Content(htmlDoc.DocumentNode.OuterHtml, "text/html");
    }
}
