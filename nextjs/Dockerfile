# Single-stage Dockerfile
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
    fi

# Install dependencies
RUN npm install

# Build the application
RUN npm run build

EXPOSE 3000

# Run the application
CMD ["npm", "run", "start"]