namespace BackendFunctions
{
    public class FunctionOptions
    {
        public string StorageAccountConnectionString { get; set; }
        public string ContainerName { get; set; }
        public string ThesisDbConnectionString { get; set; }
        public string CvEndpoint { get; set; }
        public string CvKey { get; set; }
    }
}
