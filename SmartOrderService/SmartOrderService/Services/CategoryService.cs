using AutoMapper;
using SmartOrderService.DB;
using SmartOrderService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Services
{
    public class CategoryService
    {

        private SmartOrderModel db = new SmartOrderModel();

        public List<CategoryDto> getCategories()
        {
            List<CategoryDto> categories = null;

            var dbcategories = db.so_category.ToList();

            categories = Mapper.Map<List<CategoryDto>>(dbcategories);

            return categories;
            
        }
    }
}