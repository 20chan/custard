using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CreamRoll;
using static CreamRoll.RouteServer;

namespace custard {
    public class Server {
        [Get("/")]
        public async Task Index(RouteServerBase.RouteContext ctx) {
            var file = File.OpenRead("Pages/index.html");
            await file.CopyToAsync(ctx.Response.OutputStream);
        }

        [Get("/api/v1/files/{name}", ManuallyResponse = true)]
        public async Task Upload(RouteServerBase.RouteContext ctx) {
            string path = Path.Combine("Files", ctx.Query.name);
            if (!File.Exists(path)) {
                ctx.Response.StatusCode = 404;
                ctx.Response.OutputStream.Close();
                return;
            }

            ctx.Response.ContentType = "application/octet-stream";

            try {
                using (var file = new FileStream(path, FileMode.Open)) {
                    await file.CopyToAsync(ctx.Response.OutputStream);
                    await ctx.Response.OutputStream.FlushAsync();
                    file.Close();
                }
            }
            catch (Exception ex) {
                await Console.Error.WriteLineAsync("Error on copying file stream:");
                await Console.Error.WriteLineAsync(ex.Message);
                ctx.Response.OutputStream.Close();
                return;
            }
            finally {
                ctx.Response.OutputStream.Close();
            }
        }
    }
}