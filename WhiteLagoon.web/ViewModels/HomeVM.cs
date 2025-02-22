using RentVilla.Domain.Entities;

namespace RentVilla.web.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<Villa>? VillaList { get; set; }
        public DateOnly CheckInDate { get; set; }
        public DateOnly? ChechOutDate { get; set; }
        public int Nights { get; set; }

    }
}
