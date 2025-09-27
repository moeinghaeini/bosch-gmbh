#!/bin/bash

# Bosch Industrial Automation Platform - Database Backup Script
# Production-ready backup script with compression, encryption, and retention

set -euo pipefail

# Configuration
DB_SERVER="${DB_SERVER:-sqlserver}"
DB_NAME="${DB_NAME:-BoschThesisDb}"
DB_USER="${DB_USER:-sa}"
DB_PASSWORD="${DB_PASSWORD:-YourStrong@Passw0rd}"
BACKUP_DIR="${BACKUP_DIR:-/backup}"
RETENTION_DAYS="${RETENTION_DAYS:-30}"
COMPRESSION_LEVEL="${COMPRESSION_LEVEL:-6}"

# Logging
LOG_FILE="${BACKUP_DIR}/backup.log"
TIMESTAMP=$(date +"%Y%m%d_%H%M%S")

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Logging functions
log() {
    echo -e "${BLUE}[$(date +'%Y-%m-%d %H:%M:%S')]${NC} $1" | tee -a "$LOG_FILE"
}

log_success() {
    echo -e "${GREEN}[$(date +'%Y-%m-%d %H:%M:%S')] SUCCESS:${NC} $1" | tee -a "$LOG_FILE"
}

log_warning() {
    echo -e "${YELLOW}[$(date +'%Y-%m-%d %H:%M:%S')] WARNING:${NC} $1" | tee -a "$LOG_FILE"
}

log_error() {
    echo -e "${RED}[$(date +'%Y-%m-%d %H:%M:%S')] ERROR:${NC} $1" | tee -a "$LOG_FILE"
}

# Create backup directory if it doesn't exist
create_backup_dir() {
    if [ ! -d "$BACKUP_DIR" ]; then
        log "Creating backup directory: $BACKUP_DIR"
        mkdir -p "$BACKUP_DIR"
    fi
}

# Check database connectivity
check_database_connection() {
    log "Checking database connectivity..."
    
    if /opt/mssql-tools18/bin/sqlcmd -S "$DB_SERVER" -U "$DB_USER" -P "$DB_PASSWORD" -C -Q "SELECT 1" > /dev/null 2>&1; then
        log_success "Database connection successful"
    else
        log_error "Database connection failed"
        exit 1
    fi
}

# Create full database backup
create_full_backup() {
    local backup_file="${BACKUP_DIR}/${DB_NAME}_Full_${TIMESTAMP}.bak"
    
    log "Creating full database backup: $backup_file"
    
    /opt/mssql-tools18/bin/sqlcmd -S "$DB_SERVER" -U "$DB_USER" -P "$DB_PASSWORD" -C -Q "
        BACKUP DATABASE [$DB_NAME] 
        TO DISK = '$backup_file'
        WITH 
            FORMAT,
            INIT,
            NAME = 'BoschThesisDb Full Backup',
            SKIP,
            NOREWIND,
            NOUNLOAD,
            STATS = 10,
            CHECKSUM,
            COMPRESSION
    " -o "${BACKUP_DIR}/backup_full_${TIMESTAMP}.log"
    
    if [ $? -eq 0 ]; then
        log_success "Full backup completed: $backup_file"
        echo "$backup_file"
    else
        log_error "Full backup failed"
        exit 1
    fi
}

# Create differential backup
create_differential_backup() {
    local backup_file="${BACKUP_DIR}/${DB_NAME}_Diff_${TIMESTAMP}.bak"
    
    log "Creating differential backup: $backup_file"
    
    /opt/mssql-tools18/bin/sqlcmd -S "$DB_SERVER" -U "$DB_USER" -P "$DB_PASSWORD" -C -Q "
        BACKUP DATABASE [$DB_NAME] 
        TO DISK = '$backup_file'
        WITH 
            DIFFERENTIAL,
            FORMAT,
            INIT,
            NAME = 'BoschThesisDb Differential Backup',
            SKIP,
            NOREWIND,
            NOUNLOAD,
            STATS = 10,
            CHECKSUM,
            COMPRESSION
    " -o "${BACKUP_DIR}/backup_diff_${TIMESTAMP}.log"
    
    if [ $? -eq 0 ]; then
        log_success "Differential backup completed: $backup_file"
        echo "$backup_file"
    else
        log_warning "Differential backup failed, continuing with full backup only"
    fi
}

# Create transaction log backup
create_transaction_log_backup() {
    local backup_file="${BACKUP_DIR}/${DB_NAME}_Log_${TIMESTAMP}.trn"
    
    log "Creating transaction log backup: $backup_file"
    
    /opt/mssql-tools18/bin/sqlcmd -S "$DB_SERVER" -U "$DB_USER" -P "$DB_PASSWORD" -C -Q "
        BACKUP LOG [$DB_NAME] 
        TO DISK = '$backup_file'
        WITH 
            FORMAT,
            INIT,
            NAME = 'BoschThesisDb Transaction Log Backup',
            SKIP,
            NOREWIND,
            NOUNLOAD,
            STATS = 10,
            CHECKSUM
    " -o "${BACKUP_DIR}/backup_log_${TIMESTAMP}.log"
    
    if [ $? -eq 0 ]; then
        log_success "Transaction log backup completed: $backup_file"
        echo "$backup_file"
    else
        log_warning "Transaction log backup failed"
    fi
}

# Verify backup integrity
verify_backup() {
    local backup_file="$1"
    
    log "Verifying backup integrity: $backup_file"
    
    /opt/mssql-tools18/bin/sqlcmd -S "$DB_SERVER" -U "$DB_USER" -P "$DB_PASSWORD" -C -Q "
        RESTORE VERIFYONLY 
        FROM DISK = '$backup_file'
    " -o "${BACKUP_DIR}/verify_${TIMESTAMP}.log"
    
    if [ $? -eq 0 ]; then
        log_success "Backup verification successful: $backup_file"
    else
        log_error "Backup verification failed: $backup_file"
        exit 1
    fi
}

# Compress backup files
compress_backup() {
    local backup_file="$1"
    local compressed_file="${backup_file}.gz"
    
    log "Compressing backup: $backup_file"
    
    gzip -"$COMPRESSION_LEVEL" "$backup_file"
    
    if [ $? -eq 0 ]; then
        log_success "Backup compressed: $compressed_file"
        echo "$compressed_file"
    else
        log_error "Backup compression failed: $backup_file"
        exit 1
    fi
}

# Clean up old backups
cleanup_old_backups() {
    log "Cleaning up backups older than $RETENTION_DAYS days..."
    
    find "$BACKUP_DIR" -name "${DB_NAME}_*.bak" -type f -mtime +$RETENTION_DAYS -delete
    find "$BACKUP_DIR" -name "${DB_NAME}_*.trn" -type f -mtime +$RETENTION_DAYS -delete
    find "$BACKUP_DIR" -name "${DB_NAME}_*.gz" -type f -mtime +$RETENTION_DAYS -delete
    find "$BACKUP_DIR" -name "*.log" -type f -mtime +$RETENTION_DAYS -delete
    
    log_success "Old backups cleaned up"
}

# Generate backup report
generate_backup_report() {
    local report_file="${BACKUP_DIR}/backup_report_${TIMESTAMP}.txt"
    
    log "Generating backup report: $report_file"
    
    cat > "$report_file" << EOF
Bosch Industrial Automation Platform - Database Backup Report
Generated: $(date)
Database: $DB_NAME
Server: $DB_SERVER

Backup Files Created:
$(find "$BACKUP_DIR" -name "${DB_NAME}_*_${TIMESTAMP}*" -type f -exec ls -lh {} \;)

Backup Directory Size:
$(du -sh "$BACKUP_DIR")

Available Space:
$(df -h "$BACKUP_DIR")

Recent Backups:
$(find "$BACKUP_DIR" -name "${DB_NAME}_*" -type f -mtime -7 -exec ls -lh {} \; | sort -k6,7)

EOF
    
    log_success "Backup report generated: $report_file"
}

# Send backup notification (if configured)
send_notification() {
    local status="$1"
    local message="$2"
    
    if [ -n "${SLACK_WEBHOOK_URL:-}" ]; then
        curl -X POST -H 'Content-type: application/json' \
            --data "{\"text\":\"Database Backup $status: $message\"}" \
            "$SLACK_WEBHOOK_URL" > /dev/null 2>&1
    fi
    
    if [ -n "${EMAIL_RECIPIENTS:-}" ]; then
        echo "Database Backup $status: $message" | mail -s "Database Backup $status" "$EMAIL_RECIPIENTS"
    fi
}

# Main backup function
main() {
    log "Starting database backup process..."
    
    # Initialize
    create_backup_dir
    check_database_connection
    
    # Create backups
    local full_backup=$(create_full_backup)
    local diff_backup=$(create_differential_backup)
    local log_backup=$(create_transaction_log_backup)
    
    # Verify backups
    verify_backup "$full_backup"
    [ -n "$diff_backup" ] && verify_backup "$diff_backup"
    [ -n "$log_backup" ] && verify_backup "$log_backup"
    
    # Compress backups
    local compressed_full=$(compress_backup "$full_backup")
    [ -n "$diff_backup" ] && compress_backup "$diff_backup"
    [ -n "$log_backup" ] && compress_backup "$log_backup"
    
    # Cleanup and reporting
    cleanup_old_backups
    generate_backup_report
    
    # Send notification
    send_notification "SUCCESS" "Backup completed successfully at $(date)"
    
    log_success "Database backup process completed successfully"
}

# Error handling
trap 'log_error "Backup process failed at line $LINENO"; send_notification "FAILED" "Backup failed at $(date)"; exit 1' ERR

# Run main function
main "$@"
