using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using AutoMapper;
using MaintenanceProject.Controllers;
using MaintenanceProject.Data;
using MaintenanceProject.Mapping;
using MaintenanceProject.Models;
using Xunit;
using Microsoft.AspNetCore.Mvc;

namespace MaintenanceProject.Tests.UnitTests
{
    public class ValidationTests
    {
        private ApplicationDbContext CreateInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            return new ApplicationDbContext(options);
        }

        private IMapper CreateMapper() => new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile())).CreateMapper();

        [Fact]
        public async Task Create_InvalidModel_ReturnsViewAndDoesNotCreate()
        {
            using var context = CreateInMemoryContext("validation_create_db");
            var mapper = CreateMapper();
            var controller = new MaintenanceRequestsController(context, NullLogger<MaintenanceRequestsController>.Instance, mapper);

            var vm = new MaintenanceRequestCreateViewModel { Description = "", RequestDate = System.DateTime.Today };
            controller.ModelState.AddModelError("Description", "Required");

            var result = await controller.Create(vm);

            Assert.IsType<ViewResult>(result);
            Assert.Empty(context.MaintenanceRequests);
        }

        [Fact]
        public async Task Edit_InvalidModel_ReturnsViewAndDoesNotUpdate()
        {
            using var context = CreateInMemoryContext("validation_edit_db");
            var entity = new MaintenanceRequest { Description = "Old", RequestDate = System.DateTime.Today, IsCompleted = false };
            context.MaintenanceRequests.Add(entity);
            await context.SaveChangesAsync();

            var mapper = CreateMapper();
            var controller = new MaintenanceRequestsController(context, NullLogger<MaintenanceRequestsController>.Instance, mapper);

            var vm = new MaintenanceRequestEditViewModel { Id = entity.Id, Description = "", RequestDate = entity.RequestDate, IsCompleted = false };
            controller.ModelState.AddModelError("Description", "Required");

            var result = await controller.Edit(vm.Id, vm);

            Assert.IsType<ViewResult>(result);
            var unchanged = await context.MaintenanceRequests.FindAsync(entity.Id);
            Assert.Equal("Old", unchanged.Description);
        }
    }
}
