public class ProxyService(IHttpClientFactory httpClientFactory) : IProxyService
{
    public async Task<string> ProxyHtml(string url)
    {
        if (string.IsNullOrEmpty(url)) return $"URL({url}) not valid.";

        var sanatized_url = new Uri(url);

        var client = httpClientFactory.CreateClient();

        var response = await client.GetAsync(url);
        var html = await response.Content.ReadAsStringAsync();

        // Inject a <base> tag so images/css don't break
        var baseTag = $"<base href='{sanatized_url.GetLeftPart(UriPartial.Authority)}/'>";
        var modifiedHtml = html.Replace("<head>", $"<head>{baseTag}");

        return modifiedHtml;
    }
}