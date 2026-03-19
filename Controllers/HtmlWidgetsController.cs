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

        var result = await proxyService.ProxyHtml("https://www.bettyasfaltcomplex.nl/agenda.php");

        result = result.Replace("<script type=\"text/javascript\" src=\"v1/scripts/layout.js\"></script>", "");
        result = result.Replace("<script type=\"text/javascript\" src=\"v1/agenda/scripts.js\"></script>", "");

        ViewData["html"] = result;

        return View();
    }
}
