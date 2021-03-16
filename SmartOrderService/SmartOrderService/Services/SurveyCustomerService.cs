using Newtonsoft.Json;
using SmartOrderService.DB;
using SmartOrderService.Models.DTO;
using SmartOrderService.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Services
{
    public class SurveyCustomerService
    {

        public List<SurveyCustomerResponse> getAllByRoute(string branch, string route, bool withoutData)
        {
            using (SmartOrderModel db = new SmartOrderModel())
            {
                if(withoutData)
                return db.so_survey_customer.Where(sc => sc.route == route && sc.branch == branch).Select(sc => new SurveyCustomerResponse{
                    status= sc.status,
                    createdOn = sc.createdOn,
                    createdBy = sc.createdBy,
                    modifiedOn = sc.modifiedOn,
                    modifiedBy = sc.modifiedBy,
                    route = sc.route,
                    routeType = sc.routeType,
                    code = sc.code,
                    customer = sc.customer,
                    branch = sc.branch,
                    address = sc.address,
                    contactName = sc.contactName,
                    surveyCustomerId = sc.surveyCustomerId
                })
                    .ToList();
               else
                {
                    return db.so_survey_customer.Where(sc => sc.route == route && sc.branch == branch).Select(sc => new SurveyCustomerResponse
                    {
                        status = sc.status,
                        createdOn = sc.createdOn,
                        createdBy = sc.createdBy,
                        modifiedOn = sc.modifiedOn,
                        modifiedBy = sc.modifiedBy,
                        route = sc.route,
                        routeType = sc.routeType,
                        code = sc.code,
                        customer = sc.customer,
                        branch = sc.branch,
                        address = sc.address,
                        contactName = sc.contactName,
                        surveyCustomerId = sc.surveyCustomerId,
                        data = sc.data
                    })
                   .ToList();
                }
            }
        }

        

        

    }
}