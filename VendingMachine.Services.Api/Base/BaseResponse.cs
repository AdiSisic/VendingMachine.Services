namespace VendingMachine.Services.Api.Base
{
    public class BaseResponse<T>
    {
        public BaseResponse()
        {
            Success = false;
        }

        public T Data { get; set; }
        public bool Success { get; set; }
    }
}
