FROM node:14

WORKDIR /usr/docker/smartpage

COPY package.json ./
COPY package-lock.json ./

RUN npm install

COPY . .

EXPOSE 8000-8010

EXPOSE 8080

# secret is intended to leave blank, manually fill in when creating docker image in prod env
ENV AZURE_CLIENT_ID=9610e2ca-fbbc-49a5-9cf6-db7bdec4b206
ENV AZURE_CLIENT_SECRET=
ENV AZURE_TENANT_ID=124edf19-b350-4797-aefc-3206115ffdb3

CMD ["node", "server.js"]
