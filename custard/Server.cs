using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using CreamRoll;
using static CreamRoll.RouteServer;

namespace custard {
    public class Server {
        private readonly string indexHtml;

        public Server() {
            indexHtml = File.ReadAllText("Pages/index.html");
        }

        [Get("/custard/", ContentType = "text/html; charset=utf8")]
        public Task<string> Index(RouteServerBase.RouteContext ctx) {
            return Task.FromResult(indexHtml.Replace("{{files}}",
                            string.Join("<br>",
                                                Directory.GetFiles("Files/")
                                                                        .Select(s => s.Substring(6))
                                                                                                .Select(s => $"<a href='/custard/api/v1/files/{s}'>{s}</a>"))));
        }

        [Get("/custard/api/v1/files/{name}", ManuallyResponse = true)]
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
