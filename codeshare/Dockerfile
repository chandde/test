FROM node:10

WORKDIR /home/dengydongn/git/misc/codeshare

COPY ./package.json ./

RUN npm install

COPY . .

# ENV PORT $PORT

# EXPOSE "$PORT"

CMD node app.js $PORT

