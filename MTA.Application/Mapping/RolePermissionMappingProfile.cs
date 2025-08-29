using AutoMapper;
using MTA.Application.DTOs;
using MTA.Domain.Entities;

namespace MTA.Application.Mapping;

/// <summary>
/// AutoMapper profile for RolePermission entity
/// </summary>
public class RolePermissionMappingProfile : Profile
{
    public RolePermissionMappingProfile()
    {
        // Map from RolePermission entity to RolePermissionDto
        CreateMap<PermissionsRole, RolePermissionDto>()
            .ForMember(dest => dest.RoleTitle, opt => opt.MapFrom(src => src.Role != null ? src.Role.Title : null))
            .ForMember(dest => dest.PermissionTitle, opt => opt.MapFrom(src => src.Permission != null ? src.Permission.Title : null))
            .ForMember(dest => dest.PermissionDescription, opt => opt.MapFrom(src => src.Permission != null ? src.Permission.Description : null));

        // Map from RolePermissionDto to RolePermission entity
        CreateMap<RolePermissionDto, PermissionsRole>()
            .ForMember(dest => dest.Role, opt => opt.Ignore())
            .ForMember(dest => dest.Permission, opt => opt.Ignore());
    }
}
