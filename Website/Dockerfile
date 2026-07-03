FROM node:25-alpine

WORKDIR /app

# Copy package files from the webordering_nextjs subdirectory
COPY webordering_nextjs/package.json webordering_nextjs/package-lock.json* ./

# Install dependencies
RUN npm install

# Copy the rest of the Next.js application
COPY webordering_nextjs/ .

# Remove .next directory if it exists
RUN rm -rf .next

# Build the application
RUN npm run build

# Debug: Verify build output
RUN echo "After build - .next contents:" && ls -la .next

# Run the application
CMD ["npm", "run", "start"]