using System.Net;
using Moq;
using Moq.Protected;
using Services;

namespace Tests;

public class DoctorServiceTests
{
    [Fact]
    public async Task GetDoctorsAsync_SuccessfulResponse_ReturnsDoctors()
    {
        // Arrange
        var httpClientHandler = new Mock<HttpMessageHandler>();
        var mockLogger = new Mock<ILoggerService>();

        httpClientHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"data\": [{\"DoctorId\": 1, \"Name\": \"Dr. John Doe\", \"HospitalId\": 101, \"BranchId\": 201, \"Nationality\": \"TUR\", \"Gender\": \"Male\"}]}")
            });

        var httpClient = new HttpClient(httpClientHandler.Object);
        var doctorService = new DoctorService(httpClient, mockLogger.Object);

        // Act
        var doctors = await doctorService.GetDoctorsAsync();

        // Assert
        Assert.NotNull(doctors);
        Assert.Single(doctors);
        Assert.Equal(1, doctors.First().DoctorId);
        Assert.Equal("Dr. John Doe", doctors.First().Name);
    }

    [Fact]
    public async Task GetDoctorsAsync_UnsuccessfulResponse_ReturnsNull()
    {
        // Arrange
        var httpClientHandler = new Mock<HttpMessageHandler>();
        var mockLogger = new Mock<ILoggerService>();

        httpClientHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError));

        var httpClient = new HttpClient(httpClientHandler.Object);
        var doctorService = new DoctorService(httpClient, mockLogger.Object);

        // Act
        var doctors = await doctorService.GetDoctorsAsync();

        // Assert
        Assert.Null(doctors);
    }

    [Fact]
    public async Task GetAvailableSlotsAsync_DoctorExists_ReturnsSlots()
    {
        // Arrange
        var httpClientHandler = new Mock<HttpMessageHandler>();
        var mockLogger = new Mock<ILoggerService>();

        httpClientHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"Data\": [{\"DoctorId\": 3, \"StartTimeUtcString\": \"2024-03-21T08:00:00Z\", \"EndTimeUtcString\": \"2024-03-21T09:00:00Z\", \"VisitId\": 101, \"Id\": 201}]}")
            });

        var httpClient = new HttpClient(httpClientHandler.Object);
        var doctorService = new DoctorService(httpClient, mockLogger.Object);

        // Act
        var slots = await doctorService.GetAvailableSlotsAsync(3); 

        // Assert
        Assert.NotNull(slots);
        Assert.Single(slots); 
        Assert.Equal(3, slots.First().DoctorId);
    }

}
