FROM node:24-alpine AS debug
RUN apk add --no-cache unzip tree
WORKDIR /app

# Copy and extract zip
COPY nextjsprojectcode.zip /tmp/
RUN unzip /tmp/nextjsprojectcode.zip -d /tmp/extracted

# Debug: Print what's inside
RUN echo "===================================" && \
    echo "EXTRACTED FILES STRUCTURE:" && \
    echo "===================================" && \
    ls -laR /tmp/extracted && \
    echo "===================================" && \
    echo "LOOKING FOR PACKAGE.JSON:" && \
    find /tmp/extracted -name "package.json" && \
    echo "==================================="

# Copy files to app directory
RUN cp -r /tmp/extracted/* /app/ 2>/dev/null || true

# Debug: Show final app structure
RUN echo "===================================" && \
    echo "FINAL APP DIRECTORY STRUCTURE:" && \
    echo "===================================" && \
    ls -la /app && \
    echo "===================================" && \
    if [ -f "/app/package.json" ]; then \
        echo "✅ package.json found!"; \
    else \
        echo "❌ package.json NOT found in /app!"; \
        exit 1; \
    fi

FROM node:24-alpine AS builder
WORKDIR /app

# Copy from debug stage
COPY --from=debug /app /app
COPY --from=debug /tmp/extracted /tmp/extracted

# Install dependencies
COPY package.json package-lock.json* ./
RUN npm ci --only=production && npm cache clean --force

# Build
RUN npm run build

FROM node:24-alpine AS runner
WORKDIR /app
RUN addgroup --system --gid 1001 nodejs && \
    adduser --system --uid 1001 nextjs

ENV NODE_ENV=production
ENV PORT=3000

COPY --from=builder /app/public ./public
COPY --from=builder /app/.next/standalone ./
COPY --from=builder /app/.next/static ./.next/static

RUN chown -R nextjs:nodejs /app
USER nextjs

EXPOSE 3000
CMD ["node", "server.js"]