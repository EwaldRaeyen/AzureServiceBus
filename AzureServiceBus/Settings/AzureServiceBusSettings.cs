namespace AzureServiceBus.Settings
{
    public class AzureServiceBusSettings
    {
        public string ConnectionString { get; set; }
        //public string ProjectQueue { get; set; }
        //public string TodoListQueue { get; set; }
        public string TodoItemQueue { get; set; }
        public string TodoItemTopic { get; set; }
    }
}
