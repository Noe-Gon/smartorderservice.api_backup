using SmartOrderService.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Services
{
    public class AssistantService
    {
        private SmartOrderModel db = new SmartOrderModel();
        private InventoryService inventoryService = new InventoryService();

        public bool checkCurrentTravelState(int userId)
        {
            int inventoryState = getInventoryState(userId);
            if (inventoryState == InventoryService.INVENTORY_OPEN)
            {
                return true;
            }
            if (inventoryState == InventoryService.INVENTORY_AVAILABLE && inventoryState == InventoryService.INVENTORY_CLOSED)
            {
                return false;
            }
            throw new Exception();
        }

        private int getInventoryState(int userId)
        {
            DateTime today = DateTime.Today;
            int inventoryState = inventoryService.getInventoryState(userId,today);
            return inventoryState;
        }
    }
}