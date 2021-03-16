using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using SmartOrderService.Models.DTO;
using SmartOrderService.DB;

namespace SmartOrderService.Mappers
{
    public class ProductMapper : IMapper<ProductDto, so_product>
    {
        public so_product toEntity(ProductDto model)
        {
            throw new NotImplementedException();
        }

        public ProductDto toModel(so_product entity)
        {
            throw new NotImplementedException();
        }
    }
}