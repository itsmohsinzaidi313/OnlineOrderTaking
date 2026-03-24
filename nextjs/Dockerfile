# Debug stage to understand structure
FROM node:24-alpine AS debug
RUN apk add --no-cache unzip tree
WORKDIR /app

COPY nextjsprojectcode.zip /tmp/
RUN unzip /tmp/nextjsprojectcode.zip -d /tmp/extracted && \
    echo "===================================" && \
    echo "EXTRACTED FILES STRUCTURE:" && \
    echo "===================================" && \
    ls -la /tmp/extracted && \
    echo "===================================" && \
    echo "MOVING FILES TO CORRECT LOCATION" && \
    # Handle nested directory structure
    if [ -d "/tmp/extracted/nextjsprojectcode" ]; then \
        echo "Found nested directory: nextjsprojectcode"; \
        cp -r /tmp/extracted/nextjsprojectcode/* /app/; \
    elif [ $(ls -1 /tmp/extracted | wc -l) -eq 1 ] && [ -d "$(ls -1 /tmp/extracted)" ]; then \
        SUBDIR=$(ls -1 /tmp/extracted); \
        echo "Found single directory: $SUBDIR"; \
        cp -r /tmp/extracted/$SUBDIR/* /app/; \
    else \
        echo "Copying all files directly"; \
        cp -r /tmp/extracted/* /app/; \
    fi && \
    echo "===================================" && \
    echo "FINAL APP DIRECTORY STRUCTURE:" && \
    echo "===================================" && \
    ls -la /app && \
    echo "===================================" && \
    echo "CHECKING FOR package.json:" && \
    if [ -f "/app/package.json" ]; then \
        echo "✅ package.json found!"; \
    else \
        echo "❌ package.json NOT found!"; \
        echo "Searching for package.json:"; \
        find /app -name "package.json"; \
        exit 1; \
    fi

# Builder stage
FROM node:24-alpine AS builder
WORKDIR /app

# Copy the properly structured app from debug stage
COPY --from=debug /app /app

# Verify package.json exists
RUN test -f package.json || (echo "package.json not found!" && exit 1)

# Install dependencies
RUN npm ci --only=production && npm cache clean --force
RUN npm install -D tailwindcss postcss autoprefixer @tailwindcss/postcss

# Build the application
RUN npm run build

# Production stage
FROM node:24-alpine AS runner
WORKDIR /app

# Create non-root user
RUN addgroup --system --gid 1001 nodejs && \
    adduser --system --uid 1001 nextjs

ENV NODE_ENV=production
ENV NEXT_TELEMETRY_DISABLED=1
ENV PORT=3000

# Copy built files
COPY --from=builder /app/public ./public
COPY --from=builder /app/.next/standalone ./
COPY --from=builder /app/.next/static ./.next/static

# Copy package.json for any runtime needs
COPY --from=builder /app/package.json ./

# Set ownership
RUN chown -R nextjs:nodejs /app
USER nextjs

EXPOSE 3000

# Start the application
CMD ["node", "server.js"]