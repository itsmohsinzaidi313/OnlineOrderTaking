FROM quay.io/coreos/etcd:v3.6.5
CMD ["etcd"]

HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
    CMD curl -f http://localhost:2379/health || exit 1