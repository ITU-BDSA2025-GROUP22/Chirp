#!/usr/bin/env bash
sqlite3 chirp.db < data/schema.sql
sqlite3 chirp.db < data/dump.sql