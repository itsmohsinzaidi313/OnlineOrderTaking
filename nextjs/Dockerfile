FROM node:24-alpine

WORKDIR /app

# Copy package files from the web-ordering subdirectory
COPY web-ordering/package.json web-ordering/package-lock.json* ./

# Install dependencies
RUN npm install

# Copy the rest of the Next.js application
COPY web-ordering/ .

# Debug: List files to verify
RUN echo "Files in /app:" && ls -la
RUN echo "Checking if .next exists:" && ls -la .next 2>/dev/null || echo ".next not found"

# Build the application
RUN npm run build

# Debug: Verify build output
RUN echo "After build - .next contents:" && ls -la .next

# Run the application
CMD ["npm", "run", "start"]