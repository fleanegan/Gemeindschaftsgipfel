# This is a multi-stage build so we'll start with a Node image
FROM node:latest AS node_base
RUN echo "NODE Version:" && node --version
RUN echo "NPM Version:" && npm --version

RUN mkdir ClientApp
COPY ["./ClientApp/package.json", "./ClientApp"]
WORKDIR ./ClientApp
RUN npm install
COPY ["./ClientApp", "."]
RUN npm run build
WORKDIR ..
CMD ls -l ./wwwroot

FROM nginx:alpine AS env_inliner
ARG SERVER_PORT
ARG PROXY_PORT
ARG PROXY_PORT_SSL
ENV SERVER_PORT $SERVER_PORT
ENV PROXY_PORT $PROXY_PORT
ENV PROXY_PORT_SSL $PROXY_PORT_SSL
COPY ./nginx.conf.template /etc/nginx/nginx.conf.template
RUN envsubst '${SERVER_PORT} ${PROXY_PORT} ${PROXY_PORT_SSL}' < /etc/nginx/nginx.conf.template > /etc/nginx/nginx.conf

FROM nginx:alpine AS final
COPY --from=node_base ./wwwroot /usr/share/nginx/html
COPY --from=env_inliner /etc/nginx/nginx.conf /etc/nginx/nginx.conf
CMD ["nginx", "-g", "daemon off;"]