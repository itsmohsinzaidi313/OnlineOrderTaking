# Dockerfile
FROM node:24-alpine

RUN apk add --no-cache git openssh-client

# Set up SSH for GitHub
RUN mkdir -p /root/.ssh && \
    ssh-keyscan github.com >> /root/.ssh/known_hosts

WORKDIR /app

# Copy package files first for better caching
COPY package*.json ./

# Install dependencies
RUN npm install

# Fix next binary permissions
RUN chmod +x node_modules/.bin/next

# Copy the rest of the application (will be cloned at runtime)
COPY . .

# Build the application
RUN npm run build

# Run the application
CMD ["npm", "run", "start"]