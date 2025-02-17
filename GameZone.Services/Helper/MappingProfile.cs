using AutoMapper;
using GameZone.MVC.Models;

namespace GameZone.Services.Helper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateGame();
            EditGame();
        }

        private void CreateGame()
        {
            CreateMap<CreateGameVM, Game>()
            .ForMember(dest => dest.Cover, opt => opt.Ignore())
            .ForMember(dest => dest.GameDevices, opt => opt.MapFrom(src =>
                src.SelectedDevices.Select(deviceId => new GameDevice { DeviceId = deviceId })))
            .ReverseMap();
        }

        private void EditGame()
        {
            CreateMap<EditGameVM, Game>()
            .ForMember(dest => dest.Cover, opt => opt.Ignore())
            .ForMember(dest => dest.GameDevices, opt => opt.MapFrom(src =>
                src.SelectedDevices.Select(deviceId => new GameDevice
                {
                    DeviceId = deviceId,
                    GameId = src.Id // Make sure to assign the GameId correctly
                }).ToList()))
            .ReverseMap();

        }
    }

}
