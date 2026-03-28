# Stage 1: Builder - Where we clone and build
FROM node:24-alpine AS builder

# Install build dependencies
RUN apk add --no-cache git openssh-client

# Configure SSH for GitHub (without storing keys)
RUN mkdir -p /root/.ssh && \
    chmod 700 /root/.ssh && \
    ssh-keyscan github.com >> /root/.ssh/known_hosts

# Clone repository using BuildKit SSH mount (keys never in image)
WORKDIR /build
RUN --mount=type=ssh \
    git clone git@github.com:Samad-Developer/web-ordering.git .

# Install ALL dependencies (including dev)
RUN npm ci --only=production=false

# Build the application
RUN npm run build

# Prune dev dependencies
RUN npm prune --omit=dev

# Stage 2: Production - Minimal runtime image
FROM node:24-alpine AS production

# Install runtime dependencies
RUN apk add --no-cache curl tini

# Create non-root user
RUN addgroup -g 1001 -S nodejs && \
    adduser -S nextjs -u 1001

WORKDIR /app

# Copy built application from builder
COPY --from=builder --chown=nextjs:nodejs /build/.next ./.next
COPY --from=builder --chown=nextjs:nodejs /build/public ./public
COPY --from=builder --chown=nextjs:nodejs /build/package.json ./package.json
COPY --from=builder --chown=nextjs:nodejs /build/node_modules ./node_modules

# Healthcheck
HEALTHCHECK --interval=30s --timeout=10s --start-period=40s --retries=3 \
    CMD curl -f http://localhost:3000/api/health || exit 1

# Switch to non-root user
USER nextjs

# Use tini for proper signal handling
ENTRYPOINT ["/sbin/tini", "--"]

EXPOSE 3000

# Start the application
CMD ["npm", "run", "start"]