using SmartOrderService.Models.DTO;
using SmartOrderService.DB;
using AutoMapper;
using System;
using SmartOrderService.Models;
using SmartOrderService.Utils;
using SmartOrderService.Models.Responses;

namespace SmartOrderService.Mappers
{
    public static class MapperConfig
    {
        public static void Initialize() {

            var config = new MapperConfiguration(cfg => cfg.CreateMap<so_product, ProductDto>());
            config.CreateMapper();

            Mapper.Initialize(
                cfg => {

                    cfg.CreateMap<so_product, ProductDto>()
                    .ForMember(dest => dest.BillingDataId, opt => opt.MapFrom(src => src.billing_dataId));

                    cfg.CreateMap<so_price_list_products_detail, PriceDto>()
                    .ForMember(dest => dest.PriceValue, opt => opt.MapFrom(src => src.price))
                    .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.productId))
                    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.status))
                    .ForMember(dest => dest.PriceBaseValue, opt => opt.MapFrom(src => src.base_price))
                    .ForMember(dest => dest.DiscountPercentValue, opt => opt.MapFrom(src => src.discount_percent));

                    cfg.CreateMap<so_customer, CustomerDto>();
                    cfg.CreateMap<so_customer, CustomerWithVarioDto>();


                    cfg.CreateMap<so_reason_devolution, ReasonDevolutionDto>();

                    cfg.CreateMap<so_application, ApplicationDto>()
                    .ForMember(dest => dest.DownloadUrl, opt => opt.MapFrom(src => src.download_url))
                    .ForMember(dest => dest.NameInstaller, opt => opt.MapFrom(src => src.name_installer))
                    .ForMember(dest => dest.WSUrl, opt => opt.MapFrom(src => src.ws_url));

                    cfg.CreateMap<so_promotion, PromotionDto>()
                    .ForMember(dest => dest.ValidityStart, opt => opt.MapFrom(src => String.Format("{0:dd/MM/yyyy HH:mm:ss}", src.validity_start)))
                    .ForMember(dest => dest.ValidityEnd, opt => opt.MapFrom(src => String.Format("{0:dd/MM/yyyy HH:mm:ss}", src.validity_end)));


                    cfg.CreateMap<so_user, UserDto>();

                    cfg.CreateMap<so_branch, BranchDto>()
                        .ForMember(dest => dest.BranchId, opt => opt.MapFrom(src => src.branchId))
                        .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.code))
                        .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.name));


                    cfg.CreateMap<so_replacement, ReplacementDto>();

                    cfg.CreateMap < so_category, CategoryDto>();

                    cfg.CreateMap<so_billing_data, BillingDataDto>()
                    .ForMember(dest => dest.BillingDataId, opt => opt.MapFrom(src => src.billing_dataId))
                    .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.getAddress()))
                    .ForMember(dest => dest.Cp, opt => opt.MapFrom(src => src.postal_code))
                    .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.phone));

                    cfg.CreateMap<CustomerVisitDto, so_binnacle_visit>()
                    .ForMember(dest => dest.customerId, opt => opt.MapFrom(src => src.CustomerId))
                    .ForMember(dest => dest.checkin, opt => opt.MapFrom(src => DateUtils.getDateTime(src.CheckIn)))
                    .ForMember(dest => dest.checkout, opt => opt.MapFrom(src => DateUtils.getDateTime(src.CheckOut)))
                    .ForMember(dest => dest.latitudein, opt => opt.MapFrom(src => src.LatitudeIn))
                    .ForMember(dest => dest.latitudeout, opt => opt.MapFrom(src => src.LatitudeOut))
                    .ForMember(dest => dest.longitudein, opt => opt.MapFrom(src => src.LongitudeIn))
                    .ForMember(dest => dest.longitudeout, opt => opt.MapFrom(src => src.LongitudeOut))
                    .ForMember(dest => dest.scanned, opt => opt.MapFrom(src => src.Scanned)).ReverseMap();

                    cfg.CreateMap<so_user_reason_devolutions, UserReasonDevolutionDto>()
                     .ForMember(dest => dest.UserReasonDevolutionId, opt => opt.MapFrom(src => src.user_reason_devolutionId))
                     .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.description))
                     .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.value))
                    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.status));

                    cfg.CreateMap<so_user_notice_recharge, UserNoticeRechargeDto>()
                        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.user_notice_rechargeId))
                        .ForMember(dest => dest.BranchId, opt => opt.MapFrom(src => src.branchId))
                        .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.name))
                        .ForMember(dest => dest.Mail, opt => opt.MapFrom(src => src.mail))
                        .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.phone_number))
                        .ForMember(dest => dest.MailEnabled, opt => opt.MapFrom(src => src.mail_enabled))
                        .ForMember(dest => dest.PhoneNumberEnabled, opt => opt.MapFrom(src => src.phone_number_enabled));

                    cfg.CreateMap<so_route, RouteDto>()
                        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.routeId))
                        .ForMember(dest => dest.BranchId, opt => opt.MapFrom(src => src.branchId))
                        .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.code))
                        .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.name));


                    cfg.CreateMap<so_revision_types, RouteRevisionType>()
                    .ForMember(dest => dest.RevisionTypeId, opt => opt.MapFrom(src => src.revision_typeId))
                    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.name))
                    .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.value));

                    cfg.CreateMap<so_revision_states, RouteRevisionState>()
                    .ForMember(dest => dest.RevisionStateId, opt => opt.MapFrom(src => src.revision_stateId))
                    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.name))
                    .ForMember(dest => dest.Value, opt => opt.MapFrom(src => Int32.Parse(src.value)));

                    cfg.CreateMap<InvoiceOpeFacturaResponse, so_invoice_opefactura>();

                    cfg.CreateMap<so_route_team_inventory_available, RouteTeamInventoryDto>()
                    .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.productId))
                    .ForMember(dest => dest.AvailableAmount, opt => opt.MapFrom(src => src.Available_Amount));
                }

               );

        }
    }
}