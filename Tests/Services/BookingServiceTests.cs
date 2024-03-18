using System.Net;
using Entities.Models;
using Moq;
using Moq.Protected;
using Services;
using Services.Contracts;

namespace Tests;

public class BookingServiceTests
{
    [Fact]
    public async Task BookVisitAsync_SuccessfulBooking_ReturnsBookingResponse()
    {
        // Arrange
        var mockDoctorService = new Mock<IDoctorService>();
        var mockLogger = new Mock<ILoggerService>();

        // Set up the mock to return a doctorId when GetDoctorIdByDoctorNameAsync is called
        mockDoctorService.Setup(service => service.GetDoctorIdByDoctorNameAsync(It.IsAny<string>()))
            .ReturnsAsync(1);

        // Set up the mock to return hospitalId and branchId when GetDoctorInfoByDoctorIdAsync is called
        mockDoctorService.Setup(service => service.GetDoctorInfoByDoctorIdAsync(It.IsAny<int>()))
            .ReturnsAsync((1, 2));

        // Set up the mock to return a predefined list of visit slots when GetAvailableSlotsAsync is called
        mockDoctorService.Setup(service => service.GetAvailableSlotsAsync(It.IsAny<int?>()))
            .ReturnsAsync(new List<VisitSlot>
            {
                new VisitSlot { Id = 1, DoctorId = 1, StartTimeUtcString = "2022-05-31T10:30:00.000Z", EndTimeUtcString = "2022-05-31T10:45:00.000Z" },
                new VisitSlot { Id = 2, DoctorId = 1, StartTimeUtcString = "2022-06-01T10:30:00.000Z", EndTimeUtcString = "2022-06-01T10:45:00.000Z" }
            });

        var httpClientHandler = new Mock<HttpMessageHandler>();
        httpClientHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"status\": true, \"bookingId\": 133213}")
            });

        var httpClient = new HttpClient(httpClientHandler.Object);
        var bookingService = new BookingService(httpClient, mockDoctorService.Object, mockLogger.Object);

        var request = new BookingRequest
        {
            PatientName = "John Doe",
            StartTime = "10:30",
            EndTime = "10:45",
            Date = "31/05/2022",
            DoctorName = "Dr. Smith"
        };

        // Act
        var response = await bookingService.BookVisitAsync(request);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Status);
        Assert.Equal(133213, response.BookingId);
    }

    [Fact]
    public async Task BookVisitAsync_NoDoctorInfo_ReturnsFailureResponse()
    {
        // Arrange
        var mockDoctorService = new Mock<IDoctorService>();
        mockDoctorService.Setup(service => service.GetDoctorIdByDoctorNameAsync(It.IsAny<string>()))
            .ReturnsAsync(default(int));

        var mockLogger = new Mock<ILoggerService>();

        var httpClientHandler = new Mock<HttpMessageHandler>();
        var httpClient = new HttpClient(httpClientHandler.Object);
        var bookingService = new BookingService(httpClient, mockDoctorService.Object, mockLogger.Object);

        var request = new BookingRequest
        {
            PatientName = "John Doe",
            StartTime = "10:00",
            EndTime = "11:00",
            Date = "2024-04-01",
            DoctorName = "Dr. Smith"
        };

        // Act
        var response = await bookingService.BookVisitAsync(request);

        // Assert
        Assert.NotNull(response);
        Assert.False(response.Status);
    }

}
