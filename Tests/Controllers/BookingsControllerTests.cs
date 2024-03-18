using System.Diagnostics;
using AppointmentSystem.Controllers;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Services.Contracts;

namespace Tests;

public class BookingsControllerTests
{
    [Fact]
    public async Task BookVisit_ValidRequest_ReturnsOkResult()
    {
        // Arrange
        var mockBookingService = new Mock<IBookingService>();
        mockBookingService.Setup(service => service.BookVisitAsync(It.IsAny<BookingRequest>()))
            .ReturnsAsync(new BookingResponse { Status = true });

        var controller = new BookingsController(mockBookingService.Object);
        var request = new BookingRequest
        {
            PatientName = "John Doe",
            StartTime = "10:00",
            EndTime = "11:00",
            Date = "2024-04-01",
            DoctorName = "Dr. Smith"
        };

        // Act
        var result = await controller.BookVisit(request) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.True((result.Value as BookingResponse).Status);
    }


    [Fact]
    public async Task BookVisit_InvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        var mockBookingService = new Mock<IBookingService>();
        mockBookingService.Setup(service => service.BookVisitAsync(It.IsAny<BookingRequest>()))
            .ReturnsAsync(new BookingResponse { Status = false });


        var controller = new BookingsController(mockBookingService.Object);
        var request = new BookingRequest
        {
            PatientName = "John Doe",
            StartTime = "09:00",
            EndTime = "10:00",
            Date = "2024-04-01",
            DoctorName = "Dr. Smith"
        };

        // Act
        var result = await controller.BookVisit(request) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);
    }

    [Fact]
    public async Task CancelVisit_ValidRequest_ReturnsOkResult()
    {
        // Arrange
        var mockBookingService = new Mock<IBookingService>();
        mockBookingService.Setup(service => service.CancelVisitAsync(It.IsAny<CancellationRequest>()))
            .ReturnsAsync(new CancellationResponse
            {
                Status = false
            });

        var controller = new BookingsController(mockBookingService.Object);
        var request = new CancellationRequest { BookingId = 12345 };

        // Act
        var result = await controller.CancelVisit(request) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public async Task CancelVisit_InvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        var mockBookingService = new Mock<IBookingService>();
        mockBookingService.Setup(service => service.CancelVisitAsync(It.IsAny<CancellationRequest>()))
            .ReturnsAsync(new CancellationResponse { Status = false });

        var controller = new BookingsController(mockBookingService.Object);
        var request = new CancellationRequest { BookingId = 0 };

        // Act
        var result = await controller.CancelVisit(request) as JsonResult;

        // Debugging
        Debug.WriteLine($"Result Object: {result}");

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Value);
    }
}