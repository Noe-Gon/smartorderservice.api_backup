using SmartOrderService.Models.Responses;
using SmartOrderService.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Services
{
    public class CodePlaceService : IDisposable
    {
        public static CodePlaceService Create() => new CodePlaceService();

        private UoWConsumer UoWConsumer { get; set; }

        public CodePlaceService()
        {
            UoWConsumer = new UoWConsumer();
        }

        public ResponseBase<List<GetCodePlacesResponse>> GetCodePlaces()
        {
            try
            {
                var codePlaces = UoWConsumer.CodePlaceRepository
                    .Get(x => x.Status)
                    .Select(x => new GetCodePlacesResponse
                    {
                       Id = x.Id,
                       Name = x.Name
                    }).ToList();

                return ResponseBase<List<GetCodePlacesResponse>>.Create(codePlaces);

            }
            catch (Exception e)
            {
                return ResponseBase<List<GetCodePlacesResponse>>.Create(new List<string>()
                {
                    e.Message
                });
            }
        }

        public void Dispose()
        {
            this.UoWConsumer.Dispose();
            this.UoWConsumer = null;
        }
    }
}