//MIT License

//Copyright (c) 2026 Dimon

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using HostlistDownloader.Modules.WindowsSystem;

namespace HostlistDownloader.Modules.DownloadSystem
{
    public static class HostListManager
    {
        public static void UpdateLists()
        {
            //Is Inis blank?
            if (String.IsNullOrEmpty(File.ReadAllText(IOManager.IniBlockListFileLocation)) &
                String.IsNullOrEmpty(File.ReadAllText(IOManager.IniWhiteListFileLocation)))
            {
                TraceLogger.Log("Blocklist and Whitelist INI is not configured. Please configure HostlistDownloader.", Enums.StatusSeverityType.Fatal);
                return;
            }
            IOManager.ClearFiles(IOManager.BlockListFolderLocation);
            IOManager.ClearFiles(IOManager.WhiteListFolderLocation);
            TraceLogger.Log($"Downloading and updating blocklists and whitelists...", Enums.StatusSeverityType.Information);
            DownloadListsAsync(IOManager.IniBlockListFileLocation, IOManager.BlockListFolderLocation, IOManager.CombinedBlockListFileLocation).GetAwaiter().GetResult();
            DownloadListsAsync(IOManager.IniWhiteListFileLocation, IOManager.WhiteListFolderLocation, IOManager.CombinedWhiteListFileLocation).GetAwaiter().GetResult();
            TraceLogger.Log("Attempting to merge user defined website lists...");
            try
            {
                string[] UserWebsiteWhitelistLines = File.ReadAllLines(IOManager.UserWebsiteWhiteListFileLocation);
                File.AppendAllLines(IOManager.CombinedWhiteListFileLocation, UserWebsiteWhitelistLines);
                string[] UserWebsiteBlocklistLines = File.ReadAllLines(IOManager.UserWebsiteBlockListFileLocation);
                File.AppendAllLines(IOManager.CombinedBlockListFileLocation, UserWebsiteBlocklistLines);
            }
            catch (Exception ex)
            {
                TraceLogger.Log($"Fault during update of lists! {ex}");
            }
            TraceLogger.Log("Host lists Update Completed!");
        }

        private static async Task DownloadListsAsync(string IniLocation, string ListFolderLocation, string CombinedListLocation)
        {
            TraceLogger.Log($"Starting download async for INI {IniLocation} | ListFolderLocation: {ListFolderLocation} | CombinedListLocation: {CombinedListLocation}");
            DateTime StartOfBlockList = DateTime.Now;
            if (!File.Exists(IniLocation))
            {
                TraceLogger.Log($"List configuration file not found: {IniLocation}", Enums.StatusSeverityType.Error);
                return;
            }

            List<string> urls = ReadUrlsFromFile(IniLocation);
            int completeNumber = 0;
            foreach (var url in urls)
            {
                TraceLogger.Log($"Progress: [{completeNumber} out of {urls.Count}] - Downloading list from: {url}");
                var fileName = Path.GetFileName(url);
                var filePath = Path.Combine(ListFolderLocation, fileName);
                await DownloadController.DownloadFileAsync(url, filePath).ConfigureAwait(false);
                TraceLogger.Log($"Starting next download...");
                completeNumber++;
            }
            TraceLogger.Log("Finished downloading lists.");
            IOManager.MergeFiles(ListFolderLocation, CombinedListLocation);
            IOManager.RemoveDuplicates(CombinedListLocation);
            TraceLogger.Log("List update complete. Checking if all files have been updated recently");

            //Possible fix for "This causes IO exception when IOManager attempts to merge the files. Being used by another process."
            //We will check after downloads are complete to delete the files since then we are no longer locked.
            foreach (var file in Directory.GetFiles(ListFolderLocation))
            {
                DateTime lastWriteTime = File.GetLastWriteTime(file);
                if (lastWriteTime < StartOfBlockList)
                {
                    TraceLogger.Log($"Deleting {file} since it was not written to during downloadlistasync. (LastWriteTime is less than StartOfBlockListTime)");
                    File.Delete(file);
                }
                else
                {
                    TraceLogger.Log($"List file {file} was updated successfully. Last write time: {lastWriteTime}");
                }
            }
        }

        private static List<string> ReadUrlsFromFile(string filePath)
        {
            var urls = new List<string>();
            if (!File.Exists(filePath))
                return urls;

            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                    continue;

                urls.Add(line.Trim());
            }

            return urls;
        }
    }
}
