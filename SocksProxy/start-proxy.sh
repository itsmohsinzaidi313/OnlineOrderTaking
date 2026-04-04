#!/bin/sh

# Wait a moment for network
sleep 2

echo "Starting SOCKS proxy to $JUMP_BOX_USER@$JUMP_BOX_HOST"

# Test connection first
if ! ssh -o ConnectTimeout=5 \
         -o BatchMode=yes \
         -o PasswordAuthentication=no \
         -T $JUMP_BOX_USER@$JUMP_BOX_HOST "exit"; then
    echo "ERROR: Cannot connect to jump box"
    echo "Check that:"
    echo "  1. Your public key is in jump box's authorized_keys"
    echo "  2. The username is correct"
    echo "  3. The jump box is reachable"
    exit 1
fi

# Start proxy
exec ssh -D 0.0.0.0:1080 \
    -N \
    -o ServerAliveInterval=30 \
    $JUMP_BOX_USER@$JUMP_BOX_HOST