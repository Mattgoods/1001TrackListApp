-- 1001 TrackList App - PostgreSQL Migration Script
-- Generated: 2026-01-22
-- Description: Initial database schema migration

-- Drop tables if they exist (in reverse order of dependencies)
DROP TABLE IF EXISTS set_analytics CASCADE;
DROP TABLE IF EXISTS set_songs CASCADE;
DROP TABLE IF EXISTS song_artists CASCADE;
DROP TABLE IF EXISTS dj_sets CASCADE;
DROP TABLE IF EXISTS songs CASCADE;
DROP TABLE IF EXISTS artists CASCADE;
DROP TABLE IF EXISTS venues CASCADE;

-- Create artists table
CREATE TABLE artists (
    artist_id SERIAL PRIMARY KEY,
    display_name TEXT NOT NULL,
    country TEXT
);

-- Create songs table
CREATE TABLE songs (
    song_id SERIAL PRIMARY KEY,
    title TEXT NOT NULL,
    release_date DATE,
    duration_seconds INTEGER,
    genre TEXT,
    bpm INTEGER
);

-- Create venues table
CREATE TABLE venues (
    venue_id SERIAL PRIMARY KEY,
    name TEXT,
    capacity INTEGER,
    address TEXT
);

-- Create song_artists junction table (many-to-many relationship)
CREATE TABLE song_artists (
    song_id INTEGER NOT NULL,
    artist_id INTEGER NOT NULL,
    PRIMARY KEY (song_id, artist_id),
    CONSTRAINT FK_song_artists_songs_song_id 
        FOREIGN KEY (song_id) 
        REFERENCES songs(song_id) 
        ON DELETE CASCADE,
    CONSTRAINT FK_song_artists_artists_artist_id 
        FOREIGN KEY (artist_id) 
        REFERENCES artists(artist_id) 
        ON DELETE CASCADE
);

-- Create dj_sets table
CREATE TABLE dj_sets (
    dj_set_id SERIAL PRIMARY KEY,
    artist_id INTEGER NOT NULL,
    title TEXT,
    set_datetime TIMESTAMP WITH TIME ZONE,
    duration_minutes INTEGER,
    source_url TEXT,
    venue_id INTEGER,
    CONSTRAINT FK_dj_sets_artists_artist_id 
        FOREIGN KEY (artist_id) 
        REFERENCES artists(artist_id) 
        ON DELETE RESTRICT,
    CONSTRAINT FK_dj_sets_venues_venue_id 
        FOREIGN KEY (venue_id) 
        REFERENCES venues(venue_id) 
        ON DELETE SET NULL
);

-- Create set_analytics table (one-to-one with dj_sets)
CREATE TABLE set_analytics (
    dj_set_id INTEGER PRIMARY KEY,
    tickets_sold INTEGER,
    attendance_count INTEGER,
    gross_revenue INTEGER,
    stream_count INTEGER,
    like_count INTEGER,
    CONSTRAINT FK_set_analytics_dj_sets_dj_set_id 
        FOREIGN KEY (dj_set_id) 
        REFERENCES dj_sets(dj_set_id) 
        ON DELETE CASCADE
);

-- Create set_songs junction table
CREATE TABLE set_songs (
    set_song_id SERIAL PRIMARY KEY,
    song_id INTEGER NOT NULL,
    dj_set_id INTEGER NOT NULL,
    timestamp_in_set_seconds INTEGER,
    CONSTRAINT FK_set_songs_songs_song_id 
        FOREIGN KEY (song_id) 
        REFERENCES songs(song_id) 
        ON DELETE CASCADE,
    CONSTRAINT FK_set_songs_dj_sets_dj_set_id 
        FOREIGN KEY (dj_set_id) 
        REFERENCES dj_sets(dj_set_id) 
        ON DELETE CASCADE
);

-- Create indexes for foreign keys (improves query performance)
CREATE INDEX IX_dj_sets_artist_id ON dj_sets(artist_id);
CREATE INDEX IX_dj_sets_venue_id ON dj_sets(venue_id);
CREATE INDEX IX_set_songs_dj_set_id ON set_songs(dj_set_id);
CREATE INDEX IX_set_songs_song_id ON set_songs(song_id);
CREATE INDEX IX_song_artists_artist_id ON song_artists(artist_id);

-- Migration completed successfully
