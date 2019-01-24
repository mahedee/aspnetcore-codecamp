using AutoMapper;
using MyCodeCamp.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Mvc;


namespace MyCodeCamp.Models
{
    public class CampMappingProfile : Profile
    {
        public CampMappingProfile()
        {
            CreateMap<Camp, CampModel>()
                .ForMember(c => c.StartDate, opt => opt.MapFrom(camp => camp.EventDate))
                .ForMember(c => c.EndDate,
                opt => opt.MapFrom(camp => camp.EventDate.AddDays(camp.Length > 0 ? camp.Length - 1 : 0)))
              .ForMember(c => c.Url,
                 opt => opt.MapFrom<CampUrlResolver>())
              .ReverseMap()
              .ForMember(m => m.EventDate,
                opt => opt.MapFrom(model => model.StartDate))
              .ForMember(m => m.Length,
                opt => opt.MapFrom(model => (model.EndDate - model.StartDate).Days + 1))
              .ForMember(m => m.Location,
                opt => opt.MapFrom(c => new Location()
                {
                    Address1 = c.LocationAddress1,
                    Address2 = c.LocationAddress2,
                    Address3 = c.LocationAddress3,
                    CityTown = c.LocationCityTown,
                    StateProvince = c.LocationStateProvince,
                    PostalCode = c.LocationPostalCode,
                    Country = c.LocationCountry
                }));

            CreateMap<Speaker, SpeakerModel>()
            .ForMember(s => s.Url, opt => opt.MapFrom<SpeakerUrlResolver>())
            .ReverseMap();

            CreateMap<Talk, TalkModel>()
              .ForMember(s => s.Url, opt => opt.MapFrom<TalkUrlResolver>())
              .ReverseMap();

            CreateMap<Speaker, Speaker2Model>()
            .IncludeBase<Speaker, SpeakerModel>()
            .ForMember(s => s.BadgeName, opt => opt.MapFrom(s => $"{s.Name} (@{s.TwitterName})"));

            CreateMap<Talk, TalkModel>()
            .ForMember(s => s.Url, opt => opt.MapFrom<TalkUrlResolver>())
            .ReverseMap();
        }
    }
}

