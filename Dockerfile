FROM node:10

WORKDIR /home/dengydongn/git/misc/codeshare

COPY ./codeshare/package.json ./

RUN npm install

COPY ./codeshare .

EXPOSE 4001
EXPOSE 6179

CMD ["export PORT=4001"]

CMD ["node", "app.js"]

