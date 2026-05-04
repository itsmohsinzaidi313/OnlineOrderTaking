# Stage 1: Build React app
FROM node:25-alpine AS build

WORKDIR /app

# Install dependencies
COPY ygen_backpanel/package*.json ./
RUN npm install --force

# Copy source and build
COPY ygen_backpanel/ ./
RUN npx browserslist@latest --update-db
RUN npm run build

# Stage 2: Serve with nginx
FROM nginx:alpine

# Remove default nginx content
RUN rm -rf /usr/share/nginx/html/*

# Copy build output
COPY --from=build /app/build /usr/share/nginx/html

# Copy custom nginx config (for React Router support)
COPY nginx.conf /etc/nginx/conf.d/default.conf

CMD ["nginx", "-g", "daemon off;"]
