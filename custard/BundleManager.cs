using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace custard {
    public class BundleManager {
        public static BundleManager Instance { get; private set; }
        private readonly DirectoryInfo bundlesDirectory;

        public string LatestHash { get; private set; }

        private readonly string bundlePath, configFileName;

        public BundleManager(string bundlePath, string configFileName) {
            this.bundlePath = bundlePath;
            this.configFileName = configFileName;

            bundlesDirectory = new DirectoryInfo(bundlePath);
            LatestHash = LoadLatestHash(Path.Combine(bundlePath, configFileName));

            Instance = this;
        }

        private static string LoadLatestHash(string path) {
            using (var file = new FileStream(path, FileMode.Open))
            using (var reader = new StreamReader(file)) {
                file.Position = 0;
                return reader.ReadLine().Trim();
            }
        }

        // thread safe..?
        private static void SaveLatestHash(string path, string hash) {
            using (var file = new FileStream(path, FileMode.Open, FileAccess.Write, FileShare.None))
            using (var writer = new StreamWriter(file)) {
                writer.WriteLine(hash);
            }
        }

        public bool TrySetLatestHash(string hash) {
            if (!TryGetBundleOfHash(hash, out _)) {
                return false;
            }

            SaveLatestHash(Path.Combine(bundlePath, configFileName), hash);
            return true;
        }

        public IEnumerable<string> GetHashes() {
            return bundlesDirectory
                .GetFiles()
                .Select(d => d.Name)
                .Where(IsValidHash);
        }

        private static bool IsValidHash(string hash) {
            if (hash.Length != 32) {
                return false;
            }

            foreach (var c in hash) {
                if (!char.IsLetterOrDigit(c)) {
                    return false;
                }
            }

            return true;
        }

        public bool TryGetBundleOfHash(string hash, out string path) {
            var files = bundlesDirectory.GetFiles(hash);
            if (files.Length == 0) {
                path = null;
                return false;
            }

            path = files[0].FullName;
            return true;
        }
    }
}