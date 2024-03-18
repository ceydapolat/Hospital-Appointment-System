namespace Tests;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppointmentSystem.Controllers;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Moq;

public class DoctorsControllerTests
{
    [Fact]
    public async Task GetAllDoctorsAsync_ReturnsOkResult()
    {
        // Arrange
        var mockDoctorService = new Mock<IDoctorService>();
        mockDoctorService.Setup(repo => repo.GetDoctorsAsync())
            .ReturnsAsync(new List<Doctor> { new Doctor { DoctorId = 1, Name = "Dr. John Doe" } });

        var controller = new DoctorsController(mockDoctorService.Object);

        // Act
        var result = await controller.GetAllDoctorsAsync();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Doctor>>(okResult.Value);
        Assert.Single(model); // Ensure that the result contains one doctor
    }

    [Fact]
    public async Task GetAvailableSlots_ReturnsOkResult()
    {
        // Arrange
        var mockDoctorService = new Mock<IDoctorService>();
        mockDoctorService.Setup(repo => repo.GetAvailableSlotsAsync(It.IsAny<int>()))
            .ReturnsAsync(new List<VisitSlot> { new VisitSlot { StartTime = DateTime.UtcNow, EndTime = DateTime.UtcNow.AddHours(1) } });

        var controller = new DoctorsController(mockDoctorService.Object);

        // Act
        var result = await controller.GetAvailableSlots(1); // Pass a doctorId for testing

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<VisitSlot>>(okResult.Value);
        Assert.Single(model); // Ensure that the result contains one slot
    }

    [Fact]
    public async Task ExportToCsv_ReturnsOkResult()
    {
        // Arrange
        var mockDoctorService = new Mock<IDoctorService>();
        mockDoctorService.Setup(repo => repo.ExportTurkishDoctorsToCsvAsync())
            .Returns(Task.CompletedTask); // Simulate successful export

        var controller = new DoctorsController(mockDoctorService.Object);

        // Act
        var result = await controller.ExportToCsv();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Data exported to CSV successfully", okResult.Value);
    }
}
