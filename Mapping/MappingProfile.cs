using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using VEGA.Controllers.Resources;
using VEGA.Models;

namespace VEGA.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Domain to API Resources 
            CreateMap<Make, MakeResource>();
            CreateMap<Model, KeyValueFairResource>();
            CreateMap<Feature, KeyValueFairResource>();
            CreateMap<Vehicle, SaveVehicleResource>()
                .ForMember(vr => vr.Contact, opt => opt.MapFrom(v => new ContactResource{ 
                    Name = v.ContactName,
                    Email = v.ContactEmail,
                    Phone = v.ContactPhone}))
                .ForMember(vr => vr.Features, opt => opt.MapFrom(v => v.Features.Select(vf => vf.FeatureId)));
            CreateMap<Vehicle, VehicleResource>()
                .ForMember(vr => vr.Make , opt => opt.MapFrom(v => v.Model.Make))
                .ForMember(vr => vr.Contact, opt => opt.MapFrom(v => new ContactResource{ 
                    Name = v.ContactName,
                    Email = v.ContactEmail,
                    Phone = v.ContactPhone}))
                .ForMember(vr => vr.Features, opt => opt.MapFrom(v => v.Features.Select(vf => new KeyValueFairResource{
                    Id = vf.Feature.Id,
                    Name = vf.Feature.Name
                })));
        
            // API Resource to Domain
            CreateMap<SaveVehicleResource, Vehicle>()
            .ForMember(v => v.Id, opt => opt.Ignore())
            .ForMember(v => v.ContactName , opt => opt.MapFrom(vr => vr.Contact.Name))
            .ForMember(v => v.ContactEmail , opt => opt.MapFrom(vr => vr.Contact.Email))
            .ForMember(v => v.ContactPhone , opt => opt.MapFrom(vr => vr.Contact.Phone))
            .ForMember(v => v.Features , opt => opt.Ignore())
            .AfterMap((vr, v) => {
                // Remove selected features
                var removeFeatures = v.Features.Where(f => !vr.Features.Contains(f.FeatureId));
                foreach(var f in removeFeatures)
                    v.Features.Remove(f);

                // Add new Features                
                var addedFeatures = vr.Features.Where(id => !v.Features.Any(f => f.FeatureId == id)).Select(id => new VehicleFeature{FeatureId = id});
                foreach(var f in addedFeatures)
                    v.Features.Add(f);
            });
        }
    }
}