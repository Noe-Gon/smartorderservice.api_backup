using SmartOrderService.CustomExceptions;
using SmartOrderService.DB;
using SmartOrderService.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Services
{
    public class ApiVersionService
    {
        SmartOrderModel db = new SmartOrderModel();
        public ApiVersionDto getApiVersion(string BranchCode, string UserCode,string PackageName)
        {

            var user = new UserService().getUser(UserCode, BranchCode);

            var ApplicationWBC = new ApplicationService().getApplication(PackageName);

            if (ApplicationWBC == null)
                throw new NoUserFoundException();

            string _URL  = ApplicationWBC.WSUrl;

            var ApiVersion = db.so_api_version.FirstOrDefault(a => a.userId.Equals(user.UserId) && a.applicationId.Equals(ApplicationWBC.ApplicationId) && a.status);

            if (ApiVersion != null)
                _URL = ApiVersion.url;

            return new ApiVersionDto
            {
                BrachCode = BranchCode,
                UserCode = UserCode,
                Url = _URL
            };

        }
    }
}