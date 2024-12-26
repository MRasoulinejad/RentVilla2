using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Common.Utility;
using WhiteLagoon.Application.Services.Interface;
using WhiteLagoon.web.ViewModels;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Application.Services.Implementation
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        static int previousMonth = DateTime.Now.Month == 1 ? 12 : DateTime.Now.Month - 1;
        readonly DateTime previousMonthStartDate = new(DateTime.Now.Year, previousMonth, 1);
        readonly DateTime currentMonthStartDate = new(DateTime.Now.Year, DateTime.Now.Month, 1);
        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<PieChartDto> GetBookingPieChartData()
        {
            var totalBookings = _unitOfWork.Booking.GetAll(u => u.BookingDate >= DateTime.Now.AddDays(-30) &&
           (u.Status != SD.StatusPending || u.Status == SD.StatusCancelled));

            var customerWithOneBooking = totalBookings.GroupBy(b => b.UserId).Where(x => x.Count() == 1).Select(x => x.Key).ToList();

            int bookingsByNewCustomer = customerWithOneBooking.Count();
            int bookingsByReturningCustomer = totalBookings.Count() - bookingsByNewCustomer;

            PieChartDto PieChartDto = new()
            {
                Labels = new string[] { "New Customer Bookings", "Returning Customer Bookings" },
                Series = new decimal[] { bookingsByNewCustomer, bookingsByReturningCustomer }
            };

            return PieChartDto;
        }

        public async Task<LineChartDto> GetMemberAndBookingLineChartData()
        {
            // Fetch all bookings from the last 30 days, grouped by date (day level).
            var bookingData = _unitOfWork.Booking.GetAll(u => u.BookingDate >= DateTime.Now.AddDays(-30) &&
            u.BookingDate <= DateTime.Now).GroupBy(b => b.BookingDate.Date)
            .Select(u => new
            {
                DateTime = u.Key,// The date of the booking
                NewBookingCount = u.Count(),// Count of bookings for each date
            });
            // Fetch all customers created in the last 30 days, grouped by date (day level).
            var customerData = _unitOfWork.User.GetAll(u => u.CreatedAt >= DateTime.Now.AddDays(-30) &&
            u.CreatedAt <= DateTime.Now).GroupBy(b => b.CreatedAt.Date)
            .Select(u => new
            {
                DateTime = u.Key,// The date the customer was created
                NewCustomerCount = u.Count(),// Count of new customers for each date
            });
            // Perform a left join on bookingData with customerData to align bookings with new customer counts by date.
            var leftJoin = bookingData.GroupJoin(customerData, booking => booking.DateTime, customer => customer.DateTime,
                (booking, customer) => new
                {
                    booking.DateTime, // The date being evaluated
                    booking.NewBookingCount,// Count of bookings for the date
                    NewCustomerCount = customer.Select(x => x.NewCustomerCount).FirstOrDefault()// Count of new customers for the date
                });
            // Perform a right join on customerData with bookingData to ensure dates with only new customer entries are included.
            var rightJoin = customerData.GroupJoin(bookingData, customer => customer.DateTime, booking => booking.DateTime,
                (customer, booking) => new
                {
                    customer.DateTime, // The date being evaluated
                    NewBookingCount = booking.Select(x => x.NewBookingCount).FirstOrDefault(),// Count of bookings for the date
                    customer.NewCustomerCount // Count of new customers for the date
                });
            // Merge both joins to ensure all dates with either bookings or customer entries are included, and order by date.
            var mergedData = leftJoin.Union(rightJoin).OrderBy(x => x.DateTime).ToList();
            // Extract booking counts, customer counts, and dates as separate arrays for the line chart.
            var newBookingData = mergedData.Select(x => x.NewBookingCount).ToArray();
            var newCustomerData = mergedData.Select(x => x.NewCustomerCount).ToArray();
            var categories = mergedData.Select(x => x.DateTime.ToString("MM/dd/yy")).ToArray();
            // Set up chart data for both series (new bookings and new customers).
            List<ChartData> chartDataList = new()
            {
                new ChartData
                {
                    Name = "New Bookings",
                    Data = newBookingData
                },

                new ChartData
                {
                    Name = "New Members",
                    Data = newCustomerData
                }
            };

            // Initialize the line chart view model with categories and series data.
            LineChartDto LineChartDto = new()
            {
                Categories = categories,
                Series = chartDataList
            };
            return LineChartDto;
        }

        public async Task<RadialBarChartDto> GetRegisteredUserChartData()
        {

            var totalUsers = _unitOfWork.User.GetAll();

            var countByCurrentMonth = totalUsers.Count(u => u.CreatedAt >= currentMonthStartDate &&
            u.CreatedAt <= DateTime.Now);

            var countByPreviousMonth = totalUsers.Count(u => u.CreatedAt >= previousMonthStartDate &&
            u.CreatedAt <= currentMonthStartDate);


            return SD.GetRadialCartDataModel(totalUsers.Count(), countByCurrentMonth, countByPreviousMonth);
        }

        public async Task<RadialBarChartDto> GetRevenueChartData()
        {
            var totalBookings = _unitOfWork.Booking.GetAll(u => u.Status != SD.StatusPending
          || u.Status == SD.StatusCancelled);

            var totalRevenue = Convert.ToInt32(totalBookings.Sum(u => u.TotalCost));

            var countByCurrentMonth = totalBookings.Where(u => u.BookingDate >= currentMonthStartDate &&
            u.BookingDate <= DateTime.Now).Sum(u => u.TotalCost);

            var countByPreviousMonth = totalBookings.Where(u => u.BookingDate >= previousMonthStartDate &&
            u.BookingDate <= currentMonthStartDate).Sum(u => u.TotalCost);

            return SD.GetRadialCartDataModel(totalRevenue, countByCurrentMonth, countByPreviousMonth);
        }

        public async Task<RadialBarChartDto> GetTotalBookingRadialChartData()
        {
            var totalBookings = _unitOfWork.Booking.GetAll(u => u.Status != SD.StatusPending
          || u.Status == SD.StatusCancelled);

            var countByCurrentMonth = totalBookings.Count(u => u.BookingDate >= currentMonthStartDate &&
            u.BookingDate <= DateTime.Now);

            var countByPreviousMonth = totalBookings.Count(u => u.BookingDate >= previousMonthStartDate &&
            u.BookingDate <= currentMonthStartDate);

            return SD.GetRadialCartDataModel(totalBookings.Count(), countByCurrentMonth, countByPreviousMonth);
        }




    }
}