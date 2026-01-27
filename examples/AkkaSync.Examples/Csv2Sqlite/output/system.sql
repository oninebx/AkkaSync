CREATE TABLE IF NOT EXISTS system_events (
    id          TEXT PRIMARY KEY,
    system_id   TEXT NOT NULL,
    type        TEXT NOT NULL,
    component   TEXT,
    host        TEXT,
    metric      TEXT,
    value       REAL,
    unit        TEXT,
    severity    TEXT,
    timestamp   TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS system_metrics (
    id          TEXT PRIMARY KEY,
    system_id   TEXT NOT NULL,
    metric      TEXT NOT NULL,
    value       REAL NOT NULL,
    severity    TEXT NOT NULL,
    timestamp   TEXT NOT NULL
);