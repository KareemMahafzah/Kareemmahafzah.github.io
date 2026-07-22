using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using MaintenanceProject.Controllers;
using AutoMapper;
using MaintenanceProject.Mapping;
using MaintenanceProject.Data;
using MaintenanceProject.Models;
using Xunit;
using Microsoft.AspNetCore.Mvc;

namespace MaintenanceProject.Tests
{
    public class MaintenanceRequestsControllerTests
    {
        private ApplicationDbContext CreateInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task Details_ReturnsNotFound_ForMissingId()
        {
            using var context = CreateInMemoryContext("details_missing_db");
            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
            var mapper = mapperConfig.CreateMapper();
            var controller = new MaintenanceRequestsController(context, NullLogger<MaintenanceRequestsController>.Instance, mapper);

            var result = await controller.Details(12345);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteConfirmed_RemovesEntityAndRedirects()
        {
            using var context = CreateInMemoryContext("delete_test_db");
            var entity = new MaintenanceRequest { Description = "To delete", RequestDate = DateTime.Today, IsCompleted = false };
            context.MaintenanceRequests.Add(entity);
            await context.SaveChangesAsync();

            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
            var mapper = mapperConfig.CreateMapper();
            var controller = new MaintenanceRequestsController(context, NullLogger<MaintenanceRequestsController>.Instance, mapper);

            var result = await controller.DeleteConfirmed(entity.Id);

            Assert.IsType<RedirectToActionResult>(result);
            Assert.Empty(context.MaintenanceRequests.Where(r => r.Id == entity.Id));
        }

        [Fact]
        public async Task Index_PagingAndSearch_ReturnsFilteredPagedResults()
        {
            using var context = CreateInMemoryContext("index_test_db");
            for (int i = 1; i <= 25; i++)
            {
                context.MaintenanceRequests.Add(new MaintenanceRequest { Description = i % 2 == 0 ? $"Even {i}" : $"Odd {i}", RequestDate = DateTime.Today.AddDays(-i), IsCompleted = false });
            }
            await context.SaveChangesAsync();

            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
            var mapper = mapperConfig.CreateMapper();
            var controller = new MaintenanceRequestsController(context, NullLogger<MaintenanceRequestsController>.Instance, mapper);

            // page 1, default pageSize 10
            var result1 = await controller.Index(null, 1) as ViewResult;
            Assert.NotNull(result1);
            var vm1 = Assert.IsType<MaintenanceProject.Models.MaintenanceRequestIndexViewModel>(result1.Model);
            Assert.Equal(1, vm1.Page);
            Assert.Equal(3, vm1.TotalPages);
            Assert.Equal(10, vm1.Items.Count());

            // search for Even
            var result2 = await controller.Index("Even", 1) as ViewResult;
            var vm2 = Assert.IsType<MaintenanceProject.Models.MaintenanceRequestIndexViewModel>(result2.Model);
            Assert.All(vm2.Items, item => Assert.Contains("Even", item.Description));
        }

        [Fact]
        public async Task Create_Post_CreatesEntityAndRedirects()
        {
            using var context = CreateInMemoryContext("create_test_db");
            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
            var mapper = mapperConfig.CreateMapper();
            var controller = new MaintenanceRequestsController(context, NullLogger<MaintenanceRequestsController>.Instance, mapper);

            var vm = new MaintenanceProject.Models.MaintenanceRequestCreateViewModel
            {
                Description = "Test create",
                RequestDate = DateTime.Today,
                IsCompleted = false
            };

            var result = await controller.Create(vm);

            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(1, context.MaintenanceRequests.Count());
            var entity = context.MaintenanceRequests.First();
            Assert.Equal(vm.Description, entity.Description);
        }

        [Fact]
        public async Task Edit_Post_UpdatesEntityAndRedirects()
        {
            using var context = CreateInMemoryContext("edit_test_db");
            var entity = new MaintenanceRequest { Description = "Old", RequestDate = DateTime.Today, IsCompleted = false };
            context.MaintenanceRequests.Add(entity);
            await context.SaveChangesAsync();

            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
            var mapper = mapperConfig.CreateMapper();
            var controller = new MaintenanceRequestsController(context, NullLogger<MaintenanceRequestsController>.Instance, mapper);

            var vm = new MaintenanceProject.Models.MaintenanceRequestEditViewModel
            {
                Id = entity.Id,
                Description = "Updated",
                RequestDate = entity.RequestDate,
                IsCompleted = true
            };

            var result = await controller.Edit(vm.Id, vm);

            Assert.IsType<RedirectToActionResult>(result);
            var updated = context.MaintenanceRequests.First(r => r.Id == entity.Id);
            Assert.Equal("Updated", updated.Description);
            Assert.True(updated.IsCompleted);
        }
    }
}
