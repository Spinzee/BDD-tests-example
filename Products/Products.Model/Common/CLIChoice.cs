namespace Products.Model.Common
{
    public class CLIChoice
    {
        public string OpenReachProvidedCLI { get; set; }

        public string UserProvidedCLI { get; set; }

        public bool KeepExisting { get; set; }

        public string FinalCLI => !string.IsNullOrEmpty(OpenReachProvidedCLI) ? OpenReachProvidedCLI : !string.IsNullOrEmpty(UserProvidedCLI) ? UserProvidedCLI : string.Empty;
    }
}