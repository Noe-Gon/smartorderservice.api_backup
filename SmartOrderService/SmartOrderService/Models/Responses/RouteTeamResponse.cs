using System.Collections.Generic;

namespace SmartOrderService.Models.Responses
{
    public class GetRouteTeamResponse
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }

    public class GetTravelsInProcessResponse
    {
        public GetTravelsInProcessResponse()
        {
            IsInProcess = false;
            TravelsInProcess = new List<TravelsInProcess>();
        }
        public bool IsInProcess { get; set; }
        public List<TravelsInProcess> TravelsInProcess { get; set; }
    }

    public class TravelsInProcess
    {
        public int InventoryId { get; set; }
        public string Code { get; set; }
    }
}