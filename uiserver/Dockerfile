FROM node:14

WORKDIR /usr/docker/smartpage

COPY package.json ./
COPY package-lock.json ./

RUN npm install

COPY . .

EXPOSE 443

CMD ["node", "server.js"]
