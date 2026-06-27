// ErrorCodes.cs - Centralized error code definitions

namespace HostlistDownloader.Modules
{
    public static class ErrorCodes
    {

        // General errors
        public const int GeneralError = 1;
        public const int NetworkConnectionFailed = 2;

        // File system errors
        public const int DirectoryCreationFailed = 10;

        // Configuration errors
        public const int ConfigurationFileMissing = 20;
        public const int ConfigurationCorrupted = 21;
        public const int InvalidConfigEntry = 22; //Configuration file is present, but the attempt to use configuration failed.

        // Update process errors
        public const int UpdateProcessError = 40; //Hostfiles update failed outright.
        public const int PartialUpdateWithIssues = 41; //Hostfiles updates partially but some might've timed out.
        // Internal failures
        public const int IntegrityCheckFailure = 42; //Thrown when an operation output is checked and the output differs from what we expect.
        public const int TaskThreadTimeout = 43;
        // Environment errors
        public const int WrongExecutionDirectory = 50;

        // Helper method to get error description
        public static string GetDescription(int errorCode)
        {
            return errorCode switch
            {
                GeneralError => "General error occurred",
                NetworkConnectionFailed => "Network connection failed",
                DirectoryCreationFailed => "Failed to create directory",
                ConfigurationFileMissing => "Critical configuration file missing",
                ConfigurationCorrupted => "Configuration file corruption detected",
                InvalidConfigEntry => "Invalid configuration entry detected",
                UpdateProcessError => "Error during update process",
                PartialUpdateWithIssues => "Update completed with issues",
                WrongExecutionDirectory => "Program executed from incorrect directory",
                TaskThreadTimeout => "A multi-threaded task has reached a timeout threshold",
                IntegrityCheckFailure => "Data validation check failed",
                _ => "Unknown error occurred"
            };
        }
    }
}