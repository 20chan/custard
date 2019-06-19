using System.IO;
using System.Threading.Tasks;
using CreamRoll;
using static CreamRoll.RouteServer;
using static custard.ServerInfo;

namespace custard {
    public class AdminServer {
        private readonly string adminPanelHtml;
        BundleManager bundle => BundleManager.Instance;

        public AdminServer() {
            adminPanelHtml = File.ReadAllText("Pages/adminPanel.html");
        }

        [Get(API_PREFIX + "/admin/panel", ContentType = "text/html; charset=utf8")]
        public Task<string> AdminPanelPage() {
            return Task.FromResult(adminPanelHtml);
        }

        [Post(API_PREFIX + "/bundles/latest/{hash}")]
        public Task<string> SetLatestHash(RouteServerBase.RouteContext ctx) {
            if (!bundle.TrySetLatestHash(ctx.Query.hash)) {
                ctx.Response.StatusCode = 400;
                return Task.FromResult("hash not found");
            }

            return Task.FromResult("succeed");
        }
    }
}