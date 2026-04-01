using Dapper;
using System.Data;
using System.Text;
using VisualSoft.Surveillance.Radar.Domain.Models;

namespace VisualSoft.Surveillance.Radar.Data.Repositories
{
    public class DeviceRepository : IDeviceRepository
    {
        private readonly IDbConnection _connection;
        private readonly IUserIdentificationModel _logedinUser;

        public DeviceRepository(IDbConnection connection, IUserIdentificationModel logedinUser)
        {
            _connection = connection;
            _logedinUser = logedinUser;
        }


        public async Task<IEnumerable<DeviceDetailDto>?> GetAll()
        {
            var sql = "SELECT * FROM device;";
            return await _connection?.QueryAsync<DeviceDetailDto>(sql);
        }

        public async Task<(IEnumerable<DeviceDetailDto>?, int totalCount)> GetAllPagedItems(PaginationParameters paginationParams)
        {
            var offset = (paginationParams.PageNumber - 1) * paginationParams.PageSize;

            var sql = @"SELECT * 
                        FROM device
                        ORDER BY created_date DESC
                        OFFSET @Offset ROWS
                        FETCH NEXT @PageSize ROWS ONLY;

                        SELECT COUNT(*) 
                        FROM device;"; // Second query to get total count

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Offset", offset);
            parameters.Add("@PageSize", paginationParams.PageSize);

            using (var multi = await _connection.QueryMultipleAsync(sql, parameters))
            {
                var items = await multi.ReadAsync<DeviceDetailDto>();
                var totalCount = await multi.ReadSingleAsync<int>();

                return (items, totalCount);
            }
        }

        public async Task<IEnumerable<DeviceDetailDto>?> GetAll(Guid organisation_id)
        {
            var sql = "SELECT * FROM device WHERE organisation_id = @organisation_id;";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@organisation_id", organisation_id);

            return await _connection?.QueryAsync<DeviceDetailDto>(sql.ToString(), parameters);
        }

        public async Task<(IEnumerable<DeviceDetailDto>?, int totalCount)> GetAllPagedItems(Guid organisation_id, PaginationParameters paginationParams)
        {
            var offset = (paginationParams.PageNumber - 1) * paginationParams.PageSize;

            var sql = @"SELECT * 
                        FROM device
                        WHERE organisation_id = @organisation_id
                        ORDER BY created_date DESC
                        OFFSET @Offset ROWS
                        FETCH NEXT @PageSize ROWS ONLY;

                        SELECT COUNT(*) 
                        FROM device
                        WHERE organisation_id = @organisation_id;"; // Second query to get total count

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@organisation_id", organisation_id);
            parameters.Add("@Offset", offset);
            parameters.Add("@PageSize", paginationParams.PageSize);

            using (var multi = await _connection.QueryMultipleAsync(sql, parameters))
            {
                var items = await multi.ReadAsync<DeviceDetailDto>();
                var totalCount = await multi.ReadSingleAsync<int>();

                return (items, totalCount);
            }
        }

        public async Task<DeviceDetailDto?> GetById(Guid id)
        {
            var sql = "SELECT * FROM device WHERE id = @Id;";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id);

            return await _connection?.QueryFirstOrDefaultAsync<DeviceDetailDto>(sql, parameters);
        }

        public async Task<DeviceDetailDto?> GetByMacAddress(string macaddress)
        {
            var sql = "SELECT * FROM device WHERE mac_address = @MacAddress;";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@MacAddress", macaddress);

            return await _connection?.QueryFirstOrDefaultAsync<DeviceDetailDto>(sql, parameters);
        }


        public async Task<IEnumerable<DeviceDetailDto>?> GetByCriteria(DeviceDetailDto? model)
        {
            var sql = new StringBuilder("SELECT * FROM device WHERE 1 = 1");
            DynamicParameters parameters = new DynamicParameters();
            if (model != null)
            {
                if (model.OrganisationId.HasValue)
                {
                    sql.AppendLine($" AND organisation_id = @organisation_id");
                    parameters.Add("@organisation_id", model.OrganisationId.Value);
                }
                if (!string.IsNullOrWhiteSpace(model.MacAddress))
                {
                    sql.AppendLine($" AND mac_address ILIKE(@MacAddress)");
                    parameters.Add("@MacAddress", $"%{model.MacAddress}%");
                }
                if (model.IsActive != null)
                {
                    sql.AppendLine($" AND is_active = @IsActive");
                    parameters.Add("@IsActive", model.IsActive);
                }
                if (model.Provider != null)
                {
                    sql.AppendLine($" AND provider = @Provider");
                    parameters.Add("@Provider", model.Provider);
                }
                if (model.Category != null)
                {
                    sql.AppendLine($" AND category = @Category");
                    parameters.Add("@Category", model.Category);
                }
            }
            return await _connection?.QueryAsync<DeviceDetailDto>(sql.ToString(), parameters);

        }

        public async Task<(IEnumerable<DeviceDetailDto>?, int totalCount)> GetByCriteriaPagedItems(DeviceDetailDto model, PaginationParameters paginationParams)
        {
            var offset = (paginationParams.PageNumber - 1) * paginationParams.PageSize;
            var sql1 = new StringBuilder(@"SELECT * 
                                            FROM device 
                                            WHERE 1 = 1");

            var sql2 = new StringBuilder(@"SELECT COUNT(*)
                                            FROM device  
                                            WHERE 1 = 1");

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Offset", offset);
            parameters.Add("@PageSize", paginationParams.PageSize);

            if (model != null)
            {
                if (model.OrganisationId.HasValue)
                {
                    sql1.AppendLine($" AND organisation_id = @organisation_id");
                    sql2.AppendLine($" AND organisation_id = @organisation_id");
                    parameters.Add("@organisation_id", model.OrganisationId.Value);
                }
                if (!string.IsNullOrWhiteSpace(model.MacAddress))
                {
                    sql1.AppendLine($" AND mac_address ILIKE(@MacAddress)");
                    sql2.AppendLine($" AND mac_address ILIKE(@MacAddress)");
                    parameters.Add("@MacAddress", $"%{model.MacAddress}%");
                }
                if (model.IsActive != null)
                {
                    sql1.AppendLine($" AND is_active = @IsActive");
                    sql2.AppendLine($" AND is_active = @IsActive");
                    parameters.Add("@IsActive", model.IsActive);
                }
                if (model.Provider != null)
                {
                    sql1.AppendLine($" AND provider = @Provider");
                    sql2.AppendLine($" AND provider = @Provider");
                    parameters.Add("@Provider", model.Provider);
                }
                if (model.Category != null)
                {
                    sql1.AppendLine($" AND category = @Category");
                    sql2.AppendLine($" AND category = @Category");
                    parameters.Add("@Category", model.Category);
                }
            }
            sql1.Append(@" ORDER BY created_date DESC
                            OFFSET @Offset ROWS
                            FETCH NEXT @PageSize ROWS ONLY");

            var sql = $"{sql1}; {sql2};";

            using (var multi = await _connection.QueryMultipleAsync(sql, parameters))
            {
                var items = await multi.ReadAsync<DeviceDetailDto>();
                var totalCount = await multi.ReadSingleAsync<int>();

                return (items, totalCount);
            }
        }

        public async Task<DeviceDetailDto> Create(DeviceDetailDto model)
        {
            var sql = @"INSERT INTO device (organisation_id,serial_number, device_name, model, mac_address, provider, category,  created_by, updated_by)
                VALUES (@organisation_id,@SerialNumber, @DeviceName, @Model, @MacAddress, @Provider,@Category,  @CreatedBy,  @UpdatedBy)
                RETURNING *;";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@organisation_id", model.OrganisationId.Value);
            parameters.Add("@SerialNumber", model.SerialNumber);
            parameters.Add("@DeviceName", model.DeviceName);
            parameters.Add("@Model", model.Model);
            parameters.Add("@MacAddress", model.MacAddress);
            parameters.Add("@Provider", model.Provider);
            parameters.Add("@Category", model.Category);
            parameters.Add("@CreatedBy", _logedinUser.Id);
            parameters.Add("@UpdatedBy", _logedinUser.Id);

            return await _connection?.QueryFirstOrDefaultAsync<DeviceDetailDto>(sql, parameters);
        }

        public async Task<DeviceDetailDto?> Update(DeviceDetailDto model)
        {
            var sql = @"
                UPDATE device
                SET serial_number = @SerialNumber,
                    organisation_id = @organisation_id,
                    device_name = @DeviceName,
                    model = @Model,
                    mac_address = @MacAddress,
                    provider = @Provider,
                    category = @Category,
                    is_active = @is_active,
                    updated_date = Now(),
                    updated_by = @UpdatedBy
                WHERE id = @Id
                RETURNING *;";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", model.Id);
            parameters.Add("@organisation_id", model.OrganisationId.Value);
            parameters.Add("@is_active", model.IsActive);
            parameters.Add("@SerialNumber", model.SerialNumber);
            parameters.Add("@DeviceName", model.DeviceName);
            parameters.Add("@Model", model.Model);
            parameters.Add("@MacAddress", model.MacAddress);
            parameters.Add("@Provider", model.Provider);
            parameters.Add("@Category", model.Category);
            parameters.Add("@UpdatedBy", _logedinUser.Id);

            return await _connection?.QueryFirstOrDefaultAsync<DeviceDetailDto>(sql, parameters);
        }
    }
}
