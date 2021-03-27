using AutoMapper;
using DVLD.Data.Entities;
using DVLD.Models.Car;

namespace DVLD.AutoMapper
{
    public class CarProfile : Profile
    {
        public CarProfile()
        {
            CreateMap<CarVM, Car>();
            CreateMap<Car, CarVM>();
            CreateMap<CreateCarVM, Car>();
        }
    }
}
