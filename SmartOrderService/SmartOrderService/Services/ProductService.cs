using SmartOrderService.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;

using SmartOrderService.DB;

namespace SmartOrderService.Services
{
   public  class ProductService
    {
        private SmartOrderModel db = new SmartOrderModel();

        public List<ProductDto> getProducts(DateTime updated,int branchId)
        {
            List<ProductDto> Products = new List<ProductDto>();

            var datas = 
                db.so_product.Where(p => p.status).ToList();

            var bottles = db.so_product_bottle.Where(pb => pb.status).Select(b => new {b.productId,b.bottleId }).ToList();
            var categories = db.so_product_category_branch.Where(pcb => pcb.status).Select(c => new { c.productId, c.categoryId }).Distinct().ToList();

            foreach (var product in datas) {
                ProductDto dto = Mapper.Map<ProductDto>(product);
                var category = categories.Where(c => c.productId == dto.ProductId).FirstOrDefault();
                if (category == null)
                    dto.CategoryId = 1;
                else
                    dto.CategoryId = category.categoryId;
                dto.Status = dto.Status && product.status;
                dto.BottleId = bottles.Where(b => b.productId == product.productId).Select(x => x.bottleId).FirstOrDefault();                
                Products.Add(dto);
            }

            return Products;

        }

        public List<ProductDto> getAll(DateTime updated,int branchId)
        {

            var products = getProducts(updated,branchId);
          
            return products;
        }  

        public ProductDto getById(int productId)
        {
            var product = db.so_product.Where(p => p.productId == productId).FirstOrDefault();
            ProductDto productDto = Mapper.Map<ProductDto>(product);
            return productDto;
        }
    }
}
