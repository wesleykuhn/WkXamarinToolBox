using WkXamarinToolBox.Enums;

namespace WkXamarinToolBox.Models
{
    public class DetailedResponseModel
    {
        public ResponseEnum RequestResponse { get; set; }
        public object ResponseObject { get; set; }

        public DetailedResponseModel(ResponseEnum response)
        {
            RequestResponse = response;
        }

        public DetailedResponseModel(ResponseEnum response, object responseObject)
        {
            RequestResponse = response;
            ResponseObject = responseObject;
        }
    }
}
