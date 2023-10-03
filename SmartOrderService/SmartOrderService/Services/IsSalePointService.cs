using SmartOrderService.DB;
using SmartOrderService.UnitOfWork;
using System;
using System.Linq;

namespace SmartOrderService.Services
{
    public class IsSalePointService : IDisposable
    {
        public static IsSalePointService Create() => new IsSalePointService();

        private UoWConsumer UoWConsumer { get; set; }

        public IsSalePointService()
        {
            UoWConsumer = new UoWConsumer();
        }

        public so_is_sale_point GetIsSalePoint(string branchCode)
        {
            try
            {
                var entity = UoWConsumer.IsSalePointRepository.Get(x => x.branchCode.Equals(branchCode)).FirstOrDefault();
                if (entity == null)
                {
                    DateTime date = DateTime.Now;
                    return new so_is_sale_point
                    {
                        branchCode = branchCode,
                        isSalePoint = false,
                        createdon = date,
                        modifiedon = date
                    };
                }
                return entity;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Dispose()
        {
            this.UoWConsumer.Dispose();
            this.UoWConsumer = null;
        }
    }
}