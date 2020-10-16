docker-compose build --no-cache --parallel $@
docker-compose kill $@
docker-compose up -d --no-deps $@