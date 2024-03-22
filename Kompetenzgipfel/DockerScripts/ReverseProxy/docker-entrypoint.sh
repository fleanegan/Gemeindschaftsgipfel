#!/usr/bin/env sh
set -eu

envsubst '${SERVER_PORT} ${PROXY_PORT} ${PROXY_PORT_SSL}' < /etc/nginx/nginx.conf.template > /etc/nginx/nginx.conf

exec "$@"