using System.Threading;

namespace VisualSoft.Surveillance.Radar.Domain
{
    public static class Constants
    {
        public class Informations
        {
            public const string INFO_CONSUMING_MESSAGE_RABBIT_MQ = "Consuming message from Rabbit MQ. {@Variables}";
            public const string INFO_SENDINDG_PAYLOAD_RABBIT_MQ = "Sending Payload to Rabbit MQ. {@Variables}";
            public const string INFO_STARTING_CONSUMER_SERVICE = "Starting Parser Request Consumer service {@Variables}";
            public const string INFO_STARTING_TRANSACTION_PROCESS = "Starting Creating Transcation Process {@Variables}";
            public const string INFO_CREATING_VEHICLE = "Creating Vehicle {@Variables}";
            public const string INFO_CREATING_TRANSACTION = "Creating Transaction {@Variables}";
            public const string INFO_CREATING_IMAGES = "Creating Images {@Variables}";
            public const string INFO_LOCATION_NOT_SET = "Unable to set location of transcation as Lookup does not exits {@Variables}";
            public const string INFO_UNKNOWN_LICENCE_PLATE_DETECTED = "Unknown licence plate detected. Enriched the date {@Variables}";
        }
        public class Warnings
        {
            public const string WARNIG_DEVICE_SETTINGS_CONFLICT = "Device settings are different than setting within system. {@Variables}";

        }
        public class Errors
        {
            public const string ERROR_DESERIALIZING_MESSAGE = "Error deserializing message. {@Variables}";
            public const string ERROR_CONSUMING_MESSAGE_RABBIT_MQ = "Error Consuming message from Rabbit MQ. {@Variables}";
            public const string ERROR_PROCESSING_MESSAGE_RABBIT_MQ = "Error Processing message from Rabbit MQ. {@Variables}";
            public const string ERROR_RABBIT_MQ_TIMEOUT = "Cancellation due to explicit timeout. {@Variables}";
            public const string ERROR_RABBIT_MQ_EASYNET_TIMEOUT = "Cancellation from an external source or EasyNetQ internal timeout. {@Variables}";
            public const string ERROR_RABBIT_MQ_HEADER_NOT_VALID = "Rabbit MQ header is not valid. {@Variables}";
            public const string ERROR_NO_RECORD_INSERTED = "No Record Inserted. {@Variables}";
            public const string ERROR_NO_DEVICE_EXIST = "No Device record existed.";
            public const string ERROR_NO_VEHICLE_EXIST = "No Vehicle record existed.";
            public const string ERROR_EXCEPTION_OCCURED = "Error occured!.";
            public const string ERROR_SENDINDG_PAYLOAD_RABBIT_MQ = "Error during sending payload to Rabbit MQ. {@Variables}";

            public const string ERROR_CREATING_ANPR_HOST = "Error configuring listner host";
            public const string ERROR_ENSURE_DATABASE = "Error While Ensuring Database";
            public const string ERROR_NO_LIC_PLATE_INFO = "NO Licence Plate Information In ANPR Data";
            public const string ERROR_ORGANIATION_IS_REQUIRED = "Organisation Id is required.";
            public const string ERROR_CREATING_TRANSACTION = "Error Creating Transcation {@Variables}";
            public const string ERROR_CREATING_IMAGES = "Error Creating images {@Variables}";
            public const string ERROR_CREATING_DEVICE_SETTINGS = "Error Creating device settings {@Variables}";
            public const string ERROR_ANPR_MODEL_IS_NULL = "AnprEvetMdel model is null";
            public const string ERROR_MAC_ADDRESS_IS_INVALID = "Error processing ANPR event.MacAddress is invalid";
            public const string ERROR_RESPONSE_HANDLING = "Error handling Response from AI";
            public const string ERROR_AI_DETECTION_FAILED = "Error saving AI detection. {@Variables}";
            public const string ERROR_AI_INVALID_LICENSE_PIC_TYPE = "Invalid Licence plat picture type. {@Variables}";
            public const string ERROR_AI_UPDATE_DETECTION_FAILED = "Update failed of for Ai detections. {@Variables}";
            public const string ERROR_ANPR_MODEL_NOT_CONSTRUCTED = "Unableto to create caset AnprEvetMdel";
            public const string ERROR_INVALID_RULE_NAME = "Invalid Rule name.";

            public const string ERROR_GENERATING_ALARAM = "Error Generating alaram";
            public const string ERROR_EXCEPTION_OCCURED_DURING_PROCESSING = "Exception Occured {@Variables}";
            public const string ERROR_AI_UPDATE_DETECTION_NOT_FOUND = "Unable to find transaction for AI detection update. {@Variables}";
            public const string ERROR_AI_UPDATE_DETECTION_VEHICLE_NOT_FOUND = "Unable to find vehicle for AI detection update. {@Variables}";
            public const string ERROR_CREATING_VEHICLE = "Vehicle cannnot be created. {@Variables}";
            // error for radar
            public const string ERROR_RADAR_SAVE_FAILED = "Error saving Human Detection Radar data. {@Variables}";
        }
        public class MessageHeader
        {
            public const string CORRELATION_ID = "correlationId";
            public const string MAC_ADDESS = "macAddress";
            public const string LICENCE_PLATE = "licensePlate";
            public const string DIRECTION = "direction";
            public const string VEHICLE_TYPE = "vehicleType";
            public const string VEHICLE_MODEL = "vehicleModel";
            public const string PROVIDER = "provider";
            public const string EVENT = "event";
            public const string AI_REFERENCE = "reference_number";
            public const string AI_DETECTION_TARGET = "detection_target";
            public const string ALARAM_RULE_NAME = "rule_name";
            public const string DEVICE_ID = "device_id";
            public const string DEVICE_SETTING_NAME = "device_setting_name";
            public const string RADAR_MESSAGE_HEADER = "RadarSource_mac";
            
        }

        public class DeviceEvent
        {
            public const string ANPR = "Anpr";
            public const string HUMAN_RADAR = "Human_Detection_Radar";
        }

        public class LookUps
        {
            public class HardwareProvide
            {
                public const string LOOKUP_NAME = "HardwareProvide";
                public const string HIKVISION = "HIKVISION";
            }
            public class TransactionSource
            {
                public const string LOOKUP_NAME = "TransactionSource";
                public const string ANPR = "ANPR";
                public const string RADAR = "RADAR";

            }
            public class DeviceCategory
            {
                public const string LOOKUP_NAME = "DeviceCategory";
                public const string CAMERA = "CAMERA";
            }
            public class IpSettings
            {
                public const string LOOKUP_NAME = "IpSettings";
                public const string IP = "IP";
                public const string PORT = "PORT";
            }
            public class DeviceLocation
            {
                public const string LOOKUP_NAME = "DeviceLocation";
            }
            public class Channels
            {
                public const string LOOKUP_NAME = "Channels";
                public const string ID = "ID";
                public const string NAME = "NAME";
            }

            public class AlaramStatus
            {
                public const string LOOKUP_NAME = "AlaramStatus";
                public const string NEW = "NEW";
                public const string IN_ACTION = "INACTN";
                public const string CLOSED = "CLOSE";
            }

            public class AlaramType
            {
                public const string LOOKUP_NAME = "AlaramType";
                public const string SYSTEM = "SYSTEM";
                public const string BUSINESS = "BUSINS";
            }

            public class AlaramKeyOperation
            {
                public const string LOOKUP_NAME = "AlaramKeyOperation";
                public const string LIKE = "LIK";
                public const string EQUAL = "EQU";
                public const string NONE = "NON";
            }

            public class AlaramKey
            {
                public const string LOOKUP_NAME = "AlaramKey";
                public const string VEHICL_REGISTRATION_NO = "REGNUM";
            }

            public class CORRELATION
            {
                public const string ALARM_SETTING = "AlarmSetting";
            }

            public class AiDetectionType
            {
                public const string LICENSE_PLATE = "LICPLT";
                public const string VEHICLE_TYPE = "VEHTYP";
            }
        }


        public class JwtToken
        {
            public const string AUTHORIZATION_HEADER = "Authorization";
            public const string BEARER_PREFIX = "Bearer";
            public const string USER_IDENTITY_CLAIM = "user_id";
            public const string USER_ORGANISATION_CLAIM = "organisation_id";
            public const string USER_NAME_CLAIM = "user_name";
            public const string USER_EMAIL_CLAIM = "user_email";
            public const string USER_ROLE_CLAIM = "user_role";
        }

        public class Roles
        {
            public const string SYSADMIN = "Sysadmin";
            public const string ADMIN = "Admin";
            public const string SERVICE = "Service";
            public const string OPERATOR = "Operator";
        }

        public static class Permissions
        {
            public const string EDIT_USER = "edit_user";
            public const string VIEW_USER = "view_user";
            public const string VIEW_ROLE = "view_role";
            public const string CHANGE_USER_PASSWORD = "change_password";
            public const string EDIT_ORGANISATION = "edit_organisation";
            public const string VIEW_ORGANISATION = "view_organisation";
            public const string EDIT_DEVICE = "edit_device";
            public const string ADD_DEVICE = "add_device";
            public const string VIEW_DEVICE = "view_device";
            public const string VIEW_DEVICE_SETTING = "view_device_setting";
            public const string VIEW_VEHICLE = "view_vehicle";
            public const string EDIT_VEHICLE = "edit_vehicle";
            public const string ADD_VEHICLE = "add_vehicle";
            public const string VIEW_TRANSACTION = "view_transaction";
            public const string UPDATE_AI_RESULTS = "update_ai_results";
            public const string VIEW_ALARAMS = "view_alaram_rule";
            public const string EDIT_ALARAM_RULE = "edit_alaram_rule";
            public const string VIEW_ALARAM_RULE = "view_alaram_rule";


            public static class OrganisationPermissions
            {
                public const string ANPR_MODULE = "anpr_module";
                public const string BARRIER_MODULE = "barrier_module";
                public const string RFID_MODULE = "rfid_module";
                public const string AI_LICENCE_PLAT_DETECTION_MODULE = "ai_license_plate_detection_module";
            }


        }

        public class AlaramRules
        {
            public const string DEVICE_OFF_LINE = "Device off line";
            public const string RADAR_DETECTION = "PRESENCE_DETECTED";
        }
        public const string UPDATED_BY_DEFAULT_VALUE = "System";
        public const decimal ANPR_CONFIDENCE_THRESHHOLD_FOR_AI = 20;
    }
}
