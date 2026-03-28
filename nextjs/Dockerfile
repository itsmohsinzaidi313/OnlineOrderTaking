FROM node:24-alpine

RUN apk add --no-cache unzip

WORKDIR /app

# Extract zip file
COPY nextjsprojectcode.zip /tmp/
RUN unzip /tmp/nextjsprojectcode.zip && \
    # Move files if nested in a single directory
    if [ -d "nextjsprojectcode" ]; then \
        mv nextjsprojectcode/* . && \
        rm -rf nextjsprojectcode; \
    fi

# Install dependencies
RUN npm install

# Fix next binary permissions
RUN chmod +x node_modules/.bin/next

# Build the application
RUN npm run build

# Run the application
CMD ["npm", "run", "start"]