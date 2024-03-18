using System;
using Entities.Models;

namespace Services.Contracts
{
	public interface IBookingService
	{
        Task<BookingResponse> BookVisitAsync(BookingRequest request);
        Task<CancellationResponse> CancelVisitAsync(CancellationRequest request);
    }
}

