using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Domain.Entities;

namespace Task2Tracker.Application.Features.Users.Commands.CreateUser;

// 1. İstek (Request) Modeli
public record CreateUserCommand(string FirstName, string LastName, string Email) : IRequest<Guid>;

// 2. İşleyici (Handler) Mekanizması
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly IGenericRepository<User> _userRepository;
    private readonly IApplicationDbContext _context;

    public CreateUserCommandHandler(IGenericRepository<User> userRepository, IApplicationDbContext context)
    {
        _userRepository = userRepository;
        _context = context;
    }

    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Domain katmanında yazdığımız kurallara sahip User nesnesini canlandırıyoruz
        var user = new User(request.FirstName, request.LastName, request.Email);

        // Veri tabanına ekleme emrini repository üzerinden veriyoruz
        await _userRepository.AddAsync(user, cancellationToken);

        // Değişiklikleri Unit of Work mantığıyla veri tabanına tek seferde mühürlüyoruz
        await _context.SaveChangesAsync(cancellationToken);

        // Oluşan yeni kullanıcının ID'sini API katmanına fırlatıyoruz
        return user.Id;
    }
}