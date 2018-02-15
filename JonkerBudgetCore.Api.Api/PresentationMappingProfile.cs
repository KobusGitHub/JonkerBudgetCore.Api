using AutoMapper;
using JonkerBudgetCore.Api.Api.ViewModels;
using JonkerBudgetCore.Api.Api.ViewModels.Dashboards;
using JonkerBudgetCore.Api.Api.ViewModels.Users;
using JonkerBudgetCore.Api.Api.ViewModels.Widgets;
using JonkerBudgetCore.Api.Auth;
using JonkerBudgetCore.Api.Domain.Models.Categories;
using JonkerBudgetCore.Api.Domain.Models.Dashboards;
using JonkerBudgetCore.Api.Domain.Models.Users;
using JonkerBudgetCore.Api.Domain.Models.Widgets;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace JonkerBudgetCore.Api.Domain
{
    public class PresentationMappingProfile : Profile
    {
        public PresentationMappingProfile()
        {
            CreateMap<User, UserViewModel>().ForMember(dest => dest.Surname, opt => opt.MapFrom(src => src.Lastname))
                .ForMember(dest => dest.IsActiveDirectoryUser, opt => opt.MapFrom(src => src.IsActiveDirectoryUser))
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.UserRoles.Select(match => match.RoleId)));

            CreateMap<ActiveDirectoryUser, ActiveDirectoryUserViewModel>()
                .ForMember(dest => dest.Firstname, opt => opt.MapFrom(src => src.Firstname))
                .ForMember(dest => dest.Lastname, opt => opt.MapFrom(src => src.Lastname))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

            CreateMap<Widget, WidgetViewModel>().ForMember(dest => dest.ChartConfig, opt => opt.MapFrom(src => JObject.Parse(src.ChartConfig)))
                .ForMember(dest => dest.Heading, opt => opt.MapFrom(src => src.Heading))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.RefreshInterval, opt => opt.MapFrom(src => src.RefreshInterval))
                .ForMember(dest => dest.Size, opt => opt.MapFrom(src => src.Size))
                .ForMember(dest => dest.WidgetQueryDrilldownId, opt => opt.MapFrom(src => src.WidgetQueryDrilldownId))
                .ForMember(dest => dest.Size, opt => opt.MapFrom(src => src.Size))
                .ForMember(dest => dest.Sequence, opt => opt.MapFrom(src => src.Sequence))
                .ForMember(dest => dest.WidgetQueryId, opt => opt.MapFrom(src => src.WidgetQueryId));

            CreateMap<Dashboard, DashboardViewModel>().ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Widgets, opt => opt.MapFrom(src => src.Widgets))
                .ForMember(dest => dest.Icon, opt => opt.MapFrom(src => src.Icon));

            CreateMap<Category, CategoryModel>();
            CreateMap<CategoryCreateModel, CategoryModel>();

        }
    }
}
