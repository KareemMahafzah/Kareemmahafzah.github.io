using AutoMapper;
using MaintenanceProject.Mapping;
using MaintenanceProject.Models;
using Xunit;

namespace MaintenanceProject.Tests.UnitTests
{
    public class AutoMapperProfileTests
    {
        private readonly IMapper _mapper;

        public AutoMapperProfileTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
            _mapper = config.CreateMapper();
        }

        [Fact]
        public void CreateViewModel_MapsToEntity()
        {
            var vm = new MaintenanceRequestCreateViewModel
            {
                Description = "Test",
                RequestDate = System.DateTime.Today,
                IsCompleted = false
            };

            var entity = _mapper.Map<MaintenanceRequest>(vm);

            Assert.Equal(vm.Description, entity.Description);
            Assert.Equal(vm.RequestDate, entity.RequestDate.Date);
        }

        [Fact]
        public void EditViewModel_MapsToEntity()
        {
            var vm = new MaintenanceRequestEditViewModel
            {
                Id = 5,
                Description = "Edit",
                RequestDate = System.DateTime.Today,
                IsCompleted = true
            };

            var entity = _mapper.Map<MaintenanceRequest>(vm);

            Assert.Equal(vm.Id, entity.Id);
            Assert.Equal(vm.Description, entity.Description);
        }
    }
}
