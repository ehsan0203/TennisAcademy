using AutoMapper;
using MTA.Domain.Entities;
using MTA.Application.DTOs;
using MTA.Application.DTOs.Auth;
using MTA.Application.DTOs.User;

namespace MTA.Application.Mapping;

/// <summary>
/// Mapping profile for Account and UserProfile entities
/// </summary>
public class AccountMappingProfile : BaseMappingProfile
{
	public AccountMappingProfile()
	{
		CreateMap<Account, AccountDto>()
			.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
			.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
			.ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
			.ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.RoleId))
			.ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.StatusId))
			.ForMember(dest => dest.RoleTitle, opt => opt.MapFrom(src => src.Role.Title))
			.ForMember(dest => dest.StatusValue, opt => opt.MapFrom(src => src.Status.Value))
			// Single media file mapping
			.ForMember(dest => dest.MediaFileId, opt => opt.MapFrom(src => src.MediaFileId))
			.ForMember(dest => dest.MediaFileUrl, opt => opt.MapFrom(src => src.MediaFile != null ? src.MediaFile.Url : null))
			// Ignore collections to prevent large payloads or loops
			.ForMember(dest => dest.UserProfile, opt => opt.MapFrom(src => src.UserProfile))
			.ForAllMembers(opt => opt.Ignore());

		// AccountDto -> Account (Input DTO)
		CreateMap<AccountDto, Account>()
			.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
			.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
			.ForMember(dest => dest.Password, opt => opt.MapFrom(src => src)) 
			.ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
			.ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.RoleId))
			.ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.StatusId))
			// Single media file mapping
			.ForMember(dest => dest.MediaFileId, opt => opt.MapFrom(src => src.MediaFileId))
			// Ignore navigation collections to prevent loop
			.ForAllMembers(opt => opt.Ignore());

		// UserProfile <-> UserProfileDto
		CreateMap<UserProfile, UserProfileDto>()
			.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
			.ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
			.ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
			.ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
			.ForMember(dest => dest.Experience, opt => opt.MapFrom(src => src.Experience))
			.ForMember(dest => dest.SkillLevelId, opt => opt.MapFrom(src => src.SkillLevelId))
			.ForMember(dest => dest.SkillLevelValue, opt => opt.MapFrom(src => src.SkillLevel.Title))
			.ForMember(dest => dest.HealthCondition, opt => opt.MapFrom(src => src.HealthCondition))
			.ForMember(dest => dest.HealthDescription, opt => opt.MapFrom(src => src.HealthDescription))
			// Ignore Account to prevent loop
			.ForAllMembers(opt => opt.Ignore());

		CreateMap<UserProfileDto, UserProfile>()
			.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
			.ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
			.ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
			.ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
			.ForMember(dest => dest.Experience, opt => opt.MapFrom(src => src.Experience))
			.ForMember(dest => dest.SkillLevelId, opt => opt.MapFrom(src => src.SkillLevelId))
			.ForMember(dest => dest.HealthCondition, opt => opt.MapFrom(src => src.HealthCondition))
			.ForMember(dest => dest.HealthDescription, opt => opt.MapFrom(src => src.HealthDescription))
			.ForAllMembers(opt => opt.Ignore());

		// Role -> RoleDto
		CreateMap<Role, RoleDto>()
			.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
			.ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
			.ForMember(dest => dest.AccountCount, opt => opt.MapFrom(src => src.Accounts.Count))
			.ForMember(dest => dest.PermissionCount, opt => opt.MapFrom(src => src.RolePermissions.Count));

		// RoleDto -> Role
		CreateMap<RoleDto, Role>()
			.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
			.ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
			.ForMember(dest => dest.Accounts, opt => opt.Ignore())
			.ForMember(dest => dest.RolePermissions, opt => opt.Ignore());



		// Permission -> PermissionDto
		CreateMap<Permission, PermissionDto>()
			.ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
			.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
			.ForAllMembers(opt => opt.Ignore()); 


		// PermissionDto -> Permission
		CreateMap<PermissionDto, Permission>()
			.ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
			.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
			.ForAllMembers(opt => opt.Ignore());


		// RolePermission -> RolePermissionDto
		CreateMap<PermissionsRole, RolePermissionDto>()
			.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
			.ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.RoleId))
			.ForMember(dest => dest.PermissionId, opt => opt.MapFrom(src => src.PermissionId))
			.ForMember(dest => dest.RoleTitle, opt => opt.MapFrom(src => src.Role.Title))
			.ForMember(dest => dest.PermissionTitle, opt => opt.MapFrom(src => src.Permission.Title))
			.ForAllMembers(opt => opt.Ignore());

		// RolePermissionDto -> RolePermission
		CreateMap<RolePermissionDto, PermissionsRole>()
			.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
			.ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.RoleId))
			.ForMember(dest => dest.PermissionId, opt => opt.MapFrom(src => src.PermissionId))
			.ForAllMembers(opt => opt.Ignore());

	}
}
