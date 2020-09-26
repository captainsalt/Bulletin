namespace Bulliten.API.Models
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
