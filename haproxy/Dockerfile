FROM alpine
RUN apk add --no-cache haproxy curl
RUN adduser --disabled-password --home /app --gecos '' appuser && chown -R appuser /app
USER appuser
COPY haproxy.cfg /usr/local/etc/haproxy/haproxy.cfg
EXPOSE 5433:5433 8404:8404
ENTRYPOINT ["haproxy", "-f", "/usr/local/etc/haproxy/haproxy.cfg"]