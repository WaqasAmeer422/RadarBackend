using Dapper;
using System.Data;
using System.Text;
using VisualSoft.Surveillance.Radar.Domain.Models;

namespace VisualSoft.Surveillance.Radar.Data.Repositories
{
    public class LookupRepository : ILookupRepository
    {
        private readonly IDbConnection _connection;

        private readonly IUserIdentificationModel _logedinUser;

        public LookupRepository(IDbConnection connection, IUserIdentificationModel logedinUser)
        {
            _connection = connection;
            _logedinUser = logedinUser;
        }


        public async Task<IEnumerable<LookupDto>?> GetAllLookups()
        {
            var sql = "SELECT * FROM look_up where is_active= 'true';";
            return await _connection?.QueryAsync<LookupDto>(sql);
        }


        public async Task<IEnumerable<LookupDto>?> GetLookupByCriteria(LookupDto? model)
        {
            var sql = new StringBuilder("SELECT * FROM look_up WHERE 1 = 1");
            DynamicParameters parameters = new DynamicParameters();
            if (model != null)
            {
                if (!string.IsNullOrWhiteSpace(model.Name))
                {
                    sql.AppendLine($" AND name = @Name");
                    parameters.Add("@Name", model.Name);
                }
                if (!string.IsNullOrWhiteSpace(model.Code))
                {
                    sql.AppendLine($" AND code = @Code");
                    parameters.Add("@Code", model.Code);
                }
                if (!string.IsNullOrWhiteSpace(model.ParentCode))
                {
                    sql.AppendLine($" AND parent_code = @ParentCode");
                    parameters.Add("@ParentCode", model.ParentCode);
                }
                if (!string.IsNullOrWhiteSpace(model.Correlation))
                {
                    sql.AppendLine($" AND correlation = @Correlation");
                    parameters.Add("@Correlation", model.Correlation);
                }

            }
            return await _connection?.QueryAsync<LookupDto>(sql.ToString(), parameters);

        }
    }
}
