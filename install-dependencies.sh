#!/bin/bash

# -----------------------------
# Script directory
# -----------------------------
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
DEPENDENCIES_DIR="$SCRIPT_DIR/Dependencies"

# -----------------------------
# Destination folders
# -----------------------------
REDIS_DEST="C:/Redis"
KAFKA_DEST="C:/Kafka/kafka"

# -----------------------------
# Filenames
# -----------------------------
REDIS_FILE="$DEPENDENCIES_DIR/Redis-x64-3.0.504.zip"
KAFKA_PART_PREFIX="$DEPENDENCIES_DIR/kafka_part_"
KAFKA_TGZ="$DEPENDENCIES_DIR/kafka_2.13-3.5.1.tgz"

# -----------------------------
# Ensure Dependencies folder exists
# -----------------------------
mkdir -p "$DEPENDENCIES_DIR" || { echo "Failed to create Dependencies folder"; exit 1; }

# -----------------------------
# Remove previous installations
# -----------------------------
[ -d "$REDIS_DEST" ] && echo "Removing existing Redis..." && rm -rf "$REDIS_DEST"
[ -d "$KAFKA_DEST" ] && echo "Removing existing Kafka..." && rm -rf "$KAFKA_DEST"

# -----------------------------
# Extract Redis
# -----------------------------
echo "Extracting Redis..."
mkdir -p "$REDIS_DEST"
unzip -o "$REDIS_FILE" -d "$REDIS_DEST" || { echo "Redis extraction failed"; exit 1; }
echo "Redis extracted to $REDIS_DEST"

# -----------------------------
# Reassemble Kafka parts
# -----------------------------
echo "Reassembling Kafka parts..."
cat ${KAFKA_PART_PREFIX}* > "$KAFKA_TGZ" || { echo "Failed to reassemble Kafka"; exit 1; }

# -----------------------------
# Extract Kafka
# -----------------------------
echo "Extracting Kafka..."
TEMP_DIR="$DEPENDENCIES_DIR/temp_kafka"
mkdir -p "$TEMP_DIR"
tar -xzf "$KAFKA_TGZ" -C "$TEMP_DIR" || { echo "Kafka extraction failed"; exit 1; }

EXTRACTED_FOLDER=$(find "$TEMP_DIR" -maxdepth 1 -type d -name "kafka_2.13*")
if [ -z "$EXTRACTED_FOLDER" ]; then
    echo "Kafka folder not found after extraction"
    rm -rf "$TEMP_DIR"
    exit 1
fi

mkdir -p "$(dirname "$KAFKA_DEST")"
mv "$EXTRACTED_FOLDER" "$KAFKA_DEST" || { echo "Failed to move Kafka folder"; exit 1; }
rm -rf "$TEMP_DIR"
echo "Kafka extracted to $KAFKA_DEST"

# -----------------------------
# Finish
# -----------------------------
echo "All setup completed successfully!"
