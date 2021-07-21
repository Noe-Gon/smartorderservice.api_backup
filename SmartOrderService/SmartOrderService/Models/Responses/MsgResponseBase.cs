using System.Collections.Generic;
using System.Web.Http.ModelBinding;

namespace SmartOrderService.Models.Responses
{
    public class ResponseBase<T>
    {
        public ResponseBase()
        {
            this.Errors = new List<string>();
        }

        public static ResponseBase<T> Create(T Data) => new ResponseBase<T>
        {
            Status = true,
            Data = Data
        };

        public static ResponseBase<T> Create(List<string> Errors) => new ResponseBase<T>
        {
            Status = false,
            Errors = Errors
        };

        public bool Status { get; set; }
        public T Data { get; set; }
        public List<string> Errors { get; set; }
    }
}