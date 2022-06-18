DOCKER_COMPOSE_FILE=docker-compose.yml
DOCKER_COMPOSE_DEV_FILE=docker-compose.dev.yml

all:
	make -C TwitterFairy $@

build:
	make -C TwitterFairy $@

clean:
	make -C TwitterFairy $@

start-dev:
	make -C TwitterFairy $@

start:
	make -C TwitterFairy $@
docker-build:
	docker-compose -f ${DOCKER_COMPOSE_FILE} build --parallel

docker-start-dev:
	docker-compose -f ${DOCKER_COMPOSE_FILE} up -d

docker-start:
	docker-compose -f ${DOCKER_COMPOSE_FILE} up -d

docker-rebuild:
	docker-compose -f ${DOCKER_COMPOSE_FILE} -f build --parallel --no-cache --force-rm --pull

docker-stop:
	docker-compose -f ${DOCKER_COMPOSE_FILE} down --remove-orphans

docker-clean:
	docker-compose -f ${DOCKER_COMPOSE_FILE} rm -s -f -v

distclean: clean docker-clean

.PHONY: all build test clean start-dev start distclean\
		docker-build docker-rebuild docker-start-dev docker-start docker-stop docker-clean

.SILENT: clean docker-clean distclean
