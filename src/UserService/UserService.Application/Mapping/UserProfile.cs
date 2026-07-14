using AutoMapper;
using UserService.Application.Inputs;
using UserService.Domain.Models;

namespace UserService.Application.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            // REGISTER (self-signup): DTO -> Entity
            CreateMap<UserRegisterDto, User>()
                // Gerenciados pela aplicação/persistência
                .ForMember(d => d.Guid, opt => opt.Ignore())
                .ForMember(d => d.CreatedAt, opt => opt.Ignore())
                .ForMember(d => d.UpdatedAt, opt => opt.Ignore())
                .ForMember(d => d.Roles, opt => opt.Ignore());
            // Password vem do DTO; o service faz o hash antes de salvar

            // CREATE (gestão): DTO -> Entity (a role é atribuída pelo service a partir de dto.Role)
            CreateMap<UserCreateDto, User>()
                .ForMember(d => d.Guid, opt => opt.Ignore())
                .ForMember(d => d.CreatedAt, opt => opt.Ignore())
                .ForMember(d => d.UpdatedAt, opt => opt.Ignore())
                .ForMember(d => d.Roles, opt => opt.Ignore());

            // UPDATE: DTO -> Entity (aplica apenas quando vier valor)
            CreateMap<UserUpdateDto, User>()
                .ForMember(d => d.Guid, opt => opt.Ignore())
                .ForMember(d => d.Cpf, opt => opt.Ignore())      // não permitir alterar CPF por update
                .ForMember(d => d.CreatedAt, opt => opt.Ignore())
                .ForMember(d => d.UpdatedAt, opt => opt.Ignore())
                .ForMember(d => d.Password, opt => opt.Ignore()) // troca de senha com hash no service
                .ForMember(d => d.Roles, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // Entity -> DTO de resposta
            CreateMap<User, UserResponseDto>();
        }
    }
}
