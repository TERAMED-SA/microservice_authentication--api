using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using microservice_authentication__api.src.Application.DTOs;
using microservice_authentication__api.src.Domain.Entities;

namespace microservice_authentication__api.src.Application.Common.Mapping
{
    public class GeneralMapping : Profile
    {
        public GeneralMapping()
        {
            CreateMap<User, MeDTO>().ReverseMap();
        }
    }
}