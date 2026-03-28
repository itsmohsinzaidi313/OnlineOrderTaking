FROM node:24-alpine

RUN apk add --no-cache unzip

WORKDIR /app

# Install dependencies
RUN npm install

# Fix next binary permissions
# RUN chmod +x node_modules/.bin/next

# Build the application
RUN npm run build

# Run the application
CMD ["npm", "run", "start"]