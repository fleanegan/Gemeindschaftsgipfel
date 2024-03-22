FROM nginx:alpine AS base
ARG SERVER_PORT
ARG PROXY_PORT
ARG PROXY_PORT_SSL
ENV SERVER_PORT $SERVER_PORT
ENV PROXY_PORT $PROXY_PORT
ENV PROXY_PORT_SSL $PROXY_PORT_SSL
COPY docker-entrypoint.sh /
RUN chmod +x ./docker-entrypoint.sh
ENTRYPOINT ["/docker-entrypoint.sh"]
CMD ["nginx", "-g", "daemon off;"]