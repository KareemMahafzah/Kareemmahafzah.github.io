using AutoMapper;
using MaintenanceProject.Models;

namespace MaintenanceProject.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<MaintenanceRequest, MaintenanceRequestEditViewModel>().ReverseMap();
            CreateMap<MaintenanceRequest, MaintenanceRequestCreateViewModel>().ReverseMap();
            CreateMap<MaintenanceRequest, MaintenanceRequestIndexViewModel>();
        }
    }
}
