# Dockerfile with zip extraction
FROM node:24-alpine AS builder

# Install unzip
RUN apk add --no-cache unzip

WORKDIR /app

# Copy and extract zip file
COPY nextjsprojectcode.zip /tmp/
RUN unzip /tmp/nextjsprojectcode.zip -d /app

# Install dependencies
COPY package.json package-lock.json* ./
RUN npm ci && npm cache clean --force

# Build the application
RUN npm run build

# Production stage
FROM node:24-alpine AS runner
WORKDIR /app

RUN addgroup --system --gid 1001 nodejs
RUN adduser --system --uid 1001 nextjs

ENV NODE_ENV=production
ENV NEXT_TELEMETRY_DISABLED=1
ENV PORT=3000

# Copy built files
COPY --from=builder /app/public ./public
COPY --from=builder /app/.next/standalone ./
COPY --from=builder /app/.next/static ./.next/static

RUN chown -R nextjs:nodejs /app
USER nextjs

EXPOSE 3000
CMD ["node", "server.js"]