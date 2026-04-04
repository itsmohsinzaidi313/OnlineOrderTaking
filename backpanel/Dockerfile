FROM node:24-alpine

WORKDIR /app

# Copy package files from the backpanel subdirectory
COPY backpanel/package.json backpanel/package-lock.json* ./

# Install dependencies
RUN npm install

# Copy the rest of the Next.js application
COPY backpanel/ .

# Remove .next directory if it exists
RUN rm -rf .next

# Build the application
RUN npm run build

# Debug: Verify build output
RUN echo "After build - .next contents:" && ls -la .next

# Run the application
CMD ["npm", "run", "start"]