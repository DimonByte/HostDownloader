namespace HostlistDownloader.Modules.DownloadSystem
{
    public static class ConsoleProgress
    {
        public static void ShowDownloadProgress(long downloadedBytes, long totalBytes, string fileName)
        {
            if (totalBytes > 0)
            {
                int percentage = (int)((downloadedBytes * 100) / totalBytes);
                int progressWidth = 50;
                int progressBlocks = (percentage * progressWidth) / 100;

                string progressBar = new string('█', progressBlocks) + new string('░', progressWidth - progressBlocks);

                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write($"[{progressBar}] {percentage}% - {fileName}\n");
            }
        }

        public static void ShowOperationProgress(int current, int total, string operationName)
        {
            if (total > 0)
            {
                double percentage = (double)current / total * 100;
                int progressWidth = 50;
                int progressBlocks = (int)(percentage * progressWidth / 100);

                string progressBar = new string('█', progressBlocks) + new string('░', progressWidth - progressBlocks);
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write($"[{progressBar}] {current}/{total} {operationName}\n");
            }
        }

        public static void ClearLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, currentLineCursor);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }
    }
}
