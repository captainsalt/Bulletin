FROM node:14 as build
WORKDIR /build
COPY . .
RUN yarn && yarn build

FROM nginx:mainline-alpine
COPY --from=build /build/dist /usr/share/nginx/html