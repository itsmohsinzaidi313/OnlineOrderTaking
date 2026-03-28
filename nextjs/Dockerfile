FROM node:24-alpine

RUN apk add --no-cache unzip

WORKDIR /app

# Extract tar.gz file and move contents to the current directory
COPY website.tar.gz /tmp/
RUN tar -xzf /tmp/website.tar.gz -C /tmp && \
    # Move files if nested in a single directory
    if [ -d "/tmp/" ]; then \
        mv /tmp//* . && \
        rm -rf /tmp/; \
    fi

# Install dependencies
RUN npm install

# Fix next binary permissions
RUN chmod +x node_modules/.bin/next

# Build the application
RUN npm run build

# Run the application
CMD ["npm", "run", "start"]