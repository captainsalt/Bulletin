namespace Bulliten.API.Models.Server
{
    public class JsonError
    {
        public string Message { get; set; }

        public JsonError(string message)
        {
            Message = message;
        }
    }
}
