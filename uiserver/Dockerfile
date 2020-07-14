FROM node:14

WORKDIR /usr/docker/smartpage

COPY package.json ./
COPY package-lock.json ./

RUN npm install

COPY . .

EXPOSE 8000 8001 8002 8003 8004 8005 8006 8007 8008 8009 8010

CMD ["node", "server.js"]
