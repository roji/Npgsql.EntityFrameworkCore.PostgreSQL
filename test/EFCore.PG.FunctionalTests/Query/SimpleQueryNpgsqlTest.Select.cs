using System.Threading.Tasks;

namespace Npgsql.EntityFrameworkCore.PostgreSQL.Query
{
    public partial class SimpleQueryNpgsqlTest
    {
        public override Task Member_binding_after_ctor_arguments_fails_with_client_eval(bool isAsync)
            => AssertTranslationFailed(() => base.Member_binding_after_ctor_arguments_fails_with_client_eval(isAsync));
    }
}
