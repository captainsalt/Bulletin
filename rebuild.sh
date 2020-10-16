docker-compose build --parallel $@
docker-compose kill $@
docker-compose up -d --no-deps $@