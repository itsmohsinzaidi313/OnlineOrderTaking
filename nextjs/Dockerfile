FROM node:24-alpine
RUN apk add --no-cache git openssh-client
WORKDIR /tmp

# Clone repository
RUN git clone git@github.com:Samad-Developer/web-ordering.git
RUN mv web-ordering/* . && rm -rf web-ordering

WORKDIR /app

# Install dependencies
RUN npm install

# Fix next binary permissions
RUN chmod +x node_modules/.bin/next

# Build the application
RUN npm run build

# Run the application
CMD ["npm", "run", "start"]