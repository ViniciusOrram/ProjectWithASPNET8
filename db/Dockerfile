FROM mysql:5.7.22
EXPOSE 3306
COPY ./ProjectWithASPNET8/db/migrations/ /home/database/
COPY ./ProjectWithASPNET8/db/dataset/ /home/database/
COPY ./ProjectWithASPNET8/ci/init_database.sh/ /docker-entrypoint-initdb.d/init_database.sh