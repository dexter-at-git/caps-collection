namespace CapsCollection.Silverlight.Infrastructure.Models
{
    public class ErrorInfo
    {
        public int ErrorCode { get; set; }
        public bool IsWarning { get; set; }
        public string ErrorMessage { get; set; }

        public override string ToString()
        {
            return (string.Format("{0}: {1}",
              IsWarning ? "Warning" : "Error",
              ErrorMessage));
        }
    }
}
