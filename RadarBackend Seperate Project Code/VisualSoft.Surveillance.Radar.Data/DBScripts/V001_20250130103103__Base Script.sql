
CREATE TABLE human_detection_transactions (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    organisation_id UUID NOT NULL,
    device_id UUID NULL,
    transaction_event VARCHAR(50) NULL, -- Increased from 12 to 50
    radar_source_mac VARCHAR(100),
    agrinode_mac VARCHAR(100),
    presence BOOLEAN,
    presence_type VARCHAR(100),
    location VARCHAR(500) NULL,
    device_timestamp TIMESTAMPTZ,
    is_active BOOLEAN DEFAULT TRUE NOT NULL,
    created_date TIMESTAMPTZ DEFAULT now() NOT NULL,
    created_by VARCHAR(50) NOT NULL,
    updated_date TIMESTAMPTZ DEFAULT now() NOT NULL,
    updated_by VARCHAR(50) NOT NULL,
    CONSTRAINT fk_device_id FOREIGN KEY (device_id) REFERENCES device (id) ON DELETE SET NULL
);



