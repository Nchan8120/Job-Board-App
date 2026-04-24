namespace JobBoard.API.Models
{
    /// <summary>
    /// A generic wrapper returned by all service methods.
    /// Allows controllers to interpret success or failure without catching exceptions.
    /// </summary>
    public class ServiceResult<T>
    {
        public bool Successful { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        public static ServiceResult<T> Success(T data, string message = "") =>
            new ServiceResult<T> { Successful = true, Data = data, Message = message };

        public static ServiceResult<T> Failure(string message) =>
            new ServiceResult<T> { Successful = false, Message = message };
    }
}