using Dapper;
using Serilog;
using System.Data;
using System.Text;
using VisualSoft.Surveillance.Radar.Domain;
using VisualSoft.Surveillance.Radar.Domain.Enums;
using VisualSoft.Surveillance.Radar.Domain.Models;
using VisualSoft.Surveillance.Radar.Domain.Models;

namespace VisualSoft.Surveillance.Radar.Data.Repositories
{
    public class RadarTransactionRepository : IRadarTransactionRepository
    {
        private readonly IDbConnection _connection;
        //private readonly IDbTransaction _transaction;
        private readonly IUserIdentificationModel _logedinUser;

        public RadarTransactionRepository(IDbConnection connection, IUserIdentificationModel logedinUser)
        {
            _connection = connection;
            //_transaction = transaction;
            _logedinUser = logedinUser;
        }

        public async Task<RadarTransactionDto?> CreateTransaction(RadarTransactionDto model)
        {

            var sql = @"INSERT INTO human_detection_transactions (
                                radar_source_mac,
                                agrinode_mac, 
                                device_id,
                                device_timestamp,
                                organisation_id,
                                transaction_event,
                                location,
                                presence, 
                                presence_type, 
                                created_by,
                                updated_by)                                
                    VALUES (
                                @RadarSourceMac,
                                @AgrinodeMac,
                                @DeviceId,
                                @DeviceTimestamp,
                                @OrganisationId,
                                @TransactionEvent,
                                @Location,
                                @Presence,                               
                                @PresenceType, 
                                @CreatedBy,
                                @UpdatedBy)
                RETURNING *;";
            // RETURNING id, source_mac AS SourceMac, radar_time_stamp AS RadarTimestamp;";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@RadarSourceMac", model.RadarSourceMac);
            parameters.Add("@AgrinodeMac", model.AgrinodeMac);
            parameters.Add("@DeviceId", model.DeviceId);
            parameters.Add("@OrganisationId", model.OrganisationId);
            parameters.Add("@DeviceTimestamp", model.DeviceTimestamp);
            parameters.Add("@TransactionEvent", model.TransactionEvent);
            parameters.Add("@Location", string.IsNullOrWhiteSpace(model.Location) ? "N/A" : model.Location);
            parameters.Add("@Presence", model.Presence);
            parameters.Add("@PresenceType", model.PresenceType);
            parameters.Add("@CreatedBy", model.CreatedBy == null ? Domain.Constants.UPDATED_BY_DEFAULT_VALUE : model.CreatedBy);
            parameters.Add("@UpdatedBy", model.UpdatedBy == null ? Domain.Constants.UPDATED_BY_DEFAULT_VALUE : model.CreatedBy);
            return await _connection.QueryFirstOrDefaultAsync<RadarTransactionDto>(sql, parameters);
        }
        public async Task<RadarTransactionDto?> Update(RadarTransactionDto model)
        {
            var sql = @"UPDATE human_detection_transactions 
                        SET 
                                radar_source_mac = @RadarSourceMac,
                                agrinode_mac = @AgrinodeMac,
                                device_id = @DeviceId,
                                organisation_id = @OrganisationId,
                                device_timestamp =  @DeviceTimestamp,                                
                                transaction_event = @TransactionEvent,
                                location = @Location,
                                presence = @Presence,
                                presence_type = @PresenceType,
                                updated_date = now(),
                                updated_by = @UpdatedBy                              
                    WHERE  Id = @Id
                    RETURNING *;";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", model.Id);
            parameters.Add("@RadarSourceMac", model.RadarSourceMac);
            parameters.Add("@DeviceId", model.DeviceId);
            parameters.Add("@OrganisationId", model.OrganisationId);
            parameters.Add("@DeviceTimestamp", model.DeviceTimestamp);
            parameters.Add("@TransactionEvent", model.TransactionEvent);
            parameters.Add("@Location", string.IsNullOrWhiteSpace(model.Location) ? "N/A" : model.Location);
            parameters.Add("@Presence", model.Presence);
            parameters.Add("@PresenceType", model.PresenceType);
            parameters.Add("@UpdatedBy", model.UpdatedBy == null ? Domain.Constants.UPDATED_BY_DEFAULT_VALUE : model.CreatedBy);

            return await _connection?.QueryFirstOrDefaultAsync<RadarTransactionDto>(sql, parameters);
        }


        public async Task<IEnumerable<RadarTransactionDto>?> GetAllTransactions()
        {
            var sql = @"SELECT tranlog.* 
                        FROM human_detection_transactions  tranlog
                        ORDER BY device_timestamp desc limit 25000";//TODO Limmit added temporrary

            return await _connection?.QueryAsync<RadarTransactionDto>(sql);
        }

        public async Task<IEnumerable<RadarTransactionDto>?> GetAllTransactions(Guid organisationId)
        {
            var sql = "SELECT * FROM human_detection_transactions WHERE organisation_id = @organisation_id;";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@organisation_id", organisationId);

            return await _connection?.QueryAsync<RadarTransactionDto>(sql, parameters);

        }

        public async Task<(IEnumerable<RadarTransactionDto>?, int totalCount)> GetAllTransactionsPagedItems(PaginationParameters paginationParams)
        {
            var offset = (paginationParams.PageNumber - 1) * paginationParams.PageSize;

            var sql = @"SELECT * 
                        FROM human_detection_transactions
                        ORDER BY device_timestamp DESC
                        OFFSET @Offset ROWS
                        FETCH NEXT @PageSize ROWS ONLY;

                        SELECT COUNT(*) 
                        FROM human_detection_transactions;";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Offset", offset);
            parameters.Add("@PageSize", paginationParams.PageSize);

            using (var multi = await _connection.QueryMultipleAsync(sql, parameters))
            {
                var items = await multi.ReadAsync<RadarTransactionDto>();
                var totalCount = await multi.ReadSingleAsync<int>();

                return (items, totalCount);
            }
        }
        public async Task<(IEnumerable<RadarTransactionDto>?, int totalCount)> GetAllTransactionsPagedItems(Guid organisation_id, PaginationParameters paginationParams)
        {
            var offset = (paginationParams.PageNumber - 1) * paginationParams.PageSize;

            var sql = @"SELECT * 
                        FROM human_detection_transactions
                        WHERE organisation_id = @organisation_id
                        ORDER BY device_timestamp DESC
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
                var items = await multi.ReadAsync<RadarTransactionDto>();
                var totalCount = await multi.ReadSingleAsync<int>();

                return (items, totalCount);
            }
        }

        //start here

        public async Task<RadarTransactionDto?> GettransactionById(Guid id)
        {
            var sql = @"SELECT tranlog.*
                        FROM human_detection_transactions tranlog
                        WHERE tranlog.id = @Id;";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@id", id);

            return await _connection?.QueryFirstOrDefaultAsync<RadarTransactionDto>(sql, parameters);
        }
        //end 3
        public async Task<IEnumerable<RadarTransactionDto>?> GetMatchingTransactions(DateTime fromDate, Guid organisationId)
        {
            var sql = new StringBuilder(@"SELECT tranlog.*
                        FROM human_detection_transactions tranlog
                        WHERE tranlog.device_timestamp::date >= @FromDate::date
                        AND tranlog.organisation_id =  @OrganisationId;"
                        );

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@FromDate", fromDate);
            parameters.Add("@OrganisationId", organisationId);

            return await _connection?.QueryAsync<RadarTransactionDto>(sql.ToString(), parameters);
        }

        public async Task<IEnumerable<RadarTransactionDto>?> GetTransactionByCriteria(TransactionFilterRequestDto? criteria)
        {
            var sql = new StringBuilder(@"SELECT tranlog.*
                                        FROM human_detection_transactions tranlog
                                        WHERE 1 = 1");
            DynamicParameters parameters = new DynamicParameters();
            if (criteria != null)
            {
                if (criteria.OrganisationId.HasValue)
                {
                    sql.AppendLine($" AND tranlog.organisation_id =  @organisation_id");
                    parameters.Add("@organisation_id", criteria.OrganisationId);
                }
                if (!string.IsNullOrWhiteSpace(criteria.RadarSourceMac))
                {
                    sql.AppendLine($" AND tranlog.radar_source_mac ILIKE(@radar_source_mac)");
                    parameters.Add("@vehicle_registration_no", $"%{criteria.RadarSourceMac}%");
                }
                if (!string.IsNullOrWhiteSpace(criteria.Source))
                {
                    sql.AppendLine($" AND tranlog.transaction_event ILIKE(@transaction_event)");
                    parameters.Add("@transaction_event", $"%{criteria.Source}%");
                }
                if (!string.IsNullOrWhiteSpace(criteria.Location))
                {
                    sql.AppendLine($" AND tranlog.location ILIKE(@location)");
                    parameters.Add("@location", $"%{criteria.Location}%");
                }

                if (criteria.FromDate.HasValue)
                {
                    sql.AppendLine($" AND  tranlog.device_timestamp >= @FromDate");
                    parameters.Add("@FromDate", criteria.FromDate);
                }
                if (criteria.ToDate.HasValue)
                {
                    sql.AppendLine($" AND  tranlog.device_timestamp <= @ToDate");
                    parameters.Add("@ToDate", criteria.ToDate);
                }
            }
            sql.AppendLine(" ORDER BY device_timestamp desc LIMIT 50000");
            return await _connection?.QueryAsync<RadarTransactionDto>(sql.ToString(), parameters);
        }

        //public async Task<(IEnumerable<RadarTransactionDto>?, int totalCount)> GetTransactionByCriteriaPagedItems(TransactionFilterRequestDto criteria, PaginationParameters paginationParams)
        //{
        //    var offset = (paginationParams.PageNumber - 1) * paginationParams.PageSize;
        //    var sql1 = new StringBuilder(@"SELECT * 
        //                                    FROM human_detection_transactions 
        //                                    WHERE 1 = 1");

        //    var sql2 = new StringBuilder(@"SELECT COUNT(*)
        //                                    FROM human_detection_transactions  
        //                                    WHERE 1 = 1");

        //    DynamicParameters parameters = new DynamicParameters();
        //    parameters.Add("@Offset", offset);
        //    parameters.Add("@PageSize", paginationParams.PageSize);

        //    if (criteria != null)
        //    {
        //        if (criteria.OrganisationId.HasValue)
        //        {
        //            sql1.AppendLine($" AND organisation_id = @organisation_id");
        //            sql2.AppendLine($" AND organisation_id = @organisation_id");
        //            parameters.Add("@organisation_id", criteria.OrganisationId.Value);
        //        }
        //        if (!string.IsNullOrWhiteSpace(criteria.RadarSourceMac))
        //        {
        //            sql1.AppendLine($" AND vehicle_registration_no ILIKE(@vehicle_registration_no)");
        //            sql2.AppendLine($" AND vehicle_registration_no ILIKE(@vehicle_registration_no)");
        //            parameters.Add("@vehicle_registration_no", $"%{criteria.RadarSourceMac}%");
        //        }
        //        if (!string.IsNullOrWhiteSpace(criteria.Source))
        //        {
        //            sql1.AppendLine($" AND transaction_event ILIKE(@transaction_event)");
        //            sql2.AppendLine($" AND transaction_event ILIKE(@transaction_event)");
        //            parameters.Add("@transaction_event", $"%{criteria.Source}%");
        //        }
        //        if (!string.IsNullOrWhiteSpace(criteria.Location))
        //        {
        //            sql1.AppendLine($" AND location ILIKE(@location)");
        //            sql2.AppendLine($" AND location ILIKE(@location)");
        //            parameters.Add("@location", $"%{criteria.Location}%");
        //        }

        //        if (criteria.FromDate.HasValue)
        //        {
        //            sql1.AppendLine($" AND  device_timestamp >= @FromDate");
        //            sql2.AppendLine($" AND  device_timestamp >= @FromDate");
        //            parameters.Add("@FromDate", criteria.FromDate);
        //        }
        //        if (criteria.ToDate.HasValue)
        //        {
        //            sql1.AppendLine($" AND  device_timestamp <= @ToDate");
        //            sql2.AppendLine($" AND  device_timestamp <= @ToDate");
        //            parameters.Add("@ToDate", criteria.ToDate);
        //        }

        //    }
        //    sql1.Append(@" ORDER BY device_timestamp DESC
        //                    OFFSET @Offset ROWS
        //                    FETCH NEXT @PageSize ROWS ONLY");

        //    var sql = $"{sql1}; {sql2};";

        //    using (var multi = await _connection.QueryMultipleAsync(sql, parameters))
        //    {
        //        var items = await multi.ReadAsync<RadarTransactionDto>();
        //        var totalCount = await multi.ReadSingleAsync<int>();

        //        return (items, totalCount);
        //    }
        //}

        //public async Task<RadarTransactionDto?> NavigateTransactionByCriteria(NavigateTransactionFilterDto? criteria)
        //{
        //    var sql = new StringBuilder(@"SELECT tranlog.*
        //                                FROM human_detection_transactions tranlog
        //                                WHERE 1 = 1");
        //    DynamicParameters parameters = new DynamicParameters();
        //    if (criteria != null)
        //    {

        //        if (!string.IsNullOrWhiteSpace(criteria.Location))
        //        {
        //            sql.AppendLine($" AND tranlog.location = @Location");
        //            parameters.Add("@location", criteria.Location);
        //        }

        //        if (criteria.FromDate.HasValue && !criteria.NavigationDirection.Equals(NavigationDirection.Latest))
        //        {
        //            // Navigatin should be based on system created data as we have milli seconds
        //            // but device time stamp have date till seconds. so it will not work if two 
        //            // transcations are at the same timestamps
        //            if (criteria.NavigationDirection.Equals(NavigationDirection.Next))
        //            {
        //                sql.AppendLine($" AND  tranlog.created_date > @FromDate");
        //            }
        //            else
        //            {
        //                sql.AppendLine($" AND  tranlog.created_date < @FromDate");
        //            }
        //            parameters.Add("@FromDate", criteria.FromDate);
        //        }
        //    }
        //    sql.AppendLine(" ORDER BY created_date desc LIMIT 1");
        //    return await _connection?.QueryFirstOrDefaultAsync<RadarTransactionDto>(sql.ToString(), parameters);
        //}
        //----


    }
}