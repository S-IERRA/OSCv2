using Mapster;
using Shared.Layouts;

namespace Shared.DTOs;

[AdaptTo(typeof(Account)), GenerateMapper]
public class AccountReturnDto : Account, IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.ForType<Account, AccountReturnDto>()
            .Ignore(x => x.HashedPassword);
    }
}