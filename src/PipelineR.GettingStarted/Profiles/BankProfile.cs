using AutoMapper;
using PipelineR.GettingStarted.Domain;
using PipelineR.GettingStarted.Models;

namespace PipelineR.GettingStarted.Profiles
{
    public class BankProfile : Profile
    {
        public BankProfile()
        {
            CreateMap<CreateAccountModel, Account>()
                .ReverseMap();
        }
    }
}