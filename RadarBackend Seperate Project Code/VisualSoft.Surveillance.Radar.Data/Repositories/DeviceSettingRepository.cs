using Dapper;
using System.Data;
using System.Text;
using VisualSoft.Surveillance.Radar.Data.Models;
using VisualSoft.Surveillance.Radar.Domain.Models;

namespace VisualSoft.Surveillance.Radar.Data.Repositories
{
    public class DeviceSettingRepository : IDeviceSettingRepository
    {
        private readonly IDbConnection _connection;

        private readonly IUserIdentificationModel _logedinUser;

        public DeviceSettingRepository(IDbConnection connection, IUserIdentificationModel logedinUser)
        {
            _connection = connection;
            _logedinUser = logedinUser;
        }

        public async Task<IEnumerable<DeviceSettingDto>?> GetAllDeviceSettings()
        {
            var sql = @"SELECT ds.*,lu_key.description as setting_key_description, lu_value.description as setting_value_description
                        FROM device_setting ds
                        LEFT JOIN look_up lu_key on ds.setting_key = lu_key.code
                        LEFT JOIN look_up lu_value on ds.setting_value = lu_value.code;";
            return await _connection?.QueryAsync<DeviceSettingDto>(sql);
        }

        public async Task<IEnumerable<DeviceSettingDto>?> GetDeviceSettingsByDeviceId(Guid deviceid)
        {
            var sql = @"SELECT ds.*,lu_key.description as setting_key_description, lu_value.description as setting_value_description
                        FROM device_setting ds
                        LEFT JOIN look_up lu_key on ds.setting_key = lu_key.code   
                        LEFT JOIN look_up lu_value on ds.setting_value = lu_value.code 
                        WHERE ds.device_id = @Id;";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", deviceid);

            return await _connection?.QueryAsync<DeviceSettingDto>(sql, parameters);
        }

        public async Task<IEnumerable<DeviceSettingDto>?> GetDeviceSettingsByCriteria(DeviceSettingDto? model)
        {
            var sql = new StringBuilder(@"SELECT ds.*,lu_key.description as setting_key_description, lu_value.description as setting_value_description
                        FROM device_setting ds
                        LEFT JOIN look_up lu_key on ds.setting_key = lu_key.code   
                        LEFT JOIN look_up lu_value on ds.setting_value = lu_value.code 
                        WHERE 1 = 1");

            DynamicParameters parameters = new DynamicParameters();
            if (model != null)
            {
                if (model.DeviceId.HasValue)
                {
                    sql.AppendLine($" AND ds.device_id = @DeviceId");
                    parameters.Add("@DeviceId", model.DeviceId);
                }
                if (!string.IsNullOrWhiteSpace(model.SettingName))
                {
                    sql.AppendLine($" AND ds.setting_name = @SettingName");
                    parameters.Add("@SettingName", model.SettingName);
                }
                if (!string.IsNullOrWhiteSpace(model.SettingKey))
                {
                    sql.AppendLine($" AND ds.setting_key = @SettingKey");
                    parameters.Add("@SettingKey", model.SettingKey);
                }

            }
            return await _connection?.QueryAsync<DeviceSettingDto>(sql.ToString(), parameters);

        }

        public async Task DeleteDeviceSettings(IList<Guid> deviceIds)
        {
            var sql = @"DELETE 
                        FROM device_setting
                        WHERE id = Any(@DeviceIds);";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@DeviceIds", deviceIds);

            await _connection?.ExecuteAsync(sql, parameters);
        }

        public async Task<IEnumerable<DeviceSettingDto>?> CreateDeviceSettings(Guid deviceId,List<DeviceSettingModel> settings)
        {
            var sql = @"
                INSERT INTO device_setting (device_id, setting_name, setting_key, setting_value, created_by, updated_by)
                VALUES (@DeviceId, @SettingName, @SettingKey, @SettingValue, @CreatedBy,  @UpdatedBy);";

            await _connection?.ExecuteAsync(sql, settings);

            return await GetDeviceSettingsByDeviceId(deviceId);
        }

        public async Task UpdateDeviceSettings(DeviceSettingDto setting)
        {
            var sql = @"
                UPDATE  device_setting
                SET setting_name = @SettingName, 
                    setting_key = @SettingKey, 
                    setting_value = @SettingValue,
                    updated_by = @UpdatedBy,
                    updated_date = now()
                WHERE Id = @Id;";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@SettingName", setting.SettingName);
            parameters.Add("@SettingKey", setting.SettingKey);
            parameters.Add("@SettingValue", setting.SettingValue);
            parameters.Add("@UpdatedBy", _logedinUser.Id);
            parameters.Add("@Id", setting.Id);

            await _connection?.ExecuteAsync(sql, parameters);

        }





    }
}
