# Single-stage Dockerfile with permission fixes
FROM node:24-alpine

RUN apk add --no-cache unzip

WORKDIR /app

# Copy and extract zip file
COPY nextjsprojectcode.zip /tmp/
RUN unzip /tmp/nextjsprojectcode.zip -d /tmp/extracted && \
    # Handle nested directory structure
    if [ -d "/tmp/extracted/nextjsprojectcode" ]; then \
        cp -r /tmp/extracted/nextjsprojectcode/* /app/; \
    elif [ $(ls -1 /tmp/extracted | wc -l) -eq 1 ] && [ -d "$(ls -1 /tmp/extracted)" ]; then \
        SUBDIR=$(ls -1 /tmp/extracted); \
        cp -r /tmp/extracted/$SUBDIR/* /app/; \
    else \
        cp -r /tmp/extracted/* /app/; \
    fi && \
    # Fix permissions for node_modules/.bin
    chmod -R +x /app/node_modules/.bin 2>/dev/null || true

# Install dependencies with proper permissions
RUN npm install && \
    # Ensure next has execute permission
    chmod +x node_modules/.bin/next

# Build the application
RUN npm run build

EXPOSE 3000

# Run the application
CMD ["npm", "run", "start"]