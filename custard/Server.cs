using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CreamRoll;
using static CreamRoll.RouteServer;
using static custard.ServerInfo;

namespace custard {
    public class Server {
        private readonly string indexHtml;
        BundleManager bundle => BundleManager.Instance;

        public Server() {
            indexHtml = File.ReadAllText("Pages/index.html");
        }

        [Get(ROUTE_PREFIX, ContentType = "text/html; charset=utf8")]
        public Task<string> Index(RouteServerBase.RouteContext ctx) {
            return Task.FromResult("currently disabled");
            return Task.FromResult(indexHtml.Replace("{{files}}",
                string.Join("<br>",
                    Directory.GetFiles("bundles/")
                        .Select(s => s.Substring(6))
                        .Select(s => $"<a href='/custard/api/v1/bundle/{s}'>{s}</a>"))));
        }

        [Get(API_PREFIX + "/bundles")]
        public Task<string> GetBundleHashList() {
            return Task.FromResult(
                $@"[{string.Join(',', bundle.GetHashes())}]");
        }

        [Get(API_PREFIX + "/bundles/latest", ManuallyResponse = true)]
        public Task GetLatestBundle(RouteServerBase.RouteContext ctx) {
            return HandleBundle(ctx.Response, bundle.LatestHash);
        }

        [Get(API_PREFIX + "/bundles/latest/hash")]
        public Task<string> GetLatestHash() {
            return Task.FromResult(bundle.LatestHash);
        }

        [Get(API_PREFIX + "/bundles/{hash}", ManuallyResponse = true)]
        public Task GetBundle(RouteServerBase.RouteContext ctx) {
            return HandleBundle(ctx.Response, ctx.Query.hash);
        }

        [Get(API_PREFIX + "/bundles/{hash}/hash")]
        public Task<string> GetBundleHash(RouteServerBase.RouteContext ctx) {
            return Task.FromResult(ctx.Query.hash);
        }

        private async Task HandleBundle(HttpListenerResponse response, string hash) {
            if (!bundle.TryGetBundleOfHash(hash, out string path)) {
                response.StatusCode = 404;
                response.OutputStream.Close();
                return;
            }

            response.ContentType = "application/octet-stream";

            try {
                using (var file = new FileStream(path, FileMode.Open)) {
                    await file.CopyToAsync(response.OutputStream);
                    await response.OutputStream.FlushAsync();
                    file.Close();
                }
            }
            catch (Exception ex) {
                response.StatusCode = 500;
                await Console.Error.WriteLineAsync("Error on copying file stream:");
                await Console.Error.WriteLineAsync(ex.Message);
                response.OutputStream.Close();
            }
            finally {
                response.OutputStream.Close();
            }
        }
    }
}
