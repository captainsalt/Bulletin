FROM node:14 as build
WORKDIR /build

# Install node packages on separate layer
COPY package.json .
COPY yarn.lock .
RUN yarn

# Run build
ARG CACHEBUST=$(date)
COPY . .
RUN yarn build

FROM nginx:mainline-alpine
COPY --from=build /build/dist /usr/share/nginx/html