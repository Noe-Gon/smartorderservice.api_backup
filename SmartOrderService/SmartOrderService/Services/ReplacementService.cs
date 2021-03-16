using AutoMapper;
using SmartOrderService.DB;
using SmartOrderService.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Services
{
    public class ReplacementService
    {
        private SmartOrderModel db = new SmartOrderModel();

        public List<ReplacementDto> getReplacements() {

            List<ReplacementDto> replacements = null;

            var dbreplacements = db.so_replacement.ToList();

            replacements = Mapper.Map<List<ReplacementDto>>(dbreplacements);


            return replacements;
        }

        public ReplacementDto getReplacement(int replacementId)
        {
            var replacement = db.so_replacement.Where(r => r.replacementId == replacementId).FirstOrDefault();
            return Mapper.Map<ReplacementDto>(replacement);
        }

    }
}